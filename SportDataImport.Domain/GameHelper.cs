namespace SportDataImport.Domain;
public static class GameHelper
{
    public static int[] WinsToInt(this string[] wins) 
        => wins == null ? [] : wins.Select(x => x.ToLowerInvariant() == "w" ? 1 : 0).ToArray();

    // Converts decimal odds to probability percentage. Probability from 0 > x <= 1
    public static float ToProbability(this double odd)
        => 100 / (float)odd / 100;
}
