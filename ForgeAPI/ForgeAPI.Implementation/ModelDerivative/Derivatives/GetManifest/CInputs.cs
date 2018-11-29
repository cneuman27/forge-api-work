using System;
using System.Collections.Generic;
using System.Text;

using ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest
{
    public class CInputs : CInputsCommon, IInputs
    {
        private string m_URN = "";
        private bool m_URNIsEncoded = false;

        public CInputs()
        {
        }

        public string URN
        {
            get { return m_URN; }
            set { m_URN = value; }
        }
        public bool URNIsEncoded
        {
            get { return m_URNIsEncoded; }
            set { m_URNIsEncoded = value; }
        }
   }
}
