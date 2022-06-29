using System.Diagnostics;

namespace DownloadAgent
{
    public class InitResource
    {
        private const string UnzipApp = "7z.exe";
        public void Init()
        {
            var zip = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UnzipApp);
            if (!File.Exists(zip))
            {
                try
                {
                    File.WriteAllBytes(zip, Properties.Resources._7z);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            var appsetting = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(appsetting))
            {
                try
                {
                    File.WriteAllBytes(appsetting, Properties.Resources.appsettings);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            try
            {
                Initwwwroot();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void Initwwwroot()
        {
            // 初始化 wwwroot
            var www = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            if (!Directory.Exists(www))
            {

                var rootzip = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot.7z");
                File.WriteAllBytes(rootzip, Properties.Resources.wwwroot);

                Console.WriteLine($"初始化：wwwroot");
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = UnzipApp,
                        Arguments = $"x \"{rootzip}\" -aoa",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    }
                };
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                //Console.WriteLine(result);

                // 完成解压，删除压缩文件
                Console.WriteLine($"完成初始化");
                File.Delete(rootzip);

            }
        }
    }
}
