using System;
using System.Linq;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Converters
{
    public class AnalysisModelConverter
    {
        public Feed.Data.Mutations.Models.AnalysisModel Convert(MutationsModel source)
        {
            var analysisModel = new Feed.Data.Mutations.Models.AnalysisModel();

            if (source.Analysis != null)
            {
                Map(source.Analysis, analysisModel);
            }

            analysisModel.AnalysedSamples = source.Samples.Select(analysedSample =>
            {
                var analysedSampleModel = new Feed.Data.Mutations.Models.AnalysedSampleModel();

                Map(analysedSample, analysedSampleModel);

                if (analysedSample.MatchedSamples != null)
                {
                    analysedSampleModel.MatchedSamples = analysedSample.MatchedSamples.Select(matchedSampleName =>
                    {
                        var matchedAnalysedSample = source.Samples.Single(analysedSample => analysedSample.Name == matchedSampleName);

                        var matchedSampleModel = new Feed.Data.Mutations.Models.MatchedSampleModel();

                        Map(matchedAnalysedSample, matchedSampleModel);

                        return matchedSampleModel;

                    }).ToArray();
                }

                if (analysedSample.Mutations != null)
                {
                    analysedSampleModel.Mutations = analysedSample.Mutations.Select(mutation =>
                    {
                        var mutationModel = new Feed.Data.Mutations.Models.MutationModel();

                        Map(mutation, mutationModel);

                        return mutationModel;

                    }).ToArray();
                }

                return analysedSampleModel;

            }).ToArray();

            return analysisModel;
        }


        private static void Map(in AnalysisModel source, Feed.Data.Mutations.Models.AnalysisModel target)
        {
            target.Type = source.Type;
            target.Date = source.Date;

            if (source.File != null)
            {
                target.File = new Feed.Data.Mutations.Models.FileModel();

                Map(source.File, target.File);
            }
        }

        private static void Map(in FileModel source, Feed.Data.Mutations.Models.FileModel target)
        {
            target.Name = source.Name;
            target.Link = source.Link;
            target.Created = source.Created;
            target.Updated = source.Updated;
        }

        private static void Map(in SampleModel source, Feed.Data.Mutations.Models.SampleModel target)
        {
            target.ReferenceId = source.Id;
            target.Date = source.Date;

            if (source.Tissue != null)
            {
                target.Specimen = new Feed.Data.Mutations.Models.TissueModel();

                Map(source.Tissue, (Feed.Data.Mutations.Models.TissueModel)target.Specimen);
            }
            else if (source.CellLine != null)
            {
                target.Specimen = new Feed.Data.Mutations.Models.CellLineModel();

                Map(source.CellLine, (Feed.Data.Mutations.Models.CellLineModel)target.Specimen);
            }
            else if (source.Xenograft != null)
            {
                throw new NotImplementedException("Xenograft specimens are not supported yet");
            }
        }

        private static void Map(TissueModel source, Feed.Data.Mutations.Models.TissueModel target)
        {
            target.Donor = new Feed.Data.Mutations.Models.DonorModel { ReferenceId = source.DonorId };

            target.ReferenceId = source.Id;
            target.Type = source.Type;
            target.TumourType = source.TumourType;
            target.ExtractionDate = source.ExtractionDate;
            target.Source = source.Source;
        }

        private static void Map(CellLineModel source, Feed.Data.Mutations.Models.CellLineModel target)
        {
            target.Donor = new Feed.Data.Mutations.Models.DonorModel { ReferenceId = source.DonorId };

            target.ReferenceId = source.Id;
        }

        private static void Map(in MutationModel source, Feed.Data.Mutations.Models.MutationModel target)
        {
            target.Code = source.GetCode();
            target.Chromosome = source.Chromosome.Value;
            target.SequenceType = source.SequenceType.Value;
            target.Start = Unite.Data.Utilities.Mutations.PositionParser.Parse(source.Position).Start;
            target.End = Unite.Data.Utilities.Mutations.PositionParser.Parse(source.Position).End;
            target.ReferenceBase = source.Ref;
            target.AlternateBase = source.Alt;
            target.Type = Unite.Data.Utilities.Mutations.MutationTypeDetector.Detect(source.Ref, source.Alt);
        }
    }
}
