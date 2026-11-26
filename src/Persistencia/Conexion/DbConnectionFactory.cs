using System.Reflection;
using Microsoft.Data.Sqlite;

namespace Persistencia.Conexion;

public class DbConnectionFactory
{
    private readonly string _cadena;

    public DbConnectionFactory(string rutaArchivo = "biblioteca.db")
    {
        _cadena = $"Data Source={rutaArchivo}";
    }

    public SqliteConnection Crear()
    {
        var conexion = new SqliteConnection(_cadena);
        conexion.Open();
        using var cmd = conexion.CreateCommand();
        cmd.CommandText = "PRAGMA foreign_keys = ON;";
        cmd.ExecuteNonQuery();
        return conexion;
    }

    public void InicializarEsquema()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDL.sql")
            ?? throw new InvalidOperationException("No se encontró DDL.sql embebido en Persistencia.");
        using var lector = new StreamReader(stream);
        var sql = lector.ReadToEnd();

        using var conexion = Crear();
        using var cmd = conexion.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }
}
