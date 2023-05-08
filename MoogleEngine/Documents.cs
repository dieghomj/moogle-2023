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
        private Dictionary<string, int> Vocabulary;

        public static Matrix _TFIDF = new Matrix(0, 0);
        public static Dictionary<string, int> _Vocabulary;
        public static string[] Doc = new string[0];
        public static Vector _IDF = new Vector();
        public static long[] _WordsPerDocument = new long[0];

        public Documents(string path)
        {

            this.path = path;
            int documents = 0;

            this.directory = GetDocuments(this.path);
            this.Vocabulary = GetVocabulary();

            foreach (string file in this.directory) documents++;
            this.documents = documents;

            this.TF = new Matrix(this.documents, this.words);
            this.IDF = new Vector(new double[words]);
            _IDF = new Vector(new double[words]);

            this.ComputeDocuments();

            _TFIDF = this.TF;
            _Vocabulary = this.Vocabulary;
            Doc = this.directory;

        }

        public void ComputeDocuments()
        {

            bool[] IsInDoc = new bool[this.words];
            this.WordsPerDocument = new long[this.documents];

            int document = 0;

            foreach (string file in directory)
            {


                string text = ReadText(file);
                string[] words = GetWords(text);

                Vector tf = CalculateTF(words, Vocabulary);

                for (int i = 0; i < tf.Count; i++) this.WordsPerDocument[document] += (int)tf[i];
                _WordsPerDocument = this.WordsPerDocument;

                for (int i = 0; i < this.words; i++)
                {
                    TF[document, i] = tf[i];
                }

                bool[] idf = WordsInDoc(words, Vocabulary);

                for (int i = 0; i < this.words; i++)
                {
                    if (idf[i]) IDF[i]++;
                }

                document++;

            }


            for (int i = 0; i < IDF.Count; i++)
            {
                _IDF[i] = IDF[i];
            }

            //CalculaIDF
            for (int i = 0; i < IDF.Count; i++)
            {
                this.IDF[i] = CapIDF(Math.Log10((double)(this.documents) / (this.IDF[i])));
            }

            for (int i = 0; i < this.TF.Size.rows; i++)
            {
                Vector currentRow = new Vector(this.TF, i);
                for (int j = 0; j < this.TF.Size.columns; j++)
                {
                    //Calcula TFIDF
                    this.TF[i, j] *= this.IDF[j];
                }
            }

        }

        private Dictionary<string, int> GetVocabulary()
        {

            Dictionary<string, int> Vocabulary = new Dictionary<string, int>();
            int cnt = 0;

            foreach (string file in directory)
            {

                string text = ReadText(file);
                string[] words = GetWords(text);

                foreach (string word in words)
                {
                    if (Vocabulary.ContainsKey(NormalForm(word))) continue;
                    else
                    {

                        Vocabulary.Add(NormalForm(word), cnt++);

                    }
                }
            }

            this.words = cnt;

            return Vocabulary;

        }

        public static Vector CalculateTF(string[] words, Dictionary<string, int> vocabulary)
        {

            double[] A = new double[vocabulary.Count];
            double max = 0;

            foreach (string word in words)
            {
                if (!vocabulary.ContainsKey(NormalForm(word))) continue;
                int wordIndex = vocabulary[NormalForm(word)];
                A[wordIndex]++;
                max = Math.Max(A[wordIndex], max);
            }

            Vector result = new Vector(A);

            foreach (string word in words)
            {
                if (!vocabulary.ContainsKey(NormalForm(word))) continue;
                int wordIndex = vocabulary[NormalForm(word)];
                if (result[wordIndex] < 1) continue;
                result[wordIndex] /= max;
            }

            return result;

        }


        public static bool[] WordsInDoc(string[] words, Dictionary<string, int> vocabulary)
        {

            bool[] IsInDoc = new bool[vocabulary.Count + 1];

            foreach (string word in words)
            {

                if (!IsInDoc[vocabulary[NormalForm(word)]])
                {

                    IsInDoc[vocabulary[NormalForm(word)]] = true;

                }

            }

            return IsInDoc;

        }

        public static string[] GetDocuments(string path)
        {
            return Directory.GetFiles(path, "*.txt");
        }
        public static string ReadText(string path)
        {
            return File.ReadAllText(path, System.Text.Encoding.UTF8);
        }

        public static string NormalForm(string s)
        {
            s = s.ToLower();
            s = Regex.Replace(s.Normalize(System.Text.NormalizationForm.FormD), @"[^a-zA-z0-9]+", "");
            s = Regex.Replace(s, @"[.,;:?!`¨'¡-]?", string.Empty);
            return s;
        }

        public static string[] GetWords(string text)
        {
            char[] separator = { ' ', '\n', '\r', '\t', '\v', '\b', '\f', '_', };
            return text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetTitle(string directory)
        {
            string raw = directory.Remove(directory.Length - 4);
            raw = raw.Remove(0, 11);
            string[] words = GetWords(raw);
            // raw = NormalForm(raw);


            string title = "";
            if (words == null) title = raw;

            foreach (string word in words)
            {
                title += " " + word;
            }

            return title;
        }

        public static double CapIDF(double idf){
            if(idf < Math.Log10(100f/85f))return 0;
            else return idf;
        }

        public static int EditDistance(string s, string t)
        {
            // d es una tabla con m+1 renglones y n+1 columnas
            int costo = 0;
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];

            // Verifica que exista algo que comparar
            if (n == 0) return m;
            if (m == 0) return n;

            // Llena la primera columna y la primera fila.
            for (int i = 0; i <= m; d[i, 0] = i++) ;
            for (int j = 0; j <= n; d[0, j] = j++) ;


            /// recorre la matriz llenando cada unos de los pesos.
            /// i columnas, j renglones
            for (int i = 1; i <= m; i++)
            {
                // recorre para j
                for (int j = 1; j <= n; j++)
                {
                    /// si son iguales en posiciones equidistantes el peso es 0
                    /// de lo contrario el peso suma a uno.
                    costo = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1,  //Eliminacion
                                d[i, j - 1] + 1),                             //Insercion 
                                d[i - 1, j - 1] + costo);                     //Sustitucion
                }
            }
            return d[m,n];
        }
    }
}