using DownloadAgent.Models;
using Downloader;
using System.ComponentModel;
using System.Diagnostics;
using Tianli.Library.Tasks;

namespace DownloadAgent.Services
{

    public class DownloadJob : BaseJob
    {
        private DownloadOption downloadOption;
        public string UnzipApp { get; set; }
        public Action<object?, DownloadStartedEventArgs> OnStart { get; set; }
        public Action<object?, DownloadProgressChangedEventArgs> OnUpdated { get; set; }
        public Action<object?, AsyncCompletedEventArgs> OnFinish { get; set; }
        public DownloadJob(DownloadOption downloadOption)
        {
            if (downloadOption == null)
            {
                throw new Exception("错误的设置");
            }
            this.downloadOption = downloadOption;
            downloadConfiguration = new DownloadConfiguration
            {
                ChunkCount = downloadOption.TaskCount
            };
        }

        private DownloadService downloader;
        private DownloadConfiguration downloadConfiguration;
        private IProgress<double> progress;

        public override JobResult Execute()
        {
            var t = Task.Run(async () =>
              {
                  await StartAsync(downloadOption.Url, downloadOption.FilePath, downloadOption.FileName);
              });
            t.Wait();
            return new JobResult { Success = true };
        }

        public async Task StartAsync(string url, string path, string filename)
        {
            var filePath = path;
            var fileName = Helper.MakeValidFileName(filename);
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建路径异常：{ex.Message}");
                return;
            }
            var download = DownloadBuilder.New()
                                          .WithUrl(url)
                                          .WithDirectory(path)
                                          .WithFileName(fileName)
                                          .WithConfiguration(downloadConfiguration)
                                          .Build();
            // Provide `FileName` and `TotalBytesToReceive` at the start of each downloads
            download.DownloadStarted += OnDownloadStarted;


            // Provide any information about chunker downloads, like progress percentage per chunk, speed, total received bytes and received bytes array to live streaming.
            download.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;


            // Provide any information about download progress, like progress percentage of sum of chunks, total speed, average speed, total received bytes and received bytes array to live streaming.
            download.DownloadProgressChanged += OnDownloadProgressChanged;


            // Download completed event that can include occurred errors or cancelled or download completed successfully.
            download.DownloadFileCompleted += OnDownloadFileCompleted;
            await download.StartAsync();
        }


        void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            var fileName = downloadOption.FileName;
            var filePath = downloadOption.FilePath;
            Console.WriteLine($"下载完成");
            var file = fileName.ToLower();
            var targetPath = fileName.Replace(".", "_");
            if (downloadOption.UnZip && IsZip(file))
            {
                Console.WriteLine($"解压：{fileName}");
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = UnzipApp,
                        Arguments = $"x \"{fileName}\" -o{targetPath} -aoa",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = filePath,
                    }
                };
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                //Console.WriteLine(result);

                // 完成解压，删除压缩文件
                Console.WriteLine($"完成解压，删除文件：{fileName}");
                File.Delete(Path.Combine(filePath, fileName));
            }

            OnFinish?.Invoke(sender, e);
        }

        bool IsZip(string filename)
        {
            var ext = new[] { ".zip", ".7z", ".rar" };
            var file = new FileInfo(filename);
            foreach (var item in ext)
            {
                if (file.Extension.Contains(item))
                    return true;
            }
            return false;
        }

        void OnDownloadStarted(object? sender, DownloadStartedEventArgs e)
        {
            OnStart?.Invoke(sender, e);
            //Console.WriteLine($"正在下载：{e.FileName}，文件大小：{Helper.CalcMemoryMensurableUnit(e.TotalBytesToReceive)}");
        }
        void OnChunkDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            //Console.WriteLine($"{e.ProgressPercentage}");
            //progress?.Report(e.ProgressPercentage);
            OnUpdated?.Invoke(sender, e);
        }

    }
}
