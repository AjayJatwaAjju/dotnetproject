namespace VCE.Shared.Enum
{
    public enum EDataType
    {
        Xml,
        Json
    }

    /// <summary>
    /// This class is used for handling enum values
    /// </summary>
    public static class EDataTypeHelper
    {
        public static EDataType Parse(string sEnum)
        {
            return (EDataType)System.Enum.Parse(typeof(EDataType), sEnum, true);
        }

        public static string ToString(EDataType eEnum)
        {
            return System.Enum.GetName(typeof(EDataType), eEnum);
        }

        public static IEnumerable<string> List()
        {
            return System.Enum.GetNames(typeof(EDataType));
        }
    }
}