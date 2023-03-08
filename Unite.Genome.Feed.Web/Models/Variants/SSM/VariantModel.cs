using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Utilities.Mutations;

namespace Unite.Genome.Feed.Web.Models.Variants.SSM;

public class VariantModel
{
    private Chromosome? _chromosome;
    private string _position;
    private string _ref;
    private string _alt;


    /// <summary>
    /// Mutation chromosome
    /// </summary>
    public Chromosome? Chromosome { get => _chromosome; set => _chromosome = value; }

    /// <summary>
    /// Mutation position in chromosome (Number "10110" or range "10110-10115" string)
    /// </summary>
    public string Position { get => _position?.Trim(); set => _position = value; }

    /// <summary>
    /// Reference base
    /// </summary>
    public string Ref { get => _ref?.Trim(); set => _ref = value; }

    /// <summary>
    /// Alternate base
    /// </summary>
    public string Alt { get => _alt?.Trim(); set => _alt = value; }


    public string GetCode()
    {
        return HGVsCodeGenerator.Generate(Chromosome.Value, Position, Ref, Alt);
    }


    #region Equality
    public override bool Equals(object obj)
    {
        var other = obj as VariantModel;

        if (other == null) return false;

        return Chromosome == other.Chromosome
            && Position == other.Position
            && Ref == other.Ref
            && Alt == other.Alt;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 36613;

            hash = hash * 37724 + Chromosome.GetHashCode();
            hash = hash * 37724 + Position.GetHashCode();
            hash = hash * 37724 + Ref.GetHashCode();
            hash = hash * 37724 + Alt.GetHashCode();
            
            return hash;
        }
    }
    #endregion
}
