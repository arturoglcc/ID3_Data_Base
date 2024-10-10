using Gtk;
using MusicApp.Controllers;
using System;

public class MainBarView : Box
{
    private Button botonCriterios;
    private Button botonAgregarCriterio;
    private Entry textoCriterio;
    private ComboBoxText comboEtiquetas;
    private MainBarController mainBarController;

    public MainBarView(MainBarController mainBarController) : base(Orientation.Horizontal, 10)
    {
        this.mainBarController = mainBarController;    
        // Botón "Seleccionar Ruta"
        Button botonSeleccionarRuta = new Button("Seleccionar Ruta");
        botonSeleccionarRuta.SetSizeRequest(150, 30); // Tamaño fijo
        botonSeleccionarRuta.Clicked += (sender, e) => mainBarController.SeleccionarRuta(); // Llama al método en el controlador
        PackStart(botonSeleccionarRuta, false, false, 0);

        // Botón "Ejecutar Minado"
        Button botonEjecutarMinado = new Button("Ejecutar Minado");
        botonEjecutarMinado.SetSizeRequest(150, 30); // Tamaño fijo
        botonEjecutarMinado.Clicked += (sender, e) => mainBarController.EjecutarMinado(); // Llama al método en el controlador
        PackStart(botonEjecutarMinado, false, false, 0);

        // ComboBox para seleccionar etiquetas
        comboEtiquetas = new ComboBoxText();
        comboEtiquetas.AppendText("Título");
        comboEtiquetas.AppendText("Género");
        comboEtiquetas.AppendText("Intérprete");
        comboEtiquetas.AppendText("Año");
        comboEtiquetas.AppendText("Álbum");
        comboEtiquetas.AppendText("Pista");
        PackStart(comboEtiquetas, false, false, 0);
        comboEtiquetas.Changed += (sender, e) => OnCriterioOrTextoChanged();

        // TextBox para el valor del criterio
        textoCriterio = new Entry();
        PackStart(textoCriterio, false, false, 0);
        textoCriterio.Changed += (sender, e) => OnCriterioOrTextoChanged();

        // Botón "Agregar Criterio"
        botonAgregarCriterio = new Button("Agregar Criterio");
        botonAgregarCriterio.SetSizeRequest(150, 30); // Tamaño fijo
        botonAgregarCriterio.Sensitive = false; // Desactivado por defecto
        botonAgregarCriterio.Clicked += (sender, e) => mainBarController.AgregarCriterio(
            comboEtiquetas.ActiveText, textoCriterio.Text); // Llama al método en el controlador
        PackStart(botonAgregarCriterio, false, false, 0);

        // Botón "Ejecutar Búsqueda"
        Button botonEjecutarBusqueda = new Button("Ejecutar Búsqueda");
        botonEjecutarBusqueda.SetSizeRequest(150, 30); // Tamaño fijo
        botonEjecutarBusqueda.Clicked += (sender, e) => mainBarController.EjecutarBusqueda(); // Llama al método en el controlador
        PackStart(botonEjecutarBusqueda, false, false, 0);

        // Botón "Criterios"
        botonCriterios = new Button("Criterios");
        botonCriterios.SetSizeRequest(150, 30); // Tamaño fijo
        botonCriterios.Sensitive = false; // Deshabilitado al inicio
        botonCriterios.Clicked += (sender, e) => mainBarController.MostrarCriterios(); // Llama al método en el controlador
        PackStart(botonCriterios, false, false, 0);

        // Espacio expansible
        PackEnd(new Label(), true, true, 0);
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
