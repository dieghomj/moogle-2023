using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoogleEngine.Algebra;

namespace MoogleEngine
{
    public class TFIDF
    {
        public static Vector CalculateTF(string[] words, Dictionary<string, int> vocabulary)
        {

            double[] A = new double[vocabulary.Count];
            double max = 0;

            foreach (string word in words)
            {
                if (!vocabulary.ContainsKey(Documents.NormalForm(word))) continue;
                int wordIndex = vocabulary[Documents.NormalForm(word)];
                A[wordIndex]++;
                max = Math.Max(A[wordIndex], max);
            }

            Vector result = new Vector(A);

            foreach (string word in words)
            {
                if (!vocabulary.ContainsKey(Documents.NormalForm(word))) continue;
                int wordIndex = vocabulary[Documents.NormalForm(word)];
                if (result[wordIndex] < 1) continue;
                result[wordIndex] /= max;
            }

            return result;

        }

        public static Vector CalculateIDF(Vector idf, int totalDocuments)
        {
            for (int i = 0; i < idf.Count; i++)
           {    
                if(idf[i] == 0)continue;
                idf[i] = TFIDF.CapIDF(Math.Log10((double)totalDocuments / (idf[i])));
            }
            return idf;
        }

        public static double CapIDF(double idf)
        {
            if (idf < Math.Log10(100f / 85f)) return 0;
            else return idf;
        }
    }
}