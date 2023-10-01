namespace VCE.Shared.Dictionary
{
    public class Language
    {
        #region Constants and Fields

        public IDictionary<string, string> Dictionary = new Dictionary<string, string>();

        #endregion Constants and Fields

        public IDictionary<string, string> GetLanguage()
        {
            Dictionary.Add("US", "en-US");
            Dictionary.Add("IN", "en-US");
            Dictionary.Add("BS", "en-US");

            //fr-FR Language Name French
            Dictionary.Add("DZ", "fr-FR");
            Dictionary.Add("BJ", "fr-FR");
            Dictionary.Add("BE", "fr-FR");
            Dictionary.Add("BI", "fr-FR");
            Dictionary.Add("CM", "fr-FR");
            Dictionary.Add("CF", "fr-FR");
            Dictionary.Add("TD", "fr-FR");
            Dictionary.Add("CG", "fr-FR");
            Dictionary.Add("DJ", "fr-FR");
            Dictionary.Add("FR", "fr-FR");
            Dictionary.Add("GA", "fr-FR");
            Dictionary.Add("MG", "fr-FR");
            Dictionary.Add("ML", "fr-FR");
            Dictionary.Add("MV", "fr-FR");
            Dictionary.Add("MA", "fr-FR");
            Dictionary.Add("NE", "fr-FR");
            Dictionary.Add("SN", "fr-FR");
            Dictionary.Add("TG", "fr-FR");
            Dictionary.Add("GP", "fr-FR");
            Dictionary.Add("HI", "fr-FR");
            Dictionary.Add("JE", "fr-FR");

            //ar-AE Language Name Arabic
            Dictionary.Add("OM", "ar-AE");
            Dictionary.Add("PS", "ar-AE");
            Dictionary.Add("RE", "ar-AE");
            Dictionary.Add("SA", "ar-AE");
            Dictionary.Add("SO", "ar-AE");
            Dictionary.Add("TN", "ar-AE");
            Dictionary.Add("AE", "ar-AE");
            Dictionary.Add("BH", "ar-AE");
            Dictionary.Add("JO", "ar-AE");
            Dictionary.Add("KW", "ar-AE");
            Dictionary.Add("LB", "ar-AE");
            Dictionary.Add("LY", "ar-AE");
            Dictionary.Add("LS", "ar-AE");

            //es-ES Language Name Spanish
            Dictionary.Add("DO", "es-ES");
            Dictionary.Add("ED", "es-ES");
            Dictionary.Add("SV", "es-ES");
            Dictionary.Add("HN", "es-ES");
            Dictionary.Add("MX", "es-ES");
            Dictionary.Add("PA", "es-ES");
            Dictionary.Add("PY", "es-ES");
            Dictionary.Add("ES", "es-ES");
            Dictionary.Add("UY", "es-ES");
            Dictionary.Add("VE", "es-ES");

            //de-DE Language Name German
            Dictionary.Add("LI", "de-DE");
            Dictionary.Add("LU", "de-DE");
            Dictionary.Add("LV", "de-DE");
            Dictionary.Add("AT", "de-DE");
            Dictionary.Add("NL", "de-DE");
            Dictionary.Add("CH", "de-DE");

            //pt-PT Language Name Portuguese
            Dictionary.Add("PT", "pt-PT");
            Dictionary.Add("PR", "pt-PT");
            Dictionary.Add("ST", "pt-PT");
            Dictionary.Add("TL", "pt-PT");
            Dictionary.Add("AO", "pt-PT");
            Dictionary.Add("MZ", "pt-PT");

            //ru-RU Language Name Russian
            Dictionary.Add("AZ", "ru-RU");
            Dictionary.Add("MD", "ru-RU");

            //it-IT Language Name Italian
            Dictionary.Add("IT", "it-IT");
            Dictionary.Add("SM", "it-IT");

            //ru-RU Language Name Japanese
            Dictionary.Add("JP", "ja-JP");

            //ru-RU Language Name Russian
            Dictionary.Add("RU", "ru-RU");
            Dictionary.Add("TW", "zh-CN");

            //ru-RU Language Name Swedish
            Dictionary.Add("FI", "sv-SE");
            Dictionary.Add("SE", "sv-SE");

            //ru-RU Language Name Hebrews
            Dictionary.Add("IL", "he-IL");

            //ru-RU Language Name Danish
            Dictionary.Add("DK", "da-DK");

            return Dictionary;
        }
    }
}

//There are reference link to check country code and languages (http://www.nationsonline.org/oneworld/country_code_list.htm) / (https://www.distilled.net/blog/uncategorized/google-cctlds-and-associated-languages-codes-reference-sheet/)