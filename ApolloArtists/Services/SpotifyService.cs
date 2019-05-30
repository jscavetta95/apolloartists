using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;

namespace ApolloArtists.Services
{
    public class SpotifyService
    {
        private static Token token;
        private readonly IConfiguration configuration;

        public SpotifyService(IConfiguration configuration)
        {
            this.configuration = configuration;
            token = new CredentialsAuth(ClientId, SecretKey).GetToken().Result;
        }

        private string ClientId => configuration["Spotify:ClientId"];
        private string SecretKey => configuration["Spotify:SecretKey"];
        private Token Token => token.IsExpired() ? new CredentialsAuth(ClientId, SecretKey).GetToken().Result : token;

        public SpotifyWebAPI SpotifyWebApi => new SpotifyWebAPI{TokenType = Token.TokenType, AccessToken = Token.AccessToken};
    }
}