namespace Aplicacion.Entidades;

public enum EstadoPrestamo { Prestado, Devuelto }

public class Prestamo
{
    public int Id { get; set; }
    public int PersonaId { get; set; }
    public int LibroId { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public EstadoPrestamo Estado { get; set; } = EstadoPrestamo.Prestado;
}
