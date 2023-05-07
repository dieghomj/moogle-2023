# Moogle

![Moogle](moogle.png)
> 1er Proyecto de Programación
> Curso: 2023-2024
> Autor: Diego A. Martínez Jiménez
> Grupo: C121

Simple motor de búsqueda con simple interfaz gráfica.

## Arquitectura del proyecto

Aceptando la mision que se me fue otorgada, ayude en la implementacion de **Moogle**!. Para ello tuve en cuenta la informacion que me pudieron proporcionar acerca de "**TF-IDF**" y "**Algebra lineal**". Tambien me fue util este link <https://en.wikipedia.org/wiki/Tf%E2%80%93idf>

### **Cargando los documentos**

Lo primero que implemente fue una clase que nombre `Documents` que contiene varios metodos relacionados con operaciones que se le pueden a hacer a documentos, por ejemplo el metodo `Documents.ReadText()` el cual retorna como string toda el texto de un .txt. Lo mas importante de esta clase es su constructor:

```cs
    public Documents(string path){

            this.path = path;
            int documents = 0;
            
            this.directory = GetDocuments(this.path);
            this.Vocabulary = GetVocabulary();

            foreach( string file in this.directory)documents++;
            this.documents = documents;
            
            this.TF = new Matrix(this.documents,this.words);
            this.IDF = new Vector(new double[words]);
            _IDF = new Vector(new double[words]);

            this.ComputeDocuments();

            _TFIDF = this.TF;
            _Vocabulary = this.Vocabulary;
            Doc = this.directory;
    }
```

Este recibe como parametro `path` que debera ser un string con la direccion de una carpeta donde esten almacenados documentos .txt, _(de no ser asi no garantizo su correcto funcionamiento)_. Al crear una instancia de `Documents` esta asigna un numero a cada termino encontrado en el corpus, (el metodo encargado de este proceso es `Documents.GetVocabulary`) luego el metodo `ComputeDocuments` calcula el TF-IDF de cada documento, creando una matriz donde `TFIDF[i,j]` tiene guardado el TF-IDF de el termino `j` en el documento `i`. Toda la informacion util es almacenada en variables tipo `static` para su uso posterior.

En las clases `Algebra.Vector` y `Algebra.Matrix` estan implementados en metodos las operaciones relacionadas con estos conceptos provenientes del **Algebra Lineal**. Estas son fundamentales para el funcioanmiento de `MoogleEngine.Documents`.

### **Respondiendo la query**

Luego de implementar estas clases, arregle la clase `Moogle` la cual en su momento no era muy util. El objetivo principal de esta clase es responder a la query a traves del metodo `Moogle.Query`. La idea para este metodo es modelar un vector en el que cada componente de este, sea el TF-IDF de cada termino que pertenezca al corpus de documentos. Luego hallar el coseno entre este vector y cada uno de los vectores creados a partir de los documentos.

Primero guardo en variables el TF-IDF, el IDF y el vocabulario previamentes calculados al cargar los documentos. 

```cs
    Matrix TFIDF = Documents._TFIDF;
    Vector idf = Documents._IDF;
    Dictionary<string,int> vocabulary = Documents._Vocabulary;
```

Luego calcula el TF-IDF de cada termino a partir de la query:

```cs
    tfidf = Documents.CalculateTF(words,vocabulary);    
    for(int i = 0; i < idf.Count; i++){
        idf[i] = Math.Log10((double)(Documents.Doc.Length)/idf[i]);
        tfidf[i] *= idf[i];
    }
```



![Grafico de procesos](Project.png)
>Orden de los procesos del proyecto.
