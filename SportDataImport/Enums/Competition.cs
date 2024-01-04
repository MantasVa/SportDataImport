namespace SportDataImport.Enums;

internal enum Competition : byte
{
    None = 0,
    Euroleague = 1,
}

internal static class CompetitionExtensions
{
    public static Competition ToCompetition(string competition)
    {
        return competition.ToUpper() switch
        {
            "E" => Competition.Euroleague,
            _ => throw new ArgumentOutOfRangeException(nameof(competition)),
        };
    }

    public static string ToAbbreviation(this Competition competition)
    {
        return competition switch
        {
            Competition.Euroleague => "E",
            _ => throw new ArgumentOutOfRangeException(nameof(competition)),
        };
    }
}
