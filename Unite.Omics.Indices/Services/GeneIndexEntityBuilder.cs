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

public class GeneIndexEntityBuilder : IndexEntityBuilder<GeneIndex, GenesIndexingCache>
{
    public override GeneIndex[] Create(int key, GenesIndexingCache cache)
    {
        return [CreateGeneIndex(key, cache)];
    }

    private GeneIndex CreateGeneIndex(int geneId, GenesIndexingCache cache)
    {
        var gene = LoadGene(geneId, cache);

        if (gene == null)
            return null;

        return CreateGeneIndex(gene, cache);
    }
    
    private GeneIndex CreateGeneIndex(Gene gene, GenesIndexingCache cache)
    {
        var index = GeneIndexMapper.CreateFrom<GeneIndex>(gene);

        index.Specimens = CreateSpecimenIndices(gene.Id, cache);

        var hasSpecimens = index.Specimens?.Any();

        var proteinIds = cache.Proteins.Where(protein => protein.Transcript.GeneId == gene.Id).Select(protein => protein.Id).ToArray();
        var hasGeneExpressions = cache.GeneExpressions?.Any(entry => entry.EntityId == gene.Id);
        var hasProteinExpressions = cache.ProteinExpressions?.Any(entry => proteinIds.Contains(entry.EntityId));

        // If gene is not affected by any variant and has no expression data, it should be removed.
        if (hasSpecimens != true && hasGeneExpressions != true && hasProteinExpressions != true)
            return null;

        index.Stats = CreateStatsIndex(gene.Id, cache);
        index.Data = CreateDataIndex(gene.Id, cache);

        return index;
    }

    private Gene LoadGene(int geneId, GenesIndexingCache cache)
    {
        return cache.Genes.FirstOrDefault(gene => gene.Id == geneId);
    }


    private SpecimenIndex[] CreateSpecimenIndices(int geneId, GenesIndexingCache cache)
    {
        var specimens = LoadSpecimens(geneId, cache);

        return specimens.Select(specimen => CreateSpecimenIndex(geneId, specimen)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(int geneId, Specimen specimen)
    {
        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, null);
        
        return index;
    }

    private Specimen[] LoadSpecimens(int geneId, GenesIndexingCache cache)
    {
        var sampleIds = LoadSamples(geneId, cache).Select(sample => sample.Id).ToArray();
        var specimenIds = cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();

        return cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).ToArray();
    }

    private Sample[] LoadSamples(int geneId, GenesIndexingCache cache)
    {
        var sms = cache.SmTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var cnvs = cache.CnvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var svs = cache.SvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();

        var smSamples = cache.SmEntries.Where(entry => sms.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var cnvSamples = cache.CnvEntries.Where(entry => cnvs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var svSamples = cache.SvEntries.Where(entry => svs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        
        var sampleIds = smSamples.Concat(cnvSamples).Concat(svSamples).Distinct().ToArray();

        return cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).ToArray();
    }


    private StatsIndex CreateStatsIndex(int geneId, GenesIndexingCache cache)
    {
        var sms = cache.SmTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var cnvs = cache.CnvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var svs = cache.SvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).Distinct().ToArray();

        var smSamples = cache.SmEntries.Where(entry => sms.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var cnvSamples = cache.CnvEntries.Where(entry => cnvs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var svSamples = cache.SvEntries.Where(entry => svs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();

        var sampleIds = smSamples.Concat(cnvSamples).Concat(svSamples).Distinct().ToArray();
        var specimenIds = cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();
        var donorIds = cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).Select(specimen => specimen.DonorId).Distinct().ToArray();
        
        return new StatsIndex
        {
            Donors = donorIds.Length,
            Sms = sms.Length,
            Cnvs = cnvs.Length,
            Svs = svs.Length
        };
    }


    private DataIndex CreateDataIndex(int geneId, GenesIndexingCache cache)
    {
        var sampleIds = LoadSamples(geneId, cache).Select(sample => sample.Id).ToArray();
        var specimenIds = cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();
        var donorIds = cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).Select(specimen => specimen.DonorId).Distinct().ToArray();
        var imageIds = cache.Images.Where(image => donorIds.Contains(image.DonorId)).Select(image => image.Id).Distinct().ToArray();
        
        return new DataIndex
        {
            Donors = true,
            Clinical = CheckClinicalData(donorIds, cache),
            Treatments = CheckTreatments(donorIds, cache),
            Mrs = CheckImages(imageIds, ImageType.MR, cache),
            Cts = CheckImages(imageIds, ImageType.CT, cache),
            Materials = CheckSpecimens(specimenIds, SpecimenType.Material, cache),
            MaterialsMolecular = CheckMolecularData(specimenIds, SpecimenType.Material, cache),
            Lines = CheckSpecimens(specimenIds, SpecimenType.Line, cache),
            LinesMolecular = CheckMolecularData(specimenIds, SpecimenType.Line, cache),
            LinesInterventions = CheckInterventions(specimenIds, SpecimenType.Line, cache),
            LinesDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Line, cache),
            Organoids = CheckSpecimens(specimenIds, SpecimenType.Organoid, cache),
            OrganoidsMolecular = CheckMolecularData(specimenIds, SpecimenType.Organoid, cache),
            OrganoidsInterventions = CheckInterventions(specimenIds, SpecimenType.Organoid, cache),
            OrganoidsDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Organoid, cache),
            Xenografts = CheckSpecimens(specimenIds, SpecimenType.Xenograft, cache),
            XenograftsMolecular = CheckMolecularData(specimenIds, SpecimenType.Xenograft, cache),
            XenograftsInterventions = CheckInterventions(specimenIds, SpecimenType.Xenograft, cache),
            XenograftsDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Xenograft, cache),
            Sms = CheckSms(geneId, cache),
            Cnvs = CheckCnvs(geneId, cache),
            Svs = CheckSvs(geneId, cache),
            Meth = CheckMethylation(sampleIds, cache),
            Exp = CheckGeneExp(geneId, cache),
            ExpSc = CheckGeneExpSc(sampleIds, cache)
        };
    }

    private bool CheckClinicalData(int[] donorIds, GenesIndexingCache cache)
    {
        return cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.ClinicalData != null
        );
    }

    private bool CheckTreatments(int[] donorIds, GenesIndexingCache cache)
    {
        return cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.Treatments?.Any() == true
        );
    }

    private bool CheckImages(int[] imageIds, ImageType type, GenesIndexingCache cache)
    {
        return cache.Images.Any(image => 
            imageIds.Contains(image.Id) &&
            image.TypeId == type
        );
    }

    private bool CheckSpecimens(int[] specimenIds, SpecimenType type, GenesIndexingCache cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type
        );
    }
    
    private bool CheckMolecularData(int[] specimenIds, SpecimenType type, GenesIndexingCache cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.MolecularData != null
        );
    }

    private bool CheckInterventions(int[] specimenIds, SpecimenType type, GenesIndexingCache cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.Interventions?.Any() == true
        );
    }

    private bool CheckDrugScreenings(int[] specimenIds, SpecimenType type, GenesIndexingCache cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.SpecimenSamples?.Any(sample => sample.DrugScreenings?.Any() == true) == true
        );
    }

    private bool CheckSms(int geneId, GenesIndexingCache cache)
    {
        return cache.SmTranscripts.Any(transcript => transcript.Feature.GeneId == geneId);
    }

    private bool CheckCnvs(int geneId, GenesIndexingCache cache)
    {
        return cache.CnvTranscripts.Any(transcript => transcript.Feature.GeneId == geneId);
    }

    private bool CheckSvs(int geneId, GenesIndexingCache cache)
    {
        return cache.SvTranscripts.Any(transcript => transcript.Feature.GeneId == geneId);
    }

    private bool CheckMethylation(int[] sampleIds, GenesIndexingCache cache)
    {
        return cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => 
                (resource.Type == DataTypes.Omics.Methylation.Sample && resource.Format == FileTypes.Sequence.Idat) ||
                (resource.Type == DataTypes.Omics.Methylation.Level)) == true
        );
    }

    private bool CheckGeneExp(int geneId, GenesIndexingCache cache)
    {
        return cache.GeneExpressions?.Any(entry => entry.EntityId == geneId) == true;
    }

    private bool CheckGeneExpSc(int[] sampleIds, GenesIndexingCache cache)
    {
        return cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => resource.Type == DataTypes.Omics.Rnasc.Expression) == true
        );
    }
}
