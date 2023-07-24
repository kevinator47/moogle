namespace MoogleEngine;

public static class Moogle
{
    public static SearchResult Query(string query) {

        // Convierte la consulta en un array de objetos tipo Term que representan cada palabra estandarizada y con el op guardado aparte
        Term[] terms = Tools.ConvertToTerms(Tools.Stdrize(query).Split());

        Dataserver.BaseDatos.ExecuteQuery(terms);

        int max = Tools.RelevantDocs();
        SearchItem[] items = new SearchItem[max] ;
        
        for(int i = 0 ; i < max ; i ++)
        {
            Document actual = Dataserver.BaseDatos.docs[i];
            items[i] = new SearchItem(actual.GetTitle() , actual.GetSnippet(actual.PointerWord(terms)) , actual.score); 

            Console.WriteLine(actual.GetTitle() + " : " + actual.score.ToString());
        }

        return new SearchResult( items, GetSuggestion(terms));
    }

    //-------------------------------------------------------------------------------------------------------------------

    public static string GetSuggestion(Term[] terms)
    {
        string suggestion = "" ;
        for (int i = 0; i < terms.Length; i++)
        {
            suggestion += terms[i].Text + " " ;
        }
        
        return suggestion ;
    }

    
}
