using Gtk;
using MusicApp.Controllers;

public class MainView : Window
{
    private SongsListView songsListView;
    private MainBarController controlador;
    private Entry textoCriterio;
    private ComboBoxText comboEtiquetas;

    public MainView() : base("Gestor de Música") {
         // Crear e integrar SongsListView con FlowBox
        songsListView = new SongsListView();
        controlador = new MainBarController(this, songsListView);
        

        SetDefaultSize(600, 400);
        SetPosition(WindowPosition.Center);

        Grid mainLayout = new Grid();
        mainLayout.RowSpacing = 5;
        mainLayout.ColumnSpacing = 5;

        mainLayout.Attach(songsListView, 0, 1, 1, 1);

        // Añadir el layout al contenedor de la ventana
        Add(mainLayout);
        ShowAll();

        DeleteEvent += delegate { Application.Quit(); };

        // Botón para seleccionar ruta
        Button botonSeleccionarRuta = new Button("Seleccionar Ruta");
        botonSeleccionarRuta.Clicked += (sender, e) => SeleccionarRuta();
        mainLayout.Attach(botonSeleccionarRuta, 0, 0, 1, 1);  // Colocar en la fila 0, columna 0

        // Botón para ejecutar minado
        Button botonEjecutarMinado = new Button("Ejecutar Minado");
        botonEjecutarMinado.Clicked += (sender, e) => controlador.EjecutarMinado();
        mainLayout.Attach(botonEjecutarMinado, 1, 0, 1, 1);  // Colocar en la fila 0, columna 1

        // ComboBox para seleccionar etiquetas
        comboEtiquetas = new ComboBoxText();
        comboEtiquetas.AppendText("Título");
        comboEtiquetas.AppendText("Género");
        comboEtiquetas.AppendText("Intérprete");
        comboEtiquetas.AppendText("Año");
        comboEtiquetas.AppendText("Álbum");
        comboEtiquetas.AppendText("Pista");
        mainLayout.Attach(comboEtiquetas, 2, 0, 1, 1);

        // TextBox para el valor del criterio
        textoCriterio = new Entry();
        mainLayout.Attach(textoCriterio, 3, 0, 1, 1);

        // Botón para agregar criterio
        Button botonAgregarCriterio = new Button("Agregar Criterio");
        botonAgregarCriterio.Clicked += (sender, e) =>
        {
            string etiquetaSeleccionada = comboEtiquetas.ActiveText;
            string valorCriterio = textoCriterio.Text;
            MostrarDialogoInclusivoExclusivo(etiquetaSeleccionada, valorCriterio);
        };
        mainLayout.Attach(botonAgregarCriterio, 6, 0, 1, 1);

        // Botón para ejecutar búsqueda
        Button botonEjecutarBusqueda = new Button("Ejecutar Búsqueda");
        botonEjecutarBusqueda.Clicked += (sender, e) => controlador.EjecutarBusqueda();
        mainLayout.Attach(botonEjecutarBusqueda, 7, 0, 1, 1);

        // Configuración final del layout
        Add(mainLayout);
        ShowAll();

        DeleteEvent += delegate { Application.Quit(); };
    }

    public void ActualizarTitulo(string ruta)
    {
        Title = $"Gestor de Música ({ruta})";
    }

    private void SeleccionarRuta()
    {
        FileChooserDialog fileChooser = new FileChooserDialog("Selecciona una carpeta", null, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Select", ResponseType.Accept);
        if (fileChooser.Run() == (int)ResponseType.Accept)
        {
            string ruta = fileChooser.Filename;
            controlador.SeleccionarRutaMinado(ruta);
        }
        fileChooser.Destroy();
    }

    private void MostrarDialogoInclusivoExclusivo(string etiqueta, string valor)
    {
        // Crear el diálogo
        Dialog dialog = new Dialog("Tipo de Criterio", this, DialogFlags.Modal);
        dialog.AddButton("Inclusivo", ResponseType.Yes);
        dialog.AddButton("Exclusivo", ResponseType.No);
        dialog.DefaultResponse = ResponseType.Yes;

        // Mostrar el diálogo y esperar la respuesta
        dialog.ShowAll();
        ResponseType respuesta = (ResponseType)dialog.Run();

        // Determinar si es inclusivo o exclusivo basado en la respuesta del usuario
        bool esExclusivo = respuesta == ResponseType.No;
        controlador.AgregarCriterio(etiqueta, valor, esExclusivo);

        // Cerrar el diálogo
        dialog.Destroy();
    }
}
