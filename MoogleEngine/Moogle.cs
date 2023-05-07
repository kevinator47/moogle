namespace MoogleEngine;

public static class Moogle
{
    public static SearchResult Query(string query) {

        string[] terms = Tools.Stdrize(query).Split();
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
            Console.WriteLine(actual.score);
        }

        return new SearchResult(items,query);
    }
}
