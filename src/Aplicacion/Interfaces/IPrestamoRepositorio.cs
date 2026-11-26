using Aplicacion.Entidades;

namespace Aplicacion.Interfaces;

public interface IPrestamoRepositorio
{
    // Guarda el préstamo y marca el libro como PRESTADO (en una sola transacción)
    int Registrar(Prestamo prestamo);

    Prestamo? ObtenerAbiertoPorLibro(int libroId);

    // Marca el préstamo como DEVUELTO y el libro como DISPONIBLE (en una sola transacción)
    void MarcarDevuelto(int prestamoId, DateTime fecha);

    List<Prestamo> Listar();
}
