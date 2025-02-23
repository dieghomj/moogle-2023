﻿using System.Diagnostics;
using MoogleEngine.Algebra;
namespace MoogleEngine;



public static class Moogle
{
    public static SearchResult Query(string query,bool suggested = false, string NotFound = "") {
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Console.WriteLine("Ansewring query...");

        const int documentsLimit = 10;


        Matrix TFIDF = new Matrix(Documents._TFIDF.Size.rows,Documents._TFIDF.Size.columns);
        Dictionary<string,int> vocabulary = Documents._Vocabulary;
        Vector tfidf = new Vector(new double[vocabulary.Count]);
        Vector idf = new Vector(new double[Documents._IDF.Count]);


        //Initializing TFIDF and idf
        for(int i = 0; i < Documents._TFIDF.Size.rows; i++){
            for(int j = 0; j < Documents._TFIDF.Size.columns; j++){
                TFIDF[i,j] = Documents._TFIDF[i,j]; 
            }
        }

        for(int i = 0; i < idf.Count; i++){
            idf[i] = Documents._IDF[i];
        }
        //+++++++++++++++++++++++++++=

        // Vector tf = new Vector(new double[vocabulary.])


        string[] words = Documents.GetWords(query);

        if(words == null){ // En caso de que la query sea solo una palabra
        words = new string[1];
        words[0] = query;
        }

        string? suggestion = OperatorsAndUtils.Suggestion(words);
        suggestion = suggestion.Remove(0,1);
        // if(suggestion == query)suggestion = null;

        bool[] mk = new bool[vocabulary.Count]; //Para marcar las palabras ya contadas mientras se calcula el IDF


        for(int i = 0; i < words.Length; i++){ // Agrega los terminos de la query al IDF
            string word = words[i];
            string s = Documents.NormalForm(word);
            
            if(!vocabulary.ContainsKey(s))continue;
            if(mk[vocabulary[s]])continue;

            mk[vocabulary[s]] = true;
            idf[vocabulary[s]]++;
            // tfidf[vocabulary[s]]++;

        }

        tfidf = Documents.CalculateTF(words,vocabulary);    
        
        for(int i = 0; i < idf.Count; i++){//Calcula el TF-IDF
            if(idf[i]!=0){
            idf[i] = Math.Log10(Documents.Doc.Length / idf[i]);
            }
            tfidf[i] *= idf[i];
        }

        double mod = Vector.Module(tfidf);//Calcula el modulo del TF-IDF

        double[] Scores = new double[Documents.Doc.Length];//Donde se guardara el score de cada documento
     
        if(mod == 0){// Si el modulo del TF-IDF es 0 significa que ningun termino de la query esta en el corpus, por lo tanto no hay coincidencias
            return Query(suggestion,true,query);
        } 

        int NotRelevant = 0;//Contador para los documentos no relevantes

        for(int i = 0; i < Scores.Length; i++){

            Vector doc = new Vector(TFIDF,i);

            double docMod = Vector.Module(doc);

            Scores[i] = Vector.DotProduct(tfidf,doc)/(docMod*mod);

            if (Scores[i] == 0 || Scores[i] == double.NaN)NotRelevant++;
        }

        TimeSpan ts = stopwatch.Elapsed;
        Console.WriteLine($"({ts.Minutes}m {ts.Seconds}s)This documents should respond to the query({query}): ");
        string[] documents = new string[Documents.Doc.Length];
        for(int i = 0; i < Documents.Doc.Length; i++){
            if(Scores[i] == 0)continue;
            documents[i] = Documents.Doc[i];
            Console.WriteLine(documents[i]);
        }
        Console.WriteLine($"{documents.Length - NotRelevant} documents found");

        Array.Sort(Scores,documents);
        int notFoundMsg = 0;
        if(suggested == true)notFoundMsg = 1;
        SearchItem[] items = new SearchItem[documents.Length + notFoundMsg - NotRelevant];
        Console.WriteLine(items.Length);
        if (items.Length <= 0)
        {
            Console.WriteLine("This is working");
            Array.Resize(ref items, 1);
        }
        
        items[0] = new SearchItem($"Resultados encontrados con {query}", $"No se ha encontrado nada relacionado con {NotFound}", 10);
        
        for (int i = Scores.Length - 1; i >= 0; i--){
            // if(i > items.Length - 1)break;
            if(Scores[i] == 0 || Scores[i] == double.NaN)break;
            items[Scores.Length -1 - i + notFoundMsg ] = new SearchItem(Documents.GetTitle(documents[i]),OperatorsAndUtils.Snippet(words,documents[i]),(float)Scores[i]);
            // if(Scores.Length - 1 - docume ntsLimit == i)break;
        }

        stopwatch.Stop();
        ts = stopwatch.Elapsed;
        Console.WriteLine("Query completly responded in " + ts.Minutes + "m " + ts.Seconds + "s");

        return new SearchResult(items, suggestion);
    }
  
}
