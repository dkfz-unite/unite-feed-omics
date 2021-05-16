namespace Unite.Mutations.Indices
{
    public interface IMapper<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        void Map(in TSource source, TTarget target);
    }
}
