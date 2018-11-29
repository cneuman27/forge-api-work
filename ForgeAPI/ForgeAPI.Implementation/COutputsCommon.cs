using ForgeAPI.Interface;

namespace ForgeAPI.Implementation
{
    public abstract class COutputsCommon : IOutputsCommon
    {
        private ForgeAPI.Interface.REST.IResult m_Result = null;

        public ForgeAPI.Interface.REST.IResult Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }

        public bool Success()
        {
            if (Result != null)
            {
                return Result.IsOK();
            }

            return false;
        }
        public string FailureReason()
        {
            if (Result != null)
            {
                if (Result.IsOK())
                {
                    return "SUCCESS";
                }

                if (string.IsNullOrWhiteSpace(Result.ResponseData) == false)
                {
                    return Result.ResponseData;
                }
            }

            return "API RESULT IS NULL OR RESULT DATA NOT SET";
        }
    }
}
