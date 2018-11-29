using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface.Authentication;

namespace ForgeAPI.Implementation.Authentication
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CToken : IToken
    {
        private string m_TokenType = "Bearer";
        private int m_LifetimeSeconds = 0;
        private string m_AccessToken = "";
        private DateTime m_CreateDate = DateTime.Now;
        private List<ForgeAPI.Interface.Enums.E_AccessScope> m_AccessScopes = null;

        [JsonProperty("token_type")]
        public string TokenType
        {
            get { return m_TokenType; }
            internal set { m_TokenType = value; }
        }

        [JsonProperty("expires_in")]
        public int LifetimeSeconds
        {
            get { return m_LifetimeSeconds; }
            internal set { m_LifetimeSeconds = value; }
        }

        [JsonProperty("access_token")]
        public string AccessToken
        {
            get { return m_AccessToken; }
            internal set { m_AccessToken = value; }
        }

        public DateTime CreateDate
        {
            get { return m_CreateDate; }
        }
        public DateTime ExpirationDate
        {
            get { return m_CreateDate.AddSeconds(LifetimeSeconds); }
        }
        public bool IsExpired
        {
            get
            {
                return ExpirationDate.Ticks <= DateTime.Now.Ticks;
            }
        }
        public List<ForgeAPI.Interface.Enums.E_AccessScope> AccessScopes
        {
            get { return m_AccessScopes; }
            set { m_AccessScopes = value; }
        }
    }
}
