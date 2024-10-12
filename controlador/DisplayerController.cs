using MusicApp.Modelo;
using MusicApp.Vista;

namespace MusicApp.Controlador {

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class DisplayerController
{
    public DisplayerView view;
    private Buscador.Cancion cancionActual = null!;
    private Editor editor;

    public DisplayerController()
    {
        this.view = new DisplayerView(this);
        editor = new Editor();
    }

    // Método para recibir una canción y determinar si mostrar datos de grupo
    public void CargarCancion(Buscador.Cancion cancion)
    {
        cancionActual = cancion;

        // Comprobar si hay datos del grupo (si existen integrantes o fechas de grupo)
        bool mostrarDatosGrupo = cancion.Integrantes != null || 
                                 cancion.FechaInicioGrupo != null || 
                                 cancion.FechaFinGrupo != null;

        // Mostrar la canción en la vista, tenga o no datos del grupo
        view.MostrarDatosCancion(cancion, mostrarDatosGrupo);
    }

    // Método para editar la canción
    public void EditarCancion(string? nuevoTitulo, string nuevaFecha, string nuevoGenero, string nuevoPerformer,
                              string nuevoAlbum, string nuevaPista, bool esGrupo, string? nuevosIntegrantes,
                              string? nuevaFechaInicio, string? nuevaFechaFin)
    {
        // Convertir los datos según sea necesario
        int.TryParse(nuevaFecha, out int newYear);
        int.TryParse(nuevaPista, out int pista);
        
        if (nuevoTitulo == null) {
            nuevoTitulo = "unknown";
        }

        // Preparar las fechas (si se proporcionaron)
        DateTime? fechaInicio = !string.IsNullOrEmpty(nuevaFechaInicio) ? DateTime.Parse(nuevaFechaInicio) : (DateTime?)null;
        DateTime? fechaFin = !string.IsNullOrEmpty(nuevaFechaFin) ? DateTime.Parse(nuevaFechaFin) : (DateTime?)null;

        // Actualizar los datos de la canción seleccionada
        cancionActual.Titulo = nuevoTitulo;
        cancionActual.Año = newYear;
        cancionActual.Genero = nuevoGenero;
        cancionActual.Intérprete = nuevoPerformer;
        cancionActual.Album = nuevoAlbum;
        cancionActual.Pista = pista;

        // Si hay nuevos integrantes, actualizar la lista
        if (nuevosIntegrantes != null)
        {
            string[] separadores = new string[] { ",", "|", "-" };
                cancionActual.Integrantes = new List<string>(nuevosIntegrantes.Split(separadores, StringSplitOptions.RemoveEmptyEntries));

                // Limpiar los espacios en blanco de cada integrante
                for (int i = 0; i < cancionActual.Integrantes.Count; i++)
                {
                    cancionActual.Integrantes[i] = cancionActual.Integrantes[i].Trim();
                }
        }

        // Actualizar la canción y sus datos relacionados (grupo o solista)
        editor.EditarRola(
            cancionActual.Id, 
            nuevoTitulo, 
            newYear, 
            nuevoGenero, 
            pista, 
            esGrupo,
            nuevoPerformer,
            nuevoAlbum,
            cancionActual.Path,  
            cancionActual.Integrantes, 
            fechaInicio, 
            fechaFin
        );
    }

     // Este método se llamará cuando se haga clic en "Reproducir"
    public void ReproducirCancion()
    {
        if (cancionActual != null && !string.IsNullOrEmpty(cancionActual.Path))
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(cancionActual.Path) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", cancionActual.Path);
                    Console.WriteLine($"Se intento reproducir la cancion en:  {cancionActual.Path}");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", cancionActual.Path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar reproducir la canción: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("No hay una canción seleccionada o la ruta es inválida.");
        }
    }
}
}