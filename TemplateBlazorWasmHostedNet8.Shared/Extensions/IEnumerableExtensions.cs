namespace TemplateBlazorWasmHostedNet8.Shared.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        Random rnd = new Random();
        return source.OrderBy((item) => rnd.Next());
    }

}
