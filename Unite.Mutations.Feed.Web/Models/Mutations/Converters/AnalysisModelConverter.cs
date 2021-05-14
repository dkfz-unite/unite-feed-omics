using System.Linq;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Converters
{
    public class AnalysisModelConverter
    {
        public Feed.Mutations.Data.Models.AnalysisModel Convert(MutationsModel source)
        {
            var analysisModel = new Feed.Mutations.Data.Models.AnalysisModel();

            if(source.Analysis != null)
            {
                Map(source.Analysis, analysisModel);
            }

            analysisModel.AnalysedSamples = source.Samples.Select(analysedSample =>
            {
                var analysedSampleModel = new Feed.Mutations.Data.Models.AnalysedSampleModel();

                Map(analysedSample, analysedSampleModel);

                if (analysedSample.MatchedSamples != null)
                {
                    analysedSampleModel.MatchedSamples = analysedSample.MatchedSamples.Select(matchedSampleName =>
                    {
                        var matchedAnalysedSample = source.Samples.Single(analysedSample => analysedSample.Name == matchedSampleName);

                        var matchedSampleModel = new Feed.Mutations.Data.Models.MatchedSampleModel();

                        Map(matchedAnalysedSample, matchedSampleModel);

                        return matchedSampleModel;

                    }).ToArray();
                }

                if (analysedSample.Mutations != null)
                {
                    analysedSampleModel.Mutations = analysedSample.Mutations.Select(mutation =>
                    {
                        var mutationModel = new Feed.Mutations.Data.Models.MutationModel();

                        Map(mutation, mutationModel);

                        return mutationModel;

                    }).ToArray();
                }

                return analysedSampleModel;

            }).ToArray();

            return analysisModel;
        }


        private static void Map(in AnalysisModel source, Feed.Mutations.Data.Models.AnalysisModel target)
        {
            target.Type = source.Type;
            target.Date = source.Date;

            if(source.File != null)
            {
                target.File = new Feed.Mutations.Data.Models.FileModel();

                Map(source.File, target.File);
            }
        }

        private static void Map(in FileModel source, Feed.Mutations.Data.Models.FileModel target)
        {
            target.Name = source.Name;
            target.Link = source.Link;
            target.Created = source.Created;
            target.Updated = source.Updated;
        }

        private static void Map(in SampleModel source, Feed.Mutations.Data.Models.SampleModel target)
        {
            target.ReferenceId = source.Id;
            target.Date = source.Date;

            if(source.Tissue != null)
            {
                target.Specimen = new Feed.Mutations.Data.Models.SpecimenModel();

                Map(source.Tissue, target.Specimen);
            }
        }

        private static void Map(in TissueModel source, Feed.Mutations.Data.Models.SpecimenModel target)
        {
            target.ReferenceId = source.Id;

            if (!string.IsNullOrEmpty(source.DonorId))
            {
                target.Donor = new Feed.Mutations.Data.Models.DonorModel();
                target.Donor.ReferenceId = source.DonorId;

                target.Tissue = new Feed.Mutations.Data.Models.TissueModel();
                target.Tissue.Type = source.Type.Value;
                target.Tissue.TumourType = source.TumourType;
            }
        }

        private static void Map(in MutationModel source, Feed.Mutations.Data.Models.MutationModel target)
        {
            target.Code = source.GetCode();
            target.Chromosome = source.Chromosome.Value;
            target.SequenceType = source.SequenceType.Value;
            target.Start = Data.Utilities.Mutations.PositionParser.Parse(source.Position).Start;
            target.End = Data.Utilities.Mutations.PositionParser.Parse(source.Position).End;
            target.ReferenceBase = source.Ref;
            target.AlternateBase = source.Alt;
            target.Type = Data.Utilities.Mutations.MutationTypeDetector.Detect(source.Ref, source.Alt);
        }
    }
}
