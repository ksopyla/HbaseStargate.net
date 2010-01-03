using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HBaseStargate.Helpers
{
    public static class StringExtension
    {
        public static string EncodeToBase64(this string textToEncode)
        {
            byte[] buffer = new UTF8Encoding().GetBytes(textToEncode);
            string encodedString = Convert.ToBase64String(buffer);

            return encodedString;

        }

        public static string DecodeFromBase64(this string encodedText)
        {
            byte[] decodedBytes = Convert.FromBase64String(encodedText);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);

            return decodedText;
        }
    }
}
