using System;

namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public class SampleModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public DateTime? Date { get; set; }

        public TissueModel Tissue { get; set; }
        public CellLineModel CellLine { get; set; }
        public XenograftModel Xenograft { get; set; }


        public virtual void Sanitise()
        {
            Id = Id?.Trim();
            Name = Name?.Trim();

            Tissue?.Sanitise();
            CellLine?.Sanitise();
            Xenograft?.Sanitise();
        }
    }
}