using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CInputs : CInputsCommon, IInputs
    {
        private bool m_ReplaceExistingDerivatives = false;

        private IInputDefinition m_Input = null;
        private IOutputDefinition m_Output = null;

        public CInputs(
            IInputDefinition inputDefinition,
            IOutputDefinition outputDefinition)
        {
            m_Input = inputDefinition;
            m_Output = outputDefinition;
        }

        public bool ReplaceExistingDerivatives
        {
            get { return m_ReplaceExistingDerivatives; }
            set { m_ReplaceExistingDerivatives = value; }
        }

        [JsonProperty("input")]
        public IInputDefinition Input
        {
            get { return m_Input; }
            set { m_Input = value; }
        }

        [JsonProperty("output")]
        public IOutputDefinition Output
        {
            get { return m_Output; }
            set { m_Output = value; }
        }
    }
}
