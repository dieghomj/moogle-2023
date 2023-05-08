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
        private int totalDocuments;
        private int totalWords;
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
            int totalDocuments = 0;

            this.directory = GetDocuments(this.path);
            this.Vocabulary = GetVocabulary();

            foreach (string file in this.directory) totalDocuments++;
            this.totalDocuments = this.directory.Length;
            this.totalDocuments = totalDocuments;

            this.TF = new Matrix(this.totalDocuments, this.totalWords);
            this.IDF = new Vector(new double[totalWords]);
            _IDF = new Vector(new double[totalWords]);

            this.ComputeDocuments();

            _TFIDF = this.TF;
            _Vocabulary = this.Vocabulary;
            Doc = this.directory;

        }

        public void ComputeDocuments()
        {

            bool[] IsInDoc = new bool[this.totalWords];
            this.WordsPerDocument = new long[this.totalDocuments];

            int documentIndex = 0;

            foreach (string file in directory)
            {


                string text = ReadText(file);
                string[] words = GetWords(text);

                Vector tf = TFIDF.CalculateTF(words, Vocabulary);

                for (int i = 0; i < tf.Count; i++) this.WordsPerDocument[documentIndex] += (int)tf[i];
                _WordsPerDocument = this.WordsPerDocument;

                for (int i = 0; i < this.totalWords; i++)
                {
                    TF[documentIndex, i] = tf[i];
                }

                bool[] wordmk = WordsInDoc(words, Vocabulary);

                for (int i = 0; i < this.totalWords; i++)
                {
                    if (wordmk[i]) IDF[i]++;
                }

                documentIndex++;

            }


            for (int i = 0; i < IDF.Count; i++)
            {
                _IDF[i] = IDF[i];
            }

            //CalculaIDF
            this.IDF = TFIDF.CalculateIDF(this.IDF, this.totalDocuments);

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

            this.totalWords = cnt;

            return Vocabulary;

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
        
    }
}