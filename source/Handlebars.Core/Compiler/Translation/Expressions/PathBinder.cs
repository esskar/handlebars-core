using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler.Translation.Expressions
{
    internal class PathBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PathBinder(context).Visit(expr);
        }

        private PathBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Variables,
                node.Expressions.Select(Visit));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return Expression.MakeUnary(
                node.NodeType,
                Visit(node.Operand),
                node.Type);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return Expression.Call(
                Visit(node.Object),
                node.Method,
                node.Arguments.Select(Visit));
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return Expression.Condition(
                Visit(node.Test),
                Visit(node.IfTrue),
                Visit(node.IfFalse));
        }

        protected override Expression VisitSubExpression(SubExpressionExpression subex)
        {
            return HandlebarsExpression.SubExpressionExpression(
                Visit(subex.Expression));
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is PathExpression)
            {
#if netstandard
                var writeMethod = typeof(TextWriter).GetRuntimeMethod("Write", new [] { typeof(object) });
#else
                var writeMethod = typeof(TextWriter).GetMethod("Write", new[] { typeof(object) });
#endif
                return Expression.Call(
                    Expression.Property(
                        CompilationContext.BindingContext,
                        "TextWriter"),
                    writeMethod, Visit(sex.Body));
            }
            return Visit(sex.Body);
        }

        protected override Expression VisitPathExpression(PathExpression pex)
        {
            return Expression.Call(
                Expression.Constant(this),
#if netstandard
                new Func<BindingContext, string, object>(ResolvePath).GetMethodInfo(),
#else
                new Func<BindingContext, string, object>(ResolvePath).Method,
#endif
                CompilationContext.BindingContext,
                Expression.Constant(pex.Path));
        }

        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            return HandlebarsExpression.HelperExpression(
                hex.HelperName,
                hex.Arguments.Select(Visit));
        }

        protected override Expression VisitHashParametersExpression(HashParametersExpression hpex)
        {
            return Expression.Call(
                Expression.Constant(this),
#if netstandard
                new Func<BindingContext, HashParametersExpression, object>(ResolveParameters).GetMethodInfo(),
#else
                new Func<BindingContext, HashParametersExpression, object>(ResolveParameters).Method,
#endif
                CompilationContext.BindingContext,
                Expression.Constant(hpex));
        }

        private object ResolveParameters(BindingContext context, HashParametersExpression hpex)
        {
            var parameters = new HashParameterDictionary();

            foreach (var parameter in hpex.Parameters)
            {
                var path = parameter.Value as PathExpression;
                parameters.Add(parameter.Key, path != null ? ResolvePath(context, path.Path) : parameter.Value);
            }

            return parameters;
        }

        //TODO: make path resolution logic smarter
        private object ResolvePath(BindingContext context, string path)
        {
            var containsVariable = path.StartsWith("@");
            if (containsVariable)
            {
                path = path.Substring(1);
                if (path.Contains(".."))
                {
                    context = context.ParentContext;
                }
            }

            var instance = context.Value;
            var hashParameters = instance as HashParameterDictionary;

            foreach (var segment in path.Split('/'))
            {
                if (segment == "..")
                {
                    context = context.ParentContext;
                    if (context == null)
                    {
                        if (containsVariable)
                            return string.Empty;

                        throw new HandlebarsCompilerException("Path expression tried to reference parent of root");
                    }
                    instance = context.Value;
                }
                else
                {
                    var objectPropertiesChain = containsVariable ? "@" + segment : segment;

                    foreach (var memberName in objectPropertiesChain.Split('.'))
                    {
                        instance = ResolveValue(context, instance, memberName);

                        if (!(instance is UndefinedBindingResult))
                            continue;

                        if (hashParameters == null || hashParameters.ContainsKey(memberName) || context.ParentContext == null)
                        {
                            if (CompilationContext.Engine.Configuration.ThrowOnUnresolvedBindingExpression)
                                throw new HandlebarsUndefinedBindingException(path, (instance as UndefinedBindingResult).Value);
                            return instance;
                        }

                        instance = ResolveValue(context.ParentContext, context.ParentContext.Value, memberName);
                        if (instance is UndefinedBindingResult undefinedBindingResult)
                        {
                            if (CompilationContext.Engine.Configuration.ThrowOnUnresolvedBindingExpression)
                                throw new HandlebarsUndefinedBindingException(path, undefinedBindingResult.Value);
                            return instance;
                        }
                    }
                }
            }
            return instance;
        }

        private object ResolveValue(BindingContext context, object instance, string segment)
        {
            object resolvedValue = CreateUndefinedBindingResult(segment);
            if (segment.StartsWith("@"))
            {
                var contextValue = context.GetContextVariable(segment.Substring(1));
                if (contextValue != null)
                {
                    resolvedValue = contextValue;
                }
            }
            else if (segment == "this" || segment == string.Empty)
            {
                resolvedValue = instance;
            }
            else
            {
                resolvedValue = AccessMember(instance, segment);
            }
            return resolvedValue;
        }

        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.None);

        private object AccessMember(object instance, string memberName)
        {
            if (instance == null)
                return CreateUndefinedBindingResult(memberName);

            var enumerable = instance as IEnumerable<object>;
            if (enumerable != null)
            {
                var match = IndexRegex.Match(memberName);
                if (match.Success)
                {
                    if (match.Groups["index"].Success == false || int.TryParse(match.Groups["index"].Value, out int index) == false)
                    {
                        return CreateUndefinedBindingResult(memberName);
                    }

                    var result = enumerable.ElementAtOrDefault(index);

                    return result ?? CreateUndefinedBindingResult(memberName);
                }
            }
            var resolvedMemberName = ResolveMemberName(instance, memberName);
            var instanceType = instance.GetType();
            //crude handling for dynamic objects that don't have metadata
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(instanceType))
            {
                try
                {
                    var result = GetProperty(instance, resolvedMemberName);
                    return result ?? CreateUndefinedBindingResult(resolvedMemberName);
                }
                catch
                {
                    return CreateUndefinedBindingResult(resolvedMemberName);
                }
            }


            // Check if the instance is IDictionary<,>
            var iDictInstance = FirstGenericDictionaryTypeInstance(instanceType);
            if (iDictInstance != null)
            {
                var genericArgs = iDictInstance.GetGenericArguments();
                object key = resolvedMemberName.Trim('[', ']');    // Ensure square brackets removed.
                if (genericArgs.Length > 0)
                {
                    // Dictionary key type isn't a string, so attempt to convert.
                    if (genericArgs[0] != typeof(string))
                    {
                        try
                        {
                            key = Convert.ChangeType(key, genericArgs[0], CultureInfo.CurrentCulture);
                        }
                        catch (Exception)
                        {
                            // Can't convert to key type.
                            return CreateUndefinedBindingResult(resolvedMemberName);
                        }
                    }
                }

                var arguments = new[] {key, null};
                if ((bool)instanceType.GetMethod("TryGetValue").Invoke(instance, arguments))
                {
                    return arguments[1];
                }
                // Key doesn't exist.
                return CreateUndefinedBindingResult(resolvedMemberName);
            }
            // Check if the instance is IDictionary (ie, System.Collections.Hashtable)
            if (typeof(IDictionary).IsAssignableFrom(instanceType))
            {
                var key = resolvedMemberName.Trim('[', ']');    // Ensure square brackets removed.
                // Only string keys supported - indexer takes an object, but no nice
                // way to check if the hashtable check if it should be a different type.
                return ((IDictionary)instance)[key];
            }

            var members = instanceType.GetMember(resolvedMemberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            MemberInfo preferredMember;
            if (members.Length == 0)
            {
                return CreateUndefinedBindingResult(resolvedMemberName);
            }
            if (members.Length > 1)
            {
                preferredMember = members.FirstOrDefault(m => m.Name == resolvedMemberName) ?? members[0];
            }
            else
            {
                preferredMember = members[0];
            }

            
            if (preferredMember is PropertyInfo propertyInfo)
            {
                var propertyValue = propertyInfo.GetValue(instance, null);
                return propertyValue ?? CreateUndefinedBindingResult(resolvedMemberName);
            }
            if (preferredMember is FieldInfo fieldInfo)
            {
                var fieldValue = fieldInfo.GetValue(instance);
                return fieldValue ?? CreateUndefinedBindingResult(resolvedMemberName);
            }
            return new UndefinedBindingResult(resolvedMemberName, CompilationContext.Engine.Configuration);
        }

        private static Type FirstGenericDictionaryTypeInstance(Type instanceType)
        {
            return instanceType.GetInterfaces()
                .FirstOrDefault(i =>
#if netstandard
                    i.GetTypeInfo().IsGenericType
#else
                    i.IsGenericType
#endif
                    &&
                    (
                        i.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                    )
                );
        }

        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        private string ResolveMemberName(object instance, string memberName)
        {
            var resolver = CompilationContext.Engine.Configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(instance, memberName) : memberName;
        }

        private UndefinedBindingResult CreateUndefinedBindingResult(string name)
        {
            return new UndefinedBindingResult(name, CompilationContext.Engine.Configuration);
        }
    }
}

