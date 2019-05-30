using System.Collections.Generic;

namespace ApolloArtists.Models
{
    public class Users
    {
        public Users()
        {
            LikedArtists = new HashSet<LikedArtists>();
            PassedArtists = new HashSet<PassedArtists>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<LikedArtists> LikedArtists { get; set; }
        public ICollection<PassedArtists> PassedArtists { get; set; }
    }
}