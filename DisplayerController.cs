using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class DisplayerController
{
    public DisplayerView view;
    private Cancion cancionActual = null!;
    private Editor editor;

    public DisplayerController()
    {
        this.view = new DisplayerView(this);
        editor = new Editor();
    }

    // Método para recibir una canción y determinar si mostrar datos de grupo
    public void CargarCancion(Cancion cancion)
    {
        cancionActual = cancion;

        // Comprobar si hay datos del grupo (si existen integrantes o fechas de grupo)
        bool mostrarDatosGrupo = cancion.Integrantes != null || 
                                 cancion.FechaInicioGrupo != null || 
                                 cancion.FechaFinGrupo != null;

        // Mostrar la canción en la vista, tenga o no datos del grupo
        view.MostrarDatosCancion(cancion, mostrarDatosGrupo);
    }

        public static void GuardarCambios(string titulo, string año, string genero, string performer, string pista,
                                      string? integrantes, string? fechaInicioGrupo, string? fechaFinGrupo)
    {
        // Llamar al editor para guardar los nuevos datos
        Console.WriteLine("Guardando cambios...");
        Console.WriteLine($"Título: {titulo}, Año: {año}, Género: {genero}, Performer: {performer}, Pista: {pista}");
        
        if (integrantes != null)
        {
            Console.WriteLine($"Integrantes: {integrantes}, Fecha Inicio: {fechaInicioGrupo}, Fecha Fin: {fechaFinGrupo}");
        }
    }

    // Método para editar la canción
    public void EditarCancion(string? nuevoTitulo, string nuevoAño, string nuevoGenero, string nuevoPerformer, string nuevaPista, string? nuevosIntegrantes, string? nuevaFechaInicio, string? nuevaFechaFin)
    {
        // Convertir los datos según sea necesario
        int.TryParse(nuevoAño, out int año);
        int.TryParse(nuevaPista, out int pista);
        
        if (nuevoTitulo == null) {
            nuevoTitulo = "unknown";
        }

        // Preparar las fechas (si se proporcionaron)
        DateTime? fechaInicio = !string.IsNullOrEmpty(nuevaFechaInicio) ? DateTime.Parse(nuevaFechaInicio) : (DateTime?)null;
        DateTime? fechaFin = !string.IsNullOrEmpty(nuevaFechaFin) ? DateTime.Parse(nuevaFechaFin) : (DateTime?)null;

        // Actualizar los datos de la canción seleccionada
        cancionActual.Titulo = nuevoTitulo;
        cancionActual.Año = año;
        cancionActual.Genero = nuevoGenero;
        cancionActual.Intérprete = nuevoPerformer;
        cancionActual.Pista = pista;

        // Si hay nuevos integrantes, actualizar la lista
        if (nuevosIntegrantes != null)
        {
            cancionActual.Integrantes = new List<string>(nuevosIntegrantes.Split(','));
        }

        String TipoPerformer = "solista";

        if (nuevosIntegrantes != null || fechaInicio != null || fechaFin != null || cancionActual.Integrantes != null) {
            TipoPerformer = "grupo";
        }

        // Actualizar la canción y sus datos relacionados (grupo o solista)
        editor.EditarRola(
            cancionActual.Id, 
            nuevoTitulo, 
            año, 
            nuevoGenero, 
            pista, 
            TipoPerformer, 
            cancionActual.Intérprete, 
            cancionActual.Integrantes, 
            fechaInicio, 
            fechaFin
        );

        Console.WriteLine("Canción actualizada en la base de datos y en el archivo MP3.");
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
