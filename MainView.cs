using Gtk;
using MusicApp.Controllers;

public class MainView : Window
{
    private SongsListView songsListView;
    private MainBarController mainBarController;
    private Grid mainLayout;
    private DisplayerController displayerController;

    public MainView() : base("Gestor de Música")
    {
        songsListView = new SongsListView();
        songsListView.Visible = false;
        displayerController = new DisplayerController();
        mainBarController = new MainBarController(songsListView, displayerController, this);

        SetDefaultSize(600, 400);
        SetPosition(WindowPosition.Center);

        // Crear el layout principal
        mainLayout = new Grid();
        mainLayout.RowSpacing = 5;
        mainLayout.ColumnSpacing = 5;

        // Añadir la barra principal a la primera fila
        mainLayout.Attach(mainBarController.vista, 0, 0, 2, 1); 

        // Añadir la lista de canciones (que por ahora está oculta)
        mainLayout.Attach(songsListView, 0, 1, 2, 1);

        // Configuración final del layout
        Add(mainLayout);
        ShowAll();

        DeleteEvent += delegate { Application.Quit(); };
    }

    // Método para mostrar la ventana emergente con los criterios
    public void MostrarVentanaCriterios()
    {
        Dialog dialogo = new Dialog("Criterios de Búsqueda", this, DialogFlags.Modal);
        Box contentBox = (Box)dialogo.ContentArea;

        // Aquí puedes añadir el código para mostrar los criterios

        dialogo.ShowAll();
        dialogo.Run();
        dialogo.Destroy();
    }

    // Método para habilitar/deshabilitar el botón "Criterios" desde el controlador
    public void SetCriteriosButtonSensitive(bool isEnabled)
    {
        mainBarController.vista.SetCriteriosButtonSensitive(isEnabled);
    }

        // Nuevo método para actualizar el título de la ventana
    public void ActualizarTitulo(string nuevoTitulo)
    {
        // Actualizar el título de la ventana
        this.Title = nuevoTitulo;
    }

        // Método para cambiar la disposición de la lista y mostrar los detalles de la canción
    public void MostrarDetallesCancion(Cancion cancion)
    {
        // Eliminar la lista de canciones de la primera columna
        mainLayout.Remove(songsListView);

        // Añadir el DisplayerView en la primera columna
        mainLayout.Attach(displayerController.view, 0, 1, 1, 1);  // Colocar en la primera columna

        // Mover la lista de canciones a la segunda columna
        mainLayout.Attach(songsListView, 1, 1, 1, 1);  // Colocar en la segunda columna

        // Mostrar los datos de la canción seleccionada en el displayer
        displayerController.view.MostrarDatosCancion(cancion, cancion.Integrantes != null);

        // Actualizar la vista
        mainLayout.ShowAll();
    }

    // Método para hacer visible la lista de canciones tras la búsqueda
    public void MostrarListaCanciones()
    {
        songsListView.Visible = true; // Hacer visible la lista
    }
}
