namespace MusicApp.Modelo {

using System;
using System.IO;
using Microsoft.Data.Sqlite;  
using TagLib;

class Minero
{
    // Ruta predeterminada para minado
    static string rutaMusicaUsuario = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));


    // Función para crear las tablas si no existen
    public void CrearTablasSiNoExisten(SqliteConnection connection)
    {
        string[] tablas = {
            "CREATE TABLE IF NOT EXISTS types (id_type INTEGER PRIMARY KEY, description TEXT)",
            "INSERT OR IGNORE INTO types VALUES(0,'Person'), (1,'Group'), (2,'Unknown')",
            "CREATE TABLE IF NOT EXISTS performers (id_performer INTEGER PRIMARY KEY, id_type INTEGER, name TEXT, FOREIGN KEY (id_type) REFERENCES types(id_type))",
            "CREATE TABLE IF NOT EXISTS persons (id_person INTEGER PRIMARY KEY, stage_name TEXT, real_name TEXT, birth_date TEXT, death_date TEXT)",
            "CREATE TABLE IF NOT EXISTS groups (id_group INTEGER PRIMARY KEY, name TEXT, start_date TEXT, end_date TEXT)",
            "CREATE TABLE IF NOT EXISTS in_group (id_person INTEGER, id_group INTEGER, PRIMARY KEY (id_person, id_group), FOREIGN KEY (id_person) REFERENCES persons(id_person), FOREIGN KEY (id_group) REFERENCES groups(id_group))",
            "CREATE TABLE IF NOT EXISTS albums (id_album INTEGER PRIMARY KEY, path TEXT, name TEXT, year INTEGER)",
            "CREATE TABLE IF NOT EXISTS rolas (id_rola INTEGER PRIMARY KEY, id_performer INTEGER, id_album INTEGER, path TEXT, title TEXT, track INTEGER, year INTEGER, genre TEXT, FOREIGN KEY (id_performer) REFERENCES performers(id_performer), FOREIGN KEY (id_album) REFERENCES albums(id_album))"
        };

        foreach (var tabla in tablas)
        {
            SqliteCommand command = new SqliteCommand(tabla, connection);  // Cambiado a SqliteCommand
            command.ExecuteNonQuery();
        }
    }

    // Función que mina la carpeta en busca de archivos MP3 y extrae etiquetas ID3v2.4
    public void MinarDirectorio(SqliteConnection connection, string ruta) {
        BorrarDatosBaseDeDatos(connection);

        foreach (string archivo in Directory.EnumerateFiles(ruta, "*.mp3", SearchOption.AllDirectories)) {
            try {
                // Reemplazar espacios en la ruta por barra baja
                string rutaTransformada = archivo.Replace(" ", "_");

                // Renombrar el archivo si la ruta se modificó
                if (rutaTransformada != archivo) System.IO.File.Move(archivo, rutaTransformada);  

                ActualizarPathEnBaseDeDatos(connection, archivo, rutaTransformada);
                // Desambiguar usando el namespace completo para TagLib.File
                TagLib.File file = TagLib.File.Create(rutaTransformada);

                // Extraer etiquetas ID3v2.4 con valores por defecto
                string performer = file.Tag.FirstPerformer ?? "Unknown";
                string title = file.Tag.Title ?? "Unknown";
                string album = file.Tag.Album ?? "Unknown";
                
                // Usar el año actual o la fecha de creación del archivo si no hay año en las etiquetas
                int year = ObtenerFechaDeArchivo(rutaTransformada);

                string genre = file.Tag.FirstGenre ?? "Unknown";
                
                // Usar 0 o 1 si no se encuentra el número de pista
                int track = (int)(file.Tag.Track != 0 ? file.Tag.Track : 1);

                // Insertar performers y álbum si no están en la base de datos
                int id_performer = InsertarPerformerSiNoExiste(connection, performer, file.Tag.AlbumArtists.Length > 1 ? 1 : 0); // 1 = Grupo, 0 = Persona
                int id_album = InsertarAlbumSiNoExiste(connection, album, year, rutaTransformada);
                
                // Insertar la canción (rola) si no existe
                InsertarRolaSiNoExiste(connection, title, id_performer, id_album, rutaTransformada, track, year, genre);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar {archivo}: {ex.Message}");
            }
        }
    }

    // Método para actualizar el path en la base de datos
private void ActualizarPathEnBaseDeDatos(SqliteConnection connection, string rutaAntigua, string rutaNueva)
{
    try
    {
        string query = "UPDATE rolas SET path = @rutaNueva WHERE path = @rutaAntigua";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@rutaNueva", rutaNueva);
            command.Parameters.AddWithValue("@rutaAntigua", rutaAntigua);
            command.ExecuteNonQuery();
        }
        Console.WriteLine($"Base de datos actualizada: {rutaAntigua} -> {rutaNueva}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al actualizar la base de datos: {ex.Message}");
    }
}

    // Función para insertar un performer (persona o grupo) si no existe
    static int InsertarPerformerSiNoExiste(SqliteConnection connection, string performer, int id_type)
    {
        // Verificar si el performer ya existe
        string queryCheck = "SELECT id_performer FROM performers WHERE name = @name AND id_type = @id_type";
        SqliteCommand commandCheck = new SqliteCommand(queryCheck, connection);  // Cambiado a SqliteCommand
        commandCheck.Parameters.AddWithValue("@name", performer);
        commandCheck.Parameters.AddWithValue("@id_type", id_type);

        object? result = commandCheck.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result); 
        }

        // Insertar el performer si no existe
        string queryInsert = "INSERT INTO performers (name, id_type) VALUES (@name, @id_type)";
        SqliteCommand commandInsert = new SqliteCommand(queryInsert, connection);  // Cambiado a SqliteCommand
        commandInsert.Parameters.AddWithValue("@name", performer);
        commandInsert.Parameters.AddWithValue("@id_type", id_type);
        commandInsert.ExecuteNonQuery();

        // Obtener el id_performer insertado
        object? r = new SqliteCommand("SELECT last_insert_rowid()", connection).ExecuteScalar();
        if (r == null) {
            throw new InvalidOperationException("Error al obtener el último ID insertado.");
        }
        int id = Convert.ToInt32(r);
        return id;
    }

    // Función para insertar un álbum si no existe
    static int InsertarAlbumSiNoExiste(SqliteConnection connection, string album, int year, string path)
    {
        // Verificar si el álbum ya existe
        string queryCheck = "SELECT id_album FROM albums WHERE name = @name AND year = @year";
        SqliteCommand commandCheck = new SqliteCommand(queryCheck, connection);  // Cambiado a SqliteCommand
        commandCheck.Parameters.AddWithValue("@name", album);
        commandCheck.Parameters.AddWithValue("@year", year);

        object? result = commandCheck.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result); // Retornar el id_album existente
        }

        // Insertar el álbum si no existe
        string queryInsert = "INSERT INTO albums (name, year, path) VALUES (@name, @year, @path)";
        SqliteCommand commandInsert = new SqliteCommand(queryInsert, connection);  // Cambiado a SqliteCommand
        commandInsert.Parameters.AddWithValue("@name", album);
        commandInsert.Parameters.AddWithValue("@year", year);
        commandInsert.Parameters.AddWithValue("@path", path);
        commandInsert.ExecuteNonQuery();

        // Obtener el id_album insertado
        int id = Convert.ToInt32(new SqliteCommand("SELECT last_insert_rowid()", connection).ExecuteScalar() ?? -1);
        return id;
    }

    // Función para insertar una rola si no existe
    static void InsertarRolaSiNoExiste(SqliteConnection connection, string title, int id_performer, int id_album, string path, int track, int year, string genre) {
        // Verificar si la rola ya existe
        string queryCheck = "SELECT id_rola FROM rolas WHERE title = @title AND path = @path";
        SqliteCommand commandCheck = new SqliteCommand(queryCheck, connection);  // Cambiado a SqliteCommand
        commandCheck.Parameters.AddWithValue("@title", title);
        commandCheck.Parameters.AddWithValue("@path", path);

        object? result = commandCheck.ExecuteScalar();
        if (result != null) {
            Console.WriteLine($"La rola '{path}' ya existe en la base de datos.");
            return; // La rola ya existe, no hacer nada
        }

        // Insertar la rola si no existe
        string queryInsert = "INSERT INTO rolas (title, id_performer, id_album, path, track, year, genre) VALUES (@title, @id_performer, @id_album, @path, @track, @year, @genre)";
        SqliteCommand commandInsert = new SqliteCommand(queryInsert, connection);  // Cambiado a SqliteCommand
        commandInsert.Parameters.AddWithValue("@title", title);
        commandInsert.Parameters.AddWithValue("@id_performer", id_performer);
        commandInsert.Parameters.AddWithValue("@id_album", id_album);
        commandInsert.Parameters.AddWithValue("@path", path);
        commandInsert.Parameters.AddWithValue("@track", track);
        commandInsert.Parameters.AddWithValue("@year", year);
        commandInsert.Parameters.AddWithValue("@genre", genre);
        commandInsert.ExecuteNonQuery();
    }

    // Función para obtener el año de creación del archivo si falta el año en las etiquetas
    static int ObtenerFechaDeArchivo(string path) {
    try {
        // Usar TagLib para abrir el archivo y obtener las etiquetas
        var file = TagLib.File.Create(path);
        
        // Verificar si tiene la etiqueta del año (ID3v2.4)
        if (file.Tag.Year > 0) {
            return (int)file.Tag.Year; // Devolver el año de la etiqueta
        }
        else {
            // Si no hay etiqueta, usar la fecha de creación del archivo
            DateTime creationTime = System.IO.File.GetCreationTime(path);
            return creationTime.Year;
        }
    }
    catch {
        // Si ocurre cualquier error, devolver el año actual
        return DateTime.Now.Year;
    }
}


private void BorrarDatosBaseDeDatos(SqliteConnection connection)
{
    try
    {
        // Deshabilitar las restricciones de claves foráneas
        using (SqliteCommand disableForeignKeysCommand = new SqliteCommand("PRAGMA foreign_keys = OFF;", connection))
        {
            disableForeignKeysCommand.ExecuteNonQuery();
        }

        // Borrar las tablas relevantes
        string[] tablas = { "in_group", "rolas", "performers", "albums", "persons", "groups" };

        foreach (var tabla in tablas)
        {
            string query = $"DELETE FROM {tabla}";
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        // Liberar espacio
        using (SqliteCommand vacuumCommand = new SqliteCommand("VACUUM;", connection))
        {
            vacuumCommand.ExecuteNonQuery();
        }

        // Habilitar de nuevo las restricciones de claves foráneas
        using (SqliteCommand enableForeignKeysCommand = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
        {
            enableForeignKeysCommand.ExecuteNonQuery();
        }

        Console.WriteLine("Se han borrado los datos de la base de datos y el espacio ha sido liberado.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al borrar los datos de la base de datos: {ex.Message}");
    }
}

}
}