using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Provider
{
    public class QueryStringOAuthBearerProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name;
            //return connection.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
