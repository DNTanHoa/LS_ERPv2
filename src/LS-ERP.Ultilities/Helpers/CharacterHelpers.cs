using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Helpers
{
    public class CharacterHelpers
    {
        public static string GetNextCharacter(string currentCharacter)
        {
            var characters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L' , 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

            var inputCharacters = currentCharacter.ToCharArray();

            for(int i = inputCharacters.Count() - 1; i >= 0; i--)
            {
                if(inputCharacters[i] != 'Z')
                {
                    inputCharacters[i] = characters
                        [characters.ToList().IndexOf(inputCharacters[i]) + 1];
                    break;
                }
                else
                {
                    inputCharacters[i] = 'A';
                }
            }

            var result = string.Join("", inputCharacters);

            if (IsContainOnly("A", result))
            {
                result = "A" + result;
            }

            return result;
        }

        public static bool IsContainOnly(string character, string input)
        {
            var inputCharacters = input.Split();

            foreach(var inputCharacter in inputCharacters)
            {
                if(inputCharacter != character)
                {
                    return false;
                }
            }

            return true;
        }

        public static Regex ConvertToUnsignRegex = null;

        public static string ConvertToUnsign(string strInput)
        {
            if (!string.IsNullOrEmpty(strInput))
            {
                if (ReferenceEquals(ConvertToUnsignRegex, null))
                {
                    ConvertToUnsignRegex = new Regex("p{IsCombiningDiacriticalMarks}+");
                }
                var temp = strInput.Normalize(NormalizationForm.FormD);
                return ConvertToUnsignRegex.Replace(temp, string.Empty).Replace("đ", "d").Replace("Đ", "D").ToUpper();
            }
            return "";
        }
    }
}
