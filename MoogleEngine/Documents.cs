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
        private Dictionary<string,int> Vocabulary;
        
        public static Matrix TFIDF;
        public static Dictionary<string,int> vocabulary; 
        public static string[] Doc;
        public static Vector idf;

        public Documents(string path){

            this.path = path;
            int documents = 0;
            
            this.directory = ReadDocuments(this.path);
            this.Vocabulary = GetVocabulary();

            foreach( string file in this.directory)documents++;
            this.documents = documents;
            
            this.TF = new Matrix(this.documents+1,this.words+1);
            this.IDF = new Vector(new double[words+1]);

            this.ComputeDocuments();

            TFIDF = this.TF;
            vocabulary = this.Vocabulary;
            Doc = this.directory;

        }

        public void ComputeDocuments(){

            bool[] IsInDoc = new bool[this.words+1];

            int document = 0;

            foreach( string file in directory){


                string text = ReadText(file);
                string[] words = GetWords(text);
 
                Vector tf = CalculateTF(words,Vocabulary);

                for(int i = 0; i < this.words+1; i++){
                    TF[document,i] = tf[i];
                }

                bool[] idf = WordsInDoc(words,Vocabulary);

                for(int i = 0; i < this.words + 1; i++){
                    if(idf[i])IDF[i]++;
                }
            
                document++;
                
            }

            idf = this.IDF;

            //CalculaIDF
            for(int i = 0; i < IDF.Count; i++){
                this.IDF[i] = Math.Log10((double)(this.documents)/(this.IDF[i]));
            }

            for(int i = 0; i < this.TF.Size.rows; i++){
                Vector currentRow = new Vector(this.TF,i);
                for(int j = 0; j < this.TF.Size.columns; j++){
                    //Normaliza el TF
                    this.TF[i,j] = NormalizeTF(TF[i,j],currentRow.MAX);
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

        public  (Matrix,Dictionary<string,int> ) GetDB{
            get{ return (TF,Vocabulary); }
        }

        public static Vector CalculateTF( string[] words, Dictionary<string,int> vocabulary){
            
            double[] A = new double[vocabulary.Count+1];

            foreach(string word in words){
                A[vocabulary[NormalForm(word)]]++;
            }

            return new Vector(A);

        }

        public static double NormalizeTF(double tf, double max){
           return 0.5f + 0.5f * (tf/max);
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

        private static string[] ReadDocuments(string path){
            return Directory.GetFiles(path,"*.txt");
        }
        private static string ReadText(string path){
            return File.ReadAllText(path,System.Text.Encoding.UTF8);
        }

        public static string NormalForm(string s){
            s = s.ToLower();
            s = Regex.Replace(s.Normalize( System.Text.NormalizationForm.FormD),@"[^a-zA-z0-9]+","");
            s = Regex.Replace(s,@"[.,;:?!`¨'¡-]?",string.Empty);
            return s;
        }

        private static string[] GetWords(string text){
            char[] separator = { ' ', '\n', '\r', '\t', '\v', '\b', '\f'};
            return text.Split(separator,StringSplitOptions.RemoveEmptyEntries);
        }

    }
}