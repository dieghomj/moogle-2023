#! /usr/bin/bash


# run: ejectutar el proyecto
run() {
    gnome-terminal --quiet -- dotnet watch run --project MoogleServer
}
# report: compilar y generar el pdf del proyecto latex relativo al informe
report(){
    pdflatex -output-directory=Informe Informe/Informe.tex 
    echo "Latex file compiled and pdf generated"
}
# slides: compilar y generar el pdf del proyecto latex relativo a la presentación
slides(){
    pdflatex -output-directory=Presentacion Presentacion/Presentacion.tex
    echo "Latex file compiled and pdf generated"
}
# show_report: ejecutar un programa que permita visualizar el informe, si el fichero en pdf no ha sido generado debe generarse. Esta opción puede recibir como parámetro el comando de la herramienta de visualización que se quiera utilizar, aunque debe tener una por defecto.
show_report(){

    dir=Informe/Informe.pdf

    if test -f $dir; then
        open_pdf "$1" "$dir"
    else
        report
        open_pdf "$1" "$dir"
    fi
}
# show_slides: ejecutar un programa que permita visualizar la presentación, si el fichero en pdf no ha sido generado debe generarse. Esta opción puede recibir puede recibir como parámetro el comando de la herramienta de visualización que se quiera utilizar, aunque debe tener una por defecto.
show_slides(){
    dir=Presentacion/Presentacion.pdf
    if test -f $dir; then
        open_pdf "$1" "$dir"
    else
        slides
        open_pdf "$1" "$dir"
    fi
}

open_pdf(){
    if [ "$1" = $"" ]
    then
        xdg-open $2
        echo "Opening with default PDF viewer"
    else
        $1 $2
    fi
}
# clean: eliminar todos los ficheros auxiliares que no forman parte del contenido del repositorio y se generan en la compilación o ejecución del proyecto, o en la generación de los pdfs del reporte o la presentación
clean(){
    find . -type f -name '*.aux' -delete
    find . -type f -name '*.fdb_latexmk' -delete
    find . -type f -name '*.fls' -delete
    find . -type f -name '*.log' -delete
    find . -type f -name '*.out' -delete
    find . -type f -name '*.synctex.gz' -delete
    find . -type f -name '*.nav' -delete
    find . -type f -name '*.snm' -delete
    find . -type f -name '*.toc' -delete
    find . -type f -name '*.vrb' -delete
    echo "All auxiliary files have been deleted"
}

while read command ; do 

    case $command in 
        run)
            run
            ;;
        report)
            report
            ;;
        slides)
            slides
            ;;
        show_report)
            show_report
            ;;
        show_slides)
            show_slides
            ;;
        show_report*)
            $command
            ;;
        show_slides*)
            $command
            ;;
        clean)
            clean
            ;;
        *)
            echo "invalid command"
esac

#     # echo $command
done


