namespace SportDataImport.Domain.Enums;

public enum Phase : byte
{
    RegularSeason = 1,
    Playoffs = 2,
    Top16 = 3,
    FinalFour = 4,
}

public static class PhaseExtensions
{
    public static Phase ToPhase(string phase)
    {
        return phase.ToUpper() switch
        {
            "RS" => Phase.RegularSeason,
            "PO" => Phase.Playoffs,
            "TS" => Phase.Top16,
            "FF" => Phase.FinalFour,
            _ => throw new ArgumentOutOfRangeException(nameof(phase)),
        };
    }

    public static string ToAbbreviation(this Phase phase)
    {
        return phase switch
        {
            Phase.RegularSeason => "RS",
            Phase.Playoffs => "PO",
            Phase.Top16 => "TS",
            Phase.FinalFour => "FF",
            _ => throw new ArgumentOutOfRangeException(nameof(phase)),
        };
    }
}