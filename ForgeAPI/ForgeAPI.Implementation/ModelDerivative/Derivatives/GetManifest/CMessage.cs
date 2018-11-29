using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest
{
    public class CMessage : IMessage
    {
        private string m_Type = "";
        private string m_Code = "";
        private string m_Message = "";

        public CMessage()
        {
        }

        [JsonProperty("type")]
        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        [JsonProperty("code")]
        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        [JsonProperty("message")]
        public string Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
    }
}
