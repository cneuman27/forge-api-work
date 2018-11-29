using System;
using System.Collections.Generic;
using System.Text;

using ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI;

namespace ForgeAPI.Implementation.DataManagement.Objects.DownloadObjectURI
{
    public class COutputs : COutputsCommon, IOutputs
    {
        private byte[] m_ObjectData = null;

        public COutputs()
        {
        }

        public byte[] ObjectData
        {
            get { return m_ObjectData; }
            set { m_ObjectData = value; }
        }
    }
}
