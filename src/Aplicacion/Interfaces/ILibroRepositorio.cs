using Aplicacion.Entidades;

namespace Aplicacion.Interfaces;

public interface ILibroRepositorio
{
    int Agregar(Libro libro);
    Libro? ObtenerPorId(int id);
    Libro? ObtenerPorCodigoBarra(string codigoBarra);
    List<Libro> Listar();
}
