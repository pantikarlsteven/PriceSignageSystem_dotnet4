using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Helper
{
    public static class BarcodeHelper
    {
        public static string GetNormalizedUPC_E(string input)
        {
            if (input.Length == 8)
                // Remove the first character
                input = input.Substring(1);

            // Get the first 2 characters
            string firstTwo = input.Substring(0, 2);

            // Add five zeros
            string withZeros = firstTwo + "00000";

            // Get the next 5 characters
            string nextFive = input.Substring(2, 5);

            // Remove the fourth character from "64904"
            string result = withZeros + nextFive.Remove(3, 1);

            return result;
        }

        public static string GetNormalizedUPC_A(string input)
        {
            return input.TrimStart('0');
        }
    }
}