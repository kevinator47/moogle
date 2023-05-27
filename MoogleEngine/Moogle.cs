namespace MoogleEngine;

public static class Moogle
{
    public static SearchResult Query(string query) {

        Term[] terms = ConvertToTerms(Tools.Stdrize(query).Split());
        Dataserver.BaseDatos.ExecuteQuery(terms);
        
        int max = 5 ;
        for(int i = 0 ; i < max ; i ++)
        {
            if(Dataserver.BaseDatos.docs[i].score == 0 )
            {
                max = i ;
                break ;
            }
        }
        SearchItem[] items = new SearchItem[max] ;
        
        for(int i = 0 ; i < max ; i ++)
        {
            Document actual = Dataserver.BaseDatos.docs[i];
            items[i] = new SearchItem(actual.GetTitle() , actual.GetSnippet(terms) , actual.score); 
            //Console.WriteLine(actual.score);
        }

        string suggestion = "" ;
        for (int i = 0; i < terms.Length; i++)
        {
            suggestion += terms[i].Text + " " ;
        }

        return new SearchResult(items,suggestion);
    }

    public static Term[] ConvertToTerms(string[] words)
    {
        // convierte la query en una lista de terminos
        Term[] output = new Term[words.Length] ;

        for (int i = 0; i < words.Length; i++)
        {
            output[i] = new Term(words[i]);            
        }
 
        return output ;
    }
}
