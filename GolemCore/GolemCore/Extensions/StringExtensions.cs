namespace GolemCore.Extensions;

public static class StringExtensions
{
    public static int ToPartId(this string partIdWithSuffix)
    {
        return int.Parse(partIdWithSuffix.Split('.')[0]);
    }
}