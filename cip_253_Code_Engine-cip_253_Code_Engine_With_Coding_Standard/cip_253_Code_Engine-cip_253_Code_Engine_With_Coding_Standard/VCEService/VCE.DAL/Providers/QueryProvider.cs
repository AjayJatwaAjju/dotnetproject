namespace VCE.DAL.Providers
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// This class will contain the property of Qurey.
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :- Sepration of Query SQL or PGSQL.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public static class QueryProvider
    {
        #region SQL Query's

        public static string qSQL_SysDataBases = "SELECT NAME, DATABASE_ID, CREATE_DATE  FROM SYS.DATABASES ORDER BY 1;";

        #endregion SQL Query's

        #region PG Query's

        public static string qPG_SysDataBases = "SELECT DATNAME \"NAME\",OID DATABASE_ID FROM PG_DATABASE /*WHERE HAS_DATABASE_PRIVILEGE('PMERAVAT', DATNAME, 'CONNECT')=TRUE*/ ORDER  BY 1;";

        #endregion PG Query's

        #region SQL INFORMATION SCHEMA Query's

        public static string qSQL_TableSchema = "SELECT TABLE_SCHEMA+'.'+TABLE_NAME TABLE_NAME1,TABLE_SCHEMA+'.'+TABLE_NAME TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' ORDER BY 1;";

        #endregion SQL INFORMATION SCHEMA Query's

        #region PG INFORMATION SCHEMA Query's

        public static string qPG_TableSchema = "SELECT TABLE_SCHEMA ||'.'||TABLE_NAME \"TABLE_NAME1\",TABLE_SCHEMA ||'.'||TABLE_NAME \"TABLE_NAME\" FROM INFORMATION_SCHEMA.TABLES WHERE 1=1 AND TABLE_SCHEMA NOT IN ('PG_CATALOG','INFORMATION_SCHEMA','pg_catalog') AND TABLE_TYPE='BASE TABLE';";

        #endregion PG INFORMATION SCHEMA Query's
    }
}