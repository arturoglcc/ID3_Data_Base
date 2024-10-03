using Gtk;
using MusicApp.Controllers;

public class MainView : Window
{
    private SongsListView songsListView;
    private MainBarController controlador;
    private Button botonCriterios;
    private Button botonAgregarCriterio = null!;
    private Entry textoCriterio = null!;
    private ComboBoxText comboEtiquetas = null!;

    public MainView() : base("Gestor de Música") {
         // Crear e integrar SongsListView con FlowBox
        songsListView = new SongsListView();
        controlador = new MainBarController(this, songsListView);
        

        SetDefaultSize(600, 400);
        SetPosition(WindowPosition.Center);

        Grid mainLayout = new Grid();
        mainLayout.RowSpacing = 5;
        mainLayout.ColumnSpacing = 5;

        // Crear botón "Criterios"
        botonCriterios = new Button("Criterios");
        botonCriterios.Sensitive = false;  // Deshabilitado al inicio
        botonCriterios.Clicked += (sender, e) => MostrarVentanaCriterios();
        mainLayout.Attach(botonCriterios, 7, 0, 1, 1);

        mainLayout.Attach(songsListView, 0, 1, 1, 1);

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
        comboEtiquetas.Changed += OnCriterioOrTextoChanged;

        // TextBox para el valor del criterio
        textoCriterio = new Entry();
        mainLayout.Attach(textoCriterio, 3, 0, 1, 1);
        textoCriterio.Changed += OnCriterioOrTextoChanged; 

        // Botón para agregar criterio
        botonAgregarCriterio = new Button("Agregar Criterio");
        botonAgregarCriterio.Sensitive = false;
        botonAgregarCriterio.Clicked += (sender, e) => AgregarCriterio();
        mainLayout.Attach(botonAgregarCriterio, 6, 0, 1, 1);

        // Botón para ejecutar búsqueda
        Button botonEjecutarBusqueda = new Button("Ejecutar Búsqueda");
        botonEjecutarBusqueda.Clicked += (sender, e) => {
            controlador.EjecutarBusqueda();
            botonCriterios.Sensitive = false;
            };
 
        mainLayout.Attach(botonEjecutarBusqueda, 8, 0, 1, 1);

        // Configuración final del layout
        Add(mainLayout);
        ShowAll();

        DeleteEvent += delegate { Application.Quit(); };
    }

    // Método para escuchar cambios en el criterio o texto
    private void OnCriterioOrTextoChanged(object? sender, EventArgs e)
    {
    if (comboEtiquetas == null || textoCriterio == null || botonAgregarCriterio == null)
        return;

    bool criterioSeleccionado = !string.IsNullOrEmpty(comboEtiquetas.ActiveText);
    bool textoIngresado = textoCriterio.Text.Length > 0;
    botonAgregarCriterio.Sensitive = criterioSeleccionado && textoIngresado;
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

        // Método para mostrar la ventana emergente con los criterios
    public void MostrarVentanaCriterios() {
        Dialog dialogo = new Dialog("Criterios de Búsqueda", this, DialogFlags.Modal);
        Box contentBox = (Box)dialogo.ContentArea;  // Cambiar a Box

        // Crear lista de botones para los criterios
        List<Button> botonesCriterios = new List<Button>();
        List<int> criteriosSeleccionados = new List<int>();  // Mantiene el índice de criterios seleccionados

        foreach (var criterio in controlador.ObtenerCriterios()) {
            Button botonCriterio = new Button($"{criterio.Nombre}: {criterio.Valor}");
            botonCriterio.Clicked += (sender, e) =>
            {
                // Alternar selección
                if (criteriosSeleccionados.Contains(criteriosSeleccionados.Count))
                {
                    criteriosSeleccionados.Remove(criteriosSeleccionados.Count);  // Desmarcar
                }
                else
                {
                    criteriosSeleccionados.Add(criteriosSeleccionados.Count);  // Marcar
                }
            };
            botonesCriterios.Add(botonCriterio);
            contentBox.PackStart(botonCriterio, false, false, 5);  // Usar contentBox directamente
        }

        // Botón para eliminar los criterios seleccionados
        Button botonEliminar = new Button("Eliminar Criterios Seleccionados");
        botonEliminar.Clicked += (sender, e) =>
        {
            controlador.EliminarCriteriosSeleccionados(criteriosSeleccionados);
            dialogo.Destroy();  // Cerrar la ventana después de eliminar
        };

        contentBox.PackEnd(botonEliminar, false, false, 5);

        dialogo.ShowAll();
        dialogo.Run();
    }

        // Método para agregar criterio
    private void AgregarCriterio()
    {
        string etiquetaSeleccionada = comboEtiquetas.ActiveText;
        string valorCriterio = textoCriterio.Text;

        MostrarDialogoInclusivoExclusivo(etiquetaSeleccionada, valorCriterio);

        // Limpiar el campo de texto y restablecer el ComboBox
        textoCriterio.Text = string.Empty;
        comboEtiquetas.Active = -1;
        botonCriterios.Sensitive = true;
    }
}
    // Método para mostrar el Displayer en la parte izquierda
