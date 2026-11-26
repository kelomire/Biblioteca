namespace Aplicacion.Entidades;

public enum EstadoLibro { Disponible, Prestado, Baja }

public class Libro
{
    public int Id { get; set; }
    public string CodigoBarra { get; set; } = "";
    public string Titulo { get; set; } = "";
    public string Autor { get; set; } = "";
    public string Editorial { get; set; } = "";
    public int Anio { get; set; }
    public EstadoLibro Estado { get; set; } = EstadoLibro.Disponible;
}
