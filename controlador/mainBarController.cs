using MusicApp.Vista;
using MusicApp.Modelo;

namespace MusicApp.Controlador {

using System;
using System.Collections.Generic;
using Gtk;
using Microsoft.Data.Sqlite;

    public class MainBarController
    {
        private SongsListController songsListController;
        private string rutaMinado; // Ruta donde se hará el minado
        public MainBarView vista;
        private MainView mainView;
        public List<Buscador.Criterio> criteriosBusqueda;

        public MainBarController(SongsListView vistaCanciones, DisplayerController displayercon, MainView mainView)
        {
            this.mainView = mainView;
            vista = new MainBarView(this);  // Pasar la referencia del controlador a la vista
            criteriosBusqueda = new List<Buscador.Criterio>();
            rutaMinado = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            mainView.ActualizarTitulo(rutaMinado);
            songsListController = new SongsListController(vistaCanciones, displayercon, mainView);
        }

        // Seleccionar la ruta de minado
        public void SeleccionarRuta()
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Selecciona una carpeta", null, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Select", ResponseType.Accept);
            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                rutaMinado = fileChooser.Filename;
                mainView.ActualizarTitulo(rutaMinado);
            }
            fileChooser.Destroy();
        }

        // Ejecutar el minado
        public void EjecutarMinado()
        {
            if (!string.IsNullOrEmpty(rutaMinado))
            {
                string connectionString = "Data Source=music_library.db";
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    Minero minero = new Minero();
                    minero.MinarDirectorio(connection, rutaMinado);
                    Console.WriteLine("Minado ejecutado.");
                }
            }
        }

        // Agregar criterio
        public void AgregarCriterio(string etiqueta, string valor, bool inclusivo)
        {
            if (!string.IsNullOrEmpty(etiqueta) && !string.IsNullOrEmpty(valor))
            {
                string columnaBaseDatos = TransformarEtiquetaAColumna(etiqueta);
                criteriosBusqueda.Add(new Buscador.Criterio(columnaBaseDatos, valor, inclusivo)); // Especifica exclusividad si es necesario
                Console.WriteLine($"Criterio agregado: {etiqueta} = {valor} esInclusivo={inclusivo}");
            }
        }

        // Ejecutar búsqueda
        public void EjecutarBusqueda()
        {
            Buscador buscador = new Buscador();
            List<Buscador.Cancion> resultados = buscador.Buscar(criteriosBusqueda);
            Console.WriteLine($"Se encontraron {resultados.Count} canciones.");
            songsListController.CargarCanciones(resultados);
            criteriosBusqueda.Clear();
        }

 

        // Transformar etiqueta a columna de base de datos
        private string TransformarEtiquetaAColumna(string etiquetaUsuario)
        {
            switch (etiquetaUsuario.ToLower())
            {
                case "título": return "title";
                case "género": return "genre";
                case "intérprete": return "performer";
                case "álbum": return "id_album";
                case "año": return "year";
                case "pista": return "track";
                default: throw new ArgumentException($"Etiqueta no reconocida: {etiquetaUsuario}");
            }
        }

        // Método para eliminar los criterios seleccionados de la lista CriteriosBusqueda
    public void EliminarCriterios(List<Buscador.Criterio> criteriosSeleccionados)
    {
        // Eliminar cada criterio seleccionado de la lista CriteriosBusqueda
        foreach (Buscador.Criterio criterio in criteriosSeleccionados)
        {
            if (criteriosBusqueda.Contains(criterio))
            {
                criteriosBusqueda.Remove(criterio);
            }
        }
    }
    }
}