using System;
using System.Collections.Generic;

namespace ForgeAPI.Interface.Authentication
{
    public interface IToken
    {
        string TokenType
        {
            get;
        }

        int LifetimeSeconds
        {
            get;
        }

        string AccessToken
        {
            get;
        }

        DateTime CreateDate
        {
            get;
        }

        DateTime ExpirationDate
        {
            get;
        }

        bool IsExpired
        {
            get;
        }

        List<Enums.E_AccessScope> AccessScopes
        {
            get;
        }
    }
}
