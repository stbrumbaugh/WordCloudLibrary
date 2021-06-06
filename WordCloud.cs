using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
// This class was last checked 9-23-20, not tested
namespace Thinker
{
    public class WordCloud
    {
        // words are all lower case so as to not have multiple of the same word with different cases
        public Words Words { get; protected set; } = new Words();
        // number of words in cloud
        public int Count { get => Words.Count; }
        // Term Frequency of each word
        public double[] Tf { get; private set; }
        // Weight of each word if in a collection
        public double[] TfIdf { get; set; }
        // number of words removed from the cloud
        public int WordsRemovedCount { get; protected set; }
        // count of characters in a cloud       
        public int CharCount { get; protected set; }
        // count of digits in a cloud
        public int DigitCount { get; protected set; }
        // count of symbols in a cloud
        public int SymbolCount { get; protected set; }
        
        // makes a word cloud from a string input
        public WordCloud(string input)
        {
            // initialize properties
            CharCount = input.Length;
            DigitCount = 0;
            SymbolCount = 0;
            WordsRemovedCount = 0;
            // string to individually add the words that are in input
            string newWord = "";
            // was the last char looked at a letter? this will indicate when to start a new word
            bool lastCharIsLetter = false;
            // goes through all the characters in a string seperating them into the cloud format
            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    // if the last letter was not a letter start a new word
                    if (!lastCharIsLetter)
                    {
                        // creates a new word that starts witht he character
                        newWord = c.ToString();
                        lastCharIsLetter = true;
                    }
                    else
                    {
                        // adds to the new word sense the last character was a letter
                        newWord += c;
                    }
                }
                // counts the digit and adds the newWord if the last character was a letter
                else if (char.IsDigit(c))
                {
                    if (lastCharIsLetter)
                    {
                        Words.AddWord(newWord.ToLower());
                        lastCharIsLetter = false;
                    }
                    DigitCount++;
                }
                // counts the symbol and adds the newWord if the last character was a letter
                else
                {
                    if (lastCharIsLetter)
                    {
                        Words.AddWord(newWord.ToLower());                       
                    }
                    // don't count spaces tabs or returns
                    if (c != ' ' && c != '\t' && c != '\n')
                    {
                        SymbolCount++;
                    }
                    lastCharIsLetter = false;
                }
            }
            // if the string ended in a letter the last word will need to be added
            if (lastCharIsLetter)
            {
                Words.AddWord(newWord.ToLower());
            }

            CalculateTf();
        }       
        
        private void CalculateTf()
        {
            double[] tf = new double[Count];

            Parallel.For(0, Count, i => 
            {
                double newDouble = Words[i].NumberOfWord;
                newDouble /= Words.Count;

                tf[i] = newDouble;
            });

            Tf = tf;
        }

        public void CalculateTfIdf(double[] idf)
        {
            double[] tfIdf = new double[Count];

            Parallel.For(0, Count, i =>
            {
                double newDouble;
                newDouble = Tf[i] * idf[i];

                tfIdf[i] = newDouble;
            });

            TfIdf = tfIdf;
        }

        private string NewLine(string[] row, int col)
        {
            string newLine = "";
            WordCloud newCloud = new WordCloud(row[col]);

            for (int i = 0; i < row.Length; i++)
            {
                if (i == col)
                {
                    newLine += newCloud.ToString();
                }
                else
                {
                    newLine += row[i];
                }
                if (i != row.Length - 1)
                    newLine += "\t";
            }
            return newLine;
        }
        // method to remove a word from the cloud
        public void RemoveWord(string word)
        {
            // Words.RemoveWord removes the given word and returns the number of that word
            int numRemoved = Words.RemoveWord(word);
            WordsRemovedCount += numRemoved;
        }
        // repeats above method for each word in a string array
        public void RemoveWords(string[] words)
        {
            int numRemoved = Words.RemoveWords(words);
            WordsRemovedCount += numRemoved;
        }

        // TODO: turn the string override into JSON format
        public override string ToString()
        {
            // string starts with the values of each construct
            string returnThis = "{CharCount:" + CharCount + "," + "WordCount:" + Words.Count + "," + "WordsRemoved:" + WordsRemovedCount + "," + "DigitCount:" + DigitCount.ToString() + "," + "SymbolCount:" + SymbolCount.ToString();
            // adds the words and their corresponding count
            returnThis += ",WordsIncluded:" + Words.ToString() + "}";
            return returnThis;
        }
    }
}