namespace MoogleEngine ;
using System ;
using System.IO ;
using System.Text.RegularExpressions ;

public class Document
{
    // Propiedades
    public string route ;
    public Vector Vd ;
    public double score ;
    public int maxfq ;

    //------------------- CONSTRUCTOR -----------------------------------------------------------------------------
    public Document(string route)
    {
        this.route = route ;
    }

    //------------------- METODOS ----------------------------------------------------------------------------------

    public void GetScore(Vector Vq , Term[] terms)
    {
        // Este metodo devuelve el score de un doc con respecto a la consulta, si fallan los operadores ! o ^ el score sera cero.

        if(CheckIEOperators(terms))
            this.score = Vq * this.Vd ;
        
        else
            this.score = 0.0 ; // no se si es necesario pero por si acaso
    }
    //----------------------------------------------------------------------------------------------------------------------------
    private bool CheckIEOperators(Term[] terms)
    {
        /* Este metodo chequea que se cumpla las condiciones de los operadores ^ y !
        ^ : obliga a que la palabra este , ! : obliga a que la palabra no este */
        
        for(int i = 0 ; i < terms.Length ; i ++)
        {
            if(terms[i].Mod.Contains('^') && ! this.Vd.v.ContainsKey(terms[i].Text))
                return false ;
            
            else if(terms[i].Mod.Contains('!') && this.Vd.v.ContainsKey(terms[i].Text))
                return false ;
        }
        return true ;
    } 
    //-------------------------------------------------------------------------------------------------------
    public string GetTitle()
    {
        return this.route.Substring(this.route.LastIndexOf("/") + 1);
    }
    ///-------------------------------------------------------------------------------------------------------
    public string GetSnippet(string pointer)
    {
        string output = "" ;
        string[] text = (File.ReadAllText(this.route)).Split() ;  
        
        int index = Tools.Find(pointer , text);
        if(index == -1)
            return "No ha sido posible mostrar el snippet." ;
        
        int init = Math.Max(0 , index - 25 );
        int end = Math.Min(text.Length , index + 25);

        for(int i = init ; i < end ; i++)
        {
            output += text[i] + " " ;
        }
        return output ;
    }
    //-----------------------------------------------------------------------------------------------------------------
    public string PointerWord(Term[] query)
    {
        // Este metodo devuelve la palabra mas relevante en el documento
        string output = "" ;
        double currentW = 0.0 ;

        for(int i = 0 ; i < query.Length ; i++)
        {
            if(this.Vd.v.ContainsKey(query[i].Text) && this.Vd[query[i].Text] > currentW)
            {
                currentW = this.Vd[query[i].Text] ;
                output = query[i].Text ;
            }
        }
        return output ;
    }
    //----------------------------------------------------------------------------------------------------        
    
}

