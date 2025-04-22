using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

// Maybe, but probably not move the json files together so there aren't so many of them.
// Make a neat if then game. First start with all answers being yes, then go from there.
// Add the song or find a different one. For the high scores incorperate a way to
// distinguish between what catagory was played to get that high score.

namespace SketchyTriviaProject
{
    public class TriviaQuestion
    {
        public string Question { get; set; }
        public string Category { get; set; }
        public string Answer { get; set; }
        public List<string> Choices { get; set; }
    }

    public class PlayerScore
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
    }

    class Program
    {
        static int score = 0; // Keeps track of the player's score
        static int streak = 0; // Keeps track of consecutive correct answers
        static int wrongAnswerStreak = 0; // Keeps track of consecutive wrong answers
        static string playerName; // Player's name
        const int MaxUsernameLength = 15;

        static void Main()
        {
            // Set the console window size
            Console.WindowWidth = 186;  // Adjust as needed
            Console.WindowHeight = 40;  // Adjust as needed

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            Console.WriteLine(".........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n...........:@@@@.....=@@@@.......#@@#...............................+++....+++.......=@@@-.........................*::=............+=....................................................\r\n...........*@@@@@:...@@@@@...-@@@@@@@@@@:..@@@@@@@@@@@%......@@@@@=.@@@@@@@@@@@...@@@@@@@@@@*...@@@@@@@@@@@@......@@@@@@.....@@@@@@@@@@@@..@@@@@@@@@@@@..:@@@@@...@@@@@.@@@@#............\r\n............@@@@@@#..@@@@@..@@@@@@@@@@@@@@.@@@@@@@@@@@@......@@@@@=.@@@@@@@@@@@.-@@@@@@@@@@@@@:.@@@@@@@@@@@@@....*@@@@@@=....+@@@@@@@@@@@@.@@@@@@@@@@@@@::@@@@@...@@@@@.@@@@#............\r\n............@@@@@@@@.#@@@@.@@@@@@....@@@@@=...@@@@@:.........%@@@@=.@@@@@.......@@@@@:...=@@@@@.@@@@@...*@@@@+..:@@@@@@@@:...+@@@@...@@@@@.@@@@@....@@@@@.@@@@@*.@@@@@@.@@@@.............\r\n............@@@@@@@@@#@@@@.@@@@@......@@@@@...@@@@@:.........#@@@@=.@@@@@@@@@@..@@@@......*@@@@.@@@@@...#@@@@-..@@@@@@@@@@...+@@@@@@@@@@@@.@@@@@.....@@@@..@@@@@@@@@@@..@@@@.............\r\n............@@@@.@@@@@@@@@.@@@@@......@@@@@...@@@@@:.........*@@@@=.@@@@@@@@@@..@@@@:.....@@@@@.@@@@@@@@@@@@@..%@@@@.-@@@@@..+@@@@@@@@@@=..@@@@@....*@@@@...*@@@@@@@+....@@%.............\r\n............@@@@..=@@@@@@@.-@@@@@#::@@@@@@....@@@@@:.........%@@@@-.@@@@@...:::.@@@@@@-:=@@@@@@.@@@@@@@@@@@*..-@@@@@@@@@@@@-.@@@@@.=@@@@@..@@@@@...:@@@@@....:@@@@@......--:.............\r\n...........=@@@@...:@@@@@@..-@@@@@@@@@@@@:....@@@@@:.........@@@@@..@@@@@@@@@@@..@@@@@@@@@@@@%..@@@@@.........@@@@@@@@@@@@@@.@@@@@..#@@@@+.@@@@@@@@@@@@@......@@@@@.....@@@@+............\r\n...........*@@@@.....@@@@@....-@@@@@@@@-......@@@@@-........@@@@@-..@@@@@@@@@@@...:@@@@@@@@#....@@@@@=.......@@@@@=....:@@@@@@@@@@...@@@@@.@@@@@@@@@@%:.......@@@@@.....@@@@.............\r\n.............................................................@@@:........................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................\r\n.........................................................................................................................................................................................");
            // Ask for the player's name with validation
            while (true)
            {
                Console.Write("Enter your name (letters and numbers only, max 15 characters): ");
                playerName = Console.ReadLine();

                if (IsValidUsername(playerName))
                {
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid username. Please use only letters and numbers, and keep it under 15 characters.");
                }
            }

            // Method to validate the username
            static bool IsValidUsername(string name)
            {
                // Check if the username is not null, contains only letters and numbers, and is within the character limit
                return !string.IsNullOrEmpty(name) && Regex.IsMatch(name, "^[a-zA-Z0-9]+$") && name.Length <= MaxUsernameLength;
            }

            // Dictionary to hold categories and their questions
            var categories = new Dictionary<string, List<TriviaQuestion>>();

            // List of category files
            var categoryFiles = new List<string> { "animals.json", "brain-teasers.json", "celebrities.json", "entertainment.json", "for-kids.json",
                "general.json", "geography.json", "history.json", "hobbies.json", "humanities.json", "literature.json", "movies.json", "music.json",
                "newest.json", "people.json", "rated.json", "religion-faith.json", "science-technology.json", "sports.json", "television.json",
                "video-games.json", "world.json" };

            // Load questions from each file
            foreach (var file in categoryFiles)
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File {file} not found at path {filePath}.");
                    continue;
                }

                string json;
                try
                {
                    // Read file with UTF-8 encoding
                    json = File.ReadAllText(filePath, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file {file}: {ex.Message}");
                    continue;
                }

                List<TriviaQuestion> questions;
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true, // To handle case-insensitive property names
                    };
                    questions = JsonSerializer.Deserialize<List<TriviaQuestion>>(json, options);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON deserialization error in file {file}: {ex.Message}");
                    continue;
                }

                if (questions != null && questions.Any())
                {
                    var category = questions.First().Category; // Assuming all questions in a file belong to the same category
                    categories[category] = questions;
                }
                else
                {
                    Console.WriteLine($"No questions found or failed to deserialize JSON in file {file}.");
                }
            }

            while (true)
            {
                Console.Clear(); // Clear the console for the menu display

                // Prompt user for category selection
                Console.WriteLine("Available categories:");
                int index = 1;
                foreach (var category in categories.Keys)
                {
                    Console.WriteLine($"{index}. {category}");
                    index++;
                }
                Console.WriteLine($"{index}. Random"); // Add the "Random" option
                Console.WriteLine($"{index + 1}. View High Scores"); // Add the option to view high scores
                Console.Write("Choose a category by number (or press 'e' to exit): ");
                string userInput = Console.ReadLine();

                if (userInput.Trim().ToLower() == "e")
                {
                    SaveHighScore(score); // Save the score when exiting
                    break; // Exit the program if 'e' is pressed
                }

                if (!int.TryParse(userInput, out int categoryIndex) || categoryIndex < 1 || categoryIndex > categories.Count + 2)
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please enter a valid category number.\n");
                    continue;
                }

                List<TriviaQuestion> questionsInCategory;
                if (categoryIndex == categories.Count + 1)
                {
                    // Handle the Random category
                    var allQuestions = categories.Values.SelectMany(q => q).ToList();
                    if (!allQuestions.Any())
                    {
                        Console.Clear();
                        Console.WriteLine("No questions available for random selection.\n");
                        continue;
                    }
                    questionsInCategory = allQuestions;
                }
                else if (categoryIndex == categories.Count + 2)
                {
                    DisplayHighScores(); // Display high scores
                    continue; // Return to the category menu
                }
                else
                {
                    string selectedCategory = categories.Keys.ElementAt(categoryIndex - 1);
                    questionsInCategory = categories[selectedCategory];
                }

                var random = new Random();
                Console.Clear();

                while (true)
                {
                    // Select a random question from the chosen category or all categories
                    var question = questionsInCategory[random.Next(questionsInCategory.Count)];

                    // Ask the question
                    Console.WriteLine($"Question: {question.Question}");
                    Console.WriteLine("Choices:");
                    for (int i = 0; i < question.Choices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Choices[i]}");
                    }

                    while (true)
                    {
                        Console.Write("Enter your choice (number) or press 'e' to go back: ");
                        userInput = Console.ReadLine();

                        if (userInput.Trim().ToLower() == "e")
                        {
                            Console.Clear();
                            SaveHighScore(score); // Save the score before returning to the menu
                            score = 0; // Reset the score after saving
                            break; // Go back to the category menu if 'e' is pressed
                        }

                        if (!int.TryParse(userInput, out int choiceIndex) || choiceIndex < 1 || choiceIndex > question.Choices.Count)
                        {
                            Console.WriteLine("Invalid choice. Please enter a valid choice number.\n");
                            continue; // Continue prompting the user for a valid choice
                        }

                        string userAnswer = question.Choices[choiceIndex - 1];
                        if (userAnswer == question.Answer)
                        {
                            Console.Clear();
                            Console.WriteLine($"Correct! Great job {playerName}.");
                            streak++;
                            wrongAnswerStreak = 0; // Reset wrong answer streak
                            score += 10 + (streak * 2); // Increase score based on streak
                            Console.WriteLine($"Your streak: {streak}");
                            Console.WriteLine($"Your score: {score}\n");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine($"Wrong! Do better {playerName}. The correct answer is {question.Answer}.");
                            streak = 0; // Reset streak on wrong answer
                            wrongAnswerStreak++;
                            score = Math.Max(score - 3, 0); // Subtract 3 points, ensuring score doesn't go below 0
                            Console.WriteLine($"Your streak: {streak}");
                            Console.WriteLine($"Your score: {score}\n");

                            if (wrongAnswerStreak >= 10)
                            {
                                Console.Clear();
                                Console.WriteLine($"You're a waste of oxygen {playerName}! Go play Candy Crush.");
                                SaveHighScore(score); // Save the score before exiting
                                Console.ReadLine();
                                return; // Exit the program
                            }
                        }

                        break; // Exit the question loop and get a new question
                    }

                    if (userInput.Trim().ToLower() == "e")
                    {
                        break; // Exit the question loop and go back to the category menu
                    }
                }
            }
        }

        static void SaveHighScore(int score)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscores.txt");
            var highScores = new List<PlayerScore>(); // Store player names and scores

            if (File.Exists(path))
            {
                var existingScores = File.ReadAllLines(path)
                    .Select(line => line.Split(','))
                    .Select(parts => new PlayerScore { PlayerName = parts[0], Score = int.Parse(parts[1]) })
                    .ToList();
                highScores.AddRange(existingScores);
            }

            var currentPlayerScore = highScores.FirstOrDefault(ps => ps.PlayerName == playerName);

            if (currentPlayerScore != null)
            {
                // Update the score if the new score is higher
                if (score > currentPlayerScore.Score)
                {
                    currentPlayerScore.Score = score;
                }
            }
            else
            {
                // Add a new high score for the player
                highScores.Add(new PlayerScore { PlayerName = playerName, Score = score });
            }

            // Sort high scores in descending order and take the top 10
            var topScores = highScores.OrderByDescending(ps => ps.Score).Take(10);

            File.WriteAllLines(path, topScores.Select(ps => $"{ps.PlayerName},{ps.Score}"));
        }

        static void DisplayHighScores()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscores.txt");

            if (!File.Exists(path))
            {
                Console.WriteLine("No high scores available.");
                return;
            }

            var highScores = File.ReadAllLines(path)
                .Select(line => line.Split(','))
                .Select(parts => new PlayerScore { PlayerName = parts[0], Score = int.Parse(parts[1]) })
                .ToList();

            Console.Clear();
            Console.WriteLine("- High Scores -");
            foreach (var score in highScores)
            {
                Console.WriteLine($"-{score.PlayerName}: {score.Score}");
            }

            Console.WriteLine("\nPress 'e' to return to the main menu.");
            while (true)
            {
                if (Console.ReadLine().Trim().ToLower() == "e")
                {
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Press 'e' to return to the main menu.");
                }
            }
        }
    }
}