using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WFP.ICT.Web.Security
{
    public class AppSecurity
    {
        public static string Encrypt(string raw)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                ICryptoTransform e = GetCryptoTransform(csp, true);
                byte[] inputBuffer = Encoding.UTF8.GetBytes(raw);
                byte[] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

                string encrypted = Convert.ToBase64String(output);

                return encrypted;
            }
        }

        public static string Decrypt(string encrypted)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                var d = GetCryptoTransform(csp, false);
                byte[] output = Convert.FromBase64String(encrypted);
                byte[] decryptedOutput = d.TransformFinalBlock(output, 0, output.Length);

                string decypted = Encoding.UTF8.GetString(decryptedOutput);
                return decypted;
            }
        }

        private static ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            var passWord = "Pass@word1";
            var salt = "S@1tS@1t";

            //a random Init. Vector. just for testing
            String iv = "e675f725e675f725";

            var spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(passWord), Encoding.UTF8.GetBytes(salt), 65536);
            byte[] key = spec.GetBytes(16);

            csp.IV = Encoding.UTF8.GetBytes(iv);
            csp.Key = key;
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }
    }
}