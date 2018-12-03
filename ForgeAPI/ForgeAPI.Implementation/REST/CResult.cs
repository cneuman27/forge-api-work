using System;
using System.Collections.Generic;
using System.Net;
using ForgeAPI.Interface.REST;

namespace ForgeAPI.Implementation.REST
{
    public class CResult : IResult
    {
        private string m_URI = "";
        private string m_Method = "";
        private HttpStatusCode m_StatusCode = HttpStatusCode.NotImplemented;

        private DateTime m_RequestTime = DateTime.Now;

        private string m_RequestData = "";
        private byte[] m_RequestBinaryData = null;
        private List<KeyValuePair<string, string>> m_RequestHeaders = new List<KeyValuePair<string, string>>();

        private string m_ResponseData = "";
        private byte[] m_ResponseBinaryData = null;
        private List<KeyValuePair<string, string>> m_ResponseHeaders = new List<KeyValuePair<string, string>>();

        public string URI
        {
            get { return m_URI; }
            set { m_URI = value; }
        }
        public string Method
        {
            get { return m_Method; }
            set { m_Method = value; }
        }
        public HttpStatusCode StatusCode
        {
            get { return m_StatusCode; }
            set { m_StatusCode = value; }
        }

        public DateTime RequestTime
        {
            get { return m_RequestTime; }
        }

        public string RequestData
        {
            get { return m_RequestData; }
            set { m_RequestData = value; }
        }
        public byte[] RequestBinaryData
        {
            get { return m_RequestBinaryData; }
            set { m_RequestBinaryData = value; }
        }
        public List<KeyValuePair<string, string>> RequestHeaders
        {
            get { return m_RequestHeaders; }
            set { m_RequestHeaders = value; }
        }

        public string ResponseData
        {
            get { return m_ResponseData; }
            set { m_ResponseData = value; }
        }
        public byte[] ResponseBinaryData
        {
            get { return m_ResponseBinaryData; }
            set { m_ResponseBinaryData = value; }
        }
        public List<KeyValuePair<string, string>> ResponseHeaders
        {
            get { return m_ResponseHeaders; }
            set { m_ResponseHeaders = value; }
        }

        public bool IsOK()
        {
            return (StatusCode == HttpStatusCode.OK);
        }
    }
}
