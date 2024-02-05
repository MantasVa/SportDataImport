using SportDataImport.Domain.Enums;
using SportDataImport.Import;
using SportDataImport.Mongo.Entities;

namespace SportDataImport;

internal static class Mappings
{
    public static Team ToTeam(this TeamDto dto)
    {
        return new Team(dto.Code, dto.Name, dto.Alias, dto.IsVirtual,
            dto.Country == null ? null : new Country(dto.Country.Code, dto.Country.Name), dto.Address, dto.Website,
            dto.TicketsUrl, dto.TwitterAccount, dto.InstagramAccount, dto.FacebookAccount,
            dto.Venue == null ? null : new Venue(dto.Venue?.Name, dto.Venue?.Code, dto.Venue?.Capacity, dto.Venue?.Address, dto.Venue?.Active, dto.Venue?.Notes),
            dto.VenueBackup, dto.NationalCompetitionCode, dto.City, dto.President, dto.Phone,
            dto.Images == null ? null : new Images(dto.Images.Crest), null);
    }

    public static Game ToGame(this GameDto dto)
    {
        return new Game(dto.GameCode, dto.Season?.Code, dto.Season?.Alias, CompetitionExtensions.ToCompetition(dto.Season?.CompetitionCode ?? ""),
            dto.Season?.Year, dto.Season?.StartDate, dto.Group == null ? null : new Group(dto.Group.Id, dto.Group.Order, dto.Group.Name, dto.Group.RawName),
            dto.PhaseType == null ? null : new PhaseType(dto.PhaseType.Code, dto.PhaseType.Alias, dto.PhaseType.Name, dto.PhaseType.IsGroupPhase),
            dto.Round, dto.Played, dto.LocalTimeZone, dto.LocalDate, dto.UtcDate, 
            dto.Local == null ? null : new Side(dto.Local.Club == null ? null : new Club(dto.Local.Club.Code, dto.Local.Club.Name, dto.Local.Club.AbbreviatedName), 
            dto.LocalLast5Form == null ? null : dto.LocalLast5Form, dto.Local.Score, dto.Local.StandingsScore),
            dto.Road == null ? null : new Side(dto.Road.Club == null ? null : new Club(dto.Road.Club.Code, dto.Road.Club.Name, dto.Road.Club.AbbreviatedName),
            dto.RoadLast5Form == null ? null : dto.RoadLast5Form, dto.Road.Score, dto.Road.StandingsScore));
    }

    public static ScheduledEvent ToScheduledEvent(this GameDto dto)
    {
        return new ScheduledEvent(dto.GameCode.Value, dto.Round.Value, dto.Season.Code, dto.Season.Year.Value, 
            dto.LocalDate.Value, dto.UtcDate.Value, CompetitionExtensions.ToCompetition(dto.Season?.CompetitionCode ?? ""), 
            PhaseExtensions.ToPhase(dto!.PhaseType!.Code!),
            dto!.Local!.Club!.Code, dto!.Road!.Club!.Code);
    }
}