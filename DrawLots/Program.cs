// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var consoleKeys = new List<ConsoleKey>() {
    ConsoleKey.UpArrow,
    ConsoleKey.F6,
    ConsoleKey.LeftArrow,
    ConsoleKey.F2,
    ConsoleKey.PageDown,
    ConsoleKey.F9,
    ConsoleKey.RightArrow,
    ConsoleKey.F4,
    ConsoleKey.DownArrow,
    ConsoleKey.F5,
    ConsoleKey.PageUp,
    ConsoleKey.F1,
    ConsoleKey.F8,
    ConsoleKey.F3,
    ConsoleKey.F10,
    ConsoleKey.F7
};

var dt = DateTime.Now;
var dayOfWeek = dt.DayOfWeek;
var month = dt.Month;
var day = dt.Day;
var year = dt.Year;
var hour = dt.Hour;
var minute = dt.Minute;
var second = dt.Second;
var microsecond = dt.Millisecond;
var nanosecond = dt.Nanosecond;
var meridiemHour = dt.ToString("h:mm:ss tt");
Console.WriteLine($"Hello! Today is {dayOfWeek}, {month}/{day}/{year}, and the time is {meridiemHour}.");

var attempts = 0;
var maxAttempts = 10;
var people = new List<Person>();

var morePeople = true;
var minPeople = 2;
var maxPeople = 10;

var boys = new Boys();
var girls = new Girls();
var hinter = new Hinter();
while (morePeople)
{
    if (0 != people.Count)
    {
        Console.Clear();
    }
    attempts = 0;
    var genderer = new Gender();
    var namer = new Namer();
    var hintKey = hinter.Next();
    var cheatingActive = false;
    if (args.Length > 0 && string.Equals("cheater", args[0], StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"Hey cheater, the hint key is {hintKey}");
        cheatingActive = true;
    }
    do
    {
        if (0 != attempts)
        {
            Console.WriteLine();
        }
        var actualGender = genderer.GetGender();
        string actualName = string.Empty;
        var firstAttempt = true;
        do
        {
            actualName = namer.GetName(!firstAttempt);
            if (firstAttempt)
            {
                firstAttempt = false;
            }
            var matches = people.FindAll(e => string.Equals(e.Name, actualName, StringComparison.OrdinalIgnoreCase));
            if (matches.Count != 0)
            {
                Console.WriteLine($"Sorry, the name '{actualName}' has already been taken. Please modify as necessary.");
                actualName = string.Empty;
            }
        } while (string.IsNullOrWhiteSpace(actualName));
        string fauxName;
        if (actualGender == Gender.Kind.Male || actualGender == Gender.Kind.Boy)
        {
            fauxName = girls.GetName();
        }
        else
        {
            fauxName = boys.GetName();
        }
        Console.WriteLine($"Hello {fauxName}! That's a very nice name for a {Gender.KindToString(Gender.ConfuseGender(actualGender))}.");
        Console.Write("Did I get your name right?! [y/n]  ");
        ConsoleKey key;
        do
        {
            key = Console.ReadKey(true).Key;
        } while (key != ConsoleKey.Y && key != ConsoleKey.N && key != hintKey);
        Console.WriteLine(key);

        if (ConsoleKey.N == key)
        {
            hinter.GiveHint();
            Console.WriteLine("Hmm. Sorry about that. Are you sure you typed the answers correctly?");
            TextWriter.RepeatLastChar("Let's try again", 4, '.', 1000);
            hinter.GiveHint();
        }
        else if (ConsoleKey.Y == key)
        {
            hinter.GiveHint();
            Console.WriteLine($"Hmm. You believe that '{fauxName}' and '{actualName}' are the same?!");
            TextWriter.RepeatLastChar("Let's try again", 4, '.', 1000);
            hinter.GiveHint();
        }
        else
        {
            Console.WriteLine($"Good job {(cheatingActive ? "cheater" : actualName)}! You pressed the secret {hintKey} key. Welcome!");
            people.Add(new Person() { Gender = actualGender, Name = actualName });
            break;
        }
        if (maxAttempts == attempts++)
        {
            people.Add(new Person() { Gender = actualGender, Name = actualName });
            Console.WriteLine($"You've run out of attempts, but welcome all the same {actualName}!");
            break;
        }
    } while (true);
    if (maxPeople == people.Count)
    {
        Console.WriteLine($"Maximum count of {maxPeople} participants reached!");
        break;
    }

    if (people.Count < minPeople)
    {
        Console.WriteLine();
        Console.WriteLine($"The minium number of players is {minPeople}. Press the [Enter] key when the next player is ready.");
        ConsoleKey rdyKey;
        do
        {
            rdyKey = Console.ReadKey(true).Key;
        } while (rdyKey != ConsoleKey.Enter);
    }
    else
    {
        Console.Write("Are there more people? [y/n]  ");
        ConsoleKey morePeopleKey;
        do
        {
            morePeopleKey = Console.ReadKey(true).Key;
        } while (morePeopleKey != ConsoleKey.Y && morePeopleKey != ConsoleKey.N);
        Console.WriteLine(morePeopleKey);
        if (ConsoleKey.N == morePeopleKey)
        {
            break;
        }
    }
}
Console.Clear();

var boysCount = people.Count(i => (i.Gender == Gender.Kind.Male || i.Gender == Gender.Kind.Boy));
var boysText = 1 == boysCount ? "boy" : "boys";
var girlsCount = people.Count(i => (i.Gender == Gender.Kind.Female || i.Gender == Gender.Kind.Girl));
var girlsText = 1 == girlsCount ? "girl" : "girls";
Console.WriteLine($"It looks like we have {people.Count} participants, {boysCount} {boysText} and {girlsCount} {girlsText}!");

Console.WriteLine("Best of how many rounds? (1 is the minimum, 10 is the maximum)");
var numberOfRounds = 0;
do
{
    var v = Console.ReadLine();
    if (int.TryParse(v, out numberOfRounds))
    {
        if (numberOfRounds < 1 || numberOfRounds > 10)
        {
            Console.WriteLine($"{numberOfRounds} is not between 1 and 10. Please try again.");
            numberOfRounds = 0;
        }
    }
    else
    {
        Console.WriteLine($"{v} is not a valid number. Please try again.");
    }
} while (0 == numberOfRounds);

var roundNumber = 0;
Person winner = null;
var clearConsole = false;
var competitors = people;
var isTieBreakingRound = false;
while (null == winner)
{
    if (clearConsole)
    {
        Console.Clear();
    }

    Console.WriteLine("Priming the random number generators...");
    var tt = new Tumbler(250, 1000, 100, true);
    (int inclusiveMin, int exclusiveMax) = tt.Tumble(1000, 5, 10);
    Console.WriteLine("Random number generation complete!");
    Console.WriteLine();
    Console.WriteLine();

    var numbersAlreadyChosen = new List<int>();
    foreach (Person person in competitors)
    {
        var keepTrying = true;
        do
        {
            Console.Write($"{person.Name}, please choose a number between {inclusiveMin} and {exclusiveMax}:  ");
            var value = Console.ReadLine();
            var success = int.TryParse(value, out var valueAsInt);
            if (success)
            {
                if (valueAsInt >= inclusiveMin && valueAsInt < exclusiveMax)
                {
                    if (numbersAlreadyChosen.Contains(valueAsInt))
                    {
                        Console.WriteLine($"That number has already been chosen. Please choose another.");
                    }
                    else
                    {
                        keepTrying = false;
                        person.Guess = valueAsInt;
                        numbersAlreadyChosen.Add(valueAsInt);
                    }
                }
                else
                {
                    Console.WriteLine($"{valueAsInt} is not between {inclusiveMin} and {exclusiveMax}. Please try again.");
                }
            }
            else
            {
                Console.WriteLine($"{value} is not a number between {inclusiveMin} and {exclusiveMax}. Please try again.");
            }
            if (keepTrying)
            {
                TextWriter.RepeatLastChar("Resetting the randomization engine", 1, '.', 100);
                tt.Tumble(250, 5, 10);
            }
        } while (keepTrying);
    }

    TextWriter.RepeatLastChar($"Ok, here's who is who, and what is what, and what guessed who!", 5, '!', 1000);
    foreach (Person person in competitors)
    {
        TextWriter.RepeatLastChar($"{person.Name} who is a {Gender.KindToString(person.Gender)} guessed that the number is {person.Guess}.", 1, ' ', 3000);
    }

    var theNumber = Mtf.Random.Randomizer.Next(inclusiveMin, exclusiveMax);
    TextWriter.RepeatLastChar($"The actual number is {theNumber}. Who is the closest!?", 10, '?', 350);

    var topTies = new List<Person>();
    Person currentWinner = null;
    foreach (var person in competitors)
    {
        person.HowClose = Math.Abs(theNumber - person.Guess);
        if (0 == person.HowClose)
        {
            Console.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!{person.Name} guessed the exact value!!!!!!!!!!!!!!!!!!!!!!");
        }
        // the first person is our default winner and remains so until such time as we encounter a person whose guess is closer
        if (null == currentWinner)
        {
            currentWinner = person;
        }
        else
        {
            if (person.HowClose < currentWinner.HowClose)
            {
                currentWinner = person;
                topTies.Clear();
            }
            else if (person.HowClose == currentWinner.HowClose)
            {
                // if topTies is empty, then add both
                if (0 == topTies.Count)
                {
                    topTies.Add(person);
                    topTies.Add(currentWinner);
                }
                else
                {
                    // otherwise, just add person because currentWinner is already in topTies
                    topTies.Add(person);
                }
            }
        }
    }

    // if topTies is non-empty, that means we need to do the round over again
    if (topTies.Count > 0)
    {
        isTieBreakingRound = true;
        var names = string.Join(", ", topTies.Select(e => e.Name));
        Console.WriteLine($"The guess of {names} were all {topTies[0].HowClose} away from {theNumber}. This round will be repeated for only them.");
        Console.WriteLine($"Press the [Enter] key when ready.");
        ConsoleKey rdyKey;
        do
        {
            rdyKey = Console.ReadKey(true).Key;
        } while (rdyKey != ConsoleKey.Enter);
        // our competitors are now just those within topTies
        competitors = topTies;
        clearConsole = true;
        continue;
    }
    else
    {
        isTieBreakingRound = false;
        competitors = people; // now that we're not in a runoff, all people are competitors
        roundNumber++;
        currentWinner.WinCount++;
        if (roundNumber >= numberOfRounds)
        {
            var maxScore = competitors.Max(e => e.WinCount);
            var matches = competitors.FindAll(e => maxScore == e.WinCount);
            if (matches.Count > 1)
            {
                isTieBreakingRound = true;
                var names = string.Join(", ", matches.Select(e => e.Name));
                Console.WriteLine($"{names} all have won {maxScore} rounds. There will be a runoff round between them!");
                Console.WriteLine($"Press the [Enter] key when ready.");
                ConsoleKey rdyKey;
                do
                {
                    rdyKey = Console.ReadKey(true).Key;
                } while (rdyKey != ConsoleKey.Enter);
                // our competitors are now just those within matches
                competitors = matches;
                clearConsole = true;
                continue;
            }
            else
            {
                if (numberOfRounds > 1)
                {
                    Console.WriteLine($"The winner of this final round is {currentWinner.Name}!");
                }
                winner = currentWinner;
            }
        }
        else
        {
            isTieBreakingRound = false;
            competitors = people;
            Console.WriteLine($"The winner of round {roundNumber} of {numberOfRounds} is {currentWinner.Name}!");
            Console.WriteLine($"Press the [Enter] key when ready to start the next round.");
            ConsoleKey rdyKey;
            do
            {
                rdyKey = Console.ReadKey(true).Key;
            } while (rdyKey != ConsoleKey.Enter);
            clearConsole = true;
        }
    }
}

var heShe = "she";
if (winner.Gender == Gender.Kind.Male)
{
    heShe = "he";
}

if (numberOfRounds > 1)
{
    foreach (var person in people)
    {
        Console.WriteLine($"{person.Name} win count: {person.WinCount}.");
        Thread.Sleep(1000);
    }
    // roundNumber might be > than numberOfRounds due to tie-breaking rounds
    Console.WriteLine($"{winner.Name} is our winner because {heShe} won {winner.WinCount} out of {Math.Max(numberOfRounds, roundNumber)} rounds!");
    if (winner.WinCount >= numberOfRounds)
    {
        Console.WriteLine($"Wow! Plus, {winner.Name} won all of the rounds!");
    }
}
else
{
    Console.WriteLine($"{winner.Name} is our winner because {heShe} guessed {winner.Guess} which is {winner.HowClose} away from the true number!");
}

var celebrationCount = 1000;
var maxSleep = 500;
var sleepDecrement = 10;
var toSleep = maxSleep + 10;
var ii = 0;
while (ii < celebrationCount)
{
    Console.WriteLine($"Congratulations {winner.Name}{new string('!', ii)}");
    if (toSleep > 0)
    {
        toSleep -= sleepDecrement;
        Thread.Sleep(toSleep);
    }
    else
    {
        Thread.Sleep(10);
    }
    ++ii;
}


internal class Tumbler
{
    private readonly int _maximumMin;
    private readonly int _maximumMax;
    private readonly int _minDistance;
    private const int DefaultSleep = 8;
    private readonly bool _varySleep;

    internal Tumbler(int maximumMin, int maximumMax, int minDistance, bool varySleep)
    {
        _maximumMin = maximumMin;
        _maximumMax = maximumMax;
        _minDistance = minDistance;
        _varySleep = varySleep;
        if (_maximumMin >= _maximumMax || _maximumMax - _maximumMin < minDistance)
        {
            throw new Exception($"{_maximumMin}, {_maximumMax}, {_minDistance} are an invalid combination of arguments");
        }
    }

    internal (int inclusiveMin, int exclusiveMax) Tumble(int tumbles, int tumblerCount, int sleep = DefaultSleep)
    {
        var min = Mtf.Random.Randomizer.Next(_maximumMin);
        var max = Mtf.Random.Randomizer.Next(_maximumMin, _maximumMax);
        while (max - min < _minDistance)
        {
            min = Mtf.Random.Randomizer.Next(_maximumMin);
            max = Mtf.Random.Randomizer.Next(_maximumMin, _maximumMax);
        }
        var ii = 0;
        while (ii < tumbles)
        {
            var jj = 0;
            var sb = new StringBuilder();
            while (jj < tumblerCount)
            {
                sb.Append($"{Mtf.Random.Randomizer.Next(min, max):D3}   ");
                ++jj;
            }
            Console.Write(sb.ToString());
            Thread.Sleep(_varySleep ? Mtf.Random.Randomizer.Next(sleep) : sleep);

            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            ++ii;
            if (tumbles != ii)
            {
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLineCursor);
            }
            else
            {
                Console.WriteLine();
            }
        }
        return (min, max);
    }
}


internal class Person
{
    public Gender.Kind Gender { get; set; }
    public string? Name { get; set; }
    public int Guess { get; set; }
    public int HowClose { get; set; }
    public int WinCount { get; set; }
}


internal abstract class TrackedRandomIndexer
{
    protected readonly List<int> _used = new(); // indexes
    protected int GetRandomUnusedIndex(int upperBound)
    {
        if (upperBound == _used.Count)
        {
            _used.Clear();
        }

        int result;
        do
        {
            result = Mtf.Random.Randomizer.Next(upperBound);
        } while (_used.Contains(result));
        _used.Add(result);
        return result;
    }
}


internal abstract class Names : TrackedRandomIndexer
{
    internal abstract string GetName();
}

internal class Boys : Names
{
    private readonly static List<string> _names = new() {
        "John",
        "Mark",
        "Liam",
        "Noah",
        "Oliver",
        "James",
        "Elijah",
        "William",
        "Henry",
        "Lucas",
        "Benjamin",
        "Theodore"
    };

    internal override string GetName()
    {
        return _names[GetRandomUnusedIndex(_names.Count)];
    }
}

internal class Girls : Names
{
    private readonly static List<string> _names = new() {
        "Priscilla",
        "Marjory",
        "Olivia",
        "Emma",
        "Charlotte",
        "Amelia",
        "Sophia",
        "Isabella",
        "Ava",
        "Mia",
        "Evelyn",
        "Luna"
    };

    internal override string GetName()
    {
        return _names[GetRandomUnusedIndex(_names.Count)];
    }
}

internal class Gender : TrackedRandomIndexer
{
    internal enum Kind
    {
        Male,
        Boy,
        Female,
        Girl
    }

    private const string _directionsPrompt = "('b' for boy, 'm' for male, 'g' for girl, 'f' for female)";
    private const string _maleFemalePrompt = "boy (male) or a girl (female)";
    private const string _firstPrompt = $"Please, tell me, are you a {_maleFemalePrompt}? {_directionsPrompt}  ";
    private static readonly List<string> _subsequentPrompts = new() {
        $"Hmm, let's try that again. Are you a {_maleFemalePrompt}? {_directionsPrompt}  ",
        $"That didn't go as planned. Please! Pay attention. Are you a {_maleFemalePrompt}? {_directionsPrompt}  ",
        $"Please read carefully.. Are you a {_maleFemalePrompt}? {_directionsPrompt}  ",
        $"I'm not sure what's going on here, but I'll ask again: Are you a {_maleFemalePrompt}? {_directionsPrompt}  ",
        $"Aaaack! How can you get such a simple question wrong? Seriously! Please. Are you a {_maleFemalePrompt}? {_directionsPrompt}  ",
        $"Tell me whether you are a {_maleFemalePrompt}. {_directionsPrompt}  ",
        $"My patience is being tried here! Are you a {_maleFemalePrompt}? {_directionsPrompt}  "
    };

    private bool _isFirst = true;

    internal Kind GetGender()
    {
        string promptText;
        if (_isFirst)
        {
            _isFirst = false;
            promptText = _firstPrompt;
        }
        else
        {
            promptText = _subsequentPrompts[GetRandomUnusedIndex(_subsequentPrompts.Count)];
        }

        Console.WriteLine($"{promptText}  ");
        ConsoleKey key;
        do
        {
            key = Console.ReadKey(true).Key;
        } while (key != ConsoleKey.B && key != ConsoleKey.M && key != ConsoleKey.G && key != ConsoleKey.F);
        Kind gender = default;
        switch (key)
        {
            case ConsoleKey.B:
                gender = Kind.Boy;
                break;
            case ConsoleKey.M:
                gender = Kind.Male;
                break;
            case ConsoleKey.G:
                gender = Kind.Girl;
                break;
            case ConsoleKey.F:
                gender = Kind.Female;
                break;
        }
        Console.WriteLine($"You pressed the '{key.ToString().ToLower()}' key which means that you are a {KindToString(gender)}.");
        return gender;
    }

    internal static Kind ConfuseGender(Kind gender)
    {
        switch (gender)
        {
            case Kind.Boy:
                return Kind.Girl;
            case Kind.Male:
                return Kind.Female;
            case Kind.Girl:
                return Kind.Boy;
            case Kind.Female:
                return Kind.Male;
        }
        return gender;
    }

    internal static string KindToString(Kind kind)
    {
        string asString = "";
        switch (kind)
        {
            case Kind.Boy:
                asString = "boy";
                break;
            case Kind.Male:
                asString = "male";
                break;
            case Kind.Girl:
                asString = "girl";
                break;
            case Kind.Female:
                asString = "female";
                break;
        }
        return asString;
    }
}


internal class Namer : TrackedRandomIndexer
{
    private const string _firstPrompt = $"Would you be so kind as to tell me your name?  ";
    private static readonly List<string> _subsequentPrompts = new() {
        $"Please, be more careful this time, and tell me your name:  ",
        $"Not sure what's going on here. Please tell me your real name: ",
        $"Stop fooling around! What's your name?! ",
        $"Why are you making this so difficult?! Name?  ",
        $"Is it really that hard to tell me your name? Please do so now:  ",
        $"I've never met someone who doesn't know their name. Please, ask someone and then tell me:  "
    };

    private bool _isFirst = true;

    internal string GetName(bool reset = false)
    {
        if (reset)
        {
            _isFirst = true;
        }
        string promptText;
        if (_isFirst)
        {
            _isFirst = false;
            promptText = _firstPrompt;
        }
        else
        {
            promptText = _subsequentPrompts[GetRandomUnusedIndex(_subsequentPrompts.Count)];
        }
        Console.Write(promptText);
        return KeyboardInput.GetInput(true);
    }
}

internal class HintDuration
{
    private readonly int _hintIncrement;
    private int _index = 1;

    internal HintDuration(int increment)
    {
        _hintIncrement = increment;
    }

    internal int Next()
    {
        return _hintIncrement * _index++;
    }

    internal void Reset()
    {
        _index = 1;
    }
}

internal class Hinter : TrackedRandomIndexer
{
    private readonly List<ConsoleKey> _keys = new() {
        ConsoleKey.UpArrow,
        ConsoleKey.F6,
        ConsoleKey.LeftArrow,
        ConsoleKey.F2,
        ConsoleKey.PageDown,
        ConsoleKey.F9,
        ConsoleKey.RightArrow,
        ConsoleKey.F4,
        ConsoleKey.DownArrow,
        ConsoleKey.F5,
        ConsoleKey.PageUp,
        ConsoleKey.F1,
        ConsoleKey.F8,
        ConsoleKey.F3,
        ConsoleKey.F10,
        ConsoleKey.F7
    };

    private readonly List<string> _hintTexts = new() {
        "Press the {0} key when asked if I got your name right!",
        "When asked if I got your name right, {0} is the key to press!",
        "To escape, press the {0} key when asked if I got your name right!",
        "Press {0} when asked if I got your name right!",
        "You cannot get out unless you press the {0} key when asked if I got your name right!",
        "{0}. That's the key to press the next time you're asked if I got your name right!",
        "Try pressing {0} the next time I ask if I got your name right!",
        "Asked if I got your name right? Press {0}!",
        "Not 'yes', not 'no'. {0} is the way to go!",
        "Why not try pressing the {0} key the next time I ask whether I got your name right?!"
    };
    private int _hintTextIndex = 0;

    private readonly HintDuration _hintDuration = new(250);
    private ConsoleKey _hintKey;

    internal ConsoleKey Next()
    {
        _hintKey = _keys[GetRandomUnusedIndex(_keys.Count)];
        _hintDuration.Reset();
        return _hintKey;
    }

    internal void GiveHint()
    {
        var originalBackground = Console.BackgroundColor;
        var originalForeground = Console.ForegroundColor;
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;

        var hintText = string.Format(_hintTexts[_hintTextIndex++], _hintKey.ToString().ToLowerInvariant());
        Console.Write(hintText);
        if (_hintTexts.Count == _hintTextIndex)
        {
            _hintTextIndex = 0;
        }
        Thread.Sleep(_hintDuration.Next());

        Console.BackgroundColor = originalBackground;
        Console.ForegroundColor = originalForeground;

        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
}

internal class KeyboardInput
{
    internal enum TextRequirement
    {
        None,
        /// <summary>
        /// strings of length 0 are not allowed
        /// </summary>
        NonEmpty,
        /// <summary>
        /// strings of length 0 are not allowed, nor strings of all whitespace characters
        /// </summary>
        NonWhitespace
    }

    internal static string GetInput(bool emitLineFeed = true, TextRequirement textRequirement = TextRequirement.NonWhitespace)
    {
        var inputString = string.Empty;
        do
        {
            // we collect characters and display them until we get an Enter/Return
            // this is heavily adapted from https://learn.microsoft.com/en-us/dotnet/api/system.consolekeyinfo.keychar?view=net-7.0
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
                // Ignore if Alt or Ctrl is pressed.
                if ((keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
                    continue;
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                    continue;
                // Ignore if KeyChar value can't be mapped to a Unicode character, i.e. \u0000.
                if (keyInfo.KeyChar == '\u0000')
                    continue;
                // Ignore tab key.
                if (keyInfo.Key == ConsoleKey.Tab)
                    continue;
                // Handle Escape key.
                if (keyInfo.Key == ConsoleKey.Escape)
                    continue;
                // Handle backspace
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    // Are there any characters to erase?
                    if (inputString.Length >= 1)
                    {
                        inputString = HandleBackspace(inputString);
                    }
                    continue;
                }
                if (keyInfo.Key != ConsoleKey.Enter)
                {
                    // Handle key by adding it to input string.
                    Console.Write(keyInfo.KeyChar);
                    inputString += keyInfo.KeyChar;
                }
            } while (keyInfo.Key != ConsoleKey.Enter);
            if (TextRequirement.None != textRequirement)
            {
                switch (textRequirement)
                {
                    case TextRequirement.None:
                        break;
                    case TextRequirement.NonEmpty:
                        if (0 == inputString.Length)
                        {
                            continue;
                        }
                        break;
                    case TextRequirement.NonWhitespace:
                        if (string.IsNullOrWhiteSpace(inputString))
                        {
                            var charCount = inputString.Length;
                            var ii = 0;
                            while (ii < charCount)
                            {
                                HandleBackspace(inputString);
                                ++ii;
                            }
                            inputString = string.Empty;
                            continue;
                        }
                        break;
                }
                textRequirement = TextRequirement.None;
            }
            if (emitLineFeed)
            {
                Console.Write(Environment.NewLine);
            }
        } while (TextRequirement.None != textRequirement);
        return Regex.Replace(inputString.Trim(), @"\s+", " ");
    }

    static string HandleBackspace(string inputString)
    {
        // get current cursor info
        int cursorRow = Console.CursorTop;
        int cursorCol = Console.CursorLeft - 1;
        if (-1 == cursorCol)
        {
            cursorRow -= 1;
            cursorCol = Console.BufferWidth - 1;
        }
        Console.CursorTop = cursorRow;
        Console.CursorLeft = cursorCol;
        Console.Write(' ');
        Console.CursorLeft = cursorCol;
        return inputString.Substring(0, inputString.Length - 1);
    }
}


internal class TextWriter
{
    internal static void RepeatLastChar(string msg, int count, char toRepeat, int millisecondsPause, bool writeLineFeed = true)
    {
        Console.Write($"{msg}");
        var ii = 0;
        while (ii < count)
        {
            Thread.Sleep(millisecondsPause);
            Console.Write(toRepeat);
            ++ii;
        }
        if (writeLineFeed)
        {
            Console.WriteLine();
        }
    }
}