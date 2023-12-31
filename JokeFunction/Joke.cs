﻿namespace JokeFunction
{
    public class Joke
    {
        public string id { get; set; } = null!;
        public string question { get; set; } = null!;
        public string answer { get; set; } = null!;
        public string text { get; set; } = null!;
        public string author { get; set; } = null!;
        public string created { get; set; } = null!;
        public string[] tags { get; set; } = null!;
        public int rating { get; set; }
        public string? sentiment { get; set; } = null;

        private string? _tagList = null!;
        public string tagList
        {
            get
            {
                if (_tagList == null && tags != null) _tagList = string.Join(",",tags);
                return _tagList!;
            }
        }
    }
}
