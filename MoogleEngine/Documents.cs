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

        public Documents(string path){

            this.path = path;
            int documents = 0;
            
            this.Vocabulary = GetVocabulary();
            this.directory = ReadDocuments(this.path);

            foreach( string file in this.directory)documents++;
            this.documents = documents;
            
            TF = new Matrix(this.documents+1,this.words+1);
            IDF = new Vector(new double[words+1]);

            this.ComputeDocuments();

            TFIDF = TF;

        }

        public void ComputeDocuments(){

            bool[] IsInDoc = new bool[this.words+1];

            foreach( string file in directory){

                int document = 0;

                string text = ReadText(this.path);
                text = NormalForm(text);

                Vector tf = CalculateTF(text,Vocabulary);

                for(int i = 0; i < this.words+1; i++){
                    TF[document,i] = tf[i];
                }

                bool[] idf = WordsInDoc(text,Vocabulary);

                for(int i = 0; i < words; i++){
                    if(idf[i])IDF[i]++;
                }
            
                document++;
                
            }

            for(int i = 0; i < this.TF.Size.rows; i++){
                
                Vector currentRow = new Vector(this.TF,i);

                for(int j = 0; j < this.TF.Size.columns; j++){
                    //Calcula IDF    
                    this.IDF[j] = Math.Log((double)(this.documents)/(this.IDF[j]));
                    //Normaliza el TF
                    NormalizeTF(TF[i,j],currentRow.MAX);
                    //Calcula TFID
                    this.TF[i,j] *=  this.IDF[j];
                }
            }

        }

        private Dictionary<string,int> GetVocabulary(){

            Dictionary<string,int> Vocabulary = new Dictionary<string, int>();
            int cnt = 0;

            foreach( string file in directory){
                
                string text = ReadText(path);
                
                text = NormalForm(text);

                string[] words = text.Split(' ',StringSplitOptions.RemoveEmptyEntries);
                
                foreach(string word in words){
                    if(Vocabulary.ContainsKey(word))continue;
                    else {
                
                        Vocabulary.Add(word,cnt++);
                        
                    }
                }

            }

            this.words = cnt;

            return Vocabulary;

        }

        public  (Matrix,Dictionary<string,int> ) GetDB{
            get{ return (TF,Vocabulary); }
        }

        public static Vector CalculateTF( string text, Dictionary<string,int> vocabulary){
            
            double[] A = new double[vocabulary.Count+1];
            
            text = NormalForm(text);

            string[] words = text.Split(' ',StringSplitOptions.RemoveEmptyEntries);

            foreach(string word in words){
                A[vocabulary[word]]++;
            }

            return new Vector(A);

        }

        public static double NormalizeTF(double tf, double max){
           return 0.5f + 0.5f * (tf/max);
        }

        public static bool[] WordsInDoc(string text, Dictionary<string,int> vocabulary){
                
            bool[] IsInDoc = new bool[vocabulary.Count+1];
            
            text = NormalForm(text);
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach(string word in words){
                
                if(!IsInDoc[vocabulary[word]]){
                
                    IsInDoc[vocabulary[word]]=true;
                
                }

            }

            return IsInDoc;

        }

        public static Vector CalculateTFIDF(){return new Vector(0);}

        private static string[] ReadDocuments(string path){
            return Directory.GetDirectories(path,"*.txt");
        }
        private static string ReadText(string path){
            return File.ReadAllText(path,System.Text.Encoding.UTF8);
        }

        private static string NormalForm(string s){
            s = s.ToLower();
            s = Regex.Replace(s.Normalize( System.Text.NormalizationForm.FormD),@"[^a-zA-z0-9]+","");
            s = Regex.Replace(s,@"[.,;:?!`¨'¡-]?",string.Empty);
            return s;
        }

    }
}