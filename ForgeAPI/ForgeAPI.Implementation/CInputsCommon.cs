using ForgeAPI.Interface;

namespace ForgeAPI.Implementation
{
    public class CInputsCommon : IInputsCommon
    {
        private ForgeAPI.Interface.Authentication.IToken m_AuthenticationToken = null;

        public ForgeAPI.Interface.Authentication.IToken AuthenticationToken
        {
            get { return m_AuthenticationToken; }
            set { m_AuthenticationToken = value; }
        }
    }
}
