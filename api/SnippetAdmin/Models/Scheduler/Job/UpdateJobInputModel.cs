﻿namespace SnippetAdmin.Models.Scheduler.Job
{
    public class UpdateJobInputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Describe { get; set; }

        public string Cron { get; set; }
    }
}