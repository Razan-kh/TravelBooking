public class PagedResult<T>
{
    public object Meta { get; set; } = null!;
    public IEnumerable<T> Data { get; set; } = new List<T>();
}