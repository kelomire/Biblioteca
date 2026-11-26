using System.Globalization;
using Aplicacion.Entidades;
using Aplicacion.Interfaces;
using Microsoft.Data.Sqlite;
using Persistencia.Conexion;

namespace Persistencia.Repositorios;

public class PrestamoRepositorio : IPrestamoRepositorio
{
    private const string Columnas = "id, persona_id, libro_id, fecha_prestamo, fecha_devolucion, estado";
    private const string FormatoFecha = "yyyy-MM-dd";
    private readonly DbConnectionFactory _fabrica;

    public PrestamoRepositorio(DbConnectionFactory fabrica) => _fabrica = fabrica;

    public int Registrar(Prestamo p)
    {
        using var c = _fabrica.Crear();
        using var tx = c.BeginTransaction();

        using var insertar = c.CreateCommand();
        insertar.Transaction = tx;
        insertar.CommandText = @"INSERT INTO prestamos (persona_id, libro_id, fecha_prestamo, estado)
                                 VALUES ($persona, $libro, $fecha, 'PRESTADO');
                                 SELECT last_insert_rowid();";
        insertar.Parameters.AddWithValue("$persona", p.PersonaId);
        insertar.Parameters.AddWithValue("$libro", p.LibroId);
        insertar.Parameters.AddWithValue("$fecha", p.FechaPrestamo.ToString(FormatoFecha, CultureInfo.InvariantCulture));
        var id = Convert.ToInt32(insertar.ExecuteScalar());

        using var actualizar = c.CreateCommand();
        actualizar.Transaction = tx;
        actualizar.CommandText = "UPDATE libros SET estado = 'PRESTADO' WHERE id = $libro";
        actualizar.Parameters.AddWithValue("$libro", p.LibroId);
        actualizar.ExecuteNonQuery();

        tx.Commit();
        return id;
    }

    public Prestamo? ObtenerAbiertoPorLibro(int libroId)
    {
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columnas} FROM prestamos WHERE libro_id = $libro AND estado = 'PRESTADO'";
        cmd.Parameters.AddWithValue("$libro", libroId);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public void MarcarDevuelto(int prestamoId, DateTime fecha)
    {
        using var c = _fabrica.Crear();
        using var tx = c.BeginTransaction();

        using var actualizarPrestamo = c.CreateCommand();
        actualizarPrestamo.Transaction = tx;
        actualizarPrestamo.CommandText = @"UPDATE prestamos
                                           SET estado = 'DEVUELTO', fecha_devolucion = $fecha
                                           WHERE id = $id";
        actualizarPrestamo.Parameters.AddWithValue("$fecha", fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture));
        actualizarPrestamo.Parameters.AddWithValue("$id", prestamoId);
        actualizarPrestamo.ExecuteNonQuery();

        using var actualizarLibro = c.CreateCommand();
        actualizarLibro.Transaction = tx;
        actualizarLibro.CommandText = @"UPDATE libros SET estado = 'DISPONIBLE'
                                        WHERE id = (SELECT libro_id FROM prestamos WHERE id = $id)";
        actualizarLibro.Parameters.AddWithValue("$id", prestamoId);
        actualizarLibro.ExecuteNonQuery();

        tx.Commit();
    }

    public List<Prestamo> Listar()
    {
        var lista = new List<Prestamo>();
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columnas} FROM prestamos ORDER BY id DESC";
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    private static Prestamo Mapear(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(0),
        PersonaId = r.GetInt32(1),
        LibroId = r.GetInt32(2),
        FechaPrestamo = DateTime.Parse(r.GetString(3), CultureInfo.InvariantCulture),
        FechaDevolucion = r.IsDBNull(4) ? null : DateTime.Parse(r.GetString(4), CultureInfo.InvariantCulture),
        Estado = Enum.Parse<EstadoPrestamo>(r.GetString(5), ignoreCase: true)
    };
}
