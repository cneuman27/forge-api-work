using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ForgeAPI.Interface.REST;

namespace ForgeAPI.Implementation.REST
{
    public class CService : IService, IDisposable
    {
        private HttpClient m_Client = null;

        public CService()
        {
            WinHttpHandler handler;

            handler = new WinHttpHandler();
            handler.WindowsProxyUsePolicy = WindowsProxyUsePolicy.UseWinInetProxy;          

            m_Client = new HttpClient(handler);
        }

        public IResult Post(
            string uri, 
            object data,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            HttpResponseMessage response;
            HttpRequestMessage req;
            StringContent content;
            CResult res;
            string json;
                       
            req = new HttpRequestMessage(
                HttpMethod.Post,
                uri);

            json = JsonConvert.SerializeObject(data);
            content = new StringContent(json);
                       
            req.Content = content;

            SetHeaders(req, headers);

            response = m_Client.SendAsync(req).Result;

            res = new CResult
            {
                URI = uri,
                Method = "POST",
                StatusCode = response.StatusCode,
                RequestData = json,
                ResponseData = response.Content.ReadAsStringAsync().Result
            };

            req.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.RequestHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));
            
            response.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.ResponseHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            return res;
        }

        public IResult PostFormData(
            string uri, 
            ICollection<KeyValuePair<string, string>> formData,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            HttpResponseMessage response;
            FormUrlEncodedContent content;
            CResult res;

            content = new FormUrlEncodedContent(formData);
           
            SetHeaders(content, headers);

            response = m_Client.PostAsync(uri, content).Result;

            res = new CResult
            {
                URI = uri,
                Method = "POST",
                StatusCode = response.StatusCode,
                RequestData = content.ReadAsStringAsync().Result,
                ResponseData = response.Content.ReadAsStringAsync().Result
            };

            content.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.RequestHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            response.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.ResponseHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            return res;
        }

        public IResult Get(
            string uri,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            HttpRequestMessage req;
            HttpResponseMessage response;
            CResult res;

            req = new HttpRequestMessage(
                HttpMethod.Get,
                uri);

            if (headers != null)
            {
                headers.ToList().ForEach(i => req.Headers.Add(i.Key, i.Value));
            }

            response = m_Client.SendAsync(req).Result;

            res = new CResult
            {
                URI = uri,
                Method = "GET",
                StatusCode = response.StatusCode,
                ResponseData = response.Content.ReadAsStringAsync().Result
            };

            req.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.RequestHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));
            
            response.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.ResponseHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            return res;
        }

        public IResult GetBinary(
            string uri,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            HttpRequestMessage req;
            HttpResponseMessage response;
            CResult res;

            req = new HttpRequestMessage(
                HttpMethod.Get,
                uri);

            if (headers != null)
            {
                headers.ToList().ForEach(i => req.Headers.Add(i.Key, i.Value));
            }

            response = m_Client.SendAsync(req).Result;

            res = new CResult
            {
                URI = uri,
                Method = "GET",
                StatusCode = response.StatusCode
            };

            if (response.IsSuccessStatusCode)
            {
                res.ResponseBinaryData = response.Content.ReadAsByteArrayAsync().Result;
            }
            else
            {
                res.ResponseData = response.Content.ReadAsStringAsync().Result;
            }

            req.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.RequestHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            response.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.ResponseHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            return res;
        }

        public IResult Delete(
            string uri,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            HttpRequestMessage req;
            HttpResponseMessage response;
            CResult res;

            req = new HttpRequestMessage(
                HttpMethod.Delete,
                uri);

            if (headers != null)
            {
                headers.ToList().ForEach(i => req.Headers.Add(i.Key, i.Value));
            }

            response = m_Client.SendAsync(req).Result;

            res = new CResult
            {
                URI = uri,
                Method = "DELETE",
                StatusCode = response.StatusCode,
                ResponseData = response.Content.ReadAsStringAsync().Result
            };

            req.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.RequestHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            response.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.ResponseHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            return res;
        }

        public IResult Put(
            string uri,
            byte[] data,
            ICollection<KeyValuePair<string, string>> headers = null)
        {
            HttpResponseMessage response;
            HttpRequestMessage req;
            ByteArrayContent content;
            CResult res;

            req = new HttpRequestMessage(
                HttpMethod.Put,
                uri);
            
            content = new ByteArrayContent(data);

            req.Content = content;

            SetHeaders(req, headers);

            response = m_Client.SendAsync(req).Result;

            res = new CResult
            {
                URI = uri,
                Method = "PUT",
                StatusCode = response.StatusCode,
                RequestBinaryData = data,
                ResponseData = response.Content.ReadAsStringAsync().Result
            };

            req.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.RequestHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            response.Headers
                .ToList()
                .ForEach(i => i.Value.ToList().ForEach(j => res.ResponseHeaders.Add(new KeyValuePair<string, string>(i.Key, j))));

            return res;
        }
        
        public void Dispose()
        {
            m_Client?.Dispose();
        }

        private void SetHeaders(
            HttpRequestMessage request,
            ICollection<KeyValuePair<string, string>> headers)
        {
            if (headers == null) return;

            foreach (KeyValuePair<string, string> header in headers)
            {
                if (header.Key == CConstants.HEADER__CONTENT_TYPE)
                {
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(header.Value);
                    continue;
                }

                if (header.Key == CConstants.HEADER__CONTENT_DISPOSITION)
                {
                    request.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(header.Value);
                    continue;
                }

                if (header.Key == CConstants.HEADER__CONTENT_LENGTH)
                {
                    request.Content.Headers.ContentLength = long.Parse(header.Value);
                    continue;
                }

                request.Headers.Add(header.Key, header.Value);
            }
        }

        private void SetHeaders(
            HttpContent content,
            ICollection<KeyValuePair<string, string>> headers)
        {
            if (headers == null) return;

            foreach (KeyValuePair<string, string> header in headers)
            {
                if (header.Key == CConstants.HEADER__CONTENT_TYPE)
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(header.Value);
                }
                else
                {
                    content.Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}
