using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WordLibrary
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
