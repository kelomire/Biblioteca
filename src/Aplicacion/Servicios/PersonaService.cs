using Aplicacion.Entidades;
using Aplicacion.Interfaces;

namespace Aplicacion.Servicios;

public class PersonaService
{
    private readonly IPersonaRepositorio _personas;

    public PersonaService(IPersonaRepositorio personas) => _personas = personas;

    public int Agregar(string nombre, string apellido, TipoPersona tipo,
                       string? curso = null, string? materia = null, string? cargo = null)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            throw new InvalidOperationException("El nombre y el apellido son obligatorios.");

        return _personas.Agregar(new Persona
        {
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Tipo = tipo,
            Curso = string.IsNullOrWhiteSpace(curso) ? null : curso.Trim(),
            Materia = string.IsNullOrWhiteSpace(materia) ? null : materia.Trim(),
            Cargo = string.IsNullOrWhiteSpace(cargo) ? null : cargo.Trim()
        });
    }

    public List<Persona> Listar() => _personas.Listar();
}
