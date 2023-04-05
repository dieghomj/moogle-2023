namespace MoogleEngine.Algebra
{
    public class Matrix
    {
        double[,] matrix;
        int rows;
        int columns;

        public Matrix(int rows, int columns){
            matrix = new double[rows,columns];
            this.rows = rows;
            this.columns = columns;
        }

        public double this[int row ,int columns]{

            get { return matrix[row,columns]; }
            set { matrix[row,columns] = value; }
        
        }

        public (int rows, int columns) Size{
            get { return (rows,columns); }
        } 

        public void Debug(int x){
            
            for(int j = 0; j < columns; j++){
                System.Console.Write(" " + matrix[x,j] + " ");
            }

        }

    }
}