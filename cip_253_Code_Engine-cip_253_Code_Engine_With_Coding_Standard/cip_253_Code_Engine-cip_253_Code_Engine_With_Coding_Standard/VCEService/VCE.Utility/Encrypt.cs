using System.Security.Cryptography;

namespace VCE.Utility
{
    public class Encrypt
    {
        /// <summary>
        /// Enrypt String
        /// </summary>
        /// <param name="strDecrypt"></param>
        /// <returns></returns>
        public static string GetValue(string strDecrypt)
        {
            try
            {
                var b = System.Text.Encoding.ASCII.GetBytes(strDecrypt);
                return Convert.ToBase64String(b);
            }
            catch
            {
                return strDecrypt;
            }
        }

        public static string GetMD5Value(string strDecrypt)
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

                // Step 2. Create a new TripleDESCryptoServiceProvider object // Step 3. Setup the encoder
                TripleDESCryptoServiceProvider tdesAlgorithm = new TripleDESCryptoServiceProvider { Key = tdesKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

                // Step 4. Convert the input string to a byte[]
                byte[] dataToEncrypt = utf8.GetBytes(strDecrypt);

                // Step 5. Attempt to encrypt the string
                try
                {
                    ICryptoTransform encryptor = tdesAlgorithm.CreateEncryptor();
                    results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
                }
                finally
                {
                    // Clear the TripleDes and Hashprovider services of any sensitive information
                    tdesAlgorithm.Clear();
                    hashProvider.Clear();
                }
                // Step 6. Return the encrypted string as a base64 encoded string
                return Convert.ToBase64String(results);
            }
            catch
            {
                throw;
            }
        }

        public const int SaltSize = 22;

        /// <summary>
        /// Creates a salt for using in hashes. Unit tested.
        /// </summary>
        public static string CreateSalt()
        {
            //Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[SaltSize];
            rng.GetBytes(buff);
            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }
    }
}