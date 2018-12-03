using System;
using System.Collections.Generic;
using System.Net;

namespace ForgeAPI.Interface.REST
{
    public interface IResult
    {
        string URI
        {
            get;
        }
        string Method
        {
            get;
        }
        HttpStatusCode StatusCode
        {
            get;
        }

        DateTime RequestTime
        {
            get;
        }

        string RequestData
        {
            get;
        }
        byte[] RequestBinaryData
        {
            get;
        }
        List<KeyValuePair<string, string>> RequestHeaders
        {
            get;
        }
                
        string ResponseData
        {
            get;
        }
        byte[] ResponseBinaryData
        {
            get;
        }
        List<KeyValuePair<string, string>> ResponseHeaders
        {
            get;
        }

        bool IsOK();
    }
}
