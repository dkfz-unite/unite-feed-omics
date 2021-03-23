using System;
using Unite.Data.Entities;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Data.Services.Mutations.Models
{
    public class AnalysisModel
    {
        public string Name { get; set; }
        public AnalysisType? Type { get; set; }
        public DateTime Date { get; set; }

        public FileModel File { get; set; }

        public Analysis ToEntity()
        {
            var analysis = new Analysis
            {
                Name = Name,
                TypeId = Type,
                Date = Date,

                File = new File
                {
                    Name = File.Name,
                    Link = File.Link,
                    Created = File.Created,
                    Updated = File.Updated
                }

            };

            return analysis;
        }
    }
}
