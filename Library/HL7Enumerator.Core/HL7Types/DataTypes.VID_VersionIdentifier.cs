namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class VID_VersionIdentifier
        { 
           public string VersionID { get; set; }
           public CE_CodedElement Internationalization { get; set; }
           public CE_CodedElement InternationalVersionID { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char separator)
            {
                return
                    $"{VersionID}{separator}{Internationalization.ToString()}{separator}{InternationalVersionID.ToString()}";
            }

        }

    }
}

