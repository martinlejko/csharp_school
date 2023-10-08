using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
class Program {
    static void Main(string[] args) {
        
        if (FileErrorChecker(args)){
            SortedDictionary<string, int> dictionaryOfWords = FileReader(args[0]);
            DictionaryPrinter(dictionaryOfWords);
        }
    }

    static bool FileErrorChecker(string[] args){
        try{
            if(args.Length != 1){
                Console.WriteLine("Argument Error");
                return false;
            }else{
                using (StreamReader sr = new StreamReader(args[0])){}
                return true;}
            
        }
        catch(FileNotFoundException){
            Console.WriteLine("File Error");
            return false;
        }
        catch(UnauthorizedAccessException){
            Console.WriteLine("File Error");
            return false;
        }
    }
    static SortedDictionary<string, int> FileReader(string filename){
        char[] separators = new char[] { ' ', '\n', '\t' };
        SortedDictionary<string, int> dictionaryOfWords = new SortedDictionary<string, int> ();

        using (StreamReader sr = new StreamReader(filename)){ //reading one line then processing it and repeate, so we do not run out of memory
            string line;
            while((line = sr.ReadLine()) != null){
                string[] words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach(string word in words){
                    if(dictionaryOfWords.ContainsKey(word)){
                        dictionaryOfWords[word]++;
                    }else{
                        dictionaryOfWords.Add(word, 1);
                    }
                }
            }
        }
        return dictionaryOfWords;
    }

    static void DictionaryPrinter(SortedDictionary<string, int> dictionary){
        foreach(KeyValuePair<string, int> pair in dictionary){
            Console.WriteLine(pair.Key + ": " + pair.Value);
        }
    }
}