using Unite.Genome.Annotations.Clients.Ensembl;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Resources;
using Unite.Genome.Annotations.Clients.Vep;
using Unite.Genome.Annotations.Clients.Vep.Configuration.Options;
using Unite.Genome.Annotations.Clients.Vep.Resources;
using Unite.Genome.Annotations.Data.Models;

namespace Unite.Genome.Annotations.Services;

public class AnnotationsDataLoader
{
    private readonly VepApiClient _vepApiClient;
    private readonly EnsemblApiClient _ensemblApiClient;


    public AnnotationsDataLoader(IVepOptions vepOptions, IEnsemblOptions ensemblOptions)
    {
        _vepApiClient = new VepApiClient(vepOptions);
        _ensemblApiClient = new EnsemblApiClient(ensemblOptions);
    }


    public async Task<AnnotationsModel[]> LoadData(string[] vepCodes)
    {
        var mutations = await LoadMutations(vepCodes);
        var genes = await LoadGenes(mutations);
        var transcripts = await LoadTranscripts(mutations);


        var annotationsModels = mutations.Select(mutation =>
        {
            var annotationsModel = new AnnotationsModel();

            annotationsModel.Mutation = new MutationModel();

            Map(mutation, annotationsModel.Mutation);

            if (mutation.AffectedTranscripts != null)
            {
                annotationsModel.AffectedTranscripts = mutation.AffectedTranscripts.Select(affectedTranscript =>
                {
                    var affectedTranscriptModel = new AffectedTranscriptModel();

                    Map(affectedTranscript, affectedTranscriptModel);

                    affectedTranscriptModel.Mutation = annotationsModel.Mutation;

                    if (!string.IsNullOrWhiteSpace(affectedTranscript.TranscriptId))
                    {
                        var transcript = transcripts.First(transcript => transcript.Id == affectedTranscript.TranscriptId);

                        affectedTranscriptModel.Transcript = new TranscriptModel();

                        Map(transcript, affectedTranscriptModel.Transcript);

                        if (!string.IsNullOrWhiteSpace(affectedTranscript.GeneId))
                        {
                            var gene = genes.First(gene => gene.Id == affectedTranscript.GeneId);

                            affectedTranscriptModel.Transcript.Gene = new GeneModel();

                            Map(gene, affectedTranscriptModel.Transcript.Gene);
                        }
                    }

                    return affectedTranscriptModel;

                }).ToArray();
            }

            return annotationsModel;

        }).ToArray();


        return annotationsModels;
    }


    private async Task<MutationResource[]> LoadMutations(string[] vepCodes)
    {
        var mutations = await _vepApiClient.LoadAnnotations(vepCodes);

        return mutations;
    }

    private async Task<GeneResource[]> LoadGenes(MutationResource[] mutations)
    {
        var geneIds = mutations
            .Where(mutation => mutation.AffectedTranscripts != null)
            .SelectMany(mutation => mutation.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.GeneId)
            .Distinct()
            .ToArray();

        var genes = await _ensemblApiClient.Lookup<GeneResource>(geneIds);

        return genes;
    }

    private async Task<TranscriptResource[]> LoadTranscripts(MutationResource[] mutations)
    {
        var transcriptIds = mutations
            .Where(mutation => mutation.AffectedTranscripts != null)
            .SelectMany(mutation => mutation.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.TranscriptId)
            .Distinct()
            .ToArray();

        var transcripts = await _ensemblApiClient.Lookup<TranscriptResource>(transcriptIds, expand: true);

        return transcripts;
    }


    private void Map(MutationResource resource, MutationModel model)
    {
        model.Code = resource.Code;
    }

    private void Map(AffectedTranscriptResource resource, AffectedTranscriptModel model)
    {
        model.CDSStart = resource.CDSStart;
        model.CDSEnd = resource.CDSEnd;
        model.ProteinStart = resource.ProteinStart;
        model.ProteinEnd = resource.ProteinEnd;
        model.CDNAStart = resource.CDNAStart;
        model.CDNAEnd = resource.CDNAEnd;
        model.AminoAcidChange = resource.AminoAcidChange;
        model.CodonChange = resource.CodonChange;
        model.Consequences = resource.Consequences;
    }

    private void Map(TranscriptResource resource, TranscriptModel model)
    {
        model.EnsemblId = resource.Id;
        model.Symbol = resource.Symbol;
        model.Biotype = resource.Biotype;
        model.Chromosome = resource.Chromosome;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Strand = resource.Strand == 1;

        if (resource.Protein != null)
        {
            model.Protein = new ProteinModel();

            Map(resource.Protein, model.Protein);
        }
    }

    private void Map(ProteinResource resource, ProteinModel model)
    {
        model.EnsemblId = resource.Id;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Length = resource.Length;
    }

    private void Map(GeneResource resource, GeneModel model)
    {
        model.EnsemblId = resource.Id;
        model.Symbol = resource.Symbol;
        model.Biotype = resource.Biotype;
        model.Chromosome = resource.Chromosome;
        model.Start = resource.Start;
        model.End = resource.End;
        model.Strand = resource.Strand == 1;
    }
}
