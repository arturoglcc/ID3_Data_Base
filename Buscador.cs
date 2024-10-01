using System;
using System.Collections.Generic;
using System.Data.SQLite;

public class Buscador {
    private string connectionString = "Data Source=music_library.db;Version=3;";


    public class Criterio {
        public string Nombre { get; set; }    
        public string Valor { get; set; }     
        public bool EsExclusivo { get; set; } 

        public Criterio(string nombre, string valor, bool esExclusivo) {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Valor = valor ?? throw new ArgumentNullException(nameof(valor));
            EsExclusivo = esExclusivo;
        }
    }

    public List<Cancion> Buscar(List<Criterio> criterios) {
        List<Cancion> resultados = new List<Cancion>();
    try {
        string query = ConstruirConsulta(criterios);

        using (SQLiteConnection connection = new SQLiteConnection(connectionString)) {
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(query, connection);

            // Agregar parámetros a la consulta (para evitar SQL injection)
            foreach (var criterio in criterios) {
                command.Parameters.AddWithValue($"@{criterio.Nombre}", $"%{criterio.Valor}%");
            }

            // Ejecutar la consulta y leer los resultados
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                // Leer los resultados y manejar posibles valores nulos
                Cancion cancion = new Cancion
                {
                    Id = reader["id_rola"] != DBNull.Value ? Convert.ToInt32(reader["id_rola"]) : 0,
                    Titulo = reader["title"] != DBNull.Value ? reader["title"]?.ToString() ?? "Unknown" : "Unknown",
                    Album = reader["name"] != DBNull.Value ? reader["name"]?.ToString() ?? "Unknown" : "Unknown",
                    Intérprete = reader["performer"] != DBNull.Value ? reader["performer"]?.ToString() ?? "Unknown" : "Unknown",
                    Año = reader["year"] != DBNull.Value ? Convert.ToInt32(reader["year"]) : ObtenerFechaDeArchivo(reader["path"]?.ToString()),
                    Género = reader["genre"] != DBNull.Value ? reader["genre"]?.ToString() ?? "Unknown" : "Unknown",
                    Pista = reader["track"] != DBNull.Value ? Convert.ToInt32(reader["track"]) : 1,
                    Path = reader["path"] != DBNull.Value ? reader["path"]?.ToString() ?? "Ruta no disponible" : "Ruta no disponible"
                };

                resultados.Add(cancion);
            }
        }
    }
    catch (Exception ex) {
        Console.WriteLine($"Error durante la búsqueda: {ex.Message}");
    }

    return resultados;
}

    private int ObtenerFechaDeArchivo(string? path) {
        if (string.IsNullOrEmpty(path)) {
            return DateTime.Now.Year;
        }
        try {
            DateTime creationTime = System.IO.File.GetCreationTime(path);
            return creationTime.Year;
        }
        catch {
            return DateTime.Now.Year;
        }
    }

    private string ConstruirConsulta(List<Criterio> criterios)
    {
        string query = "SELECT rolas.id_rola, rolas.title, albums.name, performers.name as performer, rolas.year, rolas.genre, rolas.track, rolas.path " +
                       "FROM rolas " +
                       "JOIN albums ON rolas.id_album = albums.id_album " +
                       "JOIN performers ON rolas.id_performer = performers.id_performer ";

        if (criterios.Count > 0) {
            List<string> condicionesExclusivas = new List<string>();
            List<string> condicionesNoExclusivas = new List<string>();
            
            foreach (var criterio in criterios) {
                if (criterio.EsExclusivo) {
                    condicionesExclusivas.Add($"{criterio.Nombre} LIKE @{criterio.Nombre}");
                }
                else {
                    condicionesNoExclusivas.Add($"{criterio.Nombre} LIKE @{criterio.Nombre}");
                }
            }

            List<string> todasCondiciones = new List<string>();

            if (condicionesExclusivas.Count > 0) {
                todasCondiciones.Add("(" + string.Join(" AND ", condicionesExclusivas) + ")");
            }
            if (condicionesNoExclusivas.Count > 0) {
                todasCondiciones.Add("(" + string.Join(" OR ", condicionesNoExclusivas) + ")");
            }
            if (todasCondiciones.Count > 0) {
                query += "WHERE " + string.Join(" AND ", todasCondiciones);
            }
        }
        return query;
    }
}

// Clase auxiliar para representar una canción
public class Cancion {
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Album { get; set; }
    public string Intérprete { get; set; }
    public int Año { get; set; }
    public string Género { get; set; }
    public int Pista { get; set; }
    public string Path { get; set; }

    public Cancion() {
        Titulo = "Unknown";
        Album = "Unknown";
        Intérprete = "Unknown";
        Año = DateTime.Now.Year;
        Género = "Unknown";
        Pista = 1;
        Path = "Ruta no disponible";
    }
}
