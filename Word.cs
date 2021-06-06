using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Thinker
{
    public class Word
    {
        public string Spelling { get; }
        public int NumberOfWord { get; set; } = 1;
        public int[] AlphaIndex { get; set; }

        public Word(string word)
        {
            Spelling = word;
            AlphaIndex = new int[word.Length + 1];

            for (int i = 0; i < AlphaIndex.Length; i++)
            {
                AlphaIndex[i] = -1;
            }

            //AlphaIndex = CalcAlphaIndex();
        }

        /*private int[] CalcAlphaIndex()
        {
            int[] startIndex = new int[3];

            int firstIndex = alphabet.IndexOf(Spelling[0]);
            // this is the case that the first letter isn't in the english alphabet
            if (firstIndex == -1)
            {
                firstIndex = 26;
            }
            if (Spelling.Length == 1)
            {
                startIndex[0] = firstIndex;
                startIndex[1] = 0;
                startIndex[2] = 0;
                return startIndex;
            }
            int secondIndex;
            secondIndex = alphabet.IndexOf(Spelling[1]);
            // the case that the second letter isnt in the english alphabet
            if (secondIndex == -1)
            {
                secondIndex = 27;
            }
            else
                secondIndex++;
            if (Spelling.Length == 2)
            {
                startIndex[0] = firstIndex;
                startIndex[1] = secondIndex;
                startIndex[2] = 0;
                return startIndex;
            }
            int thirdIndex;
            thirdIndex = alphabet.IndexOf(Spelling[2]);
            // the case that the third letter isnt in the english alphabet
            if (thirdIndex == -1)
            {
                thirdIndex = 27;
            }
            else
                thirdIndex++;
            // gather and return info
            startIndex[0] = firstIndex;
            startIndex[1] = secondIndex;
            startIndex[2] = thirdIndex;

            return startIndex;
        }*/

        public override string ToString()
        {
            return Spelling + ":" + NumberOfWord;
        }

        /*public static Word operator +(Word a, Word b)
        {
            if (b.Spelling != a.Spelling)
            {
                throw new Exception("Can only add like words");
            }
            Word addToThis = a;
            addToThis.NumberOfWord += b.NumberOfWord;

            return addToThis;
        }*/
    }
}
