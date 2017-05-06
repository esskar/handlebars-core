using System.Reflection;

namespace Handlebars.Core.Compiler.Structure
{
    internal class BindingContext
    {
        public EncodedTextWriter TextWriter { get; }

        public bool SuppressEncoding
        {
            get { return TextWriter.SuppressEncoding; }
            set { TextWriter.SuppressEncoding = value; }
        }

        public BindingContext(object value, EncodedTextWriter writer, BindingContext parent, string templateName)
        {
            TextWriter = writer;
            Value = value;
            ParentContext = parent;
            TemplateName = templateName;
        }

        public virtual object Value { get; }

        public virtual BindingContext ParentContext { get; }

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

        public string TemplateName { get; }

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
            else if (ParentContext != null)
            {
                returnValue = ParentContext.GetContextVariable(variableName);
            }
            else
            {
                returnValue = null;
            }
            return returnValue;
        }

        public virtual BindingContext CreateChildContext(object value)
        {
            return new BindingContext(value, TextWriter, this, TemplateName);
        }
    }
}

