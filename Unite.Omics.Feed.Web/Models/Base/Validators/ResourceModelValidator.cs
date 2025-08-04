using FluentValidation;
using Unite.Data.Constants;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class ResourceModelValidator : AbstractValidator<ResourceModel>
{
    private static readonly string[] _allowedTypes =
    [
        DataTypes.Omics.Dna.Sample,
        DataTypes.Omics.Dna.Sm,
        DataTypes.Omics.Dna.Cnv,
        DataTypes.Omics.Dna.Sv,
        DataTypes.Omics.Meth.Sample,
        DataTypes.Omics.Meth.Level,
        DataTypes.Omics.Rna.Sample,
        DataTypes.Omics.Rna.Exp,
        DataTypes.Omics.Rnasc.Sample,
        DataTypes.Omics.Rnasc.Exp
    ];

    private static readonly string[] _allowedFormats = 
    [
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
    ];

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

        RuleFor(model => model.Url)
            .NotEmpty()
            .WithMessage("Should not be empty");
    }
}
