using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tianli.Library.Tasks
{
    /// <summary>
    /// 线程池
    /// </summary>
    public class TaskPool : IDisposable
    {
        /// <summary>
        /// 配置
        /// </summary>
        private TaskPoolConfig m_Cfg;

        /// <summary>
        /// 工作队列
        /// </summary>
        private ConcurrentQueue<BaseJob> m_Queue;

        /// <summary>
        /// 信号量
        /// </summary>
        private Semaphore m_Sem;

        /// <summary>
        /// 是否释放
        /// </summary>
        private bool m_Exit;

        /// <summary>
        /// 已退出线程数
        /// </summary>
        private byte m_ExitCnt;

        private TaskPool()
        {
            m_Cfg = new TaskPoolConfig();
        }

        /// <summary>
        /// 创建线程池
        /// </summary>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public static TaskPool Create(Action<TaskPoolConfig> config)
        {
            TaskPool ret = new TaskPool();
            config?.Invoke(ret.m_Cfg);
            return ret;
        }

        public void Dispose()
        {
            m_Exit = true;
        }

        /// <summary>
        /// 运行线程池
        /// </summary>
        /// <returns></returns>
        public TaskPool Run()
        {
            m_Queue = new ConcurrentQueue<BaseJob>();
            m_Sem = new Semaphore(0, m_Cfg.QueueLength);
            m_Exit = false;

            for (byte i = 0; i < m_Cfg.ThreadCount; i++)
            {
                Task.Factory.StartNew(Work);
            }
            return this;
        }

        /// <summary>
        /// 向工作队列加入工作项
        /// </summary>
        /// <param name="job">工作项</param>
        public void JobEnqueue(BaseJob job)
        {
            if (m_Sem == null)
            {
                Run();
            }
            m_Sem.Release();
            m_Queue.Enqueue(job);
        }

        private void Work()
        {
            while (!m_Exit)
            {
                if (m_Sem.WaitOne(2000))
                {
                    if (m_Queue.TryDequeue(out BaseJob job))
                    {
                        job.Run();
                    }
                }
            }
            lock (m_Queue)
            {
                m_ExitCnt++;
            }
            if (m_ExitCnt == m_Cfg.ThreadCount)
            {
                m_Sem.Dispose();
            }
        }
    }


    /// <summary>
    /// 线程池配置
    /// </summary>
    public class TaskPoolConfig
    {
        /// <summary>
        /// 工作队列长度
        /// </summary>
        public int QueueLength { get; set; }

        /// <summary>
        /// 线程数
        /// </summary>
        public byte ThreadCount { get; set; }

        public TaskPoolConfig()
        {
            ThreadCount = 1;
            QueueLength = 1024;
        }
    }

}
