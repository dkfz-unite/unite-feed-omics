using FluentValidation;
using Unite.Data.Constants;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class ResourceModelValidator : AbstractValidator<ResourceModel>
{
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
        FileTypes.Sequence.Mzml,
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
