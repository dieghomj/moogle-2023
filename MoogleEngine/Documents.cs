using MoogleEngine.Algebra; 
using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public class Documents
    {
        private Matrix TF;
        private Vector IDF;
        private string path; 
        private string[] directory;
        private int documents;
        private int words;
        private long[] WordsPerDocument;
        private Dictionary<string,int> Vocabulary;

        public static Matrix _TFIDF = new Matrix(0,0);
        public static Dictionary<string,int> _Vocabulary; 
        public static string[] Doc = new string[0];
        public static Vector _IDF = new Vector();
        public static long[] _WordsPerDocument = new long[0];

        public Documents(string path){

            this.path = path;
            int documents = 0;
            
            this.directory = GetDocuments(this.path);
            this.Vocabulary = GetVocabulary();

            foreach( string file in this.directory)documents++;
            this.documents = documents;
            
            this.TF = new Matrix(this.documents,this.words);
            this.IDF = new Vector(new double[words]);
            _IDF = new Vector(new double[words]);

            this.ComputeDocuments();

            _TFIDF = this.TF;
            _Vocabulary = this.Vocabulary;
            Doc = this.directory;

        }

        public void ComputeDocuments(){

            bool[] IsInDoc = new bool[this.words];
            this.WordsPerDocument = new long[this.documents];

            int document = 0;

            foreach( string file in directory){


                string text = ReadText(file);
                string[] words = GetWords(text);
 
                Vector tf = CalculateTF(words,Vocabulary);

                for(int i = 0; i < tf.Count; i++)this.WordsPerDocument[document] += (int)tf[i];
                _WordsPerDocument = this.WordsPerDocument;

                for(int i = 0; i < this.words; i++){
                    TF[document,i] = tf[i];
                }

                bool[] idf = WordsInDoc(words,Vocabulary);

                for(int i = 0; i < this.words ; i++){
                    if(idf[i])IDF[i]++;
                }
            
                document++;
                
            }


            for(int i = 0; i < IDF.Count; i++){
                _IDF[i] = IDF[i];
            }

            //CalculaIDF
            for(int i = 0; i < IDF.Count; i++){
                this.IDF[i] = Math.Log10((double)(this.documents)/(this.IDF[i]));
            }

            for(int i = 0; i < this.TF.Size.rows; i++){
                Vector currentRow = new Vector(this.TF,i);
                for(int j = 0; j < this.TF.Size.columns; j++){
                    //Calcula TFIDF
                    this.TF[i,j] *=  this.IDF[j];
                }
            }

        }

        private Dictionary<string,int> GetVocabulary(){

            Dictionary<string,int> Vocabulary = new Dictionary<string, int>();
            int cnt = 0;

            foreach( string file in directory){
                
                string text = ReadText(file);
                string[] words = GetWords(text);
                
                foreach(string word in words){
                    if(Vocabulary.ContainsKey(NormalForm(word)))continue;
                    else {
                
                        Vocabulary.Add(NormalForm(word),cnt++);
                        
                    }
                }
            }

            this.words = cnt;

            return Vocabulary;

        }

        public static Vector CalculateTF( string[] words, Dictionary<string,int> vocabulary){
            
            double[] A = new double[vocabulary.Count+1];


            foreach(string word in words){
                int wordIndex = vocabulary[NormalForm(word)];
                A[wordIndex]++;
            }

            Vector result = new Vector(A);
            result.Max();

            foreach(string word in words){
                int wordIndex = vocabulary[NormalForm(word)];
                if(result[wordIndex] < 1)continue;
                result[wordIndex] /= result.MAX;
            }

            return result;

        }


        public static bool[] WordsInDoc(string[] words, Dictionary<string,int> vocabulary){
                
            bool[] IsInDoc = new bool[vocabulary.Count+1];

            foreach(string word in words){
                
                if(!IsInDoc[vocabulary[NormalForm(word)]]){
                
                    IsInDoc[vocabulary[NormalForm(word)]]=true;
                
                }

            }

            return IsInDoc;

        }

        public static string[] GetDocuments(string path){
            return Directory.GetFiles(path,"*.txt");
        }
        public static string ReadText(string path){
            return File.ReadAllText(path,System.Text.Encoding.UTF8);
        }

        public static string NormalForm(string s){
            s = s.ToLower();
            s = Regex.Replace(s.Normalize( System.Text.NormalizationForm.FormD),@"[^a-zA-z0-9]+","");
            s = Regex.Replace(s,@"[.,;:?!`¨'¡-]?",string.Empty);
            return s;
        }

        public static string[] GetWords(string text){
            char[] separator = { ' ', '\n', '\r', '\t', '\v', '\b', '\f' ,'_',};
            return text.Split(separator,StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetTitle(string directory){
            string raw = directory.Remove(directory.Length-4);
            raw = raw.Remove(0,11);
            string[] words = GetWords(raw);
            // raw = NormalForm(raw);
            

            string title = "";
            if(words == null)title = raw;

            foreach(string word in words){
                title+= " " + word;
            }

            return title;
        }

        public static int EditDistance(string a, string b){

        int distance = 0;

        for(int i = 0; i < Math.Min(a.Length,b.Length); i++){
            if(a[i] != b[i])distance++;
        }

        distance += Math.Abs(a.Length-b.Length);
        return distance;

    }

    }
}