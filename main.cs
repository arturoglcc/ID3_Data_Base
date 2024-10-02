using System;
using Microsoft.Data.Sqlite;
using Gtk;

class Program
{
    static void Main(string[] args)
    {
        // Ruta predeterminada para minado (por ejemplo, la carpeta de Música del usuario)
        string rutaMusicaUsuario = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));

        // Conectar a la base de datos SQLite
        string connectionString = "Data Source=music_library.db;";
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            // Crear una instancia de Minero
            Minero minero = new Minero();

            // Crear tablas si no existen
            minero.CrearTablasSiNoExisten(connection);

            // Ejecutar el minado de la carpeta predeterminada
            minero.MinarDirectorio(connection, rutaMusicaUsuario);

            // Inicializar la aplicación GTK
            Application.Init();

            // Crear una instancia de MainView y mostrarla
            MainView mainWindow = new MainView();
        
        // Ejecutar la aplicación
        Application.Run();
        }
    }
}
