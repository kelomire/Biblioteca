using Aplicacion.Entidades;
using Aplicacion.Servicios;
using Persistencia.Conexion;
using Persistencia.Repositorios;

var fabrica = new DbConnectionFactory("biblioteca.db");
fabrica.InicializarEsquema();

var repoLibros = new LibroRepositorio(fabrica);
var repoPersonas = new PersonaRepositorio(fabrica);
var repoPrestamos = new PrestamoRepositorio(fabrica);
var registro = new RegistroBlockchainArchivo("blockchain.json");

var libros = new LibroService(repoLibros);
var personas = new PersonaService(repoPersonas);
var prestamos = new PrestamoService(repoLibros, repoPersonas, repoPrestamos, registro);

while (true)
{
    Console.WriteLine("\n--- BIBLIOTECA ---");
    Console.WriteLine("1) Agregar persona");
    Console.WriteLine("2) Agregar libro");
    Console.WriteLine("3) Listar personas");
    Console.WriteLine("4) Listar libros");
    Console.WriteLine("5) Prestar libro");
    Console.WriteLine("6) Devolver libro");
    Console.WriteLine("7) Listar préstamos");
    Console.WriteLine("8) Ver historial (blockchain)");
    Console.WriteLine("9) Verificar integridad del historial");
    Console.WriteLine("0) Salir");
    Console.Write("Opción: ");

    try
    {
        switch (Console.ReadLine())
        {
            case "1":
                var nombre = LeerTexto("Nombre");
                var apellido = LeerTexto("Apellido");
                var tipo = LeerTipoPersona();
                string? curso = null, materia = null, cargo = null;
                if (tipo == TipoPersona.Alumno) curso = LeerTexto("Curso");
                if (tipo == TipoPersona.Profesor) materia = LeerTexto("Materia");
                if (tipo is TipoPersona.Directivo or TipoPersona.Personal or TipoPersona.Preceptor)
                    cargo = LeerTexto("Cargo (opcional)", opcional: true);
                var idPersona = personas.Agregar(nombre, apellido, tipo, curso, materia, cargo);
                Console.WriteLine($"Persona creada con id {idPersona}.");
                break;

            case "2":
                var idLibro = libros.Agregar(
                    LeerTexto("Código de barras"), LeerTexto("Título"), LeerTexto("Autor"),
                    LeerTexto("Editorial"), LeerInt("Año"));
                Console.WriteLine($"Libro creado con id {idLibro}.");
                break;

            case "3":
                foreach (var p in personas.Listar())
                    Console.WriteLine($"[{p.Id}] {p.Apellido}, {p.Nombre} - {p.Tipo}{(p.Activo ? "" : " (inactiva)")}");
                break;

            case "4":
                foreach (var l in libros.Listar())
                    Console.WriteLine($"[{l.Id}] {l.CodigoBarra} | {l.Titulo} - {l.Autor} ({l.Anio}) | {l.Estado}");
                break;

            case "5":
                var idPrestamo = prestamos.Prestar(LeerTexto("Código de barras del libro"), LeerInt("Id de la persona"));
                Console.WriteLine($"Préstamo {idPrestamo} registrado.");
                break;

            case "6":
                prestamos.Devolver(LeerTexto("Código de barras del libro"));
                Console.WriteLine("Devolución registrada.");
                break;

            case "7":
                foreach (var pr in prestamos.Listar())
                    Console.WriteLine($"[{pr.Id}] libro {pr.LibroId} -> persona {pr.PersonaId} | " +
                                      $"{pr.FechaPrestamo:yyyy-MM-dd} | {pr.Estado}" +
                                      (pr.FechaDevolucion is null ? "" : $" ({pr.FechaDevolucion:yyyy-MM-dd})"));
                break;

            case "8":
                foreach (var b in prestamos.ObtenerHistorial())
                    Console.WriteLine($"#{b.Index} {b.Timestamp:u} {b.Data}\n   hash: {b.Hash[..16]}...");
                break;

            case "9":
                Console.WriteLine(prestamos.VerificarIntegridad()
                    ? "Historial íntegro."
                    : "¡El historial fue alterado!");
                break;

            case "0":
                return;
        }
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static string LeerTexto(string etiqueta, bool opcional = false)
{
    while (true)
    {
        Console.Write($"{etiqueta}: ");
        var texto = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(texto) || opcional) return texto ?? "";
        Console.WriteLine("Este dato es obligatorio.");
    }
}

static int LeerInt(string etiqueta)
{
    while (true)
    {
        Console.Write($"{etiqueta}: ");
        if (int.TryParse(Console.ReadLine(), out var n)) return n;
        Console.WriteLine("Número inválido.");
    }
}

static TipoPersona LeerTipoPersona()
{
    var valores = Enum.GetValues<TipoPersona>();
    for (int i = 0; i < valores.Length; i++)
        Console.WriteLine($"  {i + 1}) {valores[i]}");
    while (true)
    {
        var n = LeerInt("Tipo de persona");
        if (n >= 1 && n <= valores.Length) return valores[n - 1];
        Console.WriteLine("Opción inválida.");
    }
}
