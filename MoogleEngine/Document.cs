namespace MoogleEngine ;
using System ;
using System.IO ;

public class Document
{
    public string route ;
    public Vector Vd ;
    public double score ;

    public Document(string route)
    {
        this.route = route ;
    }

    public void GetScore(Vector Vq , Term[] terms)
    {
        if(CheckIEOperators(terms))
            this.score = Vq * this.Vd ;
        
        else
            this.score = 0.0 ; // no se si es necesario pero por si acaso
    }

    private bool CheckIEOperators(Term[] terms)
    {
        for(int i = 0 ; i < terms.Length ; i ++)
        {
            if(terms[i].Mod.Contains('^') && !this.Vd.v.ContainsKey(terms[i].Text))
                return false ;
            
            else if(terms[i].Mod.Contains('!') && this.Vd.v.ContainsKey(terms[i].Text))
                return false ;
        }
        return true ;
    } 

    public string GetTitle()
    {
        return this.route.Substring(this.route.LastIndexOf("/") + 1);
    }

    public string GetSnippet(Term[] query)
    {
        string pointerword = "" ; // la palabra mas relevalante del doc que se va a buscar para mostrar en el snippet
        string snipe = "" ;  // string que se devuelve 
        
        double maxweigth = 0 ; // para escoger la palabra mas relevante en el doc
        
        for(int i = 0 ; i < query.Length ; i++)
        {
            string word = query[i].Text ;


            if(Vd.v.ContainsKey(word) && Vd[word] > maxweigth )
            {
                maxweigth = Vd[word];
                pointerword = word ;  // se queda con la palabra que mayor tfidf tenga en el doc
            }            
        }    
    
        string[] text = File.ReadAllText(this.route).Split();
        int index = Tools.Find(pointerword , text);

        int end = Math.Min(text.Length - 1 , index + 30);
        for(int i = Math.Max(0, index - 10) ; i < end ; i++)
        {
            snipe += text[i] + " " ;
        }
        return snipe ;
        
    }
}

