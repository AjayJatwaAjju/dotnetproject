namespace VCE.Shared.Dictionary
{
    public class Currency
    {
        #region Constants and Fields

        public IDictionary<string, string> Dictionary = new Dictionary<string, string>();

        #endregion Constants and Fields

        public IDictionary<string, string> GetCurrency()
        {
            Dictionary.Add("USD", "$");
            Dictionary.Add("EUR", "€");
            return Dictionary;
        }

        public IDictionary<string, string> GetAllCurrency()
        {
            Dictionary.Add("AUD", "€");
            Dictionary.Add("BRL", "R$");
            Dictionary.Add("CAD", "€");
            Dictionary.Add("CHF", "fr.");
            Dictionary.Add("CNY", "¥");
            Dictionary.Add("CZK", "€");
            Dictionary.Add("DKK", "kr.");
            Dictionary.Add("EUR", "€");
            Dictionary.Add("GBP", "£");
            Dictionary.Add("HKD", "HK$");
            Dictionary.Add("HUF", "Ft");
            Dictionary.Add("IDR", "Rp");
            Dictionary.Add("ILS", "₪");
            Dictionary.Add("INR", "रु");
            Dictionary.Add("JPY", "€");
            Dictionary.Add("KRW", "€");
            Dictionary.Add("MYR", "€");
            Dictionary.Add("MXN", "€");
            Dictionary.Add("NOK", "रु");
            Dictionary.Add("NZD", "$");
            Dictionary.Add("PHP", "PhP");
            Dictionary.Add("PLN", "€");
            Dictionary.Add("RUB", "р.");
            Dictionary.Add("SEK", "kr");
            Dictionary.Add("SGD", "$");
            Dictionary.Add("THB", "€");
            Dictionary.Add("TRY", "TL");
            Dictionary.Add("TWD", "NT$");
            Dictionary.Add("USD", "$");
            Dictionary.Add("VND", "₫");
            Dictionary.Add("ZAR", "R");
            Dictionary.Add("XOF", "CFA");
            return Dictionary;
        }
    }
}