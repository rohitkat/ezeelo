using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace BusinessLogicLayer
{
   public class Encryption : ISecurities
    {
        /*   
             Developed By: Gaurav Dixit and Pradnyakar Badge
             Created Date: May 19, 2015
         */

        static AesCryptoServiceProvider gDES = new AesCryptoServiceProvider();
        static MD5CryptoServiceProvider gMD5 = new MD5CryptoServiceProvider();

        /// <summary>
        /// Calculate hash value
        /// </summary>
        /// <param name="pValue">Key Value</param>
        /// <returns>Byte Array</returns>
        private static byte[] MD5Hash(string pValue)
        {
            // Compute hash value of key
            return gMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pValue));
        }

        /// <summary>
        /// This method is used for string encryption
        /// </summary>
        /// <param name="pPlainText">Plain Text</param>
        /// <returns>String</returns>
        public static string EncryptString(string pPlainText)
        {
            //This is the highly secure code It should not be change once it in come in practices
            string lKey = "123456";

            //To hash the keys value
            gDES.Key = MD5Hash(lKey);

            //Set Mode to :- The Electronic Codebook (ECB) mode encrypts each block individually
            gDES.Mode = CipherMode.ECB;

            //Break Plain Text into Byte Format
            Byte[] lBuffer = ASCIIEncoding.ASCII.GetBytes(pPlainText);

            //Return ASCII encoded Byte to Encrypted Format
            return Convert.ToBase64String(gDES.CreateEncryptor().TransformFinalBlock(lBuffer, 0, lBuffer.Length));
        }

        /// <summary>
        /// This method is used for string description
        /// </summary>
        /// <param name="pCypherText">Cypher Text</param>
        /// <returns>String</returns>
        public static string DecryptString(string pCypherText)
        {
            //This is the highly secure code It should not be change once it in come in practices
            string lKey = "123456";

            //To hash the keys value
            gDES.Key = MD5Hash(lKey);
            /*Set Mode to :- The Electronic Codebook (ECB) mode encrypts each block individually
             * It is inherited from EncryptString Method  
             */
            gDES.Mode = CipherMode.ECB;

            //Break cypher Text into Byte Format
            Byte[] lBuffer = Convert.FromBase64String(pCypherText);

            //Return ASCII encoded Encrypted Byte to Decrypt Format
            return ASCIIEncoding.ASCII.GetString(gDES.CreateDecryptor().TransformFinalBlock(lBuffer, 0, lBuffer.Length));
        }

        /// <summary>
        /// Combination of MD5 AND SH1 for password encryption
        /// </summary>
        /// <param name="pPlainText">Plain Text</param>
        /// <returns>String</returns>
        public static string EncryptPassword(string pPlainText)
        {
            // Apply SH1 algorithm for string encryption on MD5 encrypted string
            byte[] byteArr = System.Security.Cryptography.HashAlgorithm.Create("SHA1").ComputeHash(System.Text.Encoding.Unicode.GetBytes(EncryptString(pPlainText)));
            //Return Byte array
            return Convert.ToBase64String(byteArr);
        }

        public string[] AuthorizedUserRight(System.Web.HttpServerUtility server, string ApplicationName, long LoginID)
        {
            throw new NotImplementedException();
        }
        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        } //this function Convert to Decord your Password
        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}
