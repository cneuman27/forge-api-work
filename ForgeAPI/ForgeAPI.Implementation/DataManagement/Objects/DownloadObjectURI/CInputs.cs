using System;
using System.Collections.Generic;
using System.Text;

using ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI;

namespace ForgeAPI.Implementation.DataManagement.Objects.DownloadObjectURI
{
    public class CInputs : CInputsCommon, IInputs
    {
        private string m_URI = "";

        public CInputs()
        {
        }

        public string URI
        {
            get { return m_URI; }
            set { m_URI = value; }
        }
    }
}
