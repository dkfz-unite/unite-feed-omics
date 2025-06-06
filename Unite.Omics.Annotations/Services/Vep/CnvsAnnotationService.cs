﻿using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Essentials.Extensions;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Models.Dna;

namespace Unite.Omics.Annotations.Services.Vep;


public class CnvsAnnotationService
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly AnnotationsDataLoader _dataLoader;


    public CnvsAnnotationService(
        IDbContextFactory<DomainDbContext> dbContextFactory,
        IEnsemblDataOptions ensemblOptions,
        IEnsemblVepOptions ensemblVepOptions
        )
    {
        _dbContextFactory = dbContextFactory;
        _dataLoader = new AnnotationsDataLoader(ensemblOptions, ensemblVepOptions, dbContextFactory);
    }


    public EffectsDataModel[] Annotate(int[] variantIds, int grch)
    {
        var variants = LoadVariants(variantIds);

        var codes = variants.Select(GetVepVariantCode).ToArray();

        var annotations = _dataLoader.LoadData(codes, grch).Result;

        return annotations;
    }


    private Variant[] LoadVariants(int[] variantIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Variant>()
            .AsNoTracking()
            .Where(entity => variantIds.Contains(entity.Id))
            .OrderBy(entity => entity.ChromosomeId)
            .ThenBy(entity => entity.Start)
            .ToArray();
    }

    private string GetVepVariantCode(Variant variant)
    {

        var id = variant.Id;
        var chr = variant.ChromosomeId.ToDefinitionString();
        var start = variant.Start;
        var end = variant.End;
        var type = "CNV";

        return string.Join('\t', chr, start, id, ".", $"<{type}>", ".", ".", $"SVTYPE={type};END={end}", ".");
    }
}
