/// <summary>
/// This Config is responsible for supplying link and credentials.
/// </summary>
namespace VCE.Shared
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// This class will contain the property of Config links.
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :- Used for encrypting machine / web.config and custom configuration files.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public static class Config
    {
        #region Public var
        public static int ApplicationTimeOut = 360;
        public static string DatabaseName = "VCE";
        public static string Host = "abc.xyz.net";
        public static int port = 587;
        public static string UserName = "connect@support.com";
        public static string Password = "support";
        #endregion
        #region Config Key's
        public static string GetConnectionString = "UTI5dWJtVmpkR2x2YmxOMGNtbHVadz09";
        public static string GetTokenInfo = "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=";
        #endregion Config Key's
    }
}