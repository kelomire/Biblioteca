using Aplicacion.Entidades;
using Aplicacion.Interfaces;
using Microsoft.Data.Sqlite;
using Persistencia.Conexion;

namespace Persistencia.Repositorios;

public class LibroRepositorio : ILibroRepositorio
{
    private const string Columnas = "id, codigo_barra, titulo, autor, editorial, anio, estado";
    private readonly DbConnectionFactory _fabrica;

    public LibroRepositorio(DbConnectionFactory fabrica) => _fabrica = fabrica;

    public int Agregar(Libro l)
    {
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO libros (codigo_barra, titulo, autor, editorial, anio, estado)
                            VALUES ($cb, $titulo, $autor, $editorial, $anio, $estado);
                            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$cb", l.CodigoBarra);
        cmd.Parameters.AddWithValue("$titulo", l.Titulo);
        cmd.Parameters.AddWithValue("$autor", l.Autor);
        cmd.Parameters.AddWithValue("$editorial", l.Editorial);
        cmd.Parameters.AddWithValue("$anio", l.Anio);
        cmd.Parameters.AddWithValue("$estado", l.Estado.ToString().ToUpperInvariant());
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public Libro? ObtenerPorId(int id)
    {
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columnas} FROM libros WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public Libro? ObtenerPorCodigoBarra(string codigoBarra)
    {
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columnas} FROM libros WHERE codigo_barra = $cb";
        cmd.Parameters.AddWithValue("$cb", codigoBarra);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public List<Libro> Listar()
    {
        var lista = new List<Libro>();
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columnas} FROM libros ORDER BY titulo";
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    private static Libro Mapear(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(0),
        CodigoBarra = r.GetString(1),
        Titulo = r.GetString(2),
        Autor = r.GetString(3),
        Editorial = r.GetString(4),
        Anio = r.GetInt32(5),
        Estado = Enum.Parse<EstadoLibro>(r.GetString(6), ignoreCase: true)
    };
}
