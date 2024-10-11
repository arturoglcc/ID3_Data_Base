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
    private Button botonEsGrupo;
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
    private Image imagePortada;
    private Cancion cancion = null!;

    public DisplayerView(DisplayerController displayerController) : base() 
  {
    this.displayerController = displayerController;

    // Inicialización del contenedor principal
    gridContainer = new Grid();
    gridContainer.RowSpacing = 10;  // Espacio vertical entre filas
    gridContainer.ColumnSpacing = 10;  // Espacio horizontal entre columnas

    // Inicializar la imagen de portada
    imagePortada = new Image(); 
    gridContainer.Attach(imagePortada, 0, 0, 2, 1);  // La imagen ocupará las dos columnas en la fila 0

    // Campos básicos de la canción
    labelTitulo = new Label("Detalles de la Canción:");
    gridContainer.Attach(labelTitulo, 0, 1, 2, 1);  // Etiqueta "Detalles de la Canción" ocupa 2 columnas

    // Título
    Label labelTituloCancion = new Label("Título:");
    gridContainer.Attach(labelTituloCancion, 0, 2, 1, 1);
    entryTitulo = new Entry();
    entryTitulo.Sensitive = false;
    gridContainer.Attach(entryTitulo, 1, 2, 1, 1);

    // Año
    Label labelAño = new Label("Año:");
    gridContainer.Attach(labelAño, 0, 3, 1, 1);
    entryAño = new Entry();
    entryAño.Sensitive = false;
    gridContainer.Attach(entryAño, 1, 3, 1, 1);

    // Género
    Label labelGenero = new Label("Género:");
    gridContainer.Attach(labelGenero, 0, 4, 1, 1);
    entryGenero = new Entry();
    entryGenero.Sensitive = false;
    gridContainer.Attach(entryGenero, 1, 4, 1, 1);

    // Performer
    Label labelPerformer = new Label("Performer:");
    gridContainer.Attach(labelPerformer, 0, 5, 1, 1);
    entryPerformer = new Entry();
    entryPerformer.Sensitive = false;
    gridContainer.Attach(entryPerformer, 1, 5, 1, 1);

    // Pista
    Label labelPista = new Label("Pista:");
    gridContainer.Attach(labelPista, 0, 6, 1, 1);
    entryPista = new Entry();
    entryPista.Sensitive = false;
    gridContainer.Attach(entryPista, 1, 6, 1, 1);

    // Es Grupo - Botón Sí/No
    Label labelEsGrupo = new Label("Es Grupo:");
    gridContainer.Attach(labelEsGrupo, 0, 7, 1, 1);  // Etiqueta Es Grupo en la fila 7
    botonEsGrupo = new Button(GetTextoEsGrupo(false));  // Inicializar con "No"
    gridContainer.Attach(botonEsGrupo, 1, 7, 1, 1);  // Colocar en la segunda columna
    botonEsGrupo.Clicked += OnEsGrupoClicked;  // Asociar el evento

    // Campos del grupo (inicialmente ocultos)
    Label labelIntegrantes = new Label("Integrantes del Grupo:");
    gridContainer.Attach(labelIntegrantes, 0, 8, 1, 1);
    entryIntegrantes = new Entry();
    entryIntegrantes.Sensitive = false;
    gridContainer.Attach(entryIntegrantes, 1, 8, 1, 1);

    Label labelFechaInicio = new Label("Fecha de Inicio:");
    gridContainer.Attach(labelFechaInicio, 0, 9, 1, 1);
    entryFechaInicio = new Entry();
    entryFechaInicio.Sensitive = false;
    gridContainer.Attach(entryFechaInicio, 1, 9, 1, 1);

    Label labelFechaFin = new Label("Fecha de Fin:");
    gridContainer.Attach(labelFechaFin, 0, 10, 1, 1);
    entryFechaFin = new Entry();
    entryFechaFin.Sensitive = false;
    gridContainer.Attach(entryFechaFin, 1, 10, 1, 1);

    // Botón para habilitar la edición
    botonEditar = new Button("Editar");
    gridContainer.Attach(botonEditar, 0, 11, 1, 1); 
    botonEditar.Clicked += OnEditarClicked;

    // Botón para guardar los cambios
    botonGuardar = new Button("Guardar");
    gridContainer.Attach(botonGuardar, 1, 11, 1, 1);
    botonGuardar.Sensitive = false;
    botonGuardar.Clicked += OnGuardarClicked;

    // Botón para reproducir la canción
    botonReproducir = new Button("Reproducir");
    gridContainer.Attach(botonReproducir, 0, 12, 2, 1);
    botonReproducir.Clicked += OnReproducirClicked;

    // Agregar el contenedor principal al DisplayerView
    this.Attach(gridContainer, 0, 0, 1, 1);

    // Ocultar todos los elementos al inicio
    SetVisibility(false);
}

// Método para cambiar el texto del botón Es Grupo
private string GetTextoEsGrupo(bool esGrupo)
{
    return esGrupo ? "Sí" : "No";
}

// Evento cuando se hace clic en el botón Es Grupo
private void OnEsGrupoClicked(object sender, EventArgs e)
{
    // Alternar el valor de EsGrupo en la canción
    cancion.EsGrupo = !cancion.EsGrupo;

    // Actualizar el texto del botón
    botonEsGrupo.Label = GetTextoEsGrupo(cancion.EsGrupo);
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
        this.cancion = cancion;
        Console.WriteLine($"MostrarDatosCancion llamado para: {cancion.Titulo}");
        // Cargar la imagen de portada desde el byte[]

        // Tamaño deseado para la imagen
    const int imageWidth = 326;
    const int imageHeight = 326;

    try
    {
        if (cancion.ImagenPortada != null && cancion.ImagenPortada.Length > 0)
        {
            // Crear un PixbufLoader para cargar el byte[] como imagen
            using (var loader = new Gdk.PixbufLoader(cancion.ImagenPortada))
            {
                loader.Close();  // Es necesario cerrar el loader para procesar la imagen
                var pixbuf = loader.Pixbuf;
                
                // Redimensionar la imagen
                var scaledPixbuf = pixbuf.ScaleSimple(imageWidth, imageHeight, Gdk.InterpType.Bilinear);
                imagePortada.Pixbuf = scaledPixbuf;  // Asignar la imagen redimensionada
            }
        }
        else
        {
            // Si no hay imagen, mostrar una imagen por defecto
            imagePortada.Pixbuf = new Gdk.Pixbuf("ruta/a/imagen/default.png");
        }
    }
    catch (GLib.GException ex)
    {
        Console.WriteLine($"Error al cargar la imagen: {ex.Message}");
        // Mostrar la imagen por defecto en caso de error
        imagePortada.Pixbuf = new Gdk.Pixbuf("ruta/a/imagen/default.png");
    }

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