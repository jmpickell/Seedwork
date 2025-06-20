namespace Seedwork.Repositories
{
    public class Query<T>
    {
        public T Key { get; set; }
        public string Table { get; set; }
        public string Namespace { get; set; }
    }
}
