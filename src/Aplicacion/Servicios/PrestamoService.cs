using Aplicacion.Cadena;
using Aplicacion.Entidades;
using Aplicacion.Interfaces;

namespace Aplicacion.Servicios;

public class PrestamoService
{
    private readonly ILibroRepositorio _libros;
    private readonly IPersonaRepositorio _personas;
    private readonly IPrestamoRepositorio _prestamos;
    private readonly IRegistroInmutable _registro;

    public PrestamoService(ILibroRepositorio libros, IPersonaRepositorio personas,
                           IPrestamoRepositorio prestamos, IRegistroInmutable registro)
    {
        _libros = libros;
        _personas = personas;
        _prestamos = prestamos;
        _registro = registro;
    }

    public int Prestar(string codigoBarra, int personaId)
    {
        var libro = _libros.ObtenerPorCodigoBarra(codigoBarra)
            ?? throw new InvalidOperationException("No existe un libro con ese código de barras.");

        if (libro.Estado != EstadoLibro.Disponible)
            throw new InvalidOperationException($"El libro no está disponible (estado: {libro.Estado}).");

        var persona = _personas.ObtenerPorId(personaId)
            ?? throw new InvalidOperationException("No existe esa persona.");

        if (!persona.Activo)
            throw new InvalidOperationException("La persona está inactiva.");

        // 1) Base de datos
        var id = _prestamos.Registrar(new Prestamo
        {
            PersonaId = persona.Id,
            LibroId = libro.Id,
            FechaPrestamo = DateTime.Today
        });

        // 2) Historial inmutable
        _registro.Registrar("Prestamo", libro.Id, persona.Id);
        return id;
    }

    public void Devolver(string codigoBarra)
    {
        var libro = _libros.ObtenerPorCodigoBarra(codigoBarra)
            ?? throw new InvalidOperationException("No existe un libro con ese código de barras.");

        var abierto = _prestamos.ObtenerAbiertoPorLibro(libro.Id)
            ?? throw new InvalidOperationException("Ese libro no tiene un préstamo abierto.");

        _prestamos.MarcarDevuelto(abierto.Id, DateTime.Today);
        _registro.Registrar("Devolucion", libro.Id, abierto.PersonaId);
    }

    public List<Prestamo> Listar() => _prestamos.Listar();

    public IReadOnlyList<Block> ObtenerHistorial() => _registro.ObtenerHistorial();

    public bool VerificarIntegridad() => _registro.Verificar();
}
