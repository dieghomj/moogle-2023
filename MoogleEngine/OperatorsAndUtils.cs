using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public class OperatorsAndUtils
    {
    public static string Snippet()
    {   return "snippet";
    //     foreach(string word in words){
    //         if(word.Length<4)continue;
    //         if(!index.ContainsKey((i,Documents.NormalForm(word))))continue;
            
    //         int pos = index[(i,Documents.NormalForm(word))];
    //         string snippet = File.ReadAllText(document);
    //         snippet.Remove(pos + 15);
    //         snippet.Remove(0,pos-15);
    //         return snippet;
    //     }   
    //     return "No hay suficiente texto para mostrar";
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