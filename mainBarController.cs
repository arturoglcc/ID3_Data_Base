using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace MusicApp.Controllers
{
    public class MainBarController {
        private SongsListController songsListController;
        private string rutaMinado; // Ruta donde se hará el minado
        private MainView vista;
        private List<Buscador.Criterio> criteriosBusqueda; // Lista de criterios de búsqueda
        

        public MainBarController(MainView vista, SongsListView vistaCanciones, DisplayerView displayerView)
        {
            this.vista = vista;
            criteriosBusqueda = new List<Buscador.Criterio>();
            rutaMinado = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            vista.ActualizarTitulo(rutaMinado);
            songsListController = new SongsListController(vistaCanciones, displayerView);
        }

        public string GetRutaMinado()
        {
            return rutaMinado;
        }

                // Método para obtener la lista de criterios de búsqueda
        public List<Buscador.Criterio> ObtenerCriterios()
        {
            return criteriosBusqueda;
        }

        // Seleccionar la ruta de minado
        public void SeleccionarRutaMinado(string nuevaRuta)
        {
            rutaMinado = nuevaRuta;
            Console.WriteLine($"Ruta seleccionada: {rutaMinado}");
            vista.ActualizarTitulo(rutaMinado);
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
        public void AgregarCriterio(string etiqueta, string valor, bool esExclusivo)
        {
            if (!string.IsNullOrEmpty(etiqueta) && !string.IsNullOrEmpty(valor))
            {
                string columnaBaseDatos = TransformarEtiquetaAColumna(etiqueta);
                criteriosBusqueda.Add(new Buscador.Criterio(columnaBaseDatos, valor, esExclusivo));
                Console.WriteLine($"Criterio agregado: {etiqueta} = {valor}, Exclusivo: {esExclusivo}");
            }
            else
            {
                Console.WriteLine("Debe proporcionar una etiqueta y un valor.");
            }
        }

        // Eliminar criterio
        public void EliminarCriterio(int index)
        {
            if (index >= 0 && index < criteriosBusqueda.Count)
            {
                Console.WriteLine($"Criterio eliminado: {criteriosBusqueda[index].Nombre}");
                criteriosBusqueda.RemoveAt(index);
            }
            else
            {
                Console.WriteLine("Índice de criterio inválido.");
            }
        }

        // Transformar etiqueta a columna de base de datos
        private string TransformarEtiquetaAColumna(string etiquetaUsuario)
        {
            switch (etiquetaUsuario.ToLower())
            {
                case "título":
                    return "title";
                case "género":
                    return "genre";
                case "intérprete":
                    return "performer";
                case "álbum":
                    return "id_album";
                case "año":
                    return "year";
                case "pista":
                    return "track";
                default:
                    throw new ArgumentException($"Etiqueta no reconocida: {etiquetaUsuario}");
            }
        }

        // Ejecutar búsqueda
        public void EjecutarBusqueda() {
            Buscador buscador = new Buscador();
            List<Cancion> resultados = buscador.Buscar(criteriosBusqueda);
            Console.WriteLine($"Se encontraron {resultados.Count} canciones.");
            songsListController.CargarCanciones(resultados);
            criteriosBusqueda.Clear();
        }

     // Método para eliminar los criterios seleccionados
        public void EliminarCriteriosSeleccionados(List<int> indicesSeleccionados)
        {
            indicesSeleccionados.Sort((a, b) => b.CompareTo(a));  // Ordenar en orden descendente para evitar problemas al eliminar

            foreach (int indice in indicesSeleccionados)
            {
                if (indice >= 0 && indice < criteriosBusqueda.Count)
                {
                    Console.WriteLine($"Criterio eliminado: {criteriosBusqueda[indice].Nombre}");
                    criteriosBusqueda.RemoveAt(indice);
                }
            }
        }

        
    }
}
