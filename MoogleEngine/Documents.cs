using MoogleEngine.Algebra; 
using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public class Documents
    {
        private Matrix Frequency;
        private Matrix InverseD_Frequency;
        private string path; 
        private int documents;
        private int words;
        private Dictionary<string,int> map;
        

        public Documents(string path){

            this.path = path;
            int documents = 0;
            
            this.map = BuildMap();

            foreach( string file in Directory.EnumerateFiles(path,"*.txt"))documents++;
            this.documents = documents;
            
            Frequency = new Matrix(this.documents+1,this.words+1);
            InverseD_Frequency = new Matrix(this.words+1,1);

            this.CalculateTFIDF();

        }

        public void CalculateTFIDF(){

            bool[] mk = new bool[this.words+1];

            foreach( string file in Directory.EnumerateFiles(this.path,"*.txt")){

            int document = 0;

                foreach( var line in File.ReadLines(file,System.Text.Encoding.UTF8)){

                    string s = NormalForm(line);
                    string[] words = s.Split(' ',StringSplitOptions.RemoveEmptyEntries);

                    foreach(string word in words){
                        if(!mk[map[word]]){
                            this.InverseD_Frequency[map[word],0]++;
                            mk[map[word]]=true;
                        }
                        this.Frequency[document,map[word]]++;
                    }

                }
                mk = new bool[this.words+1];
                document++;
            }

            Vector currentRow = new Vector(this.Frequency,0);            

            for(int i = 0; i < this.Frequency.Size.rows; i++){
                
                currentRow = new Vector(this.Frequency,i);

                for(int j = 0; j < this.Frequency.Size.columns; j++){

                    this.InverseD_Frequency[j,0] = Math.Log((double)(this.documents)/(this.InverseD_Frequency[j,0]));
                    
                    this.Frequency[i,j] = 0.5f + 0.5f * (this.Frequency[i,j]/currentRow.MAX);

                    this.Frequency[i,j] *=  this.InverseD_Frequency[j,0];
                }
            }

        }

        private Dictionary<string,int> BuildMap(){

            Dictionary<string,int> map = new Dictionary<string, int>();
            int cnt = 0;

            foreach( string file in Directory.EnumerateFiles(this.path,"*.txt")){
                
                foreach( var line in File.ReadLines(file,System.Text.Encoding.UTF8)){

                    string s = NormalForm(line);
                    string[] words = s.Split(' ',StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach(string word in words){
                        if(map.ContainsKey(word))continue;
                        else {
                    
                            map.Add(word,cnt++);
                            
                        }
                    }
                }
            }

            this.words = cnt;

            return map;

        }

        public  (Matrix,Dictionary<string,int> ) GetDB{
            get{ return (Frequency,map); }
        }

        private string NormalForm(string s){
            s = s.ToLower();
            s = s.Normalize( System.Text.NormalizationForm.FormKD);
            s = Regex.Replace(s,@"[.,;:?!`¨'¡-]?",string.Empty);
            return s;
        }

    }
}