using System.Diagnostics;

int easyHighScore = int.MaxValue;
int mediumHighScore = int.MaxValue;
int hardHighScore = int.MaxValue;

bool playAgain = true;
while (playAgain)
{
    Console.WriteLine("Welcome to the Number Guessing Game!");
    Console.Write("Do you want to set a custom range? (y/n): ");
    bool customRange = Console.ReadLine()?.Trim().ToLower() == "y";

    int minRange = 1;
    int maxRange = 100;

    if (customRange)
    {
        Console.Write("Enter the minimum value: ");
        while (!int.TryParse(Console.ReadLine()?.Trim(), out minRange))
        {
            Console.Write("Invalid input. Please enter a valid number for the minimum value: ");
        }

        Console.Write("Enter the maximum value: ");
        while (!int.TryParse(Console.ReadLine()?.Trim(), out maxRange) || maxRange <= minRange)
        {
            Console.Write("Invalid input. Please enter a valid number greater than the minimum value for the maximum value: ");
        }
    }

    Console.WriteLine($"I'm thinking of a number between {minRange} and {maxRange}.");
    Console.WriteLine("Can you guess what it is?");
    Console.WriteLine("Please select the difficulty level:");
    Console.WriteLine("1. Easy (10 chances)");
    Console.WriteLine("2. Medium (5 chances)");
    Console.WriteLine("3. Hard (3 chances)");
    Console.WriteLine("4. Custom (set your own number of chances)");

    int chances = 0;
    int highScore = 0;
    int hintRange = 10; // Default hint range
    int hintFrequency = 1; // Default hint frequency
    string difficultyChoice = Console.ReadLine()?.Trim();
    switch (difficultyChoice)
    {
        case "1":
            chances = 10;
            highScore = easyHighScore;
            hintRange = 15; // More detailed hints
            hintFrequency = 1; // Frequent hints
            break;
        case "2":
            chances = 5;
            highScore = mediumHighScore;
            hintRange = 10; // Standard hints
            hintFrequency = 2; // Less frequent hints
            break;
        case "3":
            chances = 3;
            highScore = hardHighScore;
            hintRange = 5; // Less detailed hints
            hintFrequency = 3; // Rare hints
            break;
        case "4":
            Console.Write("Enter the number of chances: ");
            while (!int.TryParse(Console.ReadLine().Trim(), out chances) || chances <= 0)
            {
                Console.Write("Invalid input. Please enter a valid number of chances: ");
            }
            highScore = int.MaxValue; // Custom difficulty does not have a predefined high score
            hintRange = 10; // Standard hints for custom difficulty
            hintFrequency = 2; // Standard hint frequency for custom difficulty
            break;
        default:
            Console.WriteLine("Invalid choice. Defaulting to Medium difficulty.");
            chances = 5;
            highScore = mediumHighScore;
            hintRange = 10;
            hintFrequency = 2;
            break;
    }

    Console.Write("Do you want to set a time limit for each guess? (y/n): ");
    bool setTimeLimit = Console.ReadLine()?.Trim().ToLower() == "y";
    int timeLimit = 0;
    if (setTimeLimit)
    {
        Console.Write("Enter the time limit in seconds: ");
        while (!int.TryParse(Console.ReadLine()?.Trim(), out timeLimit) || timeLimit <= 0)
        {
            Console.Write("Invalid input. Please enter a valid number of seconds: ");
        }
    }

    Random random = new Random();
    int numberToGuess = random.Next(minRange, maxRange + 1);
    int attempts = 0;
    bool isGuessed = false;
    List<int> previousGuesses = new List<int>();

    Console.WriteLine($"Great! You have selected the difficulty level with {chances} chances.");
    Console.WriteLine("Let's start the game!");

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    while (attempts < chances && !isGuessed)
    {
        Console.Write("Enter your guess (or type 'view' to see previous guesses): ");
        string input = string.Empty;

        if (setTimeLimit)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task inputTask = Task.Run(() =>
            {
                input = Console.ReadLine().Trim();
            }, cts.Token);

            if (!inputTask.Wait(TimeSpan.FromSeconds(timeLimit)))
            {
                cts.Cancel();
                Console.WriteLine($"Time's up! You took longer than {timeLimit} seconds.");
                attempts++;
                continue;
            }
        }
        else
        {
            input = Console.ReadLine().Trim();
        }

        if (input.ToLower() == "view")
        {
            Console.WriteLine("Previous guesses: " + string.Join(", ", previousGuesses));
            continue;
        }

        if (int.TryParse(input, out int userGuess))
        {
            attempts++;
            previousGuesses.Add(userGuess);

            if (userGuess == numberToGuess)
            {
                stopwatch.Stop();
                Console.WriteLine($"Congratulations! You guessed the correct number in {attempts} attempts.");
                Console.WriteLine($"Time taken: {stopwatch.Elapsed.TotalSeconds} seconds.");
                isGuessed = true;

                if (attempts < highScore)
                {
                    Console.WriteLine("New high score!");
                    highScore = attempts;
                    if (chances == 10) easyHighScore = highScore;
                    else if (chances == 5) mediumHighScore = highScore;
                    else if (chances == 3) hardHighScore = highScore;
                }
            }
            else
            {
                Console.Write(userGuess < numberToGuess
                    ? "Incorrect! The number is greater than your guess. "
                    : "Incorrect! The number is less than your guess. ");

                // Provide a hint based on the difficulty level
                if (attempts % hintFrequency == 0 && Math.Abs(userGuess - numberToGuess) <= hintRange)
                {
                    if (chances == 10)
                    {
                        Console.WriteLine($"Hint: You are within {Math.Abs(userGuess - numberToGuess)} of the correct number.");
                    }
                    else if (chances == 5)
                    {
                        Console.WriteLine("Hint: You are close!");
                    }
                    else if (chances == 3)
                    {
                        Console.WriteLine("Hint: You are very close!");
                    }
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
    }

    if (!isGuessed)
    {
        stopwatch.Stop();
        Console.WriteLine($"Sorry! You've used all your chances. The correct number was {numberToGuess}.");
        Console.WriteLine($"Time taken: {stopwatch.Elapsed.TotalSeconds} seconds.");
    }

    Console.WriteLine("High Scores:");
    Console.WriteLine($"Easy: {(easyHighScore == int.MaxValue ? "N/A" : easyHighScore.ToString())}");
    Console.WriteLine($"Medium: {(mediumHighScore == int.MaxValue ? "N/A" : mediumHighScore.ToString())}");
    Console.WriteLine($"Hard: {(hardHighScore == int.MaxValue ? "N/A" : hardHighScore.ToString())}");

    Console.Write("Do you want to play again? (y/n): ");
    playAgain = Console.ReadLine()?.Trim().ToLower() == "y";
    if (playAgain) { Console.Clear(); }
}

Console.WriteLine("Thank you for playing the Number Guessing Game! See you next time!");
