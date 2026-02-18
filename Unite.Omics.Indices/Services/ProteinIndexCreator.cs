using Unite.Data.Constants;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Omics.Analysis;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Proteins;
using Unite.Omics.Indices.Services.Mappers;

namespace Unite.Omics.Indices.Services;

public class ProteinIndexCreator
{
    private readonly ProteinsIndexingCache _cache;


    public ProteinIndexCreator(ProteinsIndexingCache cache)
    {
        _cache = cache;
    }


    public ProteinIndex CreateIndex(object key)
    {
        var proteinId = (int)key;

        return CreateProteinIndex(proteinId);
    }


    private ProteinIndex CreateProteinIndex(int proteinId)
    {
        var protein = LoadProtein(proteinId);

        if (protein == null)
            return null;

        return CreateProteinIndex(protein);
    }

    private ProteinIndex CreateProteinIndex(Protein protein)
    {
        var index = ProteinIndexMapper.CreateFrom<ProteinIndex>(protein);

        index.Specimens = CreateSpecimenIndices(protein.Id);

        var hasSpecimens = index.Specimens?.Any();

        var genes = _cache.Proteins.Where(protein => protein.Transcript.GeneId == protein.Transcript.GeneId).Select(protein => protein.Transcript.GeneId).Distinct().ToArray();
        var hasGeneExpressions = _cache.GeneExpressions?.Any(entry => genes.Contains(entry.EntityId));
        var hasProteinExpressions = _cache.ProteinExpressions?.Any(entry => entry.EntityId == protein.Id);

        // If gene is not affected by any variant and has no expression data, it should be removed.
        if (hasSpecimens != true && hasGeneExpressions != true && hasProteinExpressions != true)
            return null;

        index.Stats = CreateStatsIndex(protein.Id);
        index.Data = CreateDataIndex(protein.Id);

        return index;
    }

    private Protein LoadProtein(int proteinId)
    {
        return _cache.Proteins.FirstOrDefault(protein => protein.Id == proteinId);
    }


    private SpecimenIndex[] CreateSpecimenIndices(int proteinId)
    {
        var specimens = LoadSpecimens(proteinId);

        return specimens.Select(specimen => CreateSpecimenIndex(proteinId, specimen)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(int proteinId, Specimen specimen)
    {
        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, null);

        var sampleId = _cache.Samples.FirstOrDefault(sample => sample.SpecimenId == specimen.Id)?.Id;
        index.Intensity = _cache.ProteinExpressions.FirstOrDefault(entry => entry.EntityId == proteinId && entry.SampleId == sampleId)?.Intensity;
        
        return index;
    }

    private Specimen[] LoadSpecimens(int proteinId)
    {
        var sampleIds = LoadSamples(proteinId).Select(sample => sample.Id).ToArray();
        var specimenIds = _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();

        return _cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).ToArray();
    }

    private Sample[] LoadSamples(int proteinId)
    {
        var sms = _cache.SmTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var cnvs = _cache.CnvTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var svs = _cache.SvTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var genes = _cache.Proteins.Where(protein => protein.Id == proteinId).Select(protein => protein.Transcript.GeneId).Distinct().ToArray();

        var smSamples = _cache.SmEntries.Where(entry => sms.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var cnvSamples = _cache.CnvEntries.Where(entry => cnvs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var svSamples = _cache.SvEntries.Where(entry => svs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var geneExpSamples = _cache.GeneExpressions.Where(entry => genes.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var proteinExpSamples = _cache.ProteinExpressions.Where(entry => entry.EntityId == proteinId).Select(entry => entry.SampleId).ToArray();
        
        var sampleIds = smSamples.Union(cnvSamples).Union(svSamples).Union(geneExpSamples).Union(proteinExpSamples).ToArray();

        return _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).ToArray();
    }


    private StatsIndex CreateStatsIndex(int proteinId)
    {
        var sms = _cache.SmTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var cnvs = _cache.CnvTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var svs = _cache.SvTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();

        var smSamples = _cache.SmEntries.Where(entry => sms.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var cnvSamples = _cache.CnvEntries.Where(entry => cnvs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var svSamples = _cache.SvEntries.Where(entry => svs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();

        var sampleIds = smSamples.Concat(cnvSamples).Concat(svSamples).Distinct().ToArray();
        var specimenIds = _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();
        var donorIds = _cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).Select(specimen => specimen.DonorId).Distinct().ToArray();
        
        return new StatsIndex
        {
            Donors = donorIds.Length,
            Sms = sms.Length,
            Cnvs = cnvs.Length,
            Svs = svs.Length
        };
    }


    private DataIndex CreateDataIndex(int proteinId)
    {
        var sampleIds = LoadSamples(proteinId).Select(sample => sample.Id).ToArray();
        var specimenIds = _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();
        var donorIds = _cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).Select(specimen => specimen.DonorId).Distinct().ToArray();
        var imageIds = _cache.Images.Where(image => donorIds.Contains(image.DonorId)).Select(image => image.Id).Distinct().ToArray();
        
        return new DataIndex
        {
            Donors = true,
            Clinical = CheckClinicalData(donorIds),
            Treatments = CheckTreatments(donorIds),
            Mrs = CheckImages(imageIds, ImageType.MR),
            Cts = CheckImages(imageIds, ImageType.CT),
            Materials = CheckSpecimens(specimenIds, SpecimenType.Material),
            MaterialsMolecular = CheckMolecularData(specimenIds, SpecimenType.Material),
            Lines = CheckSpecimens(specimenIds, SpecimenType.Line),
            LinesMolecular = CheckMolecularData(specimenIds, SpecimenType.Line),
            LinesInterventions = CheckInterventions(specimenIds, SpecimenType.Line),
            LinesDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Line),
            Organoids = CheckSpecimens(specimenIds, SpecimenType.Organoid),
            OrganoidsMolecular = CheckMolecularData(specimenIds, SpecimenType.Organoid),
            OrganoidsInterventions = CheckInterventions(specimenIds, SpecimenType.Organoid),
            OrganoidsDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Organoid),
            Xenografts = CheckSpecimens(specimenIds, SpecimenType.Xenograft),
            XenograftsMolecular = CheckMolecularData(specimenIds, SpecimenType.Xenograft),
            XenograftsInterventions = CheckInterventions(specimenIds, SpecimenType.Xenograft),
            XenograftsDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Xenograft),
            Sms = CheckSms(proteinId),
            Cnvs = CheckCnvs(proteinId),
            Svs = CheckSvs(proteinId),
            Meth = CheckMethylation(sampleIds),
            Exp = CheckGeneExp(proteinId),
            ExpSc = CheckGeneExpSc(sampleIds),
            Prot = CheckProteinExp(proteinId)
        };
    }

    private bool CheckClinicalData(int[] donorIds)
    {
        return _cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.ClinicalData != null
        );
    }

    private bool CheckTreatments(int[] donorIds)
    {
        return _cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.Treatments?.Any() == true
        );
    }

    private bool CheckImages(int[] imageIds, ImageType type)
    {
        return _cache.Images.Any(image => 
            imageIds.Contains(image.Id) &&
            image.TypeId == type
        );
    }

    private bool CheckSpecimens(int[] specimenIds, SpecimenType type)
    {
        return _cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type
        );
    }
    
    private bool CheckMolecularData(int[] specimenIds, SpecimenType type)
    {
        return _cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.MolecularData != null
        );
    }

    private bool CheckInterventions(int[] specimenIds, SpecimenType type)
    {
        return _cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.Interventions?.Any() == true
        );
    }

    private bool CheckDrugScreenings(int[] specimenIds, SpecimenType type)
    {
        return _cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.SpecimenSamples?.Any(sample => sample.DrugScreenings?.Any() == true) == true
        );
    }

    private bool CheckSms(int proteinId)
    {
        return _cache.SmTranscripts.Any(transcript => transcript.Feature.Protein.Id == proteinId);
    }

    private bool CheckCnvs(int proteinId)
    {
        return _cache.CnvTranscripts.Any(transcript => transcript.Feature.Protein.Id == proteinId);
    }

    private bool CheckSvs(int proteinId)
    {
        return _cache.SvTranscripts.Any(transcript => transcript.Feature.Protein.Id == proteinId);
    }

    private bool CheckMethylation(int[] sampleIds)
    {
        return _cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => 
                (resource.Type == DataTypes.Omics.Methylation.Sample && resource.Format == FileTypes.Sequence.Idat) ||
                (resource.Type == DataTypes.Omics.Methylation.Level)) == true
        );
    }

    private bool CheckGeneExp(int proteinId)
    {
        return _cache.GeneExpressions?.Any(entry => entry.EntityId == proteinId) == true;
    }

    private bool CheckGeneExpSc(int[] sampleIds)
    {
        return _cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => resource.Type == DataTypes.Omics.Rnasc.Expression) == true
        );
    }

    private bool CheckProteinExp(int proteinId)
    {
        return _cache.ProteinExpressions?.Any(entry => entry.EntityId == proteinId) == true;
    }
}
