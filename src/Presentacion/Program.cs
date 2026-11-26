using Aplicacion.Servicios;
using Persistencia.Repositorios;

var registro = new RegistroBlockchainArchivo("blockchain.json");
var prestamos = new PrestamoService(registro);

while (true)
{
    Console.WriteLine("\n1) Registrar préstamo");
    Console.WriteLine("2) Registrar devolución");
    Console.WriteLine("3) Ver historial");
    Console.WriteLine("4) Verificar integridad");
    Console.WriteLine("0) Salir");
    Console.Write("Opción: ");

    switch (Console.ReadLine())
    {
        case "1":
            prestamos.RegistrarPrestamo(LeerInt("Id del libro"), LeerInt("Id de la persona"));
            Console.WriteLine("Préstamo registrado.");
            break;
        case "2":
            prestamos.RegistrarDevolucion(LeerInt("Id del libro"), LeerInt("Id de la persona"));
            Console.WriteLine("Devolución registrada.");
            break;
        case "3":
            foreach (var b in prestamos.ObtenerHistorial())
                Console.WriteLine($"#{b.Index} {b.Timestamp:u} {b.Data}\n   hash: {b.Hash[..16]}...");
            break;
        case "4":
            Console.WriteLine(prestamos.VerificarIntegridad()
                ? "Historial íntegro."
                : "¡El historial fue alterado!");
            break;
        case "0":
            return;
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
