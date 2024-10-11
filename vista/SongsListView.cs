using MusicApp.Modelo;

namespace MusicApp.Vista {

    using Gtk;
    using System.Collections.Generic;

    public class SongsListView : FlowBox
    {
        public SongsListView() : base() {
            this.SelectionMode = SelectionMode.None;  // No es necesario habilitar la selección en FlowBox
        }

        // Método para limpiar la vista actual (remover todos los botones)
        public void LimpiarVista() {
            foreach (Widget widget in this.Children)
            {
                this.Remove(widget);  // Remover cada widget actual
            }
        }

        // Método para agregar un botón a la vista
        public void MostrarBoton(Button boton) {
            this.Add(boton);  // Agregar el botón a la vista
        }

        // Método para actualizar la vista después de agregar todos los botones
        public void ActualizarVista() {
            this.ShowAll();  // Refrescar la vista para mostrar todos los botones
        }

        public void MostrarScroll(ScrolledWindow scrolledWindow) {
            this.Add(scrolledWindow);  // Agregar el contenedor con scroll a la vista
        }

        // Nuevo método para cargar y mostrar las canciones
        public void CargarCancionesConEncabezado(List<Cancion> canciones, System.Action<Cancion> OnCancionSeleccionada)
        {
            LimpiarVista();  // Limpiar la vista actual

            // Crear un contenedor principal para todo (encabezado + lista de canciones)
            Box listaCompleta = new Box(Orientation.Vertical, 5);

            // Crear un contenedor horizontal para la barra de encabezado
            Box encabezado = new Box(Orientation.Horizontal, 10);

            // Crear los encabezados para Título, Artista y Álbum
            Label encabezadoTitulo = new Label("Título");
            encabezadoTitulo.SetSizeRequest(400, -1);  // Tamaño mínimo para que se vea bien
            Label encabezadoArtista = new Label("Artista");
            encabezadoArtista.SetSizeRequest(400, -1);  // Tamaño mínimo para que se vea bien
            Label encabezadoAlbum = new Label("Álbum");
            encabezadoAlbum.SetSizeRequest(400, -1);  // Tamaño mínimo para que se vea bien

            // Asegurar que los encabezados ocupen bien su espacio
            encabezado.PackStart(encabezadoTitulo, false, false, 0);
            encabezado.PackStart(encabezadoArtista, false, false, 0);
            encabezado.PackEnd(encabezadoAlbum, false, false, 0);

            // Añadir la barra de encabezado al contenedor principal
            listaCompleta.PackStart(encabezado, false, false, 10);  // Sin expandir, con un margen de 10

            // Crear un contenedor vertical para las canciones
            Box listaCanciones = new Box(Orientation.Vertical, 5);

            // Iterar sobre la lista de canciones
            foreach (var cancion in canciones)
            {
                // Crear un contenedor horizontal para cada canción
                Box boxCancion = new Box(Orientation.Horizontal, 10);

                // Crear etiquetas para el título, artista y álbum (sin prefijos)
                Label tituloLabel = new Label(cancion.Titulo);
                tituloLabel.SetSizeRequest(400, -1);  // Tamaño mínimo para que se vea bien
                Label artistaLabel = new Label(cancion.Intérprete);
                artistaLabel.SetSizeRequest(400, -1);  // Tamaño mínimo para que se vea bien
                Label albumLabel = new Label(cancion.Album);
                albumLabel.SetSizeRequest(400, -1);  // Tamaño mínimo para que se vea bien

                // Añadir las etiquetas de la canción al contenedor horizontal
                boxCancion.PackStart(tituloLabel, false, false, 0);   // Alinear a la izquierda
                boxCancion.PackStart(artistaLabel, false, false, 0);  // Centrar en el medio
                boxCancion.PackEnd(albumLabel, false, false, 0);      // Alinear a la derecha

                // Crear el botón con el contenedor dentro
                Button botonCancion = new Button();
                botonCancion.Add(boxCancion);
                botonCancion.Clicked += (sender, e) => OnCancionSeleccionada(cancion);
                botonCancion.Margin = 5;  // Añadir margen para separar los botones

                listaCanciones.PackStart(botonCancion, false, false, 0);  // Añadir los botones sin expandir
            }

            // Añadir la lista de canciones al contenedor principal
            listaCompleta.PackStart(listaCanciones, true, true, 0);  // Expandir y llenar

            // Crear un contenedor de desplazamiento (scroll)
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetSizeRequest(1300, 600);  // Ajustar el tamaño del ScrolledWindow
            scrolledWindow.Add(listaCompleta);  // Añadir la lista completa al contenedor con scroll

            MostrarScroll(scrolledWindow);  // Mostrar la lista con scroll en la vista

            ActualizarVista();  // Refrescar la vista
        }
    }
}
