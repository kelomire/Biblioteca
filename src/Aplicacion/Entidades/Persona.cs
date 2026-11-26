namespace Aplicacion.Entidades;

public class Persona
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public TipoPersona Tipo { get; set; }
    public string? Curso { get; set; }
    public string? Materia { get; set; }
    public string? Cargo { get; set; }
    public bool Activo { get; set; } = true;
}
