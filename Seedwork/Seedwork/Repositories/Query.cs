namespace Seedwork.Repositories
{
    public class Query<T>
    {
        public T Key { get; set; }

        //Can be null
        public string Column { get; set; }

        public string Table { get; set; }

        public string Namespace { get; set; }
    }
}
