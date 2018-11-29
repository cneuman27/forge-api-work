using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace StandardPartProcessing
{
    public class CHeartBeat : IDisposable
    {
        private Thread m_RunThread = null;
        private long m_Ticks = 0;

        public CHeartBeat(int interval = 50000)
        {
            m_RunThread = new Thread(
                () => 
                {
                    while (true)
                    {
                        Thread.Sleep(interval);
                        Trace.TraceInformation(
                            "Heart Beat {0}",
                            (long)(new TimeSpan(DateTime.Now.Ticks - m_Ticks)).TotalSeconds);
                            
                    }   
                });

            m_Ticks = DateTime.Now.Ticks;
            m_RunThread.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_RunThread != null)
                {
                    Trace.TraceInformation("Ending HeartBeat");
                    m_RunThread.Abort();
                    m_RunThread = null;
                }
            }
        }
    }
}
