using Aplicacion.Entidades;
using Aplicacion.Interfaces;
using Microsoft.Data.Sqlite;
using Persistencia.Conexion;

namespace Persistencia.Repositorios;

public class PersonaRepositorio : IPersonaRepositorio
{
    private const string Columnas = "id, nombre, apellido, tipo_persona, curso, materia, cargo, activo";
    private readonly DbConnectionFactory _fabrica;

    public PersonaRepositorio(DbConnectionFactory fabrica) => _fabrica = fabrica;

    public int Agregar(Persona p)
    {
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO personas (nombre, apellido, tipo_persona, curso, materia, cargo, activo)
                            VALUES ($nombre, $apellido, $tipo, $curso, $materia, $cargo, $activo);
                            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$nombre", p.Nombre);
        cmd.Parameters.AddWithValue("$apellido", p.Apellido);
        cmd.Parameters.AddWithValue("$tipo", p.Tipo.ToString().ToUpperInvariant());
        cmd.Parameters.AddWithValue("$curso", (object?)p.Curso ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$materia", (object?)p.Materia ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$cargo", (object?)p.Cargo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$activo", p.Activo ? 1 : 0);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public Persona? ObtenerPorId(int id)
    {
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columnas} FROM personas WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public List<Persona> Listar()
    {
        var lista = new List<Persona>();
        using var c = _fabrica.Crear();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columnas} FROM personas ORDER BY apellido, nombre";
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    private static Persona Mapear(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(0),
        Nombre = r.GetString(1),
        Apellido = r.GetString(2),
        Tipo = Enum.Parse<TipoPersona>(r.GetString(3), ignoreCase: true),
        Curso = r.IsDBNull(4) ? null : r.GetString(4),
        Materia = r.IsDBNull(5) ? null : r.GetString(5),
        Cargo = r.IsDBNull(6) ? null : r.GetString(6),
        Activo = r.GetInt32(7) == 1
    };
}
