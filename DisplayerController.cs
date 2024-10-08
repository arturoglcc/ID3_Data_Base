using System;

public class DisplayerController
{
    private DisplayerView view;
    private Cancion? cancionActual;

    public DisplayerController(DisplayerView view)
    {
        this.view = view;
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

    // Método para guardar los cambios de la canción (ya está implementado en el código anterior)
}
