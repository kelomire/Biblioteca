using System.Text.Json;
using Aplicacion.Cadena;
using Aplicacion.Interfaces;

namespace Persistencia.Repositorios;

public class RegistroBlockchainArchivo : IRegistroInmutable
{
    private readonly string _ruta;
    private readonly Blockchain _cadena;

    public RegistroBlockchainArchivo(string ruta = "blockchain.json")
    {
        _ruta = ruta;
        List<Block>? bloques = null;

        if (File.Exists(ruta))
            bloques = JsonSerializer.Deserialize<List<Block>>(File.ReadAllText(ruta));

        _cadena = new Blockchain(bloques);
        Guardar();
    }

    public void Registrar(string tipo, int libroId, int personaId)
    {
        var data = JsonSerializer.Serialize(new
        {
            tipo,
            libroId,
            personaId,
            fecha = DateTime.UtcNow
        });
        _cadena.Agregar(data);
        Guardar();
    }

    public bool Verificar() => _cadena.EsValida();

    public IReadOnlyList<Block> ObtenerHistorial() => _cadena.Bloques;

    private void Guardar()
    {
        var json = JsonSerializer.Serialize(_cadena.Bloques,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_ruta, json);
    }
}
