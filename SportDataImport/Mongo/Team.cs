using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SportDataImport.Mongo;

public sealed record class Team
{
    [BsonId]
    [BsonElement("_id_")]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("alias")]
    public string? Alias { get; set; }

    [BsonElement("isVirtual")]
    public bool? IsVirtual { get; set; }

    [BsonElement("country")]
    public Country? Country { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("website")]
    public string? Website { get; set; }

    [BsonElement("ticketsUrl")]
    public string? TicketsUrl { get; set; }

    [BsonElement("twitterAccount")]
    public string? TwitterAccount { get; set; }

    [BsonElement("instagramAccount")]
    public string? InstagramAccount { get; set; }

    [BsonElement("facebookAccount")]
    public string? FacebookAccount { get; set; }

    [BsonElement("venue")]
    public Venue? Venue { get; set; }

    [BsonElement("venueBackup")]
    public string? VenueBackup { get; set; }

    [BsonElement("nationalCompetitionCode")]
    public string? NationalCompetitionCode { get; set; }

    [BsonElement("city")]
    public string? City { get; set; }

    [BsonElement("president")]
    public string? President { get; set; }

    [BsonElement("phone")]
    public string? Phone { get; set; }

    [BsonElement("images")]
    public Images? Images { get; set; }

    [BsonElement("finalFourAppearances")]
    public int[]? FinalFourAppearances { get; set; }

    public Team(string? id, string? name, string? alias, bool? isVirtual, Country? country, string? address, string? website, string? ticketsUrl, string? twitterAccount,
        string? instagramAccount, string? facebookAccount, Venue? venue, string? venueBackup, string? nationalCompetitionCode, string? city, string? president,
        string? phone, Images? images, int[]? finalFourAppearances)
    {
        Id = id;
        Name = name;
        Alias = alias;
        IsVirtual = isVirtual;
        Country = country;
        Address = address;
        Website = website;
        TicketsUrl = ticketsUrl;
        TwitterAccount = twitterAccount;
        InstagramAccount = instagramAccount;
        FacebookAccount = facebookAccount;
        Venue = venue;
        VenueBackup = venueBackup;
        NationalCompetitionCode = nationalCompetitionCode;
        City = city;
        President = president;
        Phone = phone;
        Images = images;
        FinalFourAppearances = finalFourAppearances;
    }
}

public record class Country
{
    [BsonElement("code")]
    public string? Code { get; set; }


    [BsonElement("name")]
    public string? Name { get; set; }

    public Country(string? code, string? name)
    {
        Code = code;
        Name = name;
    }
}

public record class Images
{
    [BsonElement("crest")]
    public string? Crest { get; set; }

    public Images(string? crest)
    {
        Crest = crest;
    }
}

public record class Venue
{
    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("code")]
    public string? Code { get; set; }

    [BsonElement("capacity")]
    public long? Capacity { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("active")]
    public bool? Active { get; set; }

    [BsonElement("notes")]
    public string? Notes { get; set; }

    public Venue(string? name, string? code, long? capacity, string? address,
        bool? active, string? notes)
    {
        Name = name;
        Code = code; 
        Capacity = capacity;
        Address = address;
        Active = active;
        Notes = notes;
    }
}
