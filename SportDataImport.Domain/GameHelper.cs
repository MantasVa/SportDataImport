namespace SportDataImport.Domain;
public static class GameHelper
{
    public static int[] WinsToInt(this string[] wins) 
        => wins == null ? [] : wins.Select(x => x.ToLowerInvariant() == "w" ? 1 : 0).ToArray();
}
