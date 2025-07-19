using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatFacts.src;

public class CatFactsApp
{
    private readonly FactFetcherService _factFetcherService;
    private int _currentFactIndex = 0;

    private List<CatFact> _facts = new List<CatFact>();

    public void StartApp()
    {
        DisplayFact(0);
        var input = new ConsoleKeyInfo();
        while (true)
        {
            switch (input)
            {
                case { Key: ConsoleKey.Q }:
                case { Key: ConsoleKey.Escape }:
                    if (ConfirmExit()) return;
                    DisplayFact(_currentFactIndex);
                    break;

                case { Key: ConsoleKey.N }:
                case { Key: ConsoleKey.RightArrow }:
                case { Key: ConsoleKey.D }:
                    DisplayFact(++_currentFactIndex);
                    break;

                case { Key: ConsoleKey.P }:
                case { Key: ConsoleKey.LeftArrow }:
                case { Key: ConsoleKey.A }:
                    if (_currentFactIndex > 0) DisplayFact(--_currentFactIndex);
                    break;
            }
            try
            {
                input = Console.ReadKey(true);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Błąd: Spróbuj uruchomić aplikację w innej konsoli.");
                return;
            }
        }
    }

    public void DisplayFact(int index)
    {
        if (index == _facts.Count)
        {
            var fact = FetchAndWaitForNewFact().GetAwaiter().GetResult();
            _facts.Add(fact);
        }
        else if (index < 0)
        {
            throw new Exception("Błąd wyświetlania faktu");
        }

        Console.Clear();

        var width = Console.WindowWidth;
        var height = Console.WindowHeight;


        Console.WriteLine("Witaj w świecie kocich faktów!");
        Console.WriteLine("Czy wiedziałeś, że...?");

        var words = _facts[index].Fact.Split(' ');
        var boxSpacesFromWindow = 6;
        var boxLength = width - boxSpacesFromWindow * 2;

        var options =
            (index == 0 ? "" : "← (P)oprzedni | ") +
                        "(N)astępny → | (Q) wyjście";

        var minHeight = CalculateMinBoxHeight(boxLength, words) + 1;

        if (minHeight > height - 3 ||  
            Array.Exists(words, w => w.Length > boxLength - 2))
        {
            //In case the console window is too small, we skip the box printing
            Console.WriteLine(_facts[index].Fact);
            Console.WriteLine(options.PadLeft((width + options.Length) / 2));
            return;
        }

        PrintBox(boxLength, minHeight, boxSpacesFromWindow, 20, words);
        Console.WriteLine(options.PadLeft((width + options.Length) / 2));
    }

    public void PrintBox(int boxLength, int boxHeight, int boxSpacesFromWindow, 
        int speed, params string[] words)
    {
        var boxCharacter = '#';

        Console.ForegroundColor = ConsoleColor.DarkGray;

        Console.WriteLine();
        Console.WriteLine(new string(' ', boxSpacesFromWindow) +
                          new string(boxCharacter, boxLength));

        var boxStartColumn = Console.CursorLeft + boxSpacesFromWindow + 1;
        var boxStartRow = Console.CursorTop;

        for (int i = 0; i < boxHeight - 2; i++)
        {
            Console
                .WriteLine(new string(' ', boxSpacesFromWindow) + 
                           boxCharacter +
                           new string(' ', boxLength - 2) +
                           boxCharacter
                );
        }

        Console.WriteLine(new string(' ', boxSpacesFromWindow) +
                    new string(boxCharacter, boxLength));

        Console.CursorLeft = boxStartColumn;
        Console.CursorTop = boxStartRow;

        Console.ResetColor();

        foreach (var word in words)
        {
            if (word.Length + Console.CursorLeft >= boxLength + boxSpacesFromWindow)
            {
                Console.CursorLeft = boxStartColumn;
                Console.CursorTop++;
            }
            Console.Write(word);
            Console.Write(Console.CursorLeft + 1 >= boxLength + boxSpacesFromWindow ? "" : " ");
            Thread.Sleep(speed);
        }
        
        Console.CursorLeft = 0;                      //
        Console.CursorTop = boxStartRow + boxHeight; //Return cursor back to last line 
    }

    public int CalculateMinBoxHeight(int boxLength, params string[] words)
    {
        var height = 3;
        var charInLine = 0;
        foreach (var word in words)
        {
            if (word.Length + charInLine >= boxLength-2)
            {
                charInLine = 0;
                height++;
            }
            charInLine += word.Length;
            charInLine += (charInLine + 1 >= boxLength-2 ? 0 : 1);
        }
        return height;
    }

    public bool ConfirmExit()
    {
        {
            Console.Clear();
            Console.Write("Czy na pewno chcesz wyjść?\nZa każdym razem gdy wychodzisz płacze kociątko ... 😿😿😿 \n(");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Jestem świadom konsekwecji");
            Console.ResetColor();
            Console.Write("/Nie/Cofnij/Przepraszam)");
            Console.ResetColor();
        }


        var stayChars = new char[] { 'n', 'p', 'c' };
        var exitChars = new char[] { 'e', 'q', 't', 'j' };

        var input = '\0';

        while (!exitChars.Contains(input = Console.ReadKey(true).KeyChar))
        {
            if (stayChars.Contains(input)) return false;
        }

        return true;
    }


    public async Task<CatFact> FetchAndWaitForNewFact()
    {
        using var cts = new CancellationTokenSource();
        var anim = LoadingAnimation(cts.Token);
        var factTask = _factFetcherService.FetchNewFactAsync();

        var fact = await factTask;

        cts.Cancel();

        return fact;
    }

    public async Task LoadingAnimation(CancellationToken cancellationToken)
    {
        var dotCount = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            dotCount = (dotCount % 8) + 1;
            var dotString = new string('.', dotCount);
            Console.Write($"\rŁaduję następną ciekawostkę{dotString}        "); 

            await Task.Delay(50);
        }
    }

    public CatFactsApp(FactFetcherService factFetcherService)
    {
        _factFetcherService = factFetcherService;
    }
}
