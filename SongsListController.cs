using System;
using System.Collections.Generic;
using Gtk;

namespace MusicApp.Controllers
{
    public class SongsListController
    {
        private SongsListView viewer;
        private List<Cancion> canciones;
        private DisplayerController displayerCon;
        private MainView mainView;

        public SongsListController(SongsListView viewer, DisplayerController displayerCon, MainView mainView)
        {
            this.viewer = viewer;
            this.mainView = mainView;
            canciones = new List<Cancion>();
            this.displayerCon = displayerCon;
         }

        // Método para recibir la lista de canciones y generar y mostrar los botones en la vista
        public void CargarCanciones(List<Cancion> canciones)
        {
            this.canciones = canciones;
            viewer.LimpiarVista();  // Limpiar la vista actual antes de agregar nuevos botones

            foreach (var cancion in canciones)
            {
                Button botonCancion = new Button(cancion.Titulo);
                botonCancion.Clicked += (sender, e) => OnCancionSeleccionada(cancion);
                viewer.MostrarBoton(botonCancion);  // Enviar el botón a la vista para mostrarlo
            }

            viewer.ActualizarVista();
        }

        // Método para manejar la selección de una canción (botón clicado)
        private void OnCancionSeleccionada(Cancion cancion)
        {
            Console.WriteLine($"Botón de canción seleccionado: {cancion.Titulo}");
            mainView.MostrarDetallesCancion(cancion);
            displayerCon.CargarCancion(cancion);
            //Console.Write(cancion.ToString());
        }
    }
}
