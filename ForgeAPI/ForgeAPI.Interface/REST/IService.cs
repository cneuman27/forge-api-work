using System.Collections.Generic;

namespace ForgeAPI.Interface.REST
{
    public interface IService
    {
        IResult Post(
            string uri,
            object data,
            ICollection<KeyValuePair<string, string>> headers = null);

        IResult PostFormData(
            string uri,
            ICollection<KeyValuePair<string, string>> formData,
            ICollection<KeyValuePair<string, string>> headers = null);

        IResult Get(
            string uri,
            ICollection<KeyValuePair<string, string>> headers = null);

        IResult GetBinary(
            string uri,
            ICollection<KeyValuePair<string, string>> headers = null);

        IResult Delete(
            string uri,
            ICollection<KeyValuePair<string, string>> headers = null);

        IResult Put(
            string uri,
            byte[] data,
            ICollection<KeyValuePair<string, string>> headers = null);
    }
}
