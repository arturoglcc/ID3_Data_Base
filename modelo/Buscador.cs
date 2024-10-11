namespace MusicApp.Modelo {

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

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
                Cancion cancion = new Cancion(
                    reader["title"] != DBNull.Value ? reader["title"]?.ToString() ?? "Unknown" : "Unknown",
                    reader["name"] != DBNull.Value ? reader["name"]?.ToString() ?? "Unknown" : "Unknown",
                    reader["performer"] != DBNull.Value ? reader["performer"]?.ToString() ?? "Unknown" : "Unknown",
                    reader["year"] != DBNull.Value ? Convert.ToInt32(reader["year"]) : ObtenerFechaDeArchivo(reader["path"]?.ToString()),
                    reader["genre"] != DBNull.Value ? reader["genre"]?.ToString() ?? "Unknown" : "Unknown",
                    reader["track"] != DBNull.Value ? Convert.ToInt32(reader["track"]) : 1,
                    reader["path"] != DBNull.Value ? reader["path"]?.ToString() ?? "Ruta no disponible" : "Ruta no disponible", // Usar el path correcto
                    false
                );
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

public class Cancion
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Album { get; set; }
    public string Intérprete { get; set; }
    public int Año { get; set; }
    public string Genero { get; set; }
    public int Pista { get; set; }
    public string Path { get; set; }
    public bool EsGrupo { get; set; } // Nueva propiedad booleana
    public byte[]? ImagenPortada { get; set; } // Propiedad para la imagen de portada
    // Nuevos campos para grupos
    public string? FechaInicioGrupo { get; set; }  // Puede ser null
    public string? FechaFinGrupo { get; set; }     // Puede ser null
    public List<string>? Integrantes { get; set; } // Lista de integrantes, puede ser null
    // Constructor por defecto con valores predefinidos

        // Constructor opcional para pasar parámetros
    public Cancion(string titulo, string album, string intérprete, int año, string genero, int pista, string path, bool esGrupo,
                   string? fechaInicioGrupo = null, string? fechaFinGrupo = null, List<string>? integrantes = null, byte[]? imagenPortada = null)
    {
        Titulo = titulo;
        Album = album;
        Intérprete = intérprete;
        Año = año;
        Genero = genero;
        Pista = pista;
        Path = path;
        EsGrupo = esGrupo;
        FechaInicioGrupo = fechaInicioGrupo;
        FechaFinGrupo = fechaFinGrupo;
        Integrantes = integrantes;
        ImagenPortada = ObtenerImagenPortada(Path); // Usar la imagen proporcionada o asignar la de por defecto
    }

    // Método para intentar obtener la imagen de portada de las etiquetas ID3
    private byte[] ObtenerImagenPortada(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {   
            Console.Write("ruta por omision: " + path + " ");
            return ObtenerImagenPorDefecto(); // Si el archivo no existe o la ruta está vacía, usar la imagen por defecto
        }

        try
        {
            var archivo = TagLib.File.Create(path); // Cargar el archivo usando TagLib
            if (archivo.Tag.Pictures.Length > 0)
            {
                // Si tiene una imagen de portada, devolverla como byte[]
                return archivo.Tag.Pictures[0].Data.Data;
            }
            else
            {
                return ObtenerImagenPorDefecto(); // Si no tiene imagen, usar la imagen por defecto
            }
        }
        catch (Exception)
        {
            return ObtenerImagenPorDefecto(); // En caso de error, usar la imagen por defecto
        }
    }

        // Método para obtener la imagen por omisión desde un archivo .png
    private byte[] ObtenerImagenPorDefecto()
    {
        try
        {
            // Ruta relativa a la imagen en el directorio de salida
            string rutaImagen = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "imagen_por_omision.png");
            // Leer el archivo de la imagen
            return File.ReadAllBytes(rutaImagen);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar la imagen por defecto: {ex.Message}");
            return new byte[0]; // Retorna una imagen vacía en caso de error
        }
    }



        public override string ToString()
        {
            string infoCancion = $"ID: {Id}\n" +
                                 $"Título: {Titulo}\n" +
                                 $"Álbum: {Album}\n" +
                                 $"Intérprete: {Intérprete}\n" +
                                 $"Año: {Año}\n" +
                                 $"Género: {Genero}\n" +
                                 $"Pista: {Pista}\n" +
                                 $"Es Grupo: {(EsGrupo ? "Sí" : "No")}\n" +
                                 $"Path: {Path}\n";
            if (Integrantes != null && Integrantes.Count > 0)
            {
                string integrantes = string.Join(", ", Integrantes);
                infoCancion += $"Integrantes del Grupo: {integrantes}\n";
                infoCancion += $"Fecha de Inicio del Grupo: {FechaInicioGrupo ?? "No especificada"}\n";
                infoCancion += $"Fecha de Fin del Grupo: {FechaFinGrupo ?? "No especificada"}\n";
            }
            return infoCancion;
        }
    }
}