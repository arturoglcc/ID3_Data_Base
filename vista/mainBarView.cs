namespace MusicApp.Vista {

using Gtk;
using MusicApp.Controlador;
using System;

public class MainBarView : Box
{
    private Button botonCriterios;
    private Button botonAgregarCriterio;
    private Entry textoCriterio;
    private ComboBoxText comboEtiquetas;
    private MainBarController mainBarController;
    private EventBox barraColoreada;

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
        botonAgregarCriterio.Clicked += (sender, e) => mainBarController.AgregarCriterio(comboEtiquetas.ActiveText, textoCriterio.Text);
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
        botonCriterios.Clicked += (sender, e) => mainBarController.MostrarCriterios();
        contenedorBotones.PackStart(botonCriterios, false, false, 10);

        // Añadir los botones a la barra coloreada
        barraColoreada.Add(contenedorBotones);

        // Añadir la barra principal al contenedor principal
        barraPrincipal.PackStart(barraColoreada, false, false, 0);
        PackStart(barraPrincipal, false, false, 0);
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
}
}