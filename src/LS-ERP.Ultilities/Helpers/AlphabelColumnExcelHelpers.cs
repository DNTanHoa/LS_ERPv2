using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Helpers
{
    public class AlphabelColumnExcelHelpers
    {
        public static Dictionary<int, string> DictionaryAlphabet(int range = 1)
        {
            var dicAlphabel = new Dictionary<int, string>();
            int colNumber = 1; // start = 1
            int rangeCol = range; // A,B,C, ..., AA, AB,... BA, BB,... CA, CB... default 26 character
            string[] strAlpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                  "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            for (int i = 0; i <= rangeCol; i++)
            {
                if (i == 0) // range 1 character excel
                {
                    for (int j = 0; j < strAlpha.Count(); j++)
                    {
                        dicAlphabel.Add(colNumber++, strAlpha[j]);
                    }
                }
                else if (i <= 26) // range 2 character excel
                {
                    for (int j = 0; j < strAlpha.Count(); j++)
                    {
                        dicAlphabel.Add(colNumber++, strAlpha[i - 1] + strAlpha[j]);
                    }
                }
            }

            return dicAlphabel;
        }

        /// <summary>
        /// Create dictionary position column
        /// </summary>
        /// <param name="range"></param>
        /// <returns>Dictionary<string, int> with A = 1; B = 2; .... </returns>
        public static Dictionary<string, int> InverseDictionaryAlphabet(int range = 1)
        {
            var dicAlphabel = new Dictionary<string, int>();
            int colNumber = 1; // start = 1
            int rangeCol = range; // A,B,C, ..., AA, AB,... BA, BB,... CA, CB... default 26 character
            string[] strAlpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                  "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            for (int i = 0; i <= rangeCol; i++)
            {
                if (i == 0) // range 1 character excel
                {
                    for (int j = 0; j < strAlpha.Count(); j++)
                    {
                        dicAlphabel.Add(strAlpha[j], colNumber++);
                    }
                }
                else if (i <= 26) // range 2 character excel
                {
                    for (int j = 0; j < strAlpha.Count(); j++)
                    {
                        dicAlphabel.Add(strAlpha[i - 1] + strAlpha[j], colNumber++);
                    }
                }
            }

            return dicAlphabel;
        }
    }
}
