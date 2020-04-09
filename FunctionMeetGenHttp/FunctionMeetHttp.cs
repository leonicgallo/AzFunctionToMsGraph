using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Collections.Generic;
using Microsoft.Identity.Client;
using FunctionMeetGenHttp.Helpers;
using System.IO;
using Newtonsoft.Json;
using FunctionMeetGenHttp.Models;

namespace FunctionMeetGenHttp
{
    public static class FunctionMeetHttp
    {
        private static GraphServiceClient _graphServiceClient;

        [FunctionName("FunctionMeetHttp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var meeting  = JsonConvert.DeserializeObject<Meeting>(requestBody);

            GraphServiceClient graphClient = GetAuthenticatedGraphClient();


            var participants = new MeetingParticipants();
            var participantsInfo = new List<MeetingParticipantInfo>();

            var participantInfoPacient = new MeetingParticipantInfo();
            var identitySetUserPacient = new IdentitySet
            {
                User = new Identity()
            };
            var identityUserPacient = new Identity
            {
                DisplayName = meeting.PatientEmail
            };
            identitySetUserPacient.User = identityUserPacient;

            participantInfoPacient.Identity = identitySetUserPacient;

            /*var participantInfoDoctor = new MeetingParticipantInfo();
            var identitySetUserDoctor = new IdentitySet
            {
                User = new Identity()
            };
            var identityUserDoctor = new Identity
            {
                DisplayName = meeting.DoctorEmail
            };
            identitySetUserDoctor.User = identityUserDoctor;

            participantInfoDoctor.Identity = identitySetUserDoctor;

            participantInfoDoctor.Identity = identitySetUserDoctor;*/

            participantsInfo.Add(participantInfoPacient);
            //participantsInfo.Add(participantInfoDoctor);
            participants.Attendees = participantsInfo;

            var onlineMeeting = new OnlineMeeting
            {
                StartDateTime = DateTimeOffset.Parse(meeting.StartDate),
                EndDateTime = DateTimeOffset.Parse(meeting.EndDate),
                Participants = new MeetingParticipants(),
                Subject = meeting.Subject
            };


            var onlineMeeting1 = new OnlineMeeting
            {
                StartDateTime = DateTimeOffset.Parse("2019-07-12T21:30:34.2444915+00:00"),
                EndDateTime = DateTimeOffset.Parse("2019-07-12T22:00:34.2464912+00:00"),
                Subject = "User Token Meeting"
            };


            OnlineMeeting onlineMeetingReturned;
            try
            {
                onlineMeetingReturned = await graphClient.Me.OnlineMeetings.Request().AddAsync(onlineMeeting);
            }catch(Exception ex )
            {
                if(ex.Message.Contains("InvalidAuthenticationToken"))
                {
                    return new BadRequestObjectResult("Token Expired");
                }
                else
                {
                    return new BadRequestResult();
                }

            }
            return new OkObjectResult(onlineMeetingReturned);
        }

        private static GraphServiceClient GetAuthenticatedGraphClient()
        {
            var authenticationProvider = CreateAuthorizationProvider();
            _graphServiceClient = new GraphServiceClient(authenticationProvider);
            return _graphServiceClient;
        }

        private static IAuthenticationProvider CreateAuthorizationProvider()
        {
            var clientId = Environment.GetEnvironmentVariable("AzureADAppClientId", EnvironmentVariableTarget.Process);
            var clientSecret = System.Environment.GetEnvironmentVariable("AzureADAppClientSecret", EnvironmentVariableTarget.Process);
            var redirectUri = System.Environment.GetEnvironmentVariable("AzureADAppRedirectUri", EnvironmentVariableTarget.Process);
            var tenantId = System.Environment.GetEnvironmentVariable("AzureADAppTenantId", EnvironmentVariableTarget.Process);
            var authority = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            //this specific scope means that application will default to what is defined in the application registration rather than using dynamic scopes
            string[] scopes = new string[] { "https://graph.microsoft.com/" };
            //List<string> scopes = new List<string>();
            //scopes.Add("https://graph.microsoft.com/v1.0/me/");

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                                              .WithClientSecret(clientSecret)
                                              .WithAuthority(new Uri(authority))                                             
                                              .Build();
            return new MsalAuthenticationProvider(cca, scopes);
        }
    }
}
