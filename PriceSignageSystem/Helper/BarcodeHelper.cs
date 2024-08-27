using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Helper
{
    public static class BarcodeHelper
    {
        public static string ConvertUPCEToUPCA(string upcE)
        {
            char ch = upcE[6];
            char ch2 = upcE[7];
            string str = "";
            if (ch == '0')
            {
                str = upcE.Substring(1, 2) + "00000" + upcE.Substring(3, 3) + ch2.ToString();
            }
            else if (ch == '1')
            {
                str = upcE.Substring(1, 3) + "10000" + upcE.Substring(3, 3) + ch2.ToString();
            }
            else if (ch == '2')
            {
                str = upcE.Substring(1, 3) + "20000" + upcE.Substring(3, 3) + ch2.ToString();
            }
            else if (ch == '3')
            {
                str = upcE.Substring(1, 3) + "00000" + upcE.Substring(4, 2) + ch2.ToString();
            }
            else if (ch == '4')
            {
                str = upcE.Substring(1, 4) + "00000" + upcE.Substring(5, 1) + ch2.ToString();
            }
            else if (ch == '5')
            {
                str = upcE.Substring(1, 5) + "00005" + ch2.ToString();
            }
            else if (ch == '6')
            {
                str = upcE.Substring(1, 5) + "00006" + ch2.ToString();
            }
            else if (ch == '7')
            {
                str = upcE.Substring(1, 5) + "00007" + ch2.ToString();
            }
            else if (ch == '8')
            {
                str = upcE.Substring(1, 5) + "00008" + ch2.ToString();
            }
            else if (ch == '9')
            {
                str = upcE.Substring(1, 5) + "00009" + ch2.ToString();
            }
            return str;
        }

        public static string GetNormalizedUPC_A(string input)
        {
            char[] trimChars = new char[] { '0' };
            return input.TrimStart(trimChars);
        }

        public static string GetNormalizedUPC_E(string input)
        {
            if (input.Length == 8)
            {
                input = input.Substring(1);
            }
            return (input.Substring(0, 2) + "00000" + input.Substring(2, 5).Remove(3, 1));
        }

        public static string InHouseUPC(string input)
        {
            string str = string.Empty;
            if (input[0] == '0')
            {
                str = input.Substring(0, input.Length - 1);
            }
            else if (input.Substring(0, 3) == "270")
            {
                str = input.Substring(3, 4);
            }
            return str;
        }
    }
}