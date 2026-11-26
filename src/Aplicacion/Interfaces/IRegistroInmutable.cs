using Aplicacion.Cadena;

namespace Aplicacion.Interfaces;

public interface IRegistroInmutable
{
    void Registrar(string tipo, int libroId, int personaId);
    bool Verificar();
    IReadOnlyList<Block> ObtenerHistorial();
}
