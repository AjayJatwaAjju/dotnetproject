namespace VCE.Shared.Dictionary
{
    public class Month
    {
        #region Constants and Fields

        public IDictionary<string, int> Dictionary = new Dictionary<string, int>();

        #endregion Constants and Fields

        public IDictionary<string, int> GetMonths()
        {
            Dictionary.Add("Microsoft SQL Server", 1);
            Dictionary.Add("PostgreSQL", 2);
            return Dictionary;
        }
    }
}