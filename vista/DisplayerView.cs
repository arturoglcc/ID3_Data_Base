using MusicApp.Controlador;
using MusicApp.Modelo;

namespace MusicApp.Vista {

using Gtk;
using System;
using System.Diagnostics;  // Para Process
using System.Runtime.InteropServices;  // Para OSPlatform

public class DisplayerView : Grid
{
    private Label labelTitulo;
    private Entry entryTitulo;
    private Entry entryAño;
    private Entry entryGenero;
    private Entry entryPerformer;
    private Entry entryPista;
    private Button botonEditar;
    private Button botonGuardar;
    private Button botonReproducir;

    // Campos adicionales para datos del grupo
    private Entry entryIntegrantes;
    private Entry entryFechaInicio;
    private Entry entryFechaFin;

    // Contenedor principal para controlar la visibilidad
    private Grid gridContainer;
    private DisplayerController displayerController;
    // Ruta del archivo MP3 de la canción seleccionada

    public DisplayerView(DisplayerController displayerController) : base() {

        this.displayerController = displayerController;
        // Inicialización del contenedor principal
        gridContainer = new Grid();
        gridContainer.RowSpacing = 5;
        gridContainer.ColumnSpacing = 10;

        // Campos básicos de la canción
        labelTitulo = new Label("Detalles de la Canción:");
        gridContainer.Attach(labelTitulo, 0, 0, 2, 1);  // Ocupa 2 columnas

        entryTitulo = CrearCampoNoEditable("Título", 1);
        entryAño = CrearCampoNoEditable("Año", 2);
        entryGenero = CrearCampoNoEditable("Género", 3);
        entryPerformer = CrearCampoNoEditable("Performer", 4);
        entryPista = CrearCampoNoEditable("Pista", 5);

        // Campos del grupo (inicialmente ocultos)
        entryIntegrantes = CrearCampoNoEditable("Integrantes del Grupo", 6);
        entryFechaInicio = CrearCampoNoEditable("Fecha de Inicio", 7);
        entryFechaFin = CrearCampoNoEditable("Fecha de Fin", 8);

        // Botón para habilitar la edición
        botonEditar = new Button("Editar");
        gridContainer.Attach(botonEditar, 0, 9, 1, 1);  // Colocado en la fila 9, columna 0
        botonEditar.Clicked += OnEditarClicked;

        // Botón para guardar los cambios
        botonGuardar = new Button("Guardar");
        gridContainer.Attach(botonGuardar, 1, 9, 1, 1);  // Colocado en la fila 9, columna 1
        botonGuardar.Sensitive = false; // Deshabilitado hasta que se haga clic en "Editar"
        botonGuardar.Clicked += OnGuardarClicked;

        // Botón para reproducir la canción
        botonReproducir = new Button("Reproducir");
        this.Attach(botonReproducir, 0, 10, 2, 1);  // Ocupa dos columnas
        botonReproducir.Clicked += OnReproducirClicked;

        // Agregar el contenedor principal al DisplayerView
        this.Attach(gridContainer, 0, 0, 1, 1);

        // Ocultar todos los elementos al inicio
        SetVisibility(false);
    }

    // Método para crear un campo no editable
    private Entry CrearCampoNoEditable(string labelText, int row)
    {
        Label label = new Label(labelText);
        Entry entry = new Entry();
        entry.Sensitive = false;  // No modificable al inicio

        gridContainer.Attach(label, 0, row, 1, 1);  // Colocar la etiqueta en la primera columna
        gridContainer.Attach(entry, 1, row, 1, 1);  // Colocar el campo de texto en la segunda columna
        return entry;
    }

    // Método para actualizar la visibilidad de los elementos
    private void SetVisibility(bool visible) {
        gridContainer.Visible = visible;  // Controla la visibilidad de todos los elementos

        // Controlar la visibilidad específica de los elementos si es necesario
        labelTitulo.Visible = visible;
        entryTitulo.Visible = visible;
        entryAño.Visible = visible;
        entryGenero.Visible = visible;
        entryPerformer.Visible = visible;
        entryPista.Visible = visible;

        // Opcionalmente ocultar los datos del grupo si no son relevantes
        entryIntegrantes.Visible = visible;
        entryFechaInicio.Visible = visible;
        entryFechaFin.Visible = visible;

        botonEditar.Visible = visible;
        botonGuardar.Visible = visible;
    }

    // Evento al hacer clic en "Editar"
    private void OnEditarClicked(object sender, EventArgs e)
    {
        // Habilitar los campos para que sean editables
        entryTitulo.Sensitive = true;
        entryAño.Sensitive = true;
        entryGenero.Sensitive = true;
        entryPerformer.Sensitive = true;
        entryPista.Sensitive = true;

        // Habilitar también los campos del grupo si son visibles
        if (entryIntegrantes.Visible)
        {
            entryIntegrantes.Sensitive = true;
            entryFechaInicio.Sensitive = true;
            entryFechaFin.Sensitive = true;
        }

        // Habilitar el botón de "Guardar"
        botonGuardar.Sensitive = true;
        botonEditar.Sensitive = false;
    }

    private void OnGuardarClicked(object sender, EventArgs e)
        {
        // Rehabilitar el botón "Editar" para que se pueda volver a editar después de guardar
        botonEditar.Sensitive = true;

        // Capturar los datos editados por el usuario
        string nuevoTitulo = entryTitulo.Text;
        string nuevoAño = entryAño.Text;
        string nuevoGenero = entryGenero.Text;
        string nuevoPerformer = entryPerformer.Text;
        string nuevaPista = entryPista.Text;
        string? nuevosIntegrantes = entryIntegrantes.Visible ? entryIntegrantes.Text : null;
        string? nuevaFechaInicio = entryFechaInicio.Visible ? entryFechaInicio.Text : null;
        string? nuevaFechaFin = entryFechaFin.Visible ? entryFechaFin.Text : null;

        // Enviar los datos modificados al controlador para que los procese
        displayerController.EditarCancion(nuevoTitulo,
                                  nuevoAño,
                                  nuevoGenero,
                                  nuevoPerformer,
                                  nuevaPista,
                                  nuevosIntegrantes,
                                  nuevaFechaInicio,
                                  nuevaFechaFin);

        // Deshabilitar el botón "Guardar" y llamar inmediatamente al método NoEditar
        NoEditar(sender, e); // Llamada directa a NoEditar para deshabilitar los campos
    }

    // Método para mostrar los datos de la canción seleccionada
    public void MostrarDatosCancion(Cancion cancion, bool mostrarDatosGrupo) {
        Console.WriteLine($"MostrarDatosCancion llamado para: {cancion.Titulo}");
        // Actualizar los campos con los datos de la canción
        entryTitulo.Text = cancion.Titulo;
        entryAño.Text = cancion.Año.ToString();
        entryGenero.Text = cancion.Genero;
        entryPerformer.Text = cancion.Intérprete;
        entryPista.Text = cancion.Pista.ToString();

        // Si hay datos del grupo, mostrarlos
        if (mostrarDatosGrupo)
        {
            entryIntegrantes.Text = string.Join(", ", cancion.Integrantes ?? new List<string>());
            entryFechaInicio.Text = cancion.FechaInicioGrupo ?? "";
            entryFechaFin.Text = cancion.FechaFinGrupo ?? "";

            entryIntegrantes.Visible = true;
            entryFechaInicio.Visible = true;
            entryFechaFin.Visible = true;
        }
        else
        {
            // Ocultar los campos del grupo si no hay datos
            entryIntegrantes.Visible = false;
            entryFechaInicio.Visible = false;
            entryFechaFin.Visible = false;
        }

        // Hacer visibles todos los campos al seleccionar una canción
        SetVisibility(true);

        // Asegurarse de que el Displayer se muestre automáticamente
        Console.WriteLine($"Mostrando en pantalla los datos de: {cancion.Titulo}");
        this.ShowAll();  // Actualiza y muestra los cambios en la interfaz
    }

        private void NoEditar(object sender, EventArgs e)
    {
        // Habilitar los campos para que sean editables
        entryTitulo.Sensitive = false;
        entryAño.Sensitive = false;
        entryGenero.Sensitive = false;
        entryPerformer.Sensitive = false;
        entryPista.Sensitive = false;

        // Habilitar también los campos del grupo si son visibles
        if (entryIntegrantes.Visible)
        {
            entryIntegrantes.Sensitive = false;
            entryFechaInicio.Sensitive = false;
            entryFechaFin.Sensitive = false;
        }
        botonGuardar.Sensitive = false;
    }

    private void OnReproducirClicked(object sender, EventArgs e)
    {
        displayerController.ReproducirCancion();  // Llamar al método en el controlador
    }
}
}