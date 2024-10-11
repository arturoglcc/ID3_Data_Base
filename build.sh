#!/bin/bash

# Función para instalar .NET si no está instalado
install_dotnet() {
    echo "Instalando .NET SDK..."
    if [ "$(uname)" == "Linux" ]; then
        # Instalación para Linux
        sudo apt-get update
        sudo apt-get install -y dotnet-sdk-7.0
    elif [ "$(uname)" == "Darwin" ]; then
        # Instalación para macOS
        brew install --cask dotnet-sdk
    else
        echo "Sistema operativo no soportado para instalación automática de .NET."
        exit 1
    fi
    echo ".NET instalado correctamente."
}

# Verifica si .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo ".NET no está instalado. Procediendo con la instalación..."
    install_dotnet
else
    echo ".NET ya está instalado. Versión: $(dotnet --version)"
fi

# Instalar dependencias del proyecto
echo "Instalando dependencias del proyecto..."
dotnet restore

# Compilar el proyecto
echo "Compilando el proyecto..."
dotnet build ID3_DataBase.sln -c Release

# Crear el ejecutable basado en el sistema operativo
echo "Creando ejecutable..."
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    dotnet publish MiProyectoSQLite.csproj -r linux-x64 -c Release --self-contained true /p:PublishSingleFile=true --output ./proyecto
elif [[ "$OSTYPE" == "darwin"* ]]; then
    dotnet publish MiProyectoSQLite.csproj -r osx-x64 -c Release --self-contained true /p:PublishSingleFile=true --output ./proyecto
elif [[ "$OSTYPE" == "cygwin" || "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
    dotnet publish MiProyectoSQLite.csproj -r win-x64 -c Release --self-contained true /p:PublishSingleFile=true --output ./proyecto
else
    echo "Sistema operativo no reconocido."
    exit 1
fi

echo "¡Compilación y publicación completadas!"

# Limpiar archivos innecesarios
echo "Limpiando archivos intermedios..."
rm -rf bin/ obj/
rm -rf *.dll *.pdb *.json

echo "Archivos intermedios eliminados. ¡Todo listo!"
