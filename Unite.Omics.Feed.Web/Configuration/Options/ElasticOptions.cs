﻿using Unite.Indices.Context.Configuration.Options;

namespace Unite.Omics.Feed.Web.Configuration.Options;

public class ElasticOptions : IElasticOptions
{
    public string Host => Environment.GetEnvironmentVariable("UNITE_ELASTIC_HOST");
    public string User => Environment.GetEnvironmentVariable("UNITE_ELASTIC_USER");
    public string Password => Environment.GetEnvironmentVariable("UNITE_ELASTIC_PASSWORD");
}
