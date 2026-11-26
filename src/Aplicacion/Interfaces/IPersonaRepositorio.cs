using Aplicacion.Entidades;

namespace Aplicacion.Interfaces;

public interface IPersonaRepositorio
{
    int Agregar(Persona persona);
    Persona? ObtenerPorId(int id);
    List<Persona> Listar();
}
