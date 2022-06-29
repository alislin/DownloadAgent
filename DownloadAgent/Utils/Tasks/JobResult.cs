namespace Tianli.Library.Tasks
{
    /// <summary>
    /// 工作执行结果
    /// </summary>
    public class JobResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 可能会有的附加数据
        /// </summary>
        public object Data { get; set; }

        public JobResult()
        {
            Success = true;
        }

        public void OperateFail(string msg)
        {
            Success = false;
            Message = msg;
        }
    }

}
