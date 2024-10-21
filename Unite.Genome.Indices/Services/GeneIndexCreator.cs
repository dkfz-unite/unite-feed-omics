using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Genes;
using Unite.Mapping;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;


namespace Unite.Genome.Indices.Services;

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

        index.Data.GeneExp = _cache.ExpEntries.Any(entry => entry.EntityId == gene.Id);

        // If gene is not affected by any variant and has no expression data, it should be removed.
        if (index.Specimens.IsEmpty() && index.Data.GeneExp != true)
            return null;

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
        var donorIndex = CreateDonorIndex(specimen.DonorId, out var diagnosisDate);
        var imageIndices = CreateImageIndices(specimen.DonorId, diagnosisDate);
        var sampleIndices = CreateSampleIndices(specimen.Id, diagnosisDate);
        var variantIndices = CreateVariantIndices(specimen.Id, geneId);

        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, diagnosisDate);

        index.Donor = donorIndex;
        index.Images = imageIndices;
        index.Samples = sampleIndices;
        index.Variants = variantIndices;
        
        return index;
    }

    private Specimen[] LoadSpecimens(int geneId)
    {
        var ssms = _cache.SsmTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).ToArray();
        var cnvs = _cache.CnvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).ToArray();
        var svs = _cache.SvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId).Select(transcript => transcript.VariantId).ToArray();

        var ssmSamples = _cache.SsmEntries.Where(entry => ssms.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var cnvSamples = _cache.CnvEntries.Where(entry => cnvs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var svSamples = _cache.SvEntries.Where(entry => svs.Contains(entry.EntityId)).Select(entry => entry.SampleId).ToArray();
        var expSamples = _cache.ExpEntries.Where(entry => entry.EntityId == geneId).Select(entry => entry.SampleId).ToArray();

        var sampleIds = ssmSamples.Concat(cnvSamples).Concat(svSamples).Concat(expSamples).Distinct().ToArray();
        var specimenIds = _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();

        return _cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).ToArray();
    }


    private DonorIndex CreateDonorIndex(int donorId, out DateOnly? diagnosisDate)
    {
        var donor = LoadDonor(donorId);

        diagnosisDate = donor.ClinicalData?.DiagnosisDate;

        return CreateDonorIndex(donor);
    }

    private DonorIndex CreateDonorIndex(Donor donor)
    {
        return DonorIndexMapper.CreateFrom<DonorIndex>(donor);
    }

    private Donor LoadDonor(int donorId)
    {
        return _cache.Donors.FirstOrDefault(donor => donor.Id == donorId);
    }


    private ImageIndex[] CreateImageIndices(int donorId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(donorId);
        
        return images.Select(entity => CreateImageIndex(entity, diagnosisDate)).ToArrayOrNull();
    }

    private ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        return ImageIndexMapper.CreateFrom<ImageIndex>(image, diagnosisDate);
    }

    private Image[] LoadImages(int donorId)
    {
        return _cache.Images.Where(image => image.DonorId == donorId).ToArray();
    }


    private SampleIndex[] CreateSampleIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var samples = LoadSamples(specimenId);

        return samples.Select(sample => CreateSampleIndex(sample, diagnosisDate)).ToArrayOrNull();
    }

    private SampleIndex CreateSampleIndex(Sample sample, DateOnly? diagnosisDate)
    {
        return SampleIndexMapper.CreateFrom<SampleIndex>(sample, diagnosisDate);
    }

    private Sample[] LoadSamples(int specimenId)
    {
        return _cache.Samples.Where(sample => sample.SpecimenId == specimenId).ToArray();
    }


    private VariantIndex[] CreateVariantIndices(int specimenId, int geneId)
    {
        var indices = new List<VariantIndex>();

        LoadSsms(specimenId, geneId).ForEach(variant => indices.Add(CreateVariantIndex(variant)));
        LoadCnvs(specimenId, geneId).ForEach(variant => indices.Add(CreateVariantIndex(variant)));
        LoadSvs(specimenId, geneId).ForEach(variant => indices.Add(CreateVariantIndex(variant)));

        return indices.ToArrayOrNull();
    }

    private VariantIndex CreateVariantIndex<TVariant>(TVariant variant) where TVariant : Variant
    {
        return VariantIndexMapper.CreateFrom<VariantIndex>(variant);
    }

    private SSM.Variant[] LoadSsms(int specimenId, int geneId)
    {
        var sampleIds = _cache.Samples.Where(sample => sample.SpecimenId == specimenId).Select(sample => sample.Id).Distinct().ToArray();
        var sampleVariantIds = _cache.SsmEntries.Where(entry => sampleIds.Contains(entry.SampleId)).Select(entry => entry.EntityId).Distinct().ToArray();
        var affectedTranscripts = _cache.SsmTranscripts.Where(transcript => transcript.Feature.GeneId == geneId && sampleVariantIds.Contains(transcript.VariantId)).ToArray();
        var affectedVariantIds = affectedTranscripts.Select(transcript => transcript.VariantId).Distinct().ToArray();
        var affectedVariants = _cache.Ssms.Where(variant => affectedVariantIds.Contains(variant.Id)).ToArray();

        affectedVariants.ForEach(variant => variant.AffectedTranscripts = affectedTranscripts.Where(transcript => transcript.VariantId == variant.Id).ToArray());

        return affectedVariants.ToArray();
    }

    private CNV.Variant[] LoadCnvs(int specimenId, int geneId)
    {
        var sampleIds = _cache.Samples.Where(sample => sample.SpecimenId == specimenId).Select(sample => sample.Id).Distinct().ToArray();
        var sampleVariantIds = _cache.CnvEntries.Where(entry => sampleIds.Contains(entry.SampleId)).Select(entry => entry.EntityId).Distinct().ToArray();
        var affectedTranscripts = _cache.CnvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId && sampleVariantIds.Contains(transcript.VariantId)).ToArray();
        var affectedVariantIds = affectedTranscripts.Select(transcript => transcript.VariantId).Distinct().ToArray();
        var affectedVariants = _cache.Cnvs.Where(variant => affectedVariantIds.Contains(variant.Id)).ToArray();

        affectedVariants.ForEach(variant => variant.AffectedTranscripts = affectedTranscripts.Where(transcript => transcript.VariantId == variant.Id).ToArray());

        return affectedVariants.ToArray();
    }

    private SV.Variant[] LoadSvs(int specimenId, int geneId)
    {
        var sampleIds = _cache.Samples.Where(sample => sample.SpecimenId == specimenId).Select(sample => sample.Id).Distinct().ToArray();
        var sampleVariantIds = _cache.SvEntries.Where(entry => sampleIds.Contains(entry.SampleId)).Select(entry => entry.EntityId).Distinct().ToArray();
        var affectedTranscripts = _cache.SvTranscripts.Where(transcript => transcript.Feature.GeneId == geneId && sampleVariantIds.Contains(transcript.VariantId)).ToArray();
        var affectedVariantIds = affectedTranscripts.Select(transcript => transcript.VariantId).Distinct().ToArray();
        var affectedVariants = _cache.Svs.Where(variant => affectedVariantIds.Contains(variant.Id)).ToArray();

        affectedVariants.ForEach(variant => variant.AffectedTranscripts = affectedTranscripts.Where(transcript => transcript.VariantId == variant.Id).ToArray());

        return affectedVariants.ToArray();
    }
}
