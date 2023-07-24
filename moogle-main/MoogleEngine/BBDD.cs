using System ;
using System.IO ;

namespace MoogleEngine ;

public class BBDD
{
    // Propiedades
    public Document[] docs ;
    static Dictionary <string , Dictionary < int , int > > Content = new Dictionary <string , Dictionary < int , int > >();
    public Vector Vq ;

    // Metodos

    public void Load(string dir)
    {
        /* Este metodo carga las rutas de los documentos, almacena informacion necesaria para calcular el tf-idf y crea los vectores documento
        (se realiza antes de cargar la interfaz)*/
        
        SearchFiles(dir) ;
        SetContent();
        CreateVectors();
    }

    public void SearchFiles(string dir)
    {
        /* Busca en el directorio todos los archivos txt y guarda sus direcciones , luego a partir de cada direccion
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
    
    public void SetContent() // *
    {
        /* Este Metodo llena el diccionario Content, el cual relaciona cada palabra con otro diccionario que guarda los indices de los documentos y
        la cantidad de veces que aparece la palabra en el(si no aparece no es necesario guardarlo) */
        
        for(int i = 0 ; i < docs.Length ; i ++)
        {
            string[] text = Tools.Stdrize(File.ReadAllText(docs[i].route)).Split(); // un arr con cada palabra del documento estandarizado

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
        // Este metodo calcula el tf-idf de cada palabra en cada documento y crea el vector documento correspondiente 
        
        MaxFq(Content) ;  // obtiene las frecuencias maximas de cada documento y se la asigna a cada uno
        
        Parallel.For(0 , docs.Length , i =>
        {
            docs[i].Vd = new Vector();
            
            foreach(string word in Content.Keys)
            {
                if(Content[word].ContainsKey(i)) // si el documento contiene la palabra
                {
                    double tf = Content[word][i] / (docs[0].maxfq + 1.0) ;
                    double idf = Math.Log10( (docs.Length + 1.0) / (Content[word].Count + 1.0) );
                    docs[i].Vd[word] = tf * idf ;
                }
                
            }
        });
    }

    public void MaxFq(Dictionary<string, Dictionary<int,int> > dict)
    {
        // Halla la mayor aparicion de una palabra en cada documento y la guarda(necesario para el tf)
        foreach (string word in dict.Keys)
        {
            Parallel.ForEach(dict[word].Keys , ind =>
            {
                this.docs[ind].maxfq = Math.Max(dict[word][ind] , this.docs[ind].maxfq) ;
            });
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------
    public void ExecuteQuery(Term[] query)
    {
        ReplaceGhostWords(query); 
        CreateVQ(query);          

        // calcula el score de cada documento
        for(int i = 0 ; i < docs.Length ; i++) 
            docs[i].GetScore(Vq , query) ;
    
        Tools.Ordenar(docs);
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    private void ReplaceGhostWords(Term[] query)
    {
        // este metodo reemplaza las palabras que no aparecen en la bbdd por alguna cercana a ella que si aparezca

        Parallel.For(0 , query.Length , i =>
        {
            if(! Content.ContainsKey(query[i].Text))
            {
                query[i].Text = ClosestWord(query[i].Text);
            }
        });
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
        return output ;
    }
    //----------------------------------------------------------------------------------------------------------------------------

    private void CreateVQ(Term[] query)
    {
        this.Vq = new Vector();
        int maxfq = Tools.MaxFq(query) ; 

        for(int i = 0 ; i < query.Length ; i++)
        {
            string word = query[i].Text ;
            if(Content.ContainsKey(word))
            {      
                double tf = query.Count(s => s.Text == word) / (maxfq + 1.0);
                double idf = Math.Log10( (docs.Length + 1.0) / ( Content[word].Count + 1.0) );

                int imp = query[i].ImpOperator() ;
                        
                Vq[word] = tf * idf * imp ;
            }        
        }       
    }    
    //----------------------------------------------------------------------------------------------------------------------------


}
