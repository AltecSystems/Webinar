using System;
using System.IO;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebinarTelegram.PilotWrapper.Services
{
    public interface IServerConnector
    {
        IServerApi ServerApi { get; }

        //IServerAdminApi AdminApi { get; }

        void Connect();

        DDatabaseInfo GetDDatabaseInfo();
    }

    public class ServerConnector : IServerConnector
    {
        private readonly ILogger<ServerConnector> _logger;
        private readonly AuthenticationSettings _settings;
        private HttpPilotClient _client;
        private DDatabaseInfo _dDatabaseInfo;

        public ServerConnector(ILogger<ServerConnector> logger)
        {
            _logger = logger;
            _settings = GetSettings();
            Connect();
        }

        public IAuthenticationApi AuthenticationApi { get; set; }

        public IServerApi ServerApi { get; set; }
        //public IServerAdminApi AdminApi { get;  set; }

        public void Connect()
        {
            _client = new HttpPilotClient(_settings.Credentials.GetConnectionString(),
                _settings.Credentials.GetConnectionProxy());
            _client.Connect(false);

            AuthenticationApi = _client.GetAuthenticationApi();

            AuthenticationApi.Login(_settings.Credentials.DatabaseName,
                _settings.Credentials.Username,
                _settings.Credentials.ProtectedPassword,
                _settings.Credentials.UseWindowsAuth,
                _settings.LicenseCode);

            ServerApi = _client.GetServerApi(null);
            //AdminApi = _client.GetServerAdminApi(null);

            _client.GetAuthenticationApi().LoginServerAdministrator(_settings.Credentials.Username,
                _settings.Credentials.ProtectedPassword, _settings.Credentials.UseWindowsAuth);

            _dDatabaseInfo = ServerApi.OpenDatabase();
            //AdminApi.OpenDatabase(_settings.Credentials.DatabaseName);

            _logger.LogInformation(
                "Server {ConnectionString}/{DbName} connect successful!",
                _settings.Credentials.GetConnectionString(),
                _settings.Credentials.DatabaseName);
        }

        public DDatabaseInfo GetDDatabaseInfo()
        {
            return _dDatabaseInfo;
        }

        private static AuthenticationSettings GetSettings()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\SettingsAuthentication.json");
            var sr = new StreamReader(path);
            return JsonConvert.DeserializeObject<AuthenticationSettings>(sr.ReadToEnd());
        }
    }
}