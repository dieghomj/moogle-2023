using MoogleEngine.Algebra;
namespace MoogleEngine;



public static class Moogle
{
    public static SearchResult Query(string query) {
        
        Matrix TFIDF = Documents._TFIDF;
        Dictionary<string,int> vocabulary = Documents._Vocabulary;
        Vector tfidf = new Vector(new double[vocabulary.Count]);
        Vector idf = Documents._IDF;


        string[] words = Documents.GetWords(query);

        if(words == null){ // En caso de que la query sea solo una palabra
        words = new string[1];
        words[0] = query;
        }

        string? suggestion = Suggestion(words);
        if(suggestion == query)suggestion = null;

        bool[] mk = new bool[vocabulary.Count]; //Para marcar las palabras ya contadas mientras se calcula el IDF


        for(int i = 0; i < words.Length; i++){ // Agrega los terminos de la query al TF-IDF
            string word = words[i];
            string s = Documents.NormalForm(word);
            
            if(!vocabulary.ContainsKey(s))continue;
            if(mk[vocabulary[s]])continue;

            mk[vocabulary[s]] = true;
            idf[vocabulary[s]]++;
            tfidf[vocabulary[s]]++;

        }

        for(int i = 0; i < tfidf.Count; i++){//Calcula el TF-IDF
            // tfidf[i] = Documents.NormalizeTF(tfidf[i],MAX);
            idf[i] = Math.Log10((double)(Documents.Doc.Length)/idf[i]);
            tfidf[i] *= idf[i];
        }

        double mod = Vector.Module(tfidf);//Calcula el modulo del TF-IDF

        double[] Scores = new double[Documents.Doc.Length];//Donde se guardara el score de cada documento
     
        if(mod == 0){// Si el modulo del TF-IDF es 0 significa que ningun termino de la query esta en el corpus, por lo tanto no hay coincidencias
            SearchItem[] exc = {new SearchItem($"No se pudo encontrar nada relacionado con {query}","lo sentimos",0)};
            return new SearchResult(exc,suggestion);
        } 

        int NotRelevant = 0;//Contador para los documentos no relevantes

        for(int i = 0; i < Scores.Length; i++){

            Vector doc = new Vector(TFIDF,i);

            double docMod = Vector.Module(doc);

            Scores[i] = Vector.DotProduct(tfidf,doc)/(docMod*mod);

            if (Scores[i] == 0 || Scores[i] == double.NaN)NotRelevant++;
        }

        string[] documents = new string[Documents.Doc.Length];
        for(int i = 0; i < Documents.Doc.Length; i++){
            if(Scores[i] == 0)continue;
            documents[i] = Documents.Doc[i];
        }

        Array.Sort(Scores,documents);
        SearchItem[] items = new SearchItem[documents.Length - NotRelevant];

        for(int i = Scores.Length - 1; i >= 0; i--){
            if(Scores[i] == 0 || Scores[i] == double.NaN)break;

            string title = documents[i].Remove(documents[i].Length-4);
            title = title.Remove(0,11);

            items[Scores.Length - 1 - i] = new SearchItem(Documents.GetTitle(documents[i]),"snippet",(float)Scores[i]);
        }

        return new SearchResult(items, suggestion);
    }

    // private static string Snippet()
    // {
    //     throw new NotImplementedException();
    // }

    private static string Suggestion(string[] query){

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
