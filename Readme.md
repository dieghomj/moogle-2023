# Moogle

![Moogle](moogle.png)
> 1er Proyecto de Programación
> Curso: 2023-2024
> Autor: Diego A. Martínez Jiménez
> Grupo: C121

Simple motor de búsqueda con simple interfaz gráfica.

## Arquitectura del proyecto

Aceptando la mision que se me fue otorgada ayude en la implementacion de Moogle!. Para ello tuve en cuenta informacion dada acerca de "TF-IDF" y "Algebra lineal" que me pudieron proporcionar.

Lo primero que implemente fue una clase llamada `MoogleEngine.Documents` la cual proporciona varios metodos relacionados con operaciones que se le pueden a hacer a documentos, como por ejemplo el metodo `MoogleEngine.Documents.ReadText()` el cual retorna como string toda la texto de un .txt. Lo mas importante de esta clase es su constructor:

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
Este recibe como parametro un string que debera ser la direccion de una carpeta donde esten almacenados documentos .txt, _(de no ser asi no garantizo su correcto funcionamiento)_. Al crear una instancia de `MoogleEngine.Documents` esta asigna a cada palabra encontrada en el corpus un numero, (el metodo encargado de este proceso es `MoogleEngine.Documents.GetVocabulary`) 

![Algoritmo de Busqueda](Project.png)
>Orden de los procesos del proyecto.


