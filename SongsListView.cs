using Gtk;
using System.Collections.Generic;

public class SongsListView : FlowBox
{
    public SongsListView() : base()
    {
        this.SelectionMode = SelectionMode.None;  // No es necesario habilitar la selección en FlowBox
    }

    // Método para limpiar la vista actual (remover todos los botones)
    public void LimpiarVista()
    {
        foreach (Widget widget in this.Children)
        {
            this.Remove(widget);  // Remover cada widget actual
        }
    }

    // Método para agregar un botón a la vista
    public void MostrarBoton(Button boton)
    {
        this.Add(boton);  // Agregar el botón a la vista
    }

    // Método para actualizar la vista después de agregar todos los botones
    public void ActualizarVista()
    {
        this.ShowAll();  // Refrescar la vista para mostrar todos los botones
    }
}

