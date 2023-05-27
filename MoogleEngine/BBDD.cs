using System ;
using System.IO ;

namespace MoogleEngine ;

public class BBDD
{
    public Document[] docs ;
    static Dictionary <string , Dictionary < int , int > > Content = new Dictionary <string , Dictionary < int , int > >();
    public Vector Vq ;

    public void Load(string dir)
    {
        SearchFiles(dir);
        SetContent();
        CreateVectors();
    }

    public void SearchFiles(string dir)
    {
        /* 
        Busca en el directorio todos los archivos txt y guarda sus direcciones , luego a partir de cada direccion
        crea un objeto tipo documento.  */
        
        dir = Path.Combine(Directory.GetCurrentDirectory() , ".." , dir);
        IEnumerable<string> routes = Directory.EnumerateFiles(dir , "*txt" , SearchOption.AllDirectories);
        this.docs = new Document[routes.Count()] ;
        
        Parallel.For( 0 , docs.Length , i => 
        {
            // por cada ruta se crea un documento
            docs[i] = new Document(routes.ElementAt(i)); 
            });
    }
    

    public void SetContent()
    {
        // Rellena el diccionario Content el cual guarda cada palabra que aparezca en los docs y la relaciona con su freq en cada doc
        // < palabra : [ fq1 , fq2 , fq3 , ..... fqk]  >
        
        for(int i = 0 ; i < docs.Length ; i ++)
        {
            string[] text = Tools.Stdrize(File.ReadAllText(docs[i].route)).Split();

            foreach (string word in text)
            {
                if( !Content.ContainsKey(word) )
                    // si la palabra no se ha encontrado antes la añade al diccionario
                    Content[word] = new Dictionary <int , int>() ; 
                
                if(!Content[word].ContainsKey(i))
                    // si en el documento actual no se habia encontrado antes la palabra anade al diccionario de frecuencias el documento con freq =  0
                    Content[word].Add( i , 0 );
                
                Content[word][i] ++ ; // anade una incidencia de la palabra en el documento actual
            }
        }    
    }
    public void CreateVectors()
    {
        for(int i = 0 ; i < docs.Length ; i++)
        {
            docs[i].Vd = new Vector();

            int maxfq = Tools.MaxFq(Content , i) ; // la freq de la palabra que mas se repite en el doc(para no calcularla varias veces)
            foreach(string word in Content.Keys)
            {
                if(Content[word].ContainsKey(i)) // si el documento contiene la palabra
                {
                    double tf = Content[word][i] / (maxfq + 1.0) ;
                    double idf = Math.Log10( (docs.Length + 1.0) / (Content[word].Count + 1.0) );
                    docs[i].Vd[word] = tf * idf ;
                }
                
            }
        }
    }
    public void ExecuteQuery(Term[] query)
    {
        ReplaceGhostWords(query); 
        CreateVQ(query);          
    
        for(int i = 0 ; i < docs.Length ; i++)
            docs[i].GetScore(Vq , query) ;
    
        Tools.Ordenar(docs);
    }

    private void CreateVQ(Term[] query)
    {
        this.Vq = new Vector();
        int maxfq = Tools.MaxFq(query) ; 

        for(int i = 0 ; i < query.Length ; i++)
        {
            string word = query[i].Text ;
            if(!Vq.v.ContainsKey(word)) //(para no ingresar nuevamente palabras repetidas)
                {
                    int nj = 0 ;

                    if(Content.ContainsKey(word)) nj = (Content[word].Count) ;
                    else nj  = docs.Length ;
                    
                    double tf = query.Count(s => s.Text == word) / (maxfq + 1.0);
                    double idf = Math.Log10( (docs.Length + 1.0) / ( nj + 1.0) );

                    int imp = 1 ;
                    foreach(char c in query[i].Mod)
                    {
                        if (c == '*')   
                            imp ++ ;
                    }
                    
                    
                    Vq[word] = tf * idf * imp ;
                    Console.WriteLine("{0} : {1}" , word , Vq[word]);
                }
        }       
    }

    private void ReplaceGhostWords(Term[] query)
    {
        // este metodo reemplaza las palabras que no aparecen en la bbdd por alguna cercana a ella que si aparezca

        for(int i = 0 ; i < query.Length ; i ++)
        {
            if(! Content.ContainsKey(query[i].Text))
            {
                query[i].Text = ClosestWord(query[i].Text);
            }
        }
    }

    private string ClosestWord(string word)
    {
        // este metodo devuelve la palabra en la base de datos mas cercana a la que se le pasa por parametro

        int minDist = int.MaxValue ; // la menor distancia entre la palabra y alguna en la bbdd
        string output = "" ;         

        foreach(string k in Content.Keys)
        {
            int currentD = Tools.LevDistance(k , word) ; // la distancia con la palabra actual(para no llamar 2 veces al metodo)
            
            if(currentD < minDist)
            {
                minDist = currentD ;
                output = k ;
            }
        }
        Console.WriteLine("{0} : {1}" , output , minDist);
        return output ;
    }
}
