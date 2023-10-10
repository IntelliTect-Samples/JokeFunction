using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace JokeFunction
{
    public class JokeService
    {

        private Joke[]? _jokeList = null;

        private Joke[] JokeList
        {
            get
            {
                if (_jokeList == null)
                {
                    var json = File.ReadAllText("jokes.json");
                    _jokeList = JsonSerializer.Deserialize<Joke[]>(json);
                    _jokeList = _jokeList!.Where(f => f.question != null).ToArray();
                }
                return _jokeList!;
            }
        }

        public Joke? GetRandomJoke(string? search = null)
        {
            var list = JokeList;
            if (search != null)
            {
                list = list.Where(f => f.question.IndexOf(search,StringComparison.OrdinalIgnoreCase) >=0 ||
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
