using DownloadAgent.Models;
using DownloadAgent.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tianli.Library.Tasks;

namespace DownloadAgent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageController : ControllerBase
    {
        private readonly ILogger<ManageController> _logger;

        public ManageController(ILogger<ManageController> logger)
        {
            _logger = logger;
        }

        [Route("list")]
        [HttpGet]
        public List<JobInfo> List()
        {
            return DownloadService.Instance.GetTasks();
        }

        [Route("download/item")]
        [HttpPost]
        public void Add(DownloadOption option)
        {
            DownloadService.Instance.Add(option);
        }

        [Route("download/list")]
        [HttpPost]
        public void Add(List<DownloadOption> option)
        {
            foreach (var item in option)
            {
                DownloadService.Instance.Add(item);
            }
        }

        [HttpGet]
        [Route("config")]
        public DownloadConfig LoadConfig()
        {
            return DownloadService.Instance.GetConfig();
        }

        [HttpPost]
        [Route("config")]
        public bool SetConfig(DownloadConfig config)
        {
            DownloadService.Instance.SaveConfig(config);
            return true;
        }


    }
}
