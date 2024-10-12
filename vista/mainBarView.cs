namespace MusicApp.Vista {

using Gtk;
using MusicApp.Controlador;
    using MusicApp.Modelo;
    using System;

public class MainBarView : Box
{
    private Button botonCriterios;
    private Button botonAgregarCriterio;
    private Entry textoCriterio;
    private ComboBoxText comboEtiquetas;
    private MainBarController mainBarController;
    private EventBox barraColoreada;
    private bool inclusivo;

    public MainBarView(MainBarController mainBarController) : base(Orientation.Vertical, 0)
    {
        this.mainBarController = mainBarController;

        // Crear una caja para contener la barra de color y los botones
        Box barraPrincipal = new Box(Orientation.Vertical, 0);

        // Crear la barra coloreada como EventBox para permitir que contenga botones
        barraColoreada = new EventBox();
        barraColoreada.SetSizeRequest(600, 60); // Aumentar la altura de la barra gris para contener los botones
        barraColoreada.ModifyBg(StateType.Normal, new Gdk.Color(128, 128, 128)); // Color gris medio

        // Crear una caja horizontal para los botones dentro de la barra de color
        Box contenedorBotones = new Box(Orientation.Horizontal, 10); // Espaciado entre botones
        contenedorBotones.Halign = Align.Center; // Centrar los botones horizontalmente dentro de la barra

        // Botón "Seleccionar Ruta"
        Button botonSeleccionarRuta = new Button("Seleccionar Ruta");
        botonSeleccionarRuta.SetSizeRequest(80, 30); // Botón más pequeño
        botonSeleccionarRuta.Valign = Align.Center;  // Alinear verticalmente al centro
        botonSeleccionarRuta.Clicked += (sender, e) => mainBarController.SeleccionarRuta();
        contenedorBotones.PackStart(botonSeleccionarRuta, false, false, 10); // Padding horizontal entre botones

        // Botón "Ejecutar Minado"
        Button botonEjecutarMinado = new Button("Ejecutar Minado");
        botonEjecutarMinado.SetSizeRequest(80, 30); // Botón más pequeño
        botonEjecutarMinado.Valign = Align.Center;  // Alinear verticalmente al centro
        botonEjecutarMinado.Clicked += (sender, e) => mainBarController.EjecutarMinado();
        contenedorBotones.PackStart(botonEjecutarMinado, false, false, 10);

        // ComboBox para seleccionar etiquetas
        comboEtiquetas = new ComboBoxText();
        comboEtiquetas.SetSizeRequest(80, 30); // Tamaño reducido
        comboEtiquetas.AppendText("Título");
        comboEtiquetas.AppendText("Género");
        comboEtiquetas.AppendText("Intérprete");
        comboEtiquetas.AppendText("Año");
        comboEtiquetas.AppendText("Álbum");
        comboEtiquetas.AppendText("Pista");
        comboEtiquetas.Valign = Align.Center;  // Alinear verticalmente al centro
        contenedorBotones.PackStart(comboEtiquetas, false, false, 10); // Padding entre botones
        comboEtiquetas.Changed += (sender, e) => OnCriterioOrTextoChanged();

        // TextBox para el valor del criterio
        textoCriterio = new Entry();
        textoCriterio.SetSizeRequest(100, 30); // Tamaño reducido
        textoCriterio.Valign = Align.Center;  // Alinear verticalmente al centro
        contenedorBotones.PackStart(textoCriterio, false, false, 10);
        textoCriterio.Changed += (sender, e) => OnCriterioOrTextoChanged();

        // Botón "Agregar Criterio"
        botonAgregarCriterio = new Button("Agregar Criterio");
        botonAgregarCriterio.SetSizeRequest(80, 30); // Tamaño reducido
        botonAgregarCriterio.Sensitive = false;
        botonAgregarCriterio.Valign = Align.Center;  // Alinear verticalmente al centro
        botonAgregarCriterio.Clicked += (sender, e) => AbrirVentanaInclusivoExclusivo();
        contenedorBotones.PackStart(botonAgregarCriterio, false, false, 10);

        // Botón "Ejecutar Búsqueda"
        Button botonEjecutarBusqueda = new Button("Ejecutar Búsqueda");
        botonEjecutarBusqueda.SetSizeRequest(80, 30); // Tamaño reducido
        botonEjecutarBusqueda.Valign = Align.Center;  // Alinear verticalmente al centro
        botonEjecutarBusqueda.Clicked += (sender, e) => mainBarController.EjecutarBusqueda();
        contenedorBotones.PackStart(botonEjecutarBusqueda, false, false, 10);

        // Botón "Criterios"
        botonCriterios = new Button("Criterios");
        botonCriterios.SetSizeRequest(80, 30); // Tamaño reducido
        botonCriterios.Sensitive = false;
        botonCriterios.Valign = Align.Center;  // Alinear verticalmente al centro
        botonCriterios.Clicked += (sender, e) => MostrarCriterios();
        contenedorBotones.PackStart(botonCriterios, false, false, 10);

        // Añadir los botones a la barra coloreada
        barraColoreada.Add(contenedorBotones);

        // Añadir la barra principal al contenedor principal
        barraPrincipal.PackStart(barraColoreada, false, false, 0);
        PackStart(barraPrincipal, false, false, 0);
    }

    private void AbrirVentanaInclusivoExclusivo()
    {
        // Crear una ventana de diálogo
        Dialog dialogo = new Dialog("¿El criterio es?", null, DialogFlags.Modal);

        // Configurar la ventana
        dialogo.SetSizeRequest(300, 100);

        // Crear un contenedor vertical para la pregunta y los botones
        Box contenedorPrincipal = new Box(Orientation.Vertical, 10); 

        // Crear una caja horizontal para los botones
        Box contenedorBotones = new Box(Orientation.Horizontal, 10);

        // Botón "Inclusivo"
        Button botonInclusivo = new Button("Inclusivo");
        botonInclusivo.Clicked += (sender, e) => {
        inclusivo = true;
        mainBarController.AgregarCriterio(comboEtiquetas.ActiveText, textoCriterio.Text, inclusivo);
        // Limpiar la barra de búsqueda
        textoCriterio.Text = "";
        // Habilitar el botón "Criterios"
        botonCriterios.Sensitive = true;
        dialogo.Destroy(); // Cierra el diálogo
    };
    contenedorBotones.PackStart(botonInclusivo, false, false, 10);

    // Botón "Exclusivo"
    Button botonExclusivo = new Button("Exclusivo");
    botonExclusivo.Clicked += (sender, e) => {
        inclusivo = false;
        mainBarController.AgregarCriterio(comboEtiquetas.ActiveText, textoCriterio.Text, inclusivo);
        // Limpiar la barra de búsqueda
        textoCriterio.Text = "";
        // Habilitar el botón "Criterios"
        botonCriterios.Sensitive = true;
        dialogo.Destroy(); // Cierra el diálogo
    };
    contenedorBotones.PackStart(botonExclusivo, false, false, 10);

    // Añadir los botones al contenedor principal
    contenedorPrincipal.PackStart(contenedorBotones, false, false, 10);

    // Añadir el contenedor principal al ContentArea del diálogo
    dialogo.ContentArea.PackStart(contenedorPrincipal, true, true, 10);

    dialogo.ShowAll(); // Mostrar todos los elementos
        }

    // Método para escuchar cambios en el criterio o texto
    private void OnCriterioOrTextoChanged()
    {
        bool criterioSeleccionado = !string.IsNullOrEmpty(comboEtiquetas.ActiveText);
        bool textoIngresado = textoCriterio.Text.Length > 0;
        botonAgregarCriterio.Sensitive = criterioSeleccionado && textoIngresado;
    }

    // Método para habilitar/deshabilitar el botón "Criterios"
    public void SetCriteriosButtonSensitive(bool isEnabled)
    {
        botonCriterios.Sensitive = isEnabled;
    }

        private void MostrarCriterios()
        {
            // Obtener los criterios de búsqueda del controlador
            List<Buscador.Criterio> criterios = mainBarController.criteriosBusqueda;

            // Crear un diálogo para mostrar los criterios
            Dialog dialogoCriterios = new Dialog("Criterios", null, DialogFlags.Modal);
            dialogoCriterios.SetSizeRequest(300, 200);

            // Crear un contenedor vertical para los criterios
            Box contenedorCriterios = new Box(Orientation.Vertical, 10);

            List<Buscador.Criterio> criteriosSeleccionados = new List<Buscador.Criterio>();

            // Crear botones para cada criterio desde la lista del controlador
            foreach (Buscador.Criterio criterio in criterios)
            {
                ToggleButton botonCriterio = new ToggleButton();
                botonCriterio.Label = criterio.ToString();
                botonCriterio.Toggled += (sender, e) => {
                if (botonCriterio.Active)
                {
                    criteriosSeleccionados.Add(criterio);
                }
                else
                {
                    // Eliminar criterio deseleccionado
                    criteriosSeleccionados.Remove(criterio);
                }
            };
            contenedorCriterios.PackStart(botonCriterio, false, false, 5);
        }

        // Botón para eliminar criterios seleccionados
        Button botonEliminar = new Button("Eliminar Criterios");
        botonEliminar.Clicked += (sender, e) => {
        mainBarController.EliminarCriterios(criteriosSeleccionados); // Llamar al controlador
        dialogoCriterios.Destroy(); // Cerrar el diálogo
        };
        contenedorCriterios.PackStart(botonEliminar, false, false, 10);

        // Añadir el contenedor al área de contenido del diálogo
        dialogoCriterios.ContentArea.PackStart(contenedorCriterios, true, true, 10);

        dialogoCriterios.ShowAll(); // Mostrar todos los elementos
    }
}
}