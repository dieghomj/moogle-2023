# Moogle

![Moogle](moogle.png)
> 1er Proyecto de Programación
> Curso: 2023-2024
> Autor: Diego A. Martínez Jiménez
> Grupo: C121

Simple motor de búsqueda con simple interfaz gráfica.

## Arquitectura del proyecto

Aceptando la mision que se me fue otorgada, ayude en la implementacion de **Moogle**!. Para ello tuve en cuenta la informacion que me pudieron proporcionar acerca de "**TF-IDF**" y "**Algebra lineal**".

Lo primero que implemente fue una clase que nombre `Documents` la cual contiene varios metodos relacionados con operaciones que se le pueden a hacer a documentos, como por ejemplo el metodo `Documents.ReadText()` el cual retorna como string toda el texto de un .txt. Lo mas importante de esta clase es su constructor:

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

Este recibe como parametro `path` que debera ser un string con la direccion de una carpeta donde esten almacenados documentos .txt, _(de no ser asi no garantizo su correcto funcionamiento)_. Al crear una instancia de `Documents` esta asigna a cada termino encontrada en el corpus un numero, (el metodo encargado de este proceso es `Documents.GetVocabulary`) luego el metodo `ComputeDocuments` calcula el TF-IDF de cada documento, creando una matriz donde `TFIDF[i,j]` tiene guardado el TF-IDF de el termino `j` en el documento `i`. Toda la informacion util es almacenada en variables tipo `static` para su uso posterior.

En las clases `Algebra.Vector` y `Algebra.Matrix` estan implementadas las operaciones relacionadas con estos conceptos provenientes del **Algebra Lineal**. Estas son fundamentales para el funcioanmiento de `MoogleEngine.Documents`.

Luego de implementar estas clases, arregle la clase `Moogle` la cual en su momento solo no era muy util.

![Grafico de procesos](Project.png)
>Orden de los procesos del proyecto.
