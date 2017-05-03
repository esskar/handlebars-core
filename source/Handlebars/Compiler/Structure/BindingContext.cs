using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class BindingContext
    {
        private readonly BindingContext _parent;

        public string TemplatePath { get; }

        public EncodedTextWriter TextWriter { get; }

        public bool SuppressEncoding
        {
            get { return TextWriter.SuppressEncoding; }
            set { TextWriter.SuppressEncoding = value; }
        }

        public BindingContext(object value, EncodedTextWriter writer, BindingContext parent, string templatePath)
        {
            TemplatePath = parent != null ? (parent.TemplatePath ?? templatePath) : templatePath;
            TextWriter = writer;
            Value = value;
            _parent = parent;
        }

        public virtual object Value { get; }

        public virtual BindingContext ParentContext => _parent;

        public virtual object Root
        {
            get
            {
                var currentContext = this;
                while (currentContext.ParentContext != null)
                {
                    currentContext = currentContext.ParentContext;
                }
                return currentContext.Value;
            }
        }

        public virtual object GetContextVariable(string variableName)
        {
            var target = this;

            return GetContextVariable(variableName, target)
                   ?? GetContextVariable(variableName, target.Value);
        }

        private object GetContextVariable(string variableName, object target)
        {
            object returnValue;
            variableName = variableName.TrimStart('@');
            var member = target.GetType().GetMember(variableName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (member.Length > 0)
            {
                if (member[0] is PropertyInfo propertyInfo)
                {
                    returnValue = propertyInfo.GetValue(target, null);
                }
                else if (member[0] is FieldInfo fieldInfo)
                {
                    returnValue = fieldInfo.GetValue(target);
                }
                else
                {
                    throw new HandlebarsRuntimeException("Context variable references a member that is not a field or property");
                }
            }
            else if (_parent != null)
            {
                returnValue = _parent.GetContextVariable(variableName);
            }
            else
            {
                returnValue = null;
            }
            return returnValue;
        }

        public virtual BindingContext CreateChildContext(object value)
        {
            return new BindingContext(value, TextWriter, this, TemplatePath);
        }
    }
}

