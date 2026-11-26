using Aplicacion.Cadena;
using Aplicacion.Interfaces;

namespace Aplicacion.Servicios;

public class PrestamoService
{
    private readonly IRegistroInmutable _registro;

    public PrestamoService(IRegistroInmutable registro)
    {
        _registro = registro;
    }

    public void RegistrarPrestamo(int libroId, int personaId)
    {
        // TODO: guardar el préstamo en la BD con IPrestamoRepositorio
        _registro.Registrar("Prestamo", libroId, personaId);
    }

    public void RegistrarDevolucion(int libroId, int personaId)
    {
        // TODO: actualizar la BD con IPrestamoRepositorio
        _registro.Registrar("Devolucion", libroId, personaId);
    }

    public IReadOnlyList<Block> ObtenerHistorial() => _registro.ObtenerHistorial();

    public bool VerificarIntegridad() => _registro.Verificar();
}
