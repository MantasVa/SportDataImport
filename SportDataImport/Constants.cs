using SportDataImport.Domain;

namespace SportDataImport;

internal static class Constants
{
    public static int[] ModernEuroleagueSeasonsYears = [
        2000,
        2001,
        2002,
        2003,
        2004,
        2005,
        2006,
        2007,
        2008,
        2009,
        2010,
        2011,
        2012,
        2013,
        2014,
        2015,
        2016,
        2017,
        2018,
        2019,
        2020,
        2021,
        2022,
        2023,
    ];

    public static string[] EuroleagueSeasonsWithBetOddsData = ModernEuroleagueSeasonsYears.Where(year => year >= 2008).Select(EuroleagueHelper.ToSeason).ToArray();

    public static string[] Top16Seasons = ModernEuroleagueSeasonsYears.Where(year => year < 2016).Select(EuroleagueHelper.ToSeason).ToArray();
}
