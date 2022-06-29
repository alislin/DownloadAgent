using System;

namespace Tianli.Library.Tasks
{
    /// <summary>
    /// 工作项基类
    /// </summary>
    public abstract class BaseJob
    {
        /// <summary>
        /// 开始前回调
        /// </summary>
        public Action<BaseJob> Begin;

        /// <summary>
        /// 结束后回调
        /// </summary>
        public Action<BaseJob, JobResult> Finished;

        public void Run()
        {
            JobResult result = default;
            Begin?.Invoke(this);
            try
            {
                result = Execute();
            }
            catch (Exception ex)
            {
                result.OperateFail(ex.Message);
                result.Data = ex;
            }
            Finished?.Invoke(this, result);
        }

        public abstract JobResult Execute();
    }

}
