using System.Collections.Generic;

namespace ApolloArtists.Models
{
    public class Artists
    {
        public Artists()
        {
            LikedArtists = new HashSet<LikedArtists>();
            PassedArtists = new HashSet<PassedArtists>();
        }

        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public string Image { get; set; }

        public ICollection<LikedArtists> LikedArtists { get; set; }
        public ICollection<PassedArtists> PassedArtists { get; set; }
    }
}