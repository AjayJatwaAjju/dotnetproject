using System.Security.Cryptography;

/// <summary>
/// This namespace connects all the Decrypt string.
/// </summary>

namespace VCE.Utility
{
    public static class Decrypt
    {
        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string GetValue(string encryptString)
        {
            try
            {
                var b = Convert.FromBase64String(encryptString);
                return System.Text.Encoding.ASCII.GetString(b); ;
            }
            catch
            {
                return encryptString;
            }
        }
        public static string GetMD5Value(string encryptString)
        {
            try
            {
                byte[] results;
                System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding();

                // Step 1. We hash the passphrase using MD5
                // We use the MD5 hash generator as the result is a 128 bit byte array
                // which is a valid length for the TripleDES encoder we use below

                MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
                byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes("nt4F4/JqPLfUkRcoc7YIxFr1Mf3UJg=="));

                // Step 2. Create a new TripleDESCryptoServiceProvider object // Step 3. Setup the decoder
                TripleDESCryptoServiceProvider tdesAlgorithm = new TripleDESCryptoServiceProvider { Key = tdesKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

                // Step 4. Convert the input string to a byte[]
                byte[] dataToDecrypt = Convert.FromBase64String(encryptString);

                // Step 5. Attempt to decrypt the string
                try
                {
                    ICryptoTransform decryptor = tdesAlgorithm.CreateDecryptor();
                    results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
                }
                finally
                {
                    // Clear the TripleDes and Hashprovider services of any sensitive information
                    tdesAlgorithm.Clear();
                    hashProvider.Clear();
                }
                // Step 6. Return the decrypted string in UTF8 format
                return utf8.GetString(results);
            }
            catch
            {
                throw;
            }
        }
        ///---------------------------------------------------------------------------
        /// <summary>
        /// Using FromBase64String to convert base64String.
        /// </summary>
        /// <param name="base64EncodedBytes">Convert the code into base64EncodedBytes </param>
        /// <returns>Encoding data.</returns>
        ///---------------------------------------------------------------------------
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                throw;
            }
        }
    }
}