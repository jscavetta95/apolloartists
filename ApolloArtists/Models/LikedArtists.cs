﻿namespace ApolloArtists.Models
{
    public class LikedArtists
    {
        public int UserId { get; set; }
        public int ArtistId { get; set; }

        public Artists Artist { get; set; }
        public Users User { get; set; }
    }
}