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

    public void GetScore(Vector Vq)
    {
        this.score = Vq * this.Vd ;
    }

    public string GetTitle()
    {
        return this.route.Substring(this.route.LastIndexOf("/") + 1);
    }

    public string GetSnippet(string[] query)
    {
        string pointerword = "" ; // la palabra mas relevalante del doc que se va a buscar para mostrar en el snippet
        string snipe = "" ;  // string que se devuelve 
        
        double maxweigth = 0 ; // para escoger la palabra mas relevante en el doc
        
        foreach (string word in query)
        {
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

