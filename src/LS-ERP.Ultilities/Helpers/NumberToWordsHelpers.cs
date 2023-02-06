using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Helpers
{
    public class NumberToWordsHelpers
    {
        public static string ToVerbalCurrency(double value, string extend = "Cents")
        {
            var valueString = value.ToString("N2");
            var decimalString = valueString.Substring(valueString.LastIndexOf('.') + 1);
            var wholeString = valueString.Substring(0, valueString.LastIndexOf('.'));

            var valueArray = wholeString.Split(',');

            var unitsMap = new[] { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            var placeMap = new[] { "", " Thousand ", " Million ", " Billion ", " Trillion " };

            var outList = new List<string>();

            var placeIndex = 0;

            for (int i = valueArray.Length - 1; i >= 0; i--)
            {
                var intValue = int.Parse(valueArray[i]);
                var tensValue = intValue % 100;

                var tensString = string.Empty;
                if (tensValue < unitsMap.Length)
                {
                    tensString = unitsMap[tensValue];
                }
                else
                {
                    tensString = tensMap[(tensValue - tensValue % 10) / 10] + " " + unitsMap[tensValue % 10];
                }

                var fullValue = string.Empty;
                if (intValue >= 100)
                {
                    fullValue = unitsMap[(intValue - intValue % 100) / 100] + " Hundred " + tensString + placeMap[placeIndex++];
                }
                else if (intValue != 0)
                {
                    fullValue = tensString + placeMap[placeIndex++];
                }
                else
                    placeIndex++;

                outList.Add(fullValue);
            }

            var intCentsValue = int.Parse(decimalString);

            var centsString = string.Empty;
            if (intCentsValue < unitsMap.Length)
            {
                centsString = unitsMap[intCentsValue];
            }
            else
            {
                centsString = tensMap[(intCentsValue - intCentsValue % 10) / 10] + " " + unitsMap[intCentsValue % 10];
            }

            if (intCentsValue == 0)
            {
                centsString = "Zero";
            }

            var output = string.Empty;
            for (int i = outList.Count - 1; i >= 0; i--)
            {
                output += outList[i]; 
            }
            output += (extend == "Cents" ? (" And " + centsString) : "") + " " + extend + " Only";

            return Regex.Replace(output, @"\s+", " ").ToUpper();
        }
    }
}
