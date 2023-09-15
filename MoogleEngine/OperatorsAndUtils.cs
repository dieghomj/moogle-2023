using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public class OperatorsAndUtils
    {
    public static string Snippet(string[] query, string doc)
    {       
        
        string snippet = "";
        foreach(string s in query){
            // Console.WriteLine(Documents._Vocabulary[s]);
            // Console.WriteLine(Documents._IDF[Documents._Vocabulary[s]]);
            if(!Documents._Vocabulary.ContainsKey(s) || Documents._IDF[Documents._Vocabulary[s]] == 0)
                continue;

            int leftSpace = 200;
            int rightSpace = 300; 
            string text = Documents.ReadText(doc);

            int index = FindWord(s,text);
            if(index != -1){
                if(index - leftSpace < 0)index += leftSpace;
                snippet += " " + GetSubstring(text,index-leftSpace,rightSpace) + "...";
                return snippet;
            }
            
        }
        return snippet;
    }

        private static string GetSubstring(string text, int left, int right)
        {
            string substring = "";

            for(int i = left; i <= left+right; i++){
                if(i + 1 >= text.Length)return substring;
                substring+=text[i];
            }
            return substring;
        }

        private static int FindWord(string word, string text)
        {
            string[] t =  text.Split(" ",StringSplitOptions.None);
            int realIndex = 0;


            for (int i = 0; i < t.Length; i++)
            {
                
                // Console.WriteLine("this is " + t[i]);

                string? s = t[i];
                realIndex += s.Length + 1;
                string w = Documents.NormalForm(s);

                if(word == w){

                // Console.WriteLine(">>>>" + word + "==" + w + "Index: " + i + "Real? Index: " + realIndex);
                    return realIndex;
                }
            }
            return -1;
        }

        public static string Suggestion(string[] query){
        Dictionary<string,int> vocabulary = Documents._Vocabulary;

        string sugg = "";

        for(int i = 0; i < query.Length; i++){
            
            int c = int.MaxValue;
            string temp_sugg = "";
            foreach(string term in vocabulary.Keys){
             
                int edit_distance = Documents.EditDistance(term,Documents.NormalForm(query[i]));
                
                c = Math.Min(edit_distance,c);
                
                if(c == edit_distance){
                    temp_sugg = term;
                }
            
            }
            sugg+= " " + temp_sugg;
        }
        return sugg;

    }
    }
}