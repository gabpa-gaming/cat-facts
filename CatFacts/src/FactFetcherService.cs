
using System.Text.Json;

namespace CatFacts.src;

public class FactFetcherService
{
    private readonly HttpClient _client;
    public async Task<CatFact> FetchNewFactAsync()
    {
        try
        {
            var httpResponse = await _client.GetAsync("https://catfact.ninja/fact");

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException();
            }
            var response = await httpResponse.Content.ReadAsStringAsync();
            await LogResponse(response);
            return ParseFact(response);
        }
        catch
        {
            var err = new CatFact();
            err.Fact = "Błąd przy połączeniu. Spróbuj ponownie później.";
            err.Length = err.Fact.Length;
            return err;
        }
    }

    private async Task LogResponse(string response)
    {
        try
        {
            using var file = new StreamWriter("response.log", true);
            await file.WriteLineAsync($"{DateTime.Now}: {response}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas zapisu do pliku: {ex.Message}");
        }
    }
 

    private static CatFact ParseFact(string jsonFact)
    {
        try
        {
            var fact = JsonSerializer.Deserialize<CatFact>(jsonFact);
            if (fact == null)
            {
                throw new JsonException("Błąd odczytu faktu. Niewłaściwy format");
            }
            return fact;
        }
        catch (JsonException)
        {
            throw new JsonException("Błąd odczytu faktu. Niewłaściwy format");
        }
    }

    public FactFetcherService(HttpClient client)
    {
        _client = client;
    }
        
}
