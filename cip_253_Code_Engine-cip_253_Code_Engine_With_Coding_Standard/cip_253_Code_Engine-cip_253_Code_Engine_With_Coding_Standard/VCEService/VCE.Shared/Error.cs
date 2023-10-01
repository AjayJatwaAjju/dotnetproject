namespace VCE.Shared
{
    public static class Error
    {
        ///---------------------------------------------------------------------------
        /// <summary>
        /// This class will contain the property of get Set.
        /// </summary>
        /// <remarks>
        /// Created on :- 22-09-2023
        /// Created By :- VCE
        /// Purpose :- This Error is responsible for supplying Error messages.
        /// </remarks>
        ///---------------------------------------------------------------------------
        public class OutputDetails
        {
            public object? data { get; set; } = null;
            public string error { get; set; } = string.Empty;
            public bool isError { get; set; } = false;
        }
    }
}