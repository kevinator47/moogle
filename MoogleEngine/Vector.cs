namespace MoogleEngine ;
using System ;

public class Vector
{
    public Dictionary <string , double > v ;

    public Vector()
    {
        this.v = new Dictionary <string , double>();
    }

    public double this[string j]
    {
        get{
            return v[j];
        }

        set{
            v[j] = value ;
        }
    }

    public static double operator*(Vector Q , Vector D)
    {
        double ProductEsc = EscalarProduct(Q,D);
        double Norms = Q.GetNorma() * D.GetNorma() ;
        
        if(Norms == 0)
            return 0 ;
        
        else 
            return ProductEsc / (Norms) ;           
    }

    public static double EscalarProduct(Vector Q , Vector D)
    {
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