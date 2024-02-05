using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using SportDataImport.Domain.Enums;
using SportDataImport.Import;

namespace SportDataImport.Clients;

internal interface IEuroleagueClient
{
    Task<(bool, GameDto?)> GetGame(string seasonCode, long gameCode);
    Task<(bool, GameDto[])> GetGames(string seasonCode);
    Task<(bool, TeamDto[]?)> GetTeams();
}

internal class EuroleagueClient : IEuroleagueClient
{
    private readonly HttpClient _httpClient;

    public EuroleagueClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://api-live.euroleague.net/")
        };
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<(bool, GameDto?)> GetGame(string seasonCode, long gameCode)
    {
        var response = await _httpClient.GetAsync($"v3/competitions/{Competition.Euroleague.ToAbbreviation()}/seasons/{seasonCode}/games/{gameCode}/report");

        if (!response.IsSuccessStatusCode)
        {
            return (false, default);
        }

        var data = await response.Content.ReadFromJsonAsync<GameDto>();
        return (true, data);
    }

    public async Task<(bool, GameDto[])> GetGames(string seasonCode)
    {
        var response = await _httpClient.GetAsync($"/v2/competitions/{Competition.Euroleague.ToAbbreviation()}/seasons/{seasonCode}/games");

        if (!response.IsSuccessStatusCode)
        {
            return (false, Array.Empty<GameDto>());
        }

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (content?["data"] == null)
        {
            throw new ArgumentNullException(nameof(content), "Data of response is empty");
        }

        var data = JsonConvert.DeserializeObject<GameDto[]>(content?["data"]?.ToString() ?? "")!;
        return (true, data);
    }


    public async Task<(bool, TeamDto[]?)> GetTeams()
    {
        var response = await _httpClient.GetAsync("v3/clubs");
        if (!response.IsSuccessStatusCode)
        {
            return (false, default);
        }

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (content?["data"] == null)
        {
            throw new ArgumentNullException(nameof(content), "Data of response is empty");
        }

        var data = JsonConvert.DeserializeObject<TeamDto[]>(content?["data"]?.ToString() ?? "");
        return (true, data);
    }
}
