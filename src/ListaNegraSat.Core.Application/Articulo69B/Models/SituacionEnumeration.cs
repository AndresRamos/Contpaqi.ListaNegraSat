using ListaNegraSat.Core.Application.Common;

namespace ListaNegraSat.Core.Application.Articulo69B.Models;

public class SituacionEnumeration : Enumeration
{
    public static readonly SituacionEnumeration Todo = new(0, "Todo");
    public static readonly SituacionEnumeration Definitivos = new(1, "Definitivo");
    public static readonly SituacionEnumeration Desvirtuados = new(2, "Desvirtuado");
    public static readonly SituacionEnumeration Presuntos = new(3, "Presunto");
    public static readonly SituacionEnumeration SentenciaFavorable = new(4, "Sentencia Favorable");

    public SituacionEnumeration(int id, string name) : base(id, name)
    {
    }
}
