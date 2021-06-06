using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// This class was last checked 9-23-20, tested works of 10/1/20
namespace WordLibrary
{
    public class Words
    {
        // list of words in the collection
        public List<Word> WordList { get; private set; } = new List<Word>();
        private int startIndex = 0; // used to assist with word AlphaIndexes
        // used to assist in making collection alphabetical
        //public const string alphabet = "abcdefghijklmnopqrstuvwxyz"; // Do I need this?
        public int Count { get => (WordList.Count - WordsRemoved); }
        public int WordsRemoved { get; private set; } = 0;
        public Words(){}
        // parses words from a string
        public Words(string input) // change this to a Words.Parse(string input)
        {
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
                        AddWord(newWord.ToLower());
                        lastCharIsLetter = false;
                    }
                }
                // counts the symbol and adds the newWord if the last character was a letter
                else
                {
                    if (lastCharIsLetter)
                    {
                        AddWord(newWord.ToLower());
                    }
                    lastCharIsLetter = false;
                }
            }
            // if the string ended in a letter the last word will need to be added
            if (lastCharIsLetter)
            {
                AddWord(newWord.ToLower());
            }
        }

        // indexer indexer indexer
        public Word this[int index]
        {           
            get
            {
                int found = FindIndex(index);                
                return WordList[found];
            }
        }
        private int FindIndex(int index)
        {
            int returnThis = index;
            int numRemoved = 0;
            Parallel.For(0, (returnThis + 1), i =>
            {
                if (WordList[i].NumberOfWord == 0)
                {
                    numRemoved++;
                }
            });

            if (numRemoved == 0)
                return returnThis;

            for (int i = (returnThis + 1); i < WordList.Count; i++)
            {
                if (WordList[i].NumberOfWord == 0)
                {
                    numRemoved++;
                    if ((i - numRemoved) == index)
                        returnThis += numRemoved;

                    return returnThis;
                }
            }

            return -1;
        }

        // determines what letter comes before the other
        private bool MeFirst(char add, char compare)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < alphabet.Length; i++)
            {
                if (alphabet[i] == add)
                {
                    return true;
                }
                if (alphabet[i] == compare)
                {
                    return false;
                }
            }
            return false;
        }
        // false = -1, true = index
        public int Contains(Word word)
        {
            if (Count == 0)
            {
                return -1;
            }

            bool stopLoop = false;
            int current = startIndex;
            int currentAlpha = 0;
            while (stopLoop == false)
            {
                // the case that they are the same word
                if (word.Spelling == this[current].Spelling)
                {
                    if (this[current].NumberOfWord != 0)
                        return current;
                    else
                        return -1;
                }

                // the case that the word comes before current word
                if (MeFirst(word.Spelling[currentAlpha], this[current].Spelling[currentAlpha]))
                {
                    if (word.Spelling[currentAlpha] == this[current].Spelling[currentAlpha])
                    {
                        currentAlpha++;
                       
                        // the case of a dead end
                        if (this[current].AlphaIndex[currentAlpha] == -1 && this[current].AlphaIndex.Length == currentAlpha + 1)
                        {
                            stopLoop = true;
                        }
                    }
                    // the case that word is between two others
                    else
                    {
                        stopLoop = true;
                    }

                }
                // move to next index
                else if (this[current].AlphaIndex[currentAlpha] != -1)
                {
                    current = this[current].AlphaIndex[currentAlpha];
                }
                // the case of a dead end
                else
                {
                    stopLoop = true;
                }
            }
            // not found
            return -1;
        }

        // adds a word to the collection
        public bool AddWord(Word word)
        {
            if (WordList.Count == 0)
            {
                WordList.Add(word);
                return true;
            }

            int current = startIndex;
            int currentAlpha = 0;
            int previous = 0;
            while (true)
            {
                // the case that they are the same word
                if (word.Spelling == this[current].Spelling)
                {
                    this[current].NumberOfWord++;
                    return true;
                }

                // the case that the word comes before current word
                if (MeFirst(word.Spelling[currentAlpha], this[current].Spelling[currentAlpha]))
                {
                    if (word.Spelling[currentAlpha] == this[current].Spelling[currentAlpha])
                    {
                        currentAlpha++;

                        // case that tha is in front of than; placed here so next loop doesn't error
                        try
                        {
                            int check;
                            check = word.Spelling[currentAlpha];
                        }
                        catch
                        {
                            WordList.Add(word);

                            for (int i = 0; i < this[previous].Spelling.Length; i++)
                            {
                                if (!MeFirst(word.Spelling[i], this[previous].Spelling[i]))
                                {
                                    this[WordList.Count - 1].AlphaIndex[currentAlpha] = this[previous].AlphaIndex[i];
                                    this[previous].AlphaIndex[i] = WordList.Count - 1;
                                    return true;
                                }
                            }
                        }

                        // the case of a dead end
                        if (this[current].AlphaIndex[currentAlpha] == -1 && this[current].AlphaIndex.Length == currentAlpha + 1)
                        {
                            WordList.Add(word);
                            this[current].AlphaIndex[currentAlpha] = WordList.Count - 1;
                            return true;
                        }
                    }
                    // the case that word is between two others
                    else
                    {
                        WordList.Add(word);

                        for (int i = 0; i < this[previous].Spelling.Length; i++)
                        {
                            if (!MeFirst(word.Spelling[i], this[previous].Spelling[i]))
                            {
                                this[WordList.Count - 1].AlphaIndex[currentAlpha] = this[previous].AlphaIndex[i];
                                this[previous].AlphaIndex[i] = WordList.Count - 1;
                                return true;
                            }
                        }
                    }
                    
                }
                else if (this[current].AlphaIndex[currentAlpha] != -1)
                {
                    previous = current;
                    current = this[current].AlphaIndex[currentAlpha];
                }
                // the case of a dead end
                else
                {
                    WordList.Add(word);
                    this[current].AlphaIndex[currentAlpha] = WordList.Count - 1;
                    return true;
                }
            }
        }      
        public void AddWord(string word)
        {
            Word addWord = new Word(word);
            AddWord(addWord);
        }
        // adds a collection of words to the collection
        public void AddWords(string[] words)
        {
            string[] newWords = words;

            // add words not already in collection
            for (int i = 0; i < newWords.Length; i++)
            {
                AddWord(newWords[i]);
            }
        }
        public void AddWords(Words words)
        {
            for (int i = 0; i < words.Count; i++)
            {
                Word newWord = new Word(words[i].Spelling);
                AddWord(newWord);
            }
        }
        // removes a word from the collection
        public int RemoveWord(Word word)
        {
            if (Count == 0)
            {
                return 0;
            }

            bool stopLoop = false;
            int current = startIndex;
            int currentAlpha = 0;
            while (stopLoop == false)
            {
                // the case that they are the same word
                if (word.Spelling == this[current].Spelling)
                {
                    int returnThis = this[current].NumberOfWord;
                    WordsRemoved += returnThis;

                    this[current].NumberOfWord = 0;

                    return returnThis;
                }

                // the case that the word comes before current word
                if (MeFirst(word.Spelling[currentAlpha], this[current].Spelling[currentAlpha]))
                {
                    if (word.Spelling[currentAlpha] == this[current].Spelling[currentAlpha])
                    {
                        currentAlpha++;

                        // the case of a dead end
                        if (this[current].AlphaIndex[currentAlpha] == -1 && this[current].AlphaIndex.Length == currentAlpha + 1)
                        {
                            stopLoop = true;
                        }
                    }
                    // the case that word is between two others
                    else
                    {
                        stopLoop = true;
                    }

                }
                // move to next index
                else if (this[current].AlphaIndex[currentAlpha] != -1)
                {
                    current = this[current].AlphaIndex[currentAlpha];
                }
                // the case of a dead end
                else
                {
                    stopLoop = true;
                }
            }
            // not found so 0 words were removed
            return 0;
        }
        public int RemoveWord(string word)
        {
            Word removeThis = new Word(word);
            return RemoveWord(removeThis);
        }
        // removes a collection of words from the collection, words should all be unique... need a check for this
        public int RemoveWords(string[] words)
        {
            int numberOfWordsRemoved = 0;
            
            Parallel.For(0, words.Length, i =>
            {
                RemoveWord(words[i]);
            });

            return numberOfWordsRemoved;
        }      

        public string[] ToArray()
        {
            string[] returnThis = new string[Count];

            Parallel.For(0, Count, i =>
            {
                returnThis[i] = this[i].Spelling;
            });

            return returnThis;
        }

        public int[] ToArrayCount()
        {
            int[] returnThis = new int[Count];

            Parallel.For(0, Count, i =>
            {
                returnThis[i] = this[i].NumberOfWord;
            });

            return returnThis;
        }

        public override string ToString()
        {
            string returnThis = "{";

            for (int i = 0; i < Count; i++)
            {
                if (WordList[i].NumberOfWord != 0)
                    returnThis += WordList[i].ToString() + ",";               
            }
            // remove the last comma
            returnThis = returnThis.Remove(returnThis.Length - 1);
            returnThis += "}";

            return returnThis;
        }

        public string[] ToStringArray(int wordsPerLine = 10)
        {
            // determine number of rows/strings in the string array
            int wordsPerRow = wordsPerLine;
            int rowCount = Count / wordsPerRow;
            if (rowCount*wordsPerRow < Count)
            {
                rowCount++;
            }
            // make the array
            string[] rows = new string[rowCount];
            // populate the array
            Parallel.For(0, rows.Length, i =>
            {
                int min = i*wordsPerRow;
                int max = min + wordsPerRow;
                if (max > Count)
                {
                    max = Count;
                }

                string newRow = "";
                for (int j = min; j < max; j++)
                {
                    newRow += this[j].ToString() + ",";
                }
                // remove last comma
                newRow = newRow.Remove(newRow.Length - 1);

                rows[i] = newRow;
            });

            return rows;
        }

        public string[] Distribution()
        {
            // contains distribution report line by line
            List<string> distriReport = new List<string>();
            string[] returnThis;
            // NumberOfWord in order
            int[] distriInt = ToArrayCount();
            Array.Sort(distriInt);

            // first group has at least one in that group
            int group = distriInt[0];
            int count = 1;
            int totalWords = 0;

            string columns = "CloudsWithWord" + '\t' + "Words";
            distriReport.Add(columns);

            for (int i = 1; i < distriInt.Length; i++)
            {
                // case that loop has reached the next group
                if (distriInt[i] != distriInt[i - 1])
                {
                    // report previous group count
                    string newLine = group + "\t" + count;
                    distriReport.Add(newLine);

                    totalWords += count;
                    group = distriInt[i];
                    count = 1;

                }
                // case that the loop has reached the end of the last group
                else if (i == (distriInt.Length - 1))
                {
                    count++;
                    // report previous group count
                    string newLine = "CloudsPresent:" + group + "\t" + "Words:" + count;
                    distriReport.Add(newLine);

                    totalWords += count;
                }
                // case that the group has continued therefore count needs to increment
                else
                {
                    count++;
                }

            }

            // add total word count to the end
            string reportCount = "TotalWords:" + totalWords.ToString();
            distriReport.Add(reportCount);

            // return report
            returnThis = distriReport.ToArray();
            return returnThis;
        }

        /*public static Words operator +(Words a, Words b)
        {
            Words addToThis = a;
            int[][] bInAIndex = new int[27][]; // [alphaList][index]

            Parallel.For(0, 27, i =>
            {
                int listCount = b.WordList[i].Count;
                bInAIndex[i] = new int[listCount];
                for (int j = 0; j < listCount; j++)
                {
                    bInAIndex[i][j] = a.Contains(b[i, j].Spelling)[3];
                    // increment the b-word in addToThis if that word exists in a
                    if (bInAIndex[i][j] != -1)
                    {
                        // list index is the same
                        addToThis[i, bInAIndex[i][j]].NumberOfWord += b[i, j].NumberOfWord;
                    }
                }
            });
            // add words not already in collection
            Parallel.For (0, 27, i =>
            {
                for (int j = bInAIndex[i].Length - 1; j >= 0; j--)
                {
                    if (bInAIndex[i][j] == -1)
                    {
                        a.AddWord(b[i, j]);
                    }
                }
            });

            return addToThis;
        }*/
    }
}
