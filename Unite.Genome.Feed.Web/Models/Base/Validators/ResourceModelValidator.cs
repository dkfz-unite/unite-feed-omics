using FluentValidation;
using Unite.Data.Constants;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class ResourceModelValidator : AbstractValidator<ResourceModel>
{
    private static readonly string[] _allowedTypes =
    {
        DataTypes.Genome.Dna.Sample,
        DataTypes.Genome.Dna.Sm,
        DataTypes.Genome.Dna.Cnv,
        DataTypes.Genome.Dna.Sv,
        DataTypes.Genome.Meth.Sample,
        DataTypes.Genome.Meth.Level,
        DataTypes.Genome.Rna.Sample,
        DataTypes.Genome.Rna.Exp,
        DataTypes.Genome.Rnasc.Sample,
        DataTypes.Genome.Rnasc.Exp
    };

    private static readonly string[] _allowedFormats = 
    {
        FileTypes.General.Txt,
        FileTypes.General.Csv,
        FileTypes.General.Tsv,
        FileTypes.Sequence.Fasta,
        FileTypes.Sequence.Fastq,
        FileTypes.Sequence.Bam,
        FileTypes.Sequence.BamBai,
        FileTypes.Sequence.BamBaiMd5,
        FileTypes.Sequence.Idat,
        FileTypes.Sequence.Mtx,
        FileTypes.Sequence.Vcf
    };

    private static readonly string[] _allowedArchives = 
    {
        ArchiveTypes.Zip,
        ArchiveTypes.Gz
    };

    public ResourceModelValidator()
    {
        RuleFor(model => model.Name)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Name)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Type)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Type)
            .Must(type => _allowedTypes.Contains(type))
            .WithMessage("Type is not allowed");

        RuleFor(model => model.Format)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Format)
            .Must(format => _allowedFormats.Contains(format))
            .WithMessage("Format is not allowed");

        RuleFor(model => model.Archive)
            .Must(format => _allowedArchives.Contains(format))
            .When(model => model.Archive != null)
            .WithMessage("Archive is not allowed");

        RuleFor(model => model.Url)
            .NotEmpty()
            .WithMessage("Should not be empty");
    }
}
