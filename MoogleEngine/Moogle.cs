using MoogleEngine.Algebra;
namespace MoogleEngine;



public static class Moogle
{
    public static SearchResult Query(string query) {
        // Modifique este método para responder a la búsqueda
        
        Matrix TFIDF = Documents._TFIDF;
        Dictionary<string,int> vocabulary = Documents._Vocabulary;
        Vector tfidf = new Vector(new double[vocabulary.Count]);
        Vector idf = Documents._IDF;

        string[] words = query.Split(' ',StringSplitOptions.RemoveEmptyEntries);
        
        if(words == null){
        words = new string[1];
        words[0] = query;
        }

        bool[] mk = new bool[vocabulary.Count];


        for(int i = 0; i < words.Length; i++){
            string word = words[i];
            string s = Documents.NormalForm(word);
            
            if(!vocabulary.ContainsKey(s))continue;
            if(mk[vocabulary[s]])continue;

            mk[vocabulary[s]] = true;
            idf[vocabulary[s]]++;
            tfidf[vocabulary[s]]++;

        }

        double MAX = tfidf.Max();

        for(int i = 0; i < tfidf.Count; i++){
            // tfidf[i] = Documents.NormalizeTF(tfidf[i],MAX);
            idf[i] = Math.Log10((double)(Documents.Doc.Length)/idf[i]);
            tfidf[i] *= idf[i];
        }

        double mod = Vector.Module(tfidf);

        double[] Scores = new double[Documents.Doc.Length];

        int d = 0;


        if(mod == 0){
            SearchItem[] exc = {new SearchItem($"No se pudo encontrar nada relacionado con {query}","lo sentimos",0)};
            return new SearchResult(exc,query);
        } 

        for(int i = 0; i < Scores.Length; i++){

            Vector doc = new Vector(TFIDF,i);

            double docMod = Vector.Module(doc);

            Scores[i] = Vector.DotProduct(tfidf,doc)/(docMod*mod);
            if (Scores[i] == 0 || Scores[i] == double.NaN)d++;
        }

        string[] KeyDocs = new string[Documents.Doc.Length];
        for(int i = 0; i < Documents.Doc.Length; i++){
            if(Scores[i] == 0)continue;
            KeyDocs[i] = Documents.Doc[i];
        }

        Array.Sort(Scores,KeyDocs);
        SearchItem[] items = new SearchItem[KeyDocs.Length - d];

        for(int i = Scores.Length - 1; i >= 0; i--){
            if(Scores[i] == 0 || Scores[i] == double.NaN)break;

            string title = KeyDocs[i].Remove(KeyDocs[i].Length-4);
            title = title.Remove(0,11);

            items[Scores.Length - 1 - i] = new SearchItem(Documents.GetTitle(KeyDocs[i])," lorem ipsum ",(float)Scores[i]);
        }

        return new SearchResult(items, query);
    }

}
