using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
//using MathNet.Numerics;

// This class was last checked 9-23-20, not tested
namespace WordLibrary
{
    public class WordClouds
    {
        // list of clouds to be used by WordClouds methods
        // getters setters
        public List<WordCloud> Clouds { get; private set; } = new List<WordCloud>();
        public Words Words { get; private set; } = new Words();
        public double[] Idf { get; private set; }
        public int Count { get => Clouds.Count; }
        public int WordsRemoved { get; private set; } = 0;

        public WordClouds(){ }
        // downloads words from a text file and converts each row into a cloud
        public WordClouds(string filePath)
        {
            string file = filePath;
            List<Task<WordCloud>> cloudTasks = new List<Task<WordCloud>>();           

            using (StreamReader reader = new StreamReader(file))
            {
                List<WordCloud> newClouds = new List<WordCloud>();
                WordCloud[] wordCloudsArray;

                while (!reader.EndOfStream)
                {
                    // gets a line from the reader
                    string line = reader.ReadLine();
                    // each tasks works on making a line into a cloud
                    cloudTasks.Add(Task.Run(() => new WordCloud(line)));
                    // adds clouds to the cloud collection once they are completed and then removes the taks from the list
                    // doing this to keep memory usage down
                    while (cloudTasks.Count != 0 && cloudTasks[0].IsCompleted)
                    {
                        newClouds.Add(cloudTasks[0].Result);
                        cloudTasks.RemoveAt(0);
                    }
                }

                wordCloudsArray = new WordCloud[cloudTasks.Count];

                // adds the rest of the clouds and they are completed from earliest added
                Parallel.For(0, wordCloudsArray.Length, i =>
                {
                    wordCloudsArray[i] = cloudTasks[i].Result;
                });

                WordCloud[] returnThis = new WordCloud[wordCloudsArray.Length + newClouds.Count];

                Parallel.For(0, wordCloudsArray.Length, i =>
                {
                    returnThis[i] = wordCloudsArray[i];
                });

                Parallel.For(wordCloudsArray.Length, returnThis.Length, i =>
                {
                    returnThis[i] = wordCloudsArray[i];
                });

                Clouds.AddRange(wordCloudsArray);
                
            }
        }
        // indexer
        public WordCloud this[int index]
        {
            get
            {
                return Clouds[index];
            }           
        }
        // adds a new cloud to the collection
        public void AddCloud(WordCloud newCloud)
        {
            Clouds.Add(newCloud);
        }

        // makes a matching text file ending with Cloud.txt with one column transformed into word clouds
        public static void WordCloudsToText(string file, int column, char delimiter)
        {
            string filePath = file;
            int col = column;
            char del = delimiter;

            using (StreamWriter writer = new StreamWriter(filePath + "Cloud.txt"))
            {
                List<Task<string>> tasks = new List<Task<string>>();
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // reads from file and starts the process of converting text to WordClouds
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] row = line.Split(del);

                        tasks.Add(Task.Run(() => NewLine(row, col)));

                        while (tasks.Count !=0 && tasks[0].IsCompleted)
                        {
                            string newLine = "";
                            newLine = tasks[0].Result;
                            // no need to keep that task so time to remove it
                            tasks.RemoveAt(0);
                            // print the string from the completed task
                            writer.WriteLine(newLine);
                        }

                    }
                }
                // for loop starts with the task that has been worked on the longest
                // write the rest of the information to the new file
                for (int i = 0; i < tasks.Count; i++)
                {
                    string newLine = "";
                    newLine = tasks[i].Result;
                    writer.WriteLine(newLine);
                }

            }
        }
        // method for making new lines to be stored in a file
        private static string NewLine(string[] row, int col)
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
        // populates Words construct that represents all the words in the cloud
        public void PopulateWords()
        {
            for (int i = 0; i < Count; i++)
            {
                Words.AddWords(Clouds[i].Words.ToArray());
            }
        }

        public static void PopulateWordsText(string file, int column, char delimiter)
        {
            // contains all the words in the collection of clouds/rows, NumberOfWord is in regards to number of clouds/rows with that word
            Words wordsClouds = new Words();
            string filePath = file;
            // column that contains words to be analyzed
            int col = column;
            char del = delimiter;
            string line;
            string[] row;

            using (StreamReader reader = new StreamReader(filePath))
            {
                List<Task<Words>> tasks = new List<Task<Words>>();
                // use to skip first row
                reader.ReadLine();

                // reads from file and starts the process of converting text to WordClouds
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    row = line.Split(del);
                    // fix this
                    row = row[1].Split('{');
                    string input = "";
                    if (row.Length > 2)
                    {                       
                        input = row[col].Remove(row[col].Length - 2);
                        row = input.Split(',');
                        string[] cloudWords = new string[row.Length];
                        for (int i = 0; i < row.Length; i++)
                        {
                            cloudWords[i] = row[i].Split(':')[0];
                        }
                        wordsClouds.AddWords(cloudWords);
                    }                   

                    /*tasks.Add(Task.Run(() => new Words(input)));

                    while (tasks.Count != 0 && tasks[0].IsCompleted)
                    {
                        wordsClouds.AddWords(tasks[0].Result.ToArray());
                        tasks.RemoveAt(0);
                    }*/
                }
                
                /*for (int i = 0; i < tasks.Count; i++)
                {
                    tasks[i].Wait();
                    wordsClouds.AddWords(tasks[i].Result.ToArray());
                }*/
            }
                
            
            using (StreamWriter writer = new StreamWriter(filePath + "Distribution.txt"))
            {
                // collects distribution report
                string[] distriReport = wordsClouds.Distribution();
                // write report line by line
                for (int i = 0; i < distriReport.Length; i++)
                {
                    writer.WriteLine(distriReport[i]);
                }
                
            }

            using (StreamWriter writer = new StreamWriter(filePath + "CloudsWords.txt"))
            {
                string[] wordsReport = wordsClouds.ToStringArray();
                for (int i = 0; i < wordsReport.Length; i++)
                {
                    writer.WriteLine(wordsReport[i]);
                }              
            }
        }
        // returns an array of the most common words based on the percentage that word is in the different clouds
        /*public string[] CommonWords(byte percent)
        {
            if (percent > 100)
            {
                throw new Exception("percent must be under 100");
            }

            Words commonWords = new Words();
            // number of clouds in object
            int numClouds = Clouds.Count;
            // a word needs to appear in this many clouds to be considered common
            int minCount = numClouds * percent / 100;
            // a word can't be missing from this number of clouds to be considered common
            int maxMissing = numClouds - minCount;
            // index of word to be incremented (count)
            //int index;

            // stop adding potential commond words after 1 - percent clouds (maxMissing) have been looked at
            // TODO: once a word hits max missing drop that word

            // adds all the potential common words given that the word must appear at or before maxMissing clouds
            for (int i = 0; i < maxMissing; i++)
            {
                commonWords.AddWords(Clouds[i].Words.ToArray());         
            }
            // increments potential words if they are found in another cloud
            for (int i = maxMissing; i < numClouds; i++)
            {
                int[][] wordIndex = new int[Clouds[i].Count][];
                Parallel.For(0, Clouds[i].Count, j =>
                {
                    string[] getIndexOfThese = Clouds[i].Words.ToArray();
                    wordIndex[j] = commonWords.Contains(Clouds[i].Words[j].Spelling);
                });

                for (int w = 0; w < wordIndex.Length; w++)
                {
                    //index = commonWords.Contains(Clouds[i].Words[w]);
                    if (wordIndex[w][3] != -1)
                        commonWords[wordIndex[w][0], wordIndex[w][3]].NumberOfWord++;
                }
            }
            // removes words from the list that did not make the minimum count
            // must go from highest to lowest so as not to mess up the index
            Parallel.For(0, 27, i =>
            {
                for (int w = commonWords.WordList[i].Count - 1; w >= 0; w--)
                {
                    if (commonWords[i, w].NumberOfWord < minCount)
                    {

                    }
                }
            });
            /*for (int i = commonWords.Count - 1; i >= 0; i--)
            {
                if (commonWords[i].NumberOfWord < minCount)
                {
                    commonWords.RemoveWordAt(i);
                }
            }*/
            // convert list to array to return
            /*string[] commWords = commonWords.ToArray();
            return commWords;
        }*/
        // returns an array of the most rare words based on 1 / rarity
        /*public string[] RareWords(int rarity)
        {
            Words rareWords = new Words();
            int maxCount = Count / rarity;
            // add all words
            for (int i = 0; i < Count; i++)
            {
                rareWords.AddWords(Clouds[i].Words.ToArray());
            }
            // remove words that are too common
            for (int a = 0; a < 27; a++)
            {
                for (int i = rareWords.WordList[a].Count - 1; i >= 0; i--)
                {
                    if (rareWords[a,i].NumberOfWord > maxCount)
                    {
                        int[] containsIndex = rareWords.GetStartIndex(rareWords[a, i].Spelling);
                        containsIndex[3] = i;

                        rareWords.RemoveWordAt(containsIndex);
                    }
                }
            }
            
            // return the result
            string[] returnThis;
            returnThis = rareWords.ToArray();
            return returnThis;
        }*/

        /*public void RemoveUniqueWords(int minimum)
        {
            // must start from the end of the list to the beginning since index will change apon removal
            for (int i = Words.Count - 1; i >= 0; i--)
            {
                if (Words.NumberOfWord[i] < minimum)
                {
                    Words.RemoveWordAt(i);
                }
            }
        }*/

        public void RemoveWords(string[] removeThese)
        {
            string[] remove = removeThese;
            Parallel.For(0, Count, i =>
            {
                Clouds[i].RemoveWords(remove);
            });

        }

        public void RemoveWord(string removeThis)
        {
            string word = removeThis;
            Parallel.For(0, Count, i =>
            {
                Clouds[i].RemoveWord(word);
            });

        }

        // populates the Idf array
        public void CalculateIdf(double lowerbound, double upperbound)
        {
            double lower = lowerbound * Count;
            double upper = upperbound * Count;

            // IDF = log ( Number of Clouds / Number of Clouds with Word)
            double[] idfArray = new double[Words.Count];

            Parallel.For(0, Words.Count, i =>
            {
                double input = Clouds.Count;
                                
                if (Words[i].NumberOfWord < lower || Words[i].NumberOfWord > upper)
                {
                    idfArray[i] = 0;
                }
                else
                {
                    input /= Words[i].NumberOfWord;
                    idfArray[i] = Math.Log(input);
                }
            });

            Idf = idfArray;
        }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
        public void CalculateTfIdf()
        {            
            for (int i = 0; i < Count; i++)
            {
                
                double[] idfArray = new double[Clouds[i].Count];
                Parallel.For(0, Clouds[i].Count, j =>
                {
                    double idf;
                    int index;

                    Word word = Clouds[i].Words[j];
                    index = Words.Contains(word);
                    if (index != -1)
                    {
                        idf = Idf[index];
                    }
                    // the case that the index is -1, word was most likely removed due to being too unique for comparisons
                    else
                    {
                        idf = 0;
                    }

                    idfArray[j] = idf;
                });
                // give the cloud the idfArray so that it can calc TfIdf
                Clouds[i].CalculateTfIdf(idfArray);
            }
        }

        public override string ToString()
        {
            string returnThis = "";

            for (int i = 0; i < Count; i++)
            {
                returnThis += Clouds[i].ToString();
            }

            return returnThis;
        }
    }
}
