using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TagLib;

public class Editor
{
    private string connectionString = "Data Source=music_library.db;Version=3;";

    // Método para editar una rola
    public void EditarRola(int idRola, string nuevoNombre, int nuevoAno, string nuevoGenero, int nuevoTrack, string tipoPerformer, string nombrePerformer, List<string>? integrantes = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        // Actualizar archivo MP3
        if (!ModificarArchivoMP3(idRola, nuevoNombre, nuevoAno, nuevoGenero, nuevoTrack, nombrePerformer))
        {
            Console.WriteLine("Error al modificar el archivo MP3.");
            return;
        }

        // Actualizar la base de datos
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Actualizar la tabla rolas
                    ActualizarRolaEnBaseDeDatos(connection, idRola, nuevoNombre, nuevoAno, nuevoGenero, nuevoTrack);

                    // Verificar si es un solista o un grupo
                    if (tipoPerformer == "solista")
                    {
                        // Actualizar solista
                        ActualizarSolista(connection, nombrePerformer);
                    }
                    else if (tipoPerformer == "grupo")
                    {
                        // Actualizar grupo y sus integrantes
                        ActualizarGrupo(connection, nombrePerformer, integrantes, fechaInicio, fechaFin);
                    }

                    // Confirmar la transacción
                    transaction.Commit();
                    Console.WriteLine("Modificación exitosa en la base de datos.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al modificar la base de datos: {ex.Message}");
                    transaction.Rollback();
                }
            }
        }
    }

    // Método para modificar el archivo MP3
    private bool ModificarArchivoMP3(int idRola, string nuevoNombre, int nuevoAno, string nuevoGenero, int nuevoTrack, string nombrePerformer)
    {
        try
        {
            // Obtener el path del archivo MP3 desde la base de datos
            string pathArchivo = ObtenerPathArchivoMP3(idRola);

            // Modificar las etiquetas ID3v2.4 del archivo MP3
            TagLib.File archivo = TagLib.File.Create(pathArchivo);
            archivo.Tag.Title = nuevoNombre;
            archivo.Tag.Year = (uint)nuevoAno;
            archivo.Tag.Genres = new[] { nuevoGenero };
            archivo.Tag.Track = (uint)nuevoTrack;
            archivo.Tag.Performers = new[] { nombrePerformer };

            // Guardar los cambios
            archivo.Save();
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

    // Método para obtener el path del archivo MP3 desde la base de datos
    private string ObtenerPathArchivoMP3(int idRola)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT path FROM rolas WHERE id_rola = @idRola";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@idRola", idRola);
            return (string)command.ExecuteScalar();
        }
    }
}
