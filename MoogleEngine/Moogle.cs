using MoogleEngine.Algebra;
namespace MoogleEngine;



public static class Moogle
{
    public static SearchResult Query(string query) {
        // Modifique este método para responder a la búsqueda
        
        Matrix TFIDF = Documents.TFIDF;
        Dictionary<string,int> vocabulary = Documents.vocabulary;
        Vector tfidf = new Vector(new double[vocabulary.Count]);
        Vector idf = Documents.idf;

        string[] words = query.Split(' ',StringSplitOptions.RemoveEmptyEntries);
        bool[] mk = new bool[vocabulary.Count];


        foreach(string word in words){
            string s = Documents.NormalForm(word);
            
            if(!vocabulary.ContainsKey(s))continue;
            if(mk[vocabulary[s]])continue;

            mk[vocabulary[s]] = true;
            idf[vocabulary[s]]++;
            tfidf[vocabulary[s]]++;

        }

        double MAX = tfidf.Max();

        for(int i = 0; i < tfidf.Count; i++){
            tfidf[i] = Documents.NormalizeTF(tfidf[i],MAX);
            idf[i] = Math.Log10((double)(Documents.Doc.Length)/idf[i]);
            tfidf[i] *= idf[i];
        }

        double mod = Vector.Module(tfidf);

        double[] Scores = new double[Documents.Doc.Length];

        
        for(int i = 0; i < Scores.Length; i++){

            Vector doc = new Vector(TFIDF,i);

            double docMod = Vector.Module(doc);

            Scores[i] = Vector.DotProduct(tfidf,doc)/docMod*mod;
        }

        Array.Sort(Scores,Documents.Doc);
        SearchItem[] items = new SearchItem[Documents.Doc.Length];

        for(int i = 0; i < Scores.Length; i++){
            if(Scores[i] == 0)continue;
            items[i] = new SearchItem(Documents.Doc[i],"",(float)Scores[i]);
        }


        return new SearchResult(items, query);
    }

}
