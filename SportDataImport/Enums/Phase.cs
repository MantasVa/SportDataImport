namespace SportDataImport.Enums;

internal enum Phase : byte
{
    RegularSeason = 1,
    Playoffs = 2,
}

internal static class PhaseExtensions
{
    public static Phase ToPhase(string phase)
    {
        return phase.ToUpper() switch
        {
            "RS" => Phase.RegularSeason,
            "PO" => Phase.Playoffs,
            _ => throw new ArgumentOutOfRangeException(nameof(phase)),
        };
    }

    public static string ToAbbreviation(this Phase phase)
    {
        return phase switch
        {
            Phase.RegularSeason => "RS",
            Phase.Playoffs => "PO",
            _ => throw new ArgumentOutOfRangeException(nameof(phase)),
        };
    }
}