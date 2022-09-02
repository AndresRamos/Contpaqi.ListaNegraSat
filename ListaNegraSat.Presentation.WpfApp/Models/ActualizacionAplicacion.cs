using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Caliburn.Micro;

namespace ListaNegraSat.Presentation.WpfApp.Models;

public class ActualizacionAplicacion : PropertyChangedBase
{
    private string _mensaje;

    public bool ActualizacionDisponible { get; set; }

    public string Mensaje
    {
        get => _mensaje;
        set
        {
            if (value == _mensaje)
            {
                return;
            }

            _mensaje = value;
            NotifyOfPropertyChange(() => Mensaje);
        }
    }

    public Version VersionActual { get; set; }

    public Version VersionNueva { get; set; }

    public async Task ChecarActualizacionDisponibleAsync()
    {
        try
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var container = new BlobContainerClient(new Uri("https://arsoftware.blob.core.windows.net/arsoftwaredownloads/"));
            BlobClient blobClient = container.GetBlobClient(@"ListaNegraSat\CurrentVersion.txt");

            string currentverstion;
            using (var memoryStream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(memoryStream);
                currentverstion = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            VersionActual = new Version(version);
            VersionNueva = new Version(currentverstion);

            int result = VersionActual.CompareTo(VersionNueva);
            if (result < 0)
            {
                ActualizacionDisponible = true;
                Mensaje = $"Hay una actualizacion disponible (version: {VersionNueva}). Desea descargarla?";
            }
            else if (result == 0)
            {
                Mensaje = $"Ya esta corriendo la ultima version disponible (version: {VersionActual}).";
            }
            else
            {
                Mensaje =
                    $"Esta corriendo una verion mas reciente que la que esta en el servicio de actualizaciones. (version: {VersionActual}).";
            }
        }
        catch (Exception e)
        {
            Mensaje = $"No se pudo conectar al servicio de actualizaciones. Error: {e.Message}";
        }
    }

    public async Task DescargarActualizacionAsync(string fileName)
    {
        var container = new BlobContainerClient(new Uri("https://arsoftware.blob.core.windows.net/arsoftwaredownloads/"));
        BlobClient blobClient = container.GetBlobClient(@"ListaNegraSat\Release.zip");
        await blobClient.DownloadToAsync(fileName);
    }
}
