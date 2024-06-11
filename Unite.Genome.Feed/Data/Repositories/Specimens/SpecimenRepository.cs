using Unite.Data.Context;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Data.Repositories.Specimens;

internal class SpecimenRepository
{
    private readonly DonorRepository _donorRepository;
    private readonly MaterialRepository _materialRepository;
    private readonly LineRepository _lineRepository;
    private readonly OrganoidRepository _organoidRepository;
    private readonly XenograftRepository _xenograftRepository;


    public SpecimenRepository(DomainDbContext dbContext)
    {
        _donorRepository = new DonorRepository(dbContext);
        _materialRepository = new MaterialRepository(dbContext);
        _lineRepository = new LineRepository(dbContext);
        _organoidRepository = new OrganoidRepository(dbContext);
        _xenograftRepository = new XenograftRepository(dbContext);
    }


    public Specimen FindOrCreate(SpecimenModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Specimen Find(SpecimenModel model)
    {
        var donor = _donorRepository.Find(model.Donor);

        if (donor == null)
            return null;

        return model.Type switch
        {
            SpecimenType.Material => _materialRepository.Find(donor.Id, model.ReferenceId),
            SpecimenType.Line => _lineRepository.Find(donor.Id, model.ReferenceId),
            SpecimenType.Organoid => _organoidRepository.Find(donor.Id, model.ReferenceId),
            SpecimenType.Xenograft => _xenograftRepository.Find(donor.Id, model.ReferenceId),
            _ => throw new NotSupportedException("Specimen type is not supported")
        };
    }

    public Specimen Create(SpecimenModel model)
    {
        var donor = _donorRepository.FindOrCreate(model.Donor);

        return model.Type switch
        {
            SpecimenType.Material => _materialRepository.Create(donor.Id, model.ReferenceId),
            SpecimenType.Line => _lineRepository.Create(donor.Id, model.ReferenceId),
            SpecimenType.Organoid => _organoidRepository.Create(donor.Id, model.ReferenceId),
            SpecimenType.Xenograft => _xenograftRepository.Create(donor.Id, model.ReferenceId),
            _ => throw new NotSupportedException("Specimen type is not supported")
        };
    }
}
