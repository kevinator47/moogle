using System ;

namespace MoogleEngine ;

public class Term{

    // Propiedades

    string word ;
    string mod = "" ;

    // ------------------------------------------------------------------------------
    public string Text
    {
        get{ return word ; }
        set{ word = value ; }
    }

    public string Mod
    {
        get{ return mod ;}
    }
    // ------------------------------------------------------------------------------
    // Constructor
    public Term(string s)
    {
        // obteniendo los modificadores de la palabra
        foreach (char symb in s )
        {
            if(symb == '*' || symb == '^' || symb == '!')
            {
                this.mod += symb ;
                continue ;
            }
            break ;
        }
        
        if(mod != "")
            s = s.Replace(mod , "") ; // Remueve el operador de la palabra
        
        this.word = s ; // guarda la palabra sin operador
    }
    //------------------------------------------------------------------------------------
    public int ImpOperator()
    {
        int imp = 1 ;
        
        foreach(char c in this.Mod)                
        {
            if (c == '*')                           
                imp ++ ;
        }
    return imp ;
    }
    
}
