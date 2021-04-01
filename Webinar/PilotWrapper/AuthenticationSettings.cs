using System;
using System.Security;
using Ascon.Pilot.Server.Api;
using Newtonsoft.Json;

namespace WebinarTelegram.PilotWrapper
{
    [Serializable]
    public class AuthenticationSettings
    {
        public ConnectionCredentials Credentials
        {
            get
            {
                if (Password == null) return null;
                var pass = new SecureString();
                foreach (var c in Password.ToCharArray())
                    pass.AppendChar(c);
                return ConnectionCredentials.GetConnectionCredentials(Server, Login, pass);
            }
        }

        [JsonProperty("LicenseCode")] public int LicenseCode { get; set; }

        [JsonProperty("Login")] public string Login { get; set; }

        [JsonProperty("Password")] public string Password { get; set; }

        [JsonProperty("Server")] public string Server { get; set; }
    }
}