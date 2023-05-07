using MoogleEngine.Algebra;
namespace MoogleEngine;



public static class Moogle
{
    public static SearchResult Query(string query,bool suggested = false, string NotFound = "") {
        
        Matrix TFIDF = Documents._TFIDF;
        Dictionary<string,int> vocabulary = Documents._Vocabulary;
        Vector tfidf = new Vector(new double[vocabulary.Count]);
        Vector idf = Documents._IDF;
        // Vector tf = new Vector(new double[vocabulary.])


        string[] words = Documents.GetWords(query);

        if(words == null){ // En caso de que la query sea solo una palabra
        words = new string[1];
        words[0] = query;
        }

        string? suggestion = Utils.Suggestion(words);
        suggestion = suggestion.Remove(0,1);
        if(suggestion == query)suggestion = null;

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
            idf[i] = Math.Log10((double)(Documents.Doc.Length)/idf[i]);
            tfidf[i] *= idf[i];
        }

        double queryMod = Vector.Module(tfidf);//Calcula el modulo del TF-IDF

        double[] Scores = new double[Documents.Doc.Length];//Donde se guardara el score de cada documento
     
        if(queryMod == 0){// Si el modulo del TF-IDF es 0 significa que ningun termino de la query esta en el corpus, por lo tanto no hay coincidencias
            return Query(suggestion,true,query);
        } 

        int NotRelevant = 0;//Contador para los documentos no relevantes

        for(int i = 0; i < Scores.Length; i++){

            Vector currentDocTFIDF = new Vector(TFIDF,i);

            double docMod = Vector.Module(currentDocTFIDF);

            Scores[i] = Vector.DotProduct(tfidf,currentDocTFIDF)/(docMod*queryMod);

            if (Scores[i] == 0 || Scores[i] == double.NaN)NotRelevant++;
        }


        string[] documents = new string[Documents.Doc.Length];
        for(int i = 0; i < Documents.Doc.Length; i++){
            if(Scores[i] == 0)continue;
            documents[i] = Documents.Doc[i];
        }

        Array.Sort(Scores,documents);
        int notFoundMsg = 0;
        if(suggested == true)notFoundMsg = 1;
        SearchItem[] items = new SearchItem[documents.Length + notFoundMsg - NotRelevant];
        items[0] = new SearchItem($"Resultados encontrados con {query}",$"No se ha encontrado nada relacionado con {NotFound}",10);

        for(int i = Scores.Length - 1; i >= 0; i--){
            if(Scores[i] == 0 || Scores[i] == double.NaN)break;

            items[(Scores.Length -1) - i + notFoundMsg ] = new SearchItem(Documents.GetTitle(documents[i]),Utils.Snippet(),(float)Scores[i]);
        }

        return new SearchResult(items, suggestion);
    }
  
}
