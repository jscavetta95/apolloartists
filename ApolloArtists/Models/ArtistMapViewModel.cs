using System.Collections.Generic;

namespace ApolloArtists.Models
{
    public class ArtistMapViewModel
    {
        public ArtistMapViewModel(List<ArtistNode> artistNodes, List<(int from, int to)> edgeTuples)
        {
            ArtistNodes = artistNodes;
            EdgeTuples = edgeTuples;
        }

        public List<ArtistNode> ArtistNodes { get; set; }
        public List<(int from, int to)> EdgeTuples { get; set; }
    }
}