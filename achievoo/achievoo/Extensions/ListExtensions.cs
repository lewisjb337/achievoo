namespace achievoo.Extensions;

public static class ListExtensions
{
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, string searchText, Func<T, string> searchSelector)
    {
        return list.Where(x => searchSelector(x).Contains(searchText, StringComparison.OrdinalIgnoreCase));
    }
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, int searchId, Func<T, int> searchSelector)
    {
        return list.Where(x => searchSelector(x).Equals(searchId));
    }
}