using DownloadAgent.Models;
using Downloader;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Tianli.Library.Tasks;

namespace DownloadAgent.Services
{
    public class DownloadService
    {
        public static DownloadService Instance { get; private set; } = new DownloadService();
        private DownloadConfig Config { get; set; }
        private const string UnzipApp="7z.exe";
        public Action<string> Log { get; set; } = x => Console.WriteLine(x);
        private static List<JobInfo> jobInfos = new List<JobInfo>();
        /// <summary>
        /// 线程池
        /// </summary>
        private static TaskPool TaskPool = TaskPool.Create(config =>
        {
            config.ThreadCount = 5;
            config.QueueLength = 4096;
        });

        public DownloadService()
        {
            Config = GetConfig();
        }

        public void Add(DownloadOption downloadOption)
        {
            if (jobInfos.Any(x => x.Url == downloadOption.Url && x.JobState != JobState.Done))
            {
                return;
            }
            downloadOption.FileName = GetFileName(downloadOption);
            downloadOption.FilePath = downloadOption.FilePath ?? "";
            var info = new JobInfo
            {
                Id = Guid.NewGuid().ToString(),
                Url = downloadOption.Url,
                FileName = downloadOption.FileName,
                Path = Path.Combine(Config.DownloadPath, downloadOption.FilePath),

            };
            jobInfos.Add(info);
            if (string.IsNullOrWhiteSpace(downloadOption.FileName))
            {

                info.Error = "缺少保存文件名";
                info.JobState = JobState.Done;
                Log(info.Error);
                return;
            }

            downloadOption.FilePath = info.Path;
            var job = new DownloadJob(downloadOption);
            job.OnStart = info.JobStart;
            job.OnUpdated = info.JobUpdated;
            job.OnFinish = info.JobFinished;
            job.UnzipApp = UnzipApp;
            TaskPool.JobEnqueue(job);
        }

        public List<JobInfo> GetTasks()
        {
            jobInfos.RemoveAll(x => x.JobState == JobState.Done && x.End.AddHours(12) < DateTime.Now);
            return jobInfos;
        }

        private string DefaultPath => Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        public DownloadConfig GetConfig()
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DownloadConfig.Config);
            var config = new DownloadConfig
            {
                DownloadPath = DefaultPath,
                SkipSameFile = false,
            };
            if (!File.Exists(file))
            {
                SaveConfig(config);
            }
            try
            {
                config = JsonSerializer.Deserialize<DownloadConfig>(File.ReadAllText(file));
                if (!Directory.Exists(config.DownloadPath))
                {
                    config.DownloadPath = DefaultPath;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return config;
        }

        public void SaveConfig(DownloadConfig config)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DownloadConfig.Config);
            try
            {
                if (!Directory.Exists(config.DownloadPath))
                {
                    config.DownloadPath = DefaultPath;
                }
                File.WriteAllText(file, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
                Config = config;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetFileName(DownloadOption downloadOption)
        {
            var fileName = downloadOption.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                var url = HttpUtility.UrlDecode(downloadOption.Url);
                var src = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1);
                fileName = HttpUtility.HtmlDecode(src);
            }
            return fileName;
        }

    }

    public class JobInfo
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public long RecievedByte { get; set; }
        public double Speed { get; set; }
        public double ProcessPercentage { get; set; }
        public JobState JobState { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Dur { get; set; }
        public string? Error { get; set; } = "";
        [JsonIgnore]
        public Action<string> Log { get; set; } = m => Console.WriteLine(m);
        public void JobStart(object? sender, DownloadStartedEventArgs e)
        {
            Log($"启动下载：{FileName}");
            Size = e.TotalBytesToReceive;
            FileName = e.FileName;
            JobState = JobState.Running;
            Start = DateTime.Now;
        }

        public void JobUpdated(object? sender, DownloadProgressChangedEventArgs e)
        {
            RecievedByte = e.ReceivedBytesSize;
            ProcessPercentage = e.ProgressPercentage;
            Speed = e.AverageBytesPerSecondSpeed;
        }

        public void JobFinished(object? sender, AsyncCompletedEventArgs e)
        {
            Log($"下载完成：{FileName}");
            JobState = JobState.Done;
            End = DateTime.Now;
            Dur = End - Start;
            Error = e.Error?.Message ?? "";
        }
    }

    public enum JobState
    {
        Wait = 0,
        Running = 1,
        Done = 2,
    }
}
