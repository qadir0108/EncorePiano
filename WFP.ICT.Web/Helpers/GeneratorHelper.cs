using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransfocusTabletApp.Helpers
{
    public enum SNKeyLength
    {
        SN16 = 16, SN20 = 20, SN24 = 24, SN28 = 28, SN32 = 32
    }
    public enum SNKeyNumLength
    {
        SN4 = 4, SN8 = 8, SN12 = 12
    }

    public static class GeneratorHelper
    {
        public static string GenerateBarcode(string _prefix, long _localIncrement)
        {
            //return GetSerialKeyAlphaNumaric(SNKeyLength.SN16); // _prefix + "-" +
            return GetSerialKeyWithPrefix(_prefix, _localIncrement, SNKeyLength.SN16);
        }

        private static string GetSerialKeyWithPrefix(string _prefix, long _localIncrement, SNKeyLength sNKeyLength)
        {
            int NumberOfZeros = (int)sNKeyLength - 1 - _prefix.Length;
            string SerialKey = _prefix + _localIncrement.ToString().PadLeft(NumberOfZeros, '0');
            string Checksum = GenerateCheckDigit(SerialKey);
            string SerialKeyFinal = SerialKey + Checksum;
            return SerialKeyFinal;
        }

        private static string AppendSpecifiedStr(int length, string str, char[] newKey)
        {
            string newKeyStr = "";
            int k = 0;
            for (int i = 0; i < length; i++)
            {
                for(k = i; k < 4 + i; k++)
                {
                    newKeyStr += newKey[k];
                }
                if (k == length)
                {
                    break;
                }
                else
                {
                    i = (k) - 1;
                    newKeyStr += str;
                }
            }
            return newKeyStr;
        }
        ///

        /// Generatestandard serial key with alphanumaric format
        ///

        ///the supported length of the serialkey
        /// returns formated serialkey
        public static string GetSerialKeyAlphaNumaric(SNKeyLength keyLength)
        {
            Guid newguid = Guid.NewGuid();
            string randomStr = newguid.ToString("N");
            string tracStr = randomStr.Substring(0, (int)keyLength);
            tracStr = tracStr.ToUpper();
            char[] newKey = tracStr.ToCharArray();
            string newSerialNumber = "";
            switch (keyLength)
            {
                case SNKeyLength.SN16:
                    newSerialNumber = AppendSpecifiedStr(16,"", newKey);
                    break;
                case SNKeyLength.SN20:
                    newSerialNumber = AppendSpecifiedStr(20, "", newKey);
                    break;
                case
                SNKeyLength.SN24:
                    newSerialNumber = AppendSpecifiedStr(24, "", newKey);
                    break;
                case SNKeyLength.SN28:
                    newSerialNumber = AppendSpecifiedStr(28, "", newKey);
                    break;
                case
                SNKeyLength.SN32:
                    newSerialNumber = AppendSpecifiedStr(32, "", newKey);
                    break;
            }

            return newSerialNumber;
        }
        ///

        /// Generate serial key with only numaric
        ///

        /// the supported length of the serial key
        /// returns formated serialkey
        public static string GetSerialKeyNumaric(SNKeyNumLength keyLength)
        {
            Random rn = new Random();
            double sd = Math.Round(rn.NextDouble() * Math.Pow(10, (int)keyLength) + 4);
            return sd.ToString().Substring(0, (int)keyLength);
        }

        public static string GenerateCheckDigit(string cusip)
        {
            int sum = 0;
            char[] digits = cusip.ToUpper().ToCharArray();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ*@#";

            for (int i = 0; i < digits.Length; i++)
            {
                int val;
                if (!int.TryParse(digits[i].ToString(), out val))
                    val = alphabet.IndexOf(digits[i]) + 10;

                if ((i % 2) != 0)
                    val *= 2;

                val = (val % 10) + (val / 10);

                sum += val;
            }

            int check = (10 - (sum % 10)) % 10;

            return check.ToString();
        }

        internal static long GetLocalIncrmentFromBarCode(string _prefix, string BarCode)
        {
            string WithoutPrefix = BarCode.Replace(_prefix, string.Empty);
            string WithoutChecksum = WithoutPrefix.Remove(WithoutPrefix.Length - 1);
            return long.Parse(WithoutChecksum);
        }
    }
}