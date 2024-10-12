namespace MusicApp.Modelo {

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TagLib;

public class Editor {
    private string connectionString = "Data Source=music_library.db;Version=3;";

    // Método para editar una rola
    public void EditarRola(int idRola, string nuevoNombre, int nuevaFecha, string nuevoGenero,
                           int nuevoTrack, bool esGrupo,string nombrePerformer, string nuevoAlbum,
                           string pathArchivo, List<string>? integrantes = null,
                           DateTime? fechaInicio = null, DateTime? fechaFin = null
                           ) 
    {
        // Actualizar archivo MP3
        if (!ModificarArchivoMP3(idRola, nuevoNombre, nuevaFecha, nuevoGenero, nuevoTrack, nombrePerformer, pathArchivo, nuevoAlbum)) {
            Console.WriteLine("Error al modificar el archivo MP3.");
            return;
        }

        // Actualizar la base de datos
        using (SQLiteConnection connection = new SQLiteConnection(connectionString)) {
            connection.Open();
            using (SQLiteTransaction transaction = connection.BeginTransaction()) {
                try {
                    // Actualizar la tabla rolas
                    ActualizarRolaEnBaseDeDatos(connection, idRola, nuevoNombre, nuevaFecha, nuevoGenero, nuevoTrack);

                    // Verificar si es un solista o un grupo
                    if (esGrupo == false) {
                        // Actualizar solista
                        ActualizarSolista(connection, nombrePerformer);
                    }
                    else if (esGrupo) {
                        // Actualizar grupo y sus integrantes
                        ActualizarGrupo(connection, nombrePerformer, integrantes, fechaInicio, fechaFin);
                    }

                    // Confirmar la transacción
                    transaction.Commit();
                    Console.WriteLine("Modificación exitosa en la base de datos.");
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error al modificar la base de datos: {ex.Message}");
                    transaction.Rollback();
                }
            }
        }
    }

    // Método para modificar el archivo MP3
    private bool ModificarArchivoMP3(int idRola, string nuevoNombre, int nuevaFecha, string nuevoGenero,
                                     int nuevoTrack, string nombrePerformer, string pathArchivo, string nuevoAlbum)
    {
        try
        {
            // Modificar las etiquetas ID3v2.4 del archivo MP3
            TagLib.File archivo = TagLib.File.Create(pathArchivo);
            archivo.Tag.Title = nuevoNombre;
            archivo.Tag.Year = (uint)nuevaFecha;
            archivo.Tag.Genres = new[] { nuevoGenero };
            archivo.Tag.Track = (uint)nuevoTrack;
            archivo.Tag.Performers = new[] { nombrePerformer };
            archivo.Tag.Album = nuevoAlbum;

            // Guardar los cambios
            archivo.Save();
            archivo.Dispose();  
            GC.Collect();  
            GC.WaitForPendingFinalizers(); 
            FileInfo fileInfo = new FileInfo(pathArchivo);
            Console.WriteLine($"Título después de guardar: {archivo.Tag.Title}");
            Console.WriteLine($"Año después de guardar: {archivo.Tag.Year}");
            Console.WriteLine($"Género después de guardar: {string.Join(", ", archivo.Tag.Genres)}");
            Console.WriteLine($"Pista después de guardar: {archivo.Tag.Track}");
            Console.WriteLine($"Performers después de guardar: {string.Join(", ", archivo.Tag.Performers)}");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al modificar el archivo MP3: {ex.Message}");
            return false;
        }
    }

    // Método para actualizar los datos de una rola en la base de datos
    private void ActualizarRolaEnBaseDeDatos(SQLiteConnection connection, int idRola, string nuevoNombre, int nuevoAno, string nuevoGenero, int nuevoTrack)
    {
        string query = "UPDATE rolas SET title = @nuevoNombre, year = @nuevoAno, genre = @nuevoGenero, track = @nuevoTrack WHERE id_rola = @idRola";
        SQLiteCommand command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@nuevoNombre", nuevoNombre);
        command.Parameters.AddWithValue("@nuevoAno", nuevoAno);
        command.Parameters.AddWithValue("@nuevoGenero", nuevoGenero);
        command.Parameters.AddWithValue("@nuevoTrack", nuevoTrack);
        command.Parameters.AddWithValue("@idRola", idRola);
        command.ExecuteNonQuery();
    }

    // Método para actualizar datos de un solista
    private void ActualizarSolista(SQLiteConnection connection, string nombreSolista)
    {
        string query = "UPDATE performers SET name = @nombreSolista WHERE id_type = 0 AND name = @nombreSolista";
        SQLiteCommand command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@nombreSolista", nombreSolista);
        command.ExecuteNonQuery();
    }

    // Método para actualizar datos de un grupo
    private void ActualizarGrupo(SQLiteConnection connection, string nombreGrupo, List<string>? integrantes, DateTime? fechaInicio, DateTime? fechaFin)
    {
        // Actualizar grupo
        string queryGrupo = "UPDATE performers SET name = @nombreGrupo WHERE id_type = 1 AND name = @nombreGrupo";
        SQLiteCommand commandGrupo = new SQLiteCommand(queryGrupo, connection);
        commandGrupo.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);
        commandGrupo.ExecuteNonQuery();

        // Actualizar fechas de inicio y fin del grupo
        string queryFechas = "UPDATE groups SET start_date = @fechaInicio, end_date = @fechaFin WHERE name = @nombreGrupo";
        SQLiteCommand commandFechas = new SQLiteCommand(queryFechas, connection);
        commandFechas.Parameters.AddWithValue("@fechaInicio", fechaInicio?.ToString("yyyy-MM-dd"));
        commandFechas.Parameters.AddWithValue("@fechaFin", fechaFin?.ToString("yyyy-MM-dd"));
        commandFechas.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);
        commandFechas.ExecuteNonQuery();

        // Actualizar los integrantes del grupo
        if (integrantes != null)
        {
            // Eliminar los integrantes actuales
            string queryEliminarIntegrantes = "DELETE FROM in_group WHERE id_group = (SELECT id_group FROM groups WHERE name = @nombreGrupo)";
            SQLiteCommand commandEliminarIntegrantes = new SQLiteCommand(queryEliminarIntegrantes, connection);
            commandEliminarIntegrantes.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);
            commandEliminarIntegrantes.ExecuteNonQuery();

            // Insertar nuevos integrantes
            foreach (string integrante in integrantes)
            {
                // Obtener id_person del integrante (suponiendo que el nombre del integrante ya existe en persons)
                string queryIdPerson = "SELECT id_person FROM persons WHERE stage_name = @nombreIntegrante";
                SQLiteCommand commandIdPerson = new SQLiteCommand(queryIdPerson, connection);
                commandIdPerson.Parameters.AddWithValue("@nombreIntegrante", integrante);
                int idPerson = Convert.ToInt32(commandIdPerson.ExecuteScalar());

                // Insertar en in_group
                string queryInsertarIntegrante = "INSERT INTO in_group (id_person, id_group) VALUES (@idPerson, (SELECT id_group FROM groups WHERE name = @nombreGrupo))";
                SQLiteCommand commandInsertarIntegrante = new SQLiteCommand(queryInsertarIntegrante, connection);
                commandInsertarIntegrante.Parameters.AddWithValue("@idPerson", idPerson);
                commandInsertarIntegrante.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);
                commandInsertarIntegrante.ExecuteNonQuery();
            }
        }
    }

    private bool VerificarCambiosEnArchivoMP3(string pathArchivo, string nuevoNombre, int nuevaFecha, string nuevoGenero,
                                          int nuevoTrack, string nombrePerformer)
{
    try
    {
        // Cargar el archivo MP3 nuevamente
        TagLib.File archivoVerificado = TagLib.File.Create(pathArchivo);

        // Verificar si los cambios se aplicaron
        bool cambiosAplicados = archivoVerificado.Tag.Title == nuevoNombre &&
                                archivoVerificado.Tag.Year == (uint)nuevaFecha &&
                                archivoVerificado.Tag.Genres.Contains(nuevoGenero) &&
                                archivoVerificado.Tag.Track == (uint)nuevoTrack &&
                                archivoVerificado.Tag.Performers.Contains(nombrePerformer);

        if (cambiosAplicados)
        {
            Console.WriteLine("Los cambios se aplicaron correctamente.");
            return true;
        }
        else
        {
            Console.WriteLine("Los cambios no se aplicaron correctamente.");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al verificar los cambios en el archivo MP3: {ex.Message}");
        return false;
    }
}


}
}