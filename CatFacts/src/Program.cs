
using CatFacts.src;

using HttpClient client = new();
var factFetcher = new FactFetcherService(client);

var app = new CatFactsApp(factFetcher);
Console.WriteLine("Miłego przeglądania faktów o kotach!");
app.StartApp();