using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Models.Enums;

namespace Unite.Genome.Feed.Data.Repositories.Specimens;

internal class SpecimenRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly DonorRepository _donorRepository;
    private readonly TissueRepository _tissueRepository;
    private readonly CellLineRepository _cellLineRepository;
    private readonly OrganoidRepository _organoidRepository;
    private readonly XenograftRepository _xenograftRepository;


    public SpecimenRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _donorRepository = new DonorRepository(dbContext);
        _tissueRepository = new TissueRepository(dbContext);
        _cellLineRepository = new CellLineRepository(dbContext);
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
        {
            return null;
        }

        if (model.Type == SpecimenType.Tissue)
        {
            return _tissueRepository.Find(donor.Id, model.ReferenceId);
        }
        else if (model.Type == SpecimenType.CellLine)
        {
            return _cellLineRepository.Find(donor.Id, model.ReferenceId);
        }
        else if (model.Type == SpecimenType.Organoid)
        {
            return _organoidRepository.Find(donor.Id, model.ReferenceId);
        }
        else if (model.Type == SpecimenType.Xenograft)
        {
            return _xenograftRepository.Find(donor.Id, model.ReferenceId);
        }
        else
        {
            throw new NotSupportedException("Specimen type is not supported");
        }
    }

    public Specimen Create(SpecimenModel model)
    {
        var donor = _donorRepository.FindOrCreate(model.Donor);

        if (model.Type == SpecimenType.Tissue)
        {
            return _tissueRepository.Create(donor.Id, model.ReferenceId);
        }
        else if (model.Type == SpecimenType.CellLine)
        {
            return _cellLineRepository.Create(donor.Id, model.ReferenceId);
        }
        else if (model.Type == SpecimenType.Organoid)
        {
            return _organoidRepository.Create(donor.Id, model.ReferenceId);
        }
        else if (model.Type == SpecimenType.Xenograft)
        {
            return _xenograftRepository.Create(donor.Id, model.ReferenceId);
        }
        else
        {
            throw new NotSupportedException("Specimen type is not supported");
        }
    }
}
