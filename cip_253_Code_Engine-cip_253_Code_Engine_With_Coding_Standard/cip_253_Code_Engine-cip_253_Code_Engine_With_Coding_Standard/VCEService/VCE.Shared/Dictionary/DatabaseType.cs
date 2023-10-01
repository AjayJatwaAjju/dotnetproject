namespace VCE.Shared.Dictionary
{
    public class DatabaseType
    {
        #region Constants and Fields

        public IDictionary<string, int> Dictionary = new Dictionary<string, int>();

        #endregion Constants and Fields

        public async Task<IDictionary<string, int>> GetDatabaseType()
        {
            Dictionary.Add("MSSQL", 1);
            Dictionary.Add("PostgreSQL", 2);
            return Dictionary;
        }
    }

    //Select DataType usinf this enum
    public enum DatabaseTypes
    {
        MicrosoftSQLServer = 1,
        PostgreSQL = 2
    }
}