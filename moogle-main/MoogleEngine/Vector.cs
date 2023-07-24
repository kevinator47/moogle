namespace MoogleEngine ;
using System ;

public class Vector
{
    // Propiedades
    public Dictionary <string , double > v ;


    // ---------------------------- CONSTRUCTOR -----------------------------------------------------
    public Vector()
    {
        this.v = new Dictionary <string , double>();
    }

    
    //------------------------------ INDEXER ----------------------------------------------------------
    public double this[string j]
    {
        get{
            return v[j];
        }

        set{
            v[j] = value ;
        }
    }
    //--------------------------------------------------------------------------------------------------------
    public static double operator*(Vector Q , Vector D)
    {
        // Este metodo halla la similitud del coseno entre dos vectores(el score)
        
        double Norms = Q.GetNorma() * D.GetNorma() ;

        if(Norms == 0) // por si se tiene un documento(o consulta vacio) evitar division por cero
            return 0 ;
        
        else 
            return EscalarProduct(Q , D) / Norms ;           
    }
    //-----------------------------------------------------------------------------------------------------------------
    public static double EscalarProduct(Vector Q , Vector D)
    {
        /* Halla el producto escalar de dos vectores(multiplicando los terminos con el mismo indice) , en este caso
        los indices son las palabras */
        
        double suma = 0.0 ;
        foreach(string word in Q.v.Keys)
        {
            if(D.v.ContainsKey(word))
                suma += Q[word] * D[word];
        }
        return suma ;
    }

    public double GetNorma()
    {
        double suma = 0.0 ;
        foreach(double weigth in this.v.Values)
        {
            suma += Math.Pow(weigth,2);
        }
        return Math.Sqrt(suma);
    }
}