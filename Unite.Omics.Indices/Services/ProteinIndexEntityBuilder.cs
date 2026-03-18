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

public class ProteinIndexEntityBuilder: IndexEntityBuilder<ProteinIndex, ProteinsIndexingCache>
{ 
    public override ProteinIndex[] Create(int key, ProteinsIndexingCache cache)
    {
        var index = CreateProteinIndex(key, cache);

        return index == null ? null : [index];
    }

    private ProteinIndex CreateProteinIndex(int proteinId, ProteinsIndexingCache cache)
    {
        var protein = LoadProtein(proteinId, cache);

        if (protein == null)
            return null;

        return CreateProteinIndex(protein, cache);
    }

    private ProteinIndex CreateProteinIndex(Protein protein, ProteinsIndexingCache cache)
    {
        var index = ProteinIndexMapper.CreateFrom<ProteinIndex>(protein);

        index.Specimens = CreateSpecimenIndices(protein.Id, cache);

        var hasSpecimens = index.Specimens?.Any();

        var genes = cache.Proteins.Where(protein => protein.Transcript.GeneId == protein.Transcript.GeneId).Select(protein => protein.Transcript.GeneId).Distinct().ToArray();
        var hasGeneExpressions = cache.GeneExpressions?.Any(entry => genes.Contains(entry.EntityId));
        var hasProteinExpressions = cache.ProteinExpressions?.Any(entry => entry.EntityId == protein.Id);

        // If gene is not affected by any variant and has no expression data, it should be removed.
        if (hasSpecimens != true && hasGeneExpressions != true && hasProteinExpressions != true)
            return null;

        index.Stats = CreateStatsIndex(protein.Id, cache);
        index.Data = CreateDataIndex(protein.Id, cache);

        return index;
    }

    private Protein LoadProtein(int proteinId, ProteinsIndexingCache cache)
    {
        return cache.Proteins.FirstOrDefault(protein => protein.Id == proteinId);
    }


    private SpecimenIndex[] CreateSpecimenIndices(int proteinId, ProteinsIndexingCache cache)
    {
        var specimens = LoadSpecimens(proteinId, cache);

        return specimens.Select(specimen => CreateSpecimenIndex(proteinId, specimen)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(int proteinId, Specimen specimen)
    {
        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, null);
        
        return index;
    }

    private Specimen[] LoadSpecimens(int proteinId, ProteinsIndexingCache cache)
    {
        var sampleIds = LoadSamples(proteinId, cache).Select(sample => sample.Id).ToArray();
        var specimenIds = cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();

        return cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).ToArray();
    }

    private Sample[] LoadSamples(int proteinId, ProteinsIndexingCache cache)
    {
        var sms = cache.SmTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var cnvs = cache.CnvTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var svs = cache.SvTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var genes = cache.Proteins.Where(protein => protein.Id == proteinId).Select(protein => protein.Transcript.GeneId).Distinct().ToArray();

        var smSamples = cache.SmEntries.Where(entry => sms.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var cnvSamples = cache.CnvEntries.Where(entry => cnvs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var svSamples = cache.SvEntries.Where(entry => svs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var geneExpSamples = cache.GeneExpressions.Where(entry => genes.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var proteinExpSamples = cache.ProteinExpressions.Where(entry => entry.EntityId == proteinId).Select(entry => entry.SampleId).ToArray();
        
        var sampleIds = smSamples.Union(cnvSamples).Union(svSamples).Union(geneExpSamples).Union(proteinExpSamples).ToArray();

        return cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).ToArray();
    }


    private StatsIndex CreateStatsIndex(int proteinId, ProteinsIndexingCache cache)
    {
        var sms = cache.SmTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var cnvs = cache.CnvTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();
        var svs = cache.SvTranscripts.Where(transcript => transcript.Feature.Protein.Id == proteinId).Select(transcript => transcript.VariantId).Distinct().ToArray();

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


    private DataIndex CreateDataIndex(int proteinId, ProteinsIndexingCache cache)
    {
        var sampleIds = LoadSamples(proteinId, cache).Select(sample => sample.Id).ToArray();
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
            Sms = CheckSms(proteinId, cache),
            Cnvs = CheckCnvs(proteinId, cache),
            Svs = CheckSvs(proteinId, cache),
            Meth = CheckMethylation(sampleIds, cache),
            Exp = CheckGeneExp(proteinId, cache),
            ExpSc = CheckGeneExpSc(sampleIds, cache),
            Prot = CheckProteinExp(proteinId, cache)
        };
    }

    private bool CheckClinicalData(int[] donorIds, ProteinsIndexingCache cache)
    {
        return cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.ClinicalData != null
        );
    }

    private bool CheckTreatments(int[] donorIds, ProteinsIndexingCache cache)
    {
        return cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.Treatments?.Any() == true
        );
    }

    private bool CheckImages(int[] imageIds, ImageType type, ProteinsIndexingCache cache)
    {
        return cache.Images.Any(image => 
            imageIds.Contains(image.Id) &&
            image.TypeId == type
        );
    }

    private bool CheckSpecimens(int[] specimenIds, SpecimenType type, ProteinsIndexingCache cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type
        );
    }
    
    private bool CheckMolecularData(int[] specimenIds, SpecimenType type, ProteinsIndexingCache cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.MolecularData != null
        );
    }

    private bool CheckInterventions(int[] specimenIds, SpecimenType type, ProteinsIndexingCache cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.Interventions?.Any() == true
        );
    }

    private bool CheckDrugScreenings(int[] specimenIds, SpecimenType type, ProteinsIndexingCache cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.SpecimenSamples?.Any(sample => sample.DrugScreenings?.Any() == true) == true
        );
    }

    private bool CheckSms(int proteinId, ProteinsIndexingCache cache)
    {
        return cache.SmTranscripts.Any(transcript => transcript.Feature.Protein.Id == proteinId);
    }

    private bool CheckCnvs(int proteinId, ProteinsIndexingCache cache)
    {
        return cache.CnvTranscripts.Any(transcript => transcript.Feature.Protein.Id == proteinId);
    }

    private bool CheckSvs(int proteinId, ProteinsIndexingCache cache)
    {
        return cache.SvTranscripts.Any(transcript => transcript.Feature.Protein.Id == proteinId);
    }

    private bool CheckMethylation(int[] sampleIds, ProteinsIndexingCache cache)
    {
        return cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => 
                (resource.Type == DataTypes.Omics.Methylation.Sample && resource.Format == FileTypes.Sequence.Idat) ||
                (resource.Type == DataTypes.Omics.Methylation.Level)) == true
        );
    }

    private bool CheckGeneExp(int proteinId, ProteinsIndexingCache cache)
    {
        return cache.GeneExpressions?.Any(entry => entry.EntityId == proteinId) == true;
    }

    private bool CheckGeneExpSc(int[] sampleIds, ProteinsIndexingCache cache)
    {
        return cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => resource.Type == DataTypes.Omics.Rnasc.Expression) == true
        );
    }

    private bool CheckProteinExp(int proteinId, ProteinsIndexingCache cache)
    {
        return cache.ProteinExpressions?.Any(entry => entry.EntityId == proteinId) == true;
    }
}
