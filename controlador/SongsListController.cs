using MusicApp.Vista;
using MusicApp.Modelo;

namespace MusicApp.Controlador {

using System;
using System.Collections.Generic;
using Gtk;

    public class SongsListController
    {
        private SongsListView viewer;
        private List<Buscador.Cancion> canciones;
        private DisplayerController displayerCon;
        private MainView mainView;

        public SongsListController(SongsListView viewer, DisplayerController displayerCon, MainView mainView)
        {
            this.viewer = viewer;
            this.mainView = mainView;
            canciones = new List<Buscador.Cancion>();
            this.displayerCon = displayerCon;
         }

        // Método para recibir la lista de canciones y generar y mostrar los botones en la vista
        public void CargarCanciones(List<Buscador.Cancion> canciones)
        {
            this.canciones = canciones;
            viewer.LimpiarVista();
            viewer.CargarCancionesConEncabezado(canciones, OnCancionSeleccionada);
        }



        // Método para manejar la selección de una canción (botón clicado)
        private void OnCancionSeleccionada(Buscador.Cancion cancion)
        {
            Console.WriteLine($"Botón de canción seleccionado: {cancion.Titulo}");
            mainView.MostrarDetallesCancion(cancion);
            displayerCon.CargarCancion(cancion);
        }
    }
}
