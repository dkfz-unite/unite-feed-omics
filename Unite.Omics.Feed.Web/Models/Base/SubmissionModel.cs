using System.Text.Json.Serialization;

namespace Unite.Omics.Feed.Web.Models.Base;

public abstract record SubmissionModel
{
    public abstract ResourceModel[] Resources { get; set; }
}