using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Helpers
{
    public class SecurityHelpers
    {
        public static string Encrypt(string code, string input)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(code));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(input);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static string SHA256(string data, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(data);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string sbinary = "";
                for (int i = 0; i < hashmessage.Length; i++)
                {
                    sbinary += hashmessage[i].ToString("x2");
                }
                return sbinary;
            }
        }

        public static string SHA256(string input)
        {
            try
            {
                byte[] encData_byte = new byte[input.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(input);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public static string MD5(string input)
        {
            String str = "";
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(input);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            buffer = md5.ComputeHash(buffer);
            foreach (Byte b in buffer)
            {
                str += b.ToString("x2");
            }
            return str;
        }

        public static string Decrypt(string code, string input)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(code));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(input);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        public static bool IsVaidSignature(string requestId, string username, string password, string secret, string signature)
        {
            string raw = requestId + username + MD5(password);
            return SHA256(raw, secret).Equals(signature);
        }

        public static string DecryptMD5(string input)
        {
            try
            {
                var encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecodeByte = Convert.FromBase64String(input);
                int charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                char[] decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                string result = new String(decodedChar);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);
            }
        }
    }
}
