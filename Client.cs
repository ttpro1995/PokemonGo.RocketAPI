using System.Net;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.HttpClient;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Enums;

using System;
using PokemonGo.RocketAPI.Api.PlayerModels;

namespace PokemonGo.RocketAPI
{
    public class Client
    {
        public Rpc.Login Login;
        public Rpc.Player Player;
        public Rpc.Download Download;
        public Rpc.Inventory Inventory;
        public Rpc.Map Map;
        public Rpc.Fort Fort;
        public Rpc.Encounter Encounter;
        public Rpc.Misc Misc;

        public IApiFailureStrategy ApiFailure { get; set; }
        public ISettings Settings { get; }
        public string AuthToken { get; set; }

        public static WebProxy Proxy;

        public double CurrentLatitude { get; internal set; }
        public double CurrentLongitude { get; internal set; }
        public double CurrentAltitude { get; internal set; }

        public AuthType AuthType => Settings.AuthType;

        internal readonly PokemonHttpClient PokemonHttpClient;
        internal string ApiUrl { get; set; }
        internal AuthTicket AuthTicket { get; set; }

        internal Platform Platform { get; set; }
        internal uint AppVersion { get; set; }

        public Api.InventoryModels.Inventories Inventories;
        public Api.SettingsModels.ApiSettings ApiSettings;
        public PlayerProfile PlayerProfile;

        public Client(ISettings settings, IApiFailureStrategy apiFailureStrategy)
        {
            Settings = settings;
            ApiFailure = apiFailureStrategy;
            Proxy = InitProxy();
            PokemonHttpClient = new PokemonHttpClient();
            Login = new Rpc.Login(this);
            Player = new Rpc.Player(this);
            Download = new Rpc.Download(this);
            Inventory = new Rpc.Inventory(this);
            Map = new Rpc.Map(this);
            Fort = new Rpc.Fort(this);
            Encounter = new Rpc.Encounter(this);
            Misc = new Rpc.Misc(this);

            Player.SetCoordinates(Settings.DefaultLatitude, Settings.DefaultLongitude, Settings.DefaultAltitude);

            if (settings.DevicePlatform.Equals("ios", System.StringComparison.Ordinal))
                Platform = Platform.Ios;
            else
                Platform = Platform.Android;

            AppVersion = 3500;

            Inventories = new Api.InventoryModels.Inventories(this);
            ApiSettings = new Api.SettingsModels.ApiSettings(this);
            PlayerProfile = new PlayerProfile(this);
        }

        public static long GetCurrentTimeMillis()
        {
            return DateTime.UtcNow.ToUnixTime();
        }

        private WebProxy InitProxy()
        {
            if (!Settings.UseProxy) return null;

            WebProxy prox = new WebProxy(new System.Uri($"http://{Settings.UseProxyHost}:{Settings.UseProxyPort}"), false, null);

            if (Settings.UseProxyAuthentication)
                prox.Credentials = new NetworkCredential(Settings.UseProxyUsername, Settings.UseProxyPassword);

            return prox;
        }
    }
}