﻿namespace Data.Models
{
    public class RepositoryResult<T>
    {
        public bool Succeded { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; }
        public T? Result { get; set; }
    }

    public class RepositoryResult
    {
        public bool Succeded { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; }
    }
}
