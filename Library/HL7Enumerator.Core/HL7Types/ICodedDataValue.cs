namespace HL7Enumerator.Types
{
    public interface ICodedDataValue
    {
        string CodedValue { get; }
        string Description { get; }
        bool? IsValid { get; }
        string Value { get; set; }
        string BestValue { get; }
        string TableId { get; }
    }
}