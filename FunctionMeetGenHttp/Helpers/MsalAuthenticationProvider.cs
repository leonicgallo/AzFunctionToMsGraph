using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System;
using System.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace FunctionMeetGenHttp.Helpers
{
    // This class encapsulates the details of getting a token from MSAL and exposes it via the 
    // IAuthenticationProvider interface so that GraphServiceClient or AuthHandler can use it.
    // A significantly enhanced version of this class will in the future be available from
    // the GraphSDK team.  It will supports all the types of Client Application as defined by MSAL.
    public class MsalAuthenticationProvider : IAuthenticationProvider
    {
        private IConfidentialClientApplication _clientApplication;
        private string[] _scopes;
        public MsalAuthenticationProvider(IConfidentialClientApplication clientApplication, string[] scopes)
        {
            _clientApplication = clientApplication;
            _scopes = scopes;
        }


        //https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-net-acquire-token-silently
        /// <summary>
        /// Update HttpRequestMessage with credentials
        /// </summary>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            await Task.CompletedTask;
        }

        /*Backup
                 public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            await Task.CompletedTask;
        }
             */  
        
            /// <summary>
        /// Acquire Token 
        /// </summary>
        public async Task<string> GetTokenAsync()
        {
            Microsoft.Identity.Client.AuthenticationResult authResult = null;
            //authResult.
            //authResult = ;
            return authResult.AccessToken;
        }
   
        public string GetToken()
        {
            return Environment.GetEnvironmentVariable("token", EnvironmentVariableTarget.Process);
        }
    }
}
