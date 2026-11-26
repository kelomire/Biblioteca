using System.Security.Cryptography;
using System.Text;

namespace Aplicacion.Cadena;

public class Block
{
    public int Index { get; set; }
    public DateTime Timestamp { get; set; }
    public string Data { get; set; } = "";
    public string PreviousHash { get; set; } = "";
    public string Hash { get; set; } = "";
    public int Nonce { get; set; }

    public string CalculateHash()
    {
        var raw = $"{Index}|{Timestamp.Ticks}|{Data}|{PreviousHash}|{Nonce}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
    }

    public void Mine(int difficulty)
    {
        var prefijo = new string('0', difficulty);
        Hash = CalculateHash();
        while (!Hash.StartsWith(prefijo))
        {
            Nonce++;
            Hash = CalculateHash();
        }
    }
}
