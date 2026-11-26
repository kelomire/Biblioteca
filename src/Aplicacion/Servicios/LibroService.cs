using Aplicacion.Entidades;
using Aplicacion.Interfaces;

namespace Aplicacion.Servicios;

public class LibroService
{
    private readonly ILibroRepositorio _libros;

    public LibroService(ILibroRepositorio libros) => _libros = libros;

    public int Agregar(string codigoBarra, string titulo, string autor, string editorial, int anio)
    {
        if (string.IsNullOrWhiteSpace(codigoBarra) || string.IsNullOrWhiteSpace(titulo))
            throw new InvalidOperationException("El código de barras y el título son obligatorios.");

        if (_libros.ObtenerPorCodigoBarra(codigoBarra) is not null)
            throw new InvalidOperationException("Ya existe un ejemplar con ese código de barras.");

        return _libros.Agregar(new Libro
        {
            CodigoBarra = codigoBarra.Trim(),
            Titulo = titulo.Trim(),
            Autor = autor.Trim(),
            Editorial = editorial.Trim(),
            Anio = anio
        });
    }

    public List<Libro> Listar() => _libros.Listar();
}
