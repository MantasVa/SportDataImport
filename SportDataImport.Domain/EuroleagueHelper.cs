using SportDataImport.Domain.Enums;

namespace SportDataImport.Domain;

public static class EuroleagueHelper
{
    public static string ToSeason(int year) =>
        Competition.Euroleague.ToAbbreviation() + year.ToString();

    public static string GetCurrentEuroleagueSeasonCode() =>
        DateTime.UtcNow.Month <= 6 ? $"E{DateTime.UtcNow.Year - 1}" : $"E{DateTime.UtcNow.Year}";
}
