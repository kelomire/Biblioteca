namespace Aplicacion.Cadena;

public class Blockchain
{
    private const int Dificultad = 3;
    private readonly List<Block> _bloques;

    public IReadOnlyList<Block> Bloques => _bloques;

    public Blockchain(IEnumerable<Block>? existentes = null)
    {
        _bloques = existentes?.ToList() ?? new List<Block>();

        if (_bloques.Count == 0)
        {
            var genesis = new Block
            {
                Index = 0,
                Timestamp = DateTime.UtcNow,
                Data = "Bloque génesis",
                PreviousHash = "0"
            };
            genesis.Mine(Dificultad);
            _bloques.Add(genesis);
        }
    }

    public Block Agregar(string data)
    {
        var ultimo = _bloques[^1];
        var bloque = new Block
        {
            Index = ultimo.Index + 1,
            Timestamp = DateTime.UtcNow,
            Data = data,
            PreviousHash = ultimo.Hash
        };
        bloque.Mine(Dificultad);
        _bloques.Add(bloque);
        return bloque;
    }

    public bool EsValida()
    {
        for (int i = 1; i < _bloques.Count; i++)
        {
            if (_bloques[i].Hash != _bloques[i].CalculateHash()) return false;
            if (_bloques[i].PreviousHash != _bloques[i - 1].Hash) return false;
        }
        return _bloques[0].Hash == _bloques[0].CalculateHash();
    }
}
