namespace DownloadAgent.Models
{
    public class DownloadOption
    {
        public string Url { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public int TaskCount { get; set; } = 5;
        public bool UnZip { get; set; }
    }

    public class DownloadResult
    {
        public DownloadOption Request { get; set; }
    }

    public class DownloadConfig
    {
        public const string Config = "Config.json";
        public string DownloadPath { get; set; }
        public bool SkipSameFile { get; set; }
    }
}
