namespace MoogleEngine.Algebra
{
    public class Vector
    {
        private double[] vector;
        private int count;
        
        private double max;


        public Vector(params double[] vector){
 
            this.vector = vector;
            count = vector.Length;
            // max = this.vector.Max();
        }

        public Vector(Matrix M, int row = -1, int column = -1){
            
            if(row > -1){

                int columns = M.Size.columns;
                count = columns;
                this.vector = new double[columns];
                
                for(int i = 0; i < columns; i++){
                    this.vector[i] =  M[row,i];    
                }

            }
            else if(column > -1){

                int rows = M.Size.rows;
                count = rows;
                this.vector = new double[rows];
                
                for(int i = 0; i < rows; i++){
                    this.vector[i] =  M[i,column];    
                
                }

            }

            max = this.vector.Max();

        }
        
        public double this[int i]{
 
            get{ return vector[i]; }
            set{ vector[i] = value; }
 
        }

        public int Count{
 
            get { return this.count; }
 
        }

        public double MAX{
            get { return this.max;}
        }

        public static double DotProduct(Vector A, Vector B){
            
            double result = 0;

            for(int i = 0; i < A.count; i++){
                result += A[i]*B[i];
            }
        
            return result;
        
        }

        public static double Module(Vector A){
            
            double mod = 0;
            
            for(int i = 0; i < A.count; i++){
                mod += A[i]*A[i];
            }
            
            return (double)Math.Sqrt(mod);
        
        }

        public void Sort(){
            Array.Sort(vector);
        }

        public double Max(){
            
            double max = 0;
            
            for(int i = 0 ; i < this.count; i++){
                max = Math.Max(max,this[i]);
            }

            return max;
        }

        public static double Cosine(Vector A ,Vector B){ 
 
            return DotProduct(A,B) / Module(A) * Module(B);  
 
        }
    }
}