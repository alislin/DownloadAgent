using Downloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static ReviewFileDownloader.Helper;

namespace DownloadAgent
{
    public class Download
    {
        private DownloadService downloader;
        private DownloadConfiguration downloadConfiguration;
        const int totalTicks = 10;
        private IProgress<double> progress;
        private string unZip;
        public Download(DownloadConfiguration downloadOpt)
        {
            downloadConfiguration = downloadOpt;
        }

        private string filePath;
        private string fileName;

        public async Task StartAsync(string url, string path, string filename)
        {
            filePath = path;
            fileName = MakeValidFileName(filename);
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

            Console.WriteLine();
            await download.StartAsync();
        }

        void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
                Console.WriteLine($"下载完成");
            var file = fileName.ToLower();
            if (string.IsNullOrWhiteSpace(unZip) && IsZip(file))
            {
                Console.WriteLine($"解压：{fileName}");
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "7z",
                        Arguments = $"x \"{fileName}\" -aoa",
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
                Console.WriteLine($"正在下载：{e.FileName}，文件大小：{Helper.CalcMemoryMensurableUnit(e.TotalBytesToReceive)}");
        }
        void OnChunkDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            //Console.WriteLine($"{e.ProgressPercentage}");
            progress?.Report(e.ProgressPercentage);
        }

        public string MakeValidFileName(string text, string replacement = "_")
        {
            StringBuilder str = new StringBuilder();
            var invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (var c in text)
            {
                if (invalidFileNameChars.Contains(c))
                {
                    str.Append(replacement ?? "");
                }
                else
                {
                    str.Append(c);
                }
            }

            return str.ToString();
        }
    }
}
