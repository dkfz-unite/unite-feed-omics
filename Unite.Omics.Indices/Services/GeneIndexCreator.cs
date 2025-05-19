using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Indices.Services.Mappers;
using Unite.Indices.Entities;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Omics.Analysis;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Data.Constants;


namespace Unite.Omics.Indices.Services;

public class GeneIndexCreator
{
    private readonly GenesIndexingCache _cache;


    public GeneIndexCreator(GenesIndexingCache cache)
    {
        _cache = cache;
    }


    public GeneIndex CreateIndex(object key)
    {
        var geneId = (int)key;

        return CreateGeneIndex(geneId);
    }


    private GeneIndex CreateGeneIndex(int geneId)
    {
        var gene = LoadGene(geneId);

        if (gene == null)
            return null;

        return CreateGeneIndex(gene);
    }

    private GeneIndex CreateGeneIndex(Gene gene)
    {
        var index = GeneIndexMapper.CreateFrom<GeneIndex>(gene);

        index.Specimens = CreateSpecimenIndices(gene.Id);

        var hasSpecimens = index.Specimens?.Any();
        var hasExpressions = _cache.ExpEntries?.Any(entry => entry.EntityId == gene.Id);

        // If gene is not affected by any variant and has no expression data, it should be removed.
        if (hasSpecimens != true && hasExpressions != true)
            return null;

        index.Stats = CreateStatsIndex(gene.Id);
        index.Data = CreateDataIndex(gene.Id);

        return index;
    }

    private Gene LoadGene(int geneId)
    {
        return _cache.Genes.FirstOrDefault(gene => gene.Id == geneId);
    }


    private SpecimenIndex[] CreateSpecimenIndices(int geneId)
    {
        var specimens = LoadSpecimens(geneId);

        return specimens.Select(specimen => CreateSpecimenIndex(geneId, specimen)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(int geneId, Specimen specimen)
    {
        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, null);
        
        return index;
    }

    private Specimen[] LoadSpecimens(int geneId)
    {
        var sampleIds = LoadSamples(geneId).Select(sample => sample.Id).ToArray();
        var specimenIds = _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();

        return _cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).ToArray();
    }

    private Sample[] LoadSamples(int geneId)
    {
        var sms = _cache.SmTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var cnvs = _cache.CnvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var svs = _cache.SvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();

        var smSamples = _cache.SmEntries.Where(entry => sms.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var cnvSamples = _cache.CnvEntries.Where(entry => cnvs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var svSamples = _cache.SvEntries.Where(entry => svs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        
        var sampleIds = smSamples.Concat(cnvSamples).Concat(svSamples).Distinct().ToArray();

        return _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).ToArray();
    }


    private StatsIndex CreateStatsIndex(int geneId)
    {
        var sms = _cache.SmTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var cnvs = _cache.CnvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var svs = _cache.SvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();

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


    private DataIndex CreateDataIndex(int geneId)
    {
        var sampleIds = LoadSamples(geneId).Select(sample => sample.Id).ToArray();
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
            Sms = CheckSms(geneId),
            Cnvs = CheckCnvs(geneId),
            Svs = CheckSvs(geneId),
            Meth = CheckMethylation(sampleIds),
            Exp = CheckGeneExp(geneId),
            ExpSc = CheckGeneExpSc(sampleIds)
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

    private bool CheckSms(int geneId)
    {
        return _cache.SmTranscripts.Any(transcript => transcript.Feature.GeneId == geneId);
    }

    private bool CheckCnvs(int geneId)
    {
        return _cache.CnvTranscripts.Any(transcript => transcript.Feature.GeneId == geneId);
    }

    private bool CheckSvs(int geneId)
    {
        return _cache.SvTranscripts.Any(transcript => transcript.Feature.GeneId == geneId);
    }

    private bool CheckMethylation(int[] sampleIds)
    {
        return _cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => 
                (resource.Type == DataTypes.Omics.Meth.Sample && resource.Format == FileTypes.Sequence.Idat) ||
                (resource.Type == DataTypes.Omics.Meth.Level)) == true
        );
    }

    private bool CheckGeneExp(int geneId)
    {
        return _cache.ExpEntries?.Any(entry => entry.EntityId == geneId) == true;
    }

    private bool CheckGeneExpSc(int[] sampleIds)
    {
        return _cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => resource.Type == DataTypes.Omics.Rnasc.Exp) == true
        );
    }
}
