﻿namespace SnippetAdmin.Models.Common
{
    public record DicOutputModel<T>
    {
        public T Key { get; set; }

        public string Value { get; set; }
    }
}