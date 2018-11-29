using System.Collections.Generic;

namespace ForgeAPI.Interface.Authentication
{
    public interface IService
    {
        IToken Authenticate(List<Enums.E_AccessScope> scopes);
    }
}
