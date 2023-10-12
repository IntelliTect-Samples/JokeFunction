using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace JokeFunction
{
    public class JokeService
    {

        private static Joke[]? _jokeList = null;
        private ILogger _logger;
        
        public JokeService(ILogger logger)
        {
            _logger = logger;    
        }

        private Joke[] JokeList
        {
            get
            {
                if (_jokeList == null)
                {
                    try
                    {
                        // Write all the files in root folder to the log to see why
                        // this fails when deployed to the function app
                        _logger.LogInformation($"File List from {Directory.GetCurrentDirectory()}");
                        foreach (var file in Directory.EnumerateFiles("."))
                        {
                            _logger.LogInformation(file);
                        }

                        _logger.LogInformation("Reading jokes.json");
                        var json = File.ReadAllText("jokes.json");
                        _jokeList = JsonSerializer.Deserialize<Joke[]>(json);
                        _jokeList = _jokeList!.Where(f => f.question != null).ToArray();
                        _logger.LogInformation("Loaded Jokes from file.");
                    }
                    catch
                    {
                        _jokeList = new Joke[1];
                        _jokeList[0] = new Joke
                        {
                            question = "Why did the file fail to load?",
                            answer = "The app was knocking on the wrong fol-door.",
                            author = "Grant Erickson",
                            created = "10/10/2023",
                            rating = 5
                        };
                        _jokeList[0].tags = new string[1];
                        _jokeList[0].tags[0] = "files";
                        _jokeList[0].text = $"{_jokeList[0].question}  {_jokeList[0].answer}";
                        _logger.LogInformation("Joke file load failed. Loaded static jokes.");
                    }
                }
                return _jokeList!;
            }
        }

        public Joke? GetRandomJoke(string? search = null)
        {
            var list = JokeList;
            if (search != null)
            {
                list = list.Where(f => f.question.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                       f.answer.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                       f.tagList.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                       f.author.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToArray();
                if (!list.Any()) return null;
            }
            var index = new Random().Next(list.Length);
            return list[index];
        }
    }
}
