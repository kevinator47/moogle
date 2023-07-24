class MatrixException : Exception
{
    public MatrixException(string msj) : base(msj){
    }
}

public class Matrix
{
    // clase matriz
    /* Representa una matriz bidimensional con las siguientes operaciones :
        Calcular Determinante          [DONE]
        Multiplicar por un escalar     [DONE]
        Multiplicar por otra matriz    [DONE]
        Sumar matrices                 [DONE]
        Hallar Traspuesta              [DONE]
        Hallar Adjunta                 [DONE]
        Hallar Inversa                 [DONE]
    */

    // Campos
    private double[ , ] matriz ;
    private int m ;                 // cant de filas
    private int n ;                 // cant de columnas

    // -------------------- METODOS --------------------------------------------------
    // Indexer
    public double this[int i , int j]
    {
        get { return matriz[i , j] ; }
        set { matriz[i,j] = value ; }
    }
    // -------------------------------------------------------------------------------------
    // Constructor
    public Matrix( int m , int n , double[] values = null)
    {
        // values representa los valores de la matriz, si el usuario no pasa ningun valor creara una matriz vacia
        this.matriz = new double[ m , n] ;
        this.m = m ;
        this.n = n ;

        if (values != null)
        {
            if(values.Length != m * n)
                throw new MatrixException("size of array does not match matrix dimension") ;
            
            // llenando la matriz
            int ind = 0 ;
            for(int i = 0 ; i < this.m ; i++)
            {
                for (int j = 0; j < this.n ; j++)
                {
                    this[i,j] = values[ind ++] ;
                }
            }
        }
    }
    // --------------------------------------------------------------------------------------------------
    // Determinante
    public double Dt
    {
        // Calcula el determinante de la matriz
        get{
            if(this.m != this.n)  // la matriz debe ser cuadrada para hallar el determinante
                throw new MatrixException("Cannot calculate the determinant of a non-square matrix!") ;
            
            // caso base : matriz de 1x1
            if(n == 1)
            {
               return this.matriz[0,0] ;
            }

            // caso recursivo  : Metodo de menores
            double sum = 0.0 ;
            
            for(int i = 0 ; i < this.m ; i ++)
            {
                if(this[0,i] != 0)
                    sum += this[0,i] * this.Cofactor(0, i) ;
                    // Cofactor es el DETERMINANTE del menor asociado a la posicion a[i,j] mult por -1^(i+j)
            }
            return sum ;
        }            
    }
    // --------------------------------------------------------------------------------
    // Traspuesta
    public Matrix Transpose()
    {
        // Devuelve una matriz donde cada fila de la original se convierte en columna 

        Matrix Trans = new Matrix(this.n , this.m) ;  

        for(int i = 0 ; i < this.m ; i ++)
        {
            for(int j = 0 ; j < this.n ; j ++)
            {
                Trans[j,i] = this[i,j];
            }
        }
        return Trans ;
    }
    // ----------------------------------------------------------------------------------
    // Adjunta
    public Matrix Adj()
    {
        // Devuelve una matriz donde a cada posicion se le asigna el Cofactor asociado a ella
        Matrix Adjunta = new Matrix(this.m , this.n) ;
        for(int i = 0 ; i < this.m ; i ++)
        {
            for(int j = 0 ; j < this.n ; j ++)
            {
                Adjunta[i,j] = Cofactor(i,j);
            }
        }
        return Adjunta ;
    }
    // ----------------------------------------------------------------------------------
    public Matrix Inverse()
    {
        // Halla la inversa de la matriz por el metodo de la adjunta : A' = Adj(At) / |A| 
        double D = this.Dt ;
        if(D == 0)
            throw new MatrixException("Tried to inverse non-inversible matrix , matrix determinant equals to zero") ;
        
        Matrix At = this.Transpose();
        return At.Adj() / D ;
        
        
    }
    // ----------------------------------------------------------------------------------
    // Cofactor
    private double Cofactor(int i , int j)
    {
        double[] menor = new double[ (this.m - 1) * (this.m - 1) ] ; // los valores del menor
        int ind = 0 ; // para ir colocando valores en el array menor
        
        // recorriendo la matriz
        for(int p = 0 ; p < this.m ; p ++)
        {
            for (int q = 0; q < this.n; q++)
            {
                if(p != i && q != j)            // si el elemento no esta en la misma fila o columna 
                    menor[ind ++] = this[p,q];  
            }
        }

        Matrix min = new Matrix( this.m - 1 , this.m - 1 , menor) ; // creando el menor
        
        return Math.Pow(-1 , i + j) * min.Dt ;
    }
    // ---------------------  OPERACIONES   -----------------------------------------------------
    
    public static Matrix operator* (Matrix A , double c)
    {
        // multiplicacion por un escalar
        Matrix Output = new Matrix(A.m , A.n);

        for (int i = 0; i < A.m; i++)
        {
            for (int j = 0; j < A.n; j++)
            {
                Output[i,j] = A[i,j] * c ;        
            }
        }
        return Output ;
    }

    public static Matrix operator/ (Matrix A , double c)
    {
        // division por un escalar
        Matrix Output = new Matrix(A.m , A.n);

        for (int i = 0; i < A.m; i++)
        {
            for (int j = 0; j < A.n; j++)
            {
                Output[i,j] = A[i,j] / c ;        
            }
        }
        return Output ;
    }

    public static Matrix operator+ (Matrix A , Matrix B)
    {
        // suma de dos matrices

        if(A.m != B.m || A.n != B.n) // deben tener la misma dimension
            throw new MatrixException("It was not posible to sum the matrixes due to diference of size between them.");

        Matrix Suma = new Matrix(A.m , A.n);  

        for (int i = 0; i < A.m; i++)
        {
            for (int j = 0; j < B.n; j++)
            {
                Suma[i,j] = A[i,j] + B[i,j] ;  // suma cada elemento posicion a posicion 
            }
        }
        return Suma ;

    }

    public static Matrix operator- (Matrix A , Matrix B)
    {
        // Resta de matrices (lo mismo que suma pero con -)

        if(A.m != B.m || A.n != B.n)
            throw new MatrixException("It was not posible to rest the matrixes due to diference of size between them.");

        Matrix Resta = new Matrix(A.m , A.n);

        for (int i = 0; i < A.m; i++)
        {
            for (int j = 0; j < B.n; j++)
            {
                Resta[i,j] = A[i,j] - B[i,j];
            }
        }
        return Resta ;
    }

    public static Matrix operator* (Matrix A , Matrix B)
    {
        // Multiplicacion de Matrices
        if(A.n != B.m)
            throw new MatrixException("No-valid multiplication , amount of columns in A and rows in B must be equal");
        
        Matrix Mult = new Matrix(A.m , B.n);

        for(int i = 0 ; i < A.m ; i ++)  // iterando sobre cada fila de A
        {
            for (int j = 0; j < B.n ; j++) // iterando sobre cada columna de B
            {
                double suma = 0.0 ;
                for (int x = 0; x < A.n ; x++) // cada elemento en la fila actual de A
                {
                    suma += A[i,x] * B[x,j];    
                }
                Mult[i,j] = suma ;
            }
        }

        return Mult ;
    }


    // -------------------------------------------------------------------------
    public void PrintMatrix()
    {
        Console.WriteLine("------------------------------------");
        for(int i = 0 ; i < this.m ; i ++)
        {
            for (int j = 0; j < this.n; j++)
            {
                Console.Write("{0} " , this[i , j]);
            }
            Console.WriteLine();
        }
        Console.WriteLine("------------------------------------");
    }
}
