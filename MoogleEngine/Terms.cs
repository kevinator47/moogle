using System ;

namespace MoogleEngine ;

public class Term{

    string word ;
    string mod = "" ;

    public Term(string s)
    {
        // obteniendo los modificadores de la palabra
        for (int i = 0; i < s.Length; i++)
        {
            if(s[i] == '^' || s[i] == '!' || s[i] == '*')
            {
                this.mod += s[i] ;
                continue ;
            }
            break ;
        }

        if(mod != "") // remueve el operador de la palabra
            s = s.Replace(mod , "") ;
        
        this.word = s ; // guarda la palabra sin operador
    }

    public string Text
    {
        get{ return word ; }
        set{ word = value ; }
    }

    public string Mod
    {
        get{ return mod ;}
    }
}
