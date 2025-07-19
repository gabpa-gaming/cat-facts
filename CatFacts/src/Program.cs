
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using CatFacts.src;



Console.WriteLine("Hello, World! cat\u201ds //word//");

using HttpClient client = new();
var factFetcher = new FactFetcherService(client);

var app = new CatFactsApp(factFetcher);
app.StartApp();
