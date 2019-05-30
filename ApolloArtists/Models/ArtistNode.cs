using System.Linq;
using SpotifyAPI.Web.Models;

namespace ApolloArtists.Models
{
    public class ArtistNode : Artists
    {
        public ArtistNode(Artists artist, bool liked)
        {
            Href = artist.Href;
            Image = artist.Image;
            Name = artist.Name;
            Liked = liked;
        }

        public ArtistNode(FullArtist artist, bool liked)
        {
            Href = artist.Href;
            Image = artist.Images.Last().Url;
            Name = artist.Name;
            Liked = liked;
        }

        public bool Liked { get; set; }
    }
}