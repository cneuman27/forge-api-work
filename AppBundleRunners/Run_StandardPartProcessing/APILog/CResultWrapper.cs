﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run_StandardPartProcessing.APILog
{
    public class CResultWrapper : MOMShared.CNotifier
    {
        private ForgeAPI.Interface.REST.IResult m_Data = null;
        
        public CResultWrapper(ForgeAPI.Interface.REST.IResult data)
        {
            m_Data = data;
        }
        
        public ForgeAPI.Interface.REST.IResult Data
        {
            get { return m_Data; }
        }

        public long Ticks
        {
            get { return m_Data.RequestTime.Ticks; }
        }
        public DateTime LogDate
        {
            get { return m_Data.RequestTime; }
        }

        public string StatusCode
        {
            get
            {
                return ((int)m_Data.StatusCode).ToString();
            }
        }

        public string URI
        {
            get
            {
                return $"{m_Data.Method} {m_Data.URI}";
            }
        }

        public override void NotifyAll()
        {
            Notify(nameof(Ticks));
            Notify(nameof(LogDate));
            Notify(nameof(URI));
        }
    }
}
