using System.Collections.Generic;
using System.Linq;
using Unite.Mutations.Feed.Data.Services.Mutations.Models;
using Unite.Mutations.Feed.Web.Resources.Mutations.Converters.Helpers;

namespace Unite.Mutations.Feed.Web.Resources.Mutations.Converters
{
    public class MutationsResourceConverter
    {
        public static MutationsModel From(MutationsResource mutationsResource)
        {
            var mutationsModel = new MutationsModel();

            var donorModel = new DonorModel();

            donorModel.Id = mutationsResource.Pid;

            mutationsModel.Donor = donorModel;

            if (mutationsResource.Analysis != null)
            {
                var analysisModel = new AnalysisModel();

                analysisModel.Name = mutationsResource.Analysis.Name;
                analysisModel.Type = mutationsResource.Analysis.Type;
                analysisModel.Date = mutationsResource.Analysis.Date;

                if (mutationsResource.Analysis.File != null)
                {
                    var fileModel = new FileModel();

                    fileModel.Name = mutationsResource.Analysis.File.Name;
                    fileModel.Link = mutationsResource.Analysis.File.Link;
                    fileModel.Created = mutationsResource.Analysis.File.Created;
                    fileModel.Updated = mutationsResource.Analysis.File.Updated;

                    analysisModel.File = fileModel;
                }

                mutationsModel.Analysis = analysisModel;
            }

            if (mutationsResource.Samples != null)
            {
                var analysedSampleModels = new List<AnalysedSampleModel>();

                foreach (var analysedSampleResource in mutationsResource.Samples)
                {
                    var analysedSampleModel = new AnalysedSampleModel();

                    analysedSampleModel.Name = analysedSampleResource.Name;
                    analysedSampleModel.Type = analysedSampleResource.Type;
                    analysedSampleModel.Subtype = analysedSampleResource.Subtype;
                    analysedSampleModel.Date = analysedSampleResource.Date;

                    if (analysedSampleResource.MatchedSamples != null)
                    {
                        var matchedSampleModels = new List<MatchedSampleModel>();

                        foreach (var matchingSampleName in analysedSampleResource.MatchedSamples)
                        {
                            var matchingSampleResource = mutationsResource.Samples.Single(matchingSampleResource =>
                                matchingSampleResource.Name == matchingSampleName
                            );

                            var matchedSampleModel = new MatchedSampleModel();

                            matchedSampleModel.Name = matchingSampleResource.Name;
                            matchedSampleModel.Type = matchingSampleResource.Type;
                            matchedSampleModel.Subtype = matchingSampleResource.Subtype;
                            matchedSampleModel.Date = matchingSampleResource.Date;

                            matchedSampleModels.Add(matchedSampleModel);
                        }

                        analysedSampleModel.MatchedSamples = matchedSampleModels;
                    }

                    if (analysedSampleResource.Mutations != null)
                    {
                        var mutationModels = new List<MutationModel>();

                        foreach (var mutationResource in analysedSampleResource.Mutations)
                        {
                            var mutationModel = new MutationModel();

                            mutationModel.Code = MutationCodeHelper.GetCode(
                                mutationResource.Chromosome,
                                mutationResource.SequenceType,
                                mutationResource.Position,
                                mutationResource.Ref,
                                mutationResource.Alt);

                            mutationModel.Chromosome = mutationResource.Chromosome;
                            mutationModel.SequenceType = mutationResource.SequenceType;
                            mutationModel.Start = MutationPositionHelper.GetMutationStart(mutationResource.Position);
                            mutationModel.End = MutationPositionHelper.GetMutationEnd(mutationResource.Position);
                            mutationModel.Type = MutationTypeHelper.GetMutationType(mutationResource.Ref, mutationResource.Alt);
                            mutationModel.ReferenceBase = mutationResource.Ref;
                            mutationModel.AlternateBase = mutationResource.Alt;

                            mutationModels.Add(mutationModel);
                        }

                        analysedSampleModel.Mutations = mutationModels;
                    }

                    analysedSampleModels.Add(analysedSampleModel);
                }

                mutationsModel.AnalysedSamples = analysedSampleModels;
            }

            return mutationsModel;
        }
    }
}
