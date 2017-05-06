namespace Handlebars.Core.Internals
{
    internal class SafeString : IHandlebarsSafeString
    {
        private readonly string _value;

        public SafeString(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}