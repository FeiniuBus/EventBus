namespace EventBus.Core.Infrastructure
{
    public sealed class JSONSimpleType
    {
        public JSONSimpleType(string value, SimpleTypes simpleType)
        {
            Value = value;
            SimpleType = simpleType;
        }
        public string Value { get; set; }
        public SimpleTypes SimpleType { get; set; }
    }
}
