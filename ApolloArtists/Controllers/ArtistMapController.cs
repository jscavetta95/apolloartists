using System;
using System.Collections.Generic;
using System.Linq;
using ApolloArtists.Models;
using ApolloArtists.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web.Enums;

namespace ApolloArtists.Controllers
{
    public class ArtistMapController : Controller
    {
        private readonly ApolloArtistsContext context;
        private readonly SpotifyService spotify;

        public ArtistMapController(ApolloArtistsContext context, SpotifyService spotify)
        {
            this.context = context;
            this.spotify = spotify;
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction(nameof(UsersController.Login), "Users");

            return View(CreateArtistMapViewModel());
        }

        public IActionResult SearchForArtist(string searchInput)
        {
            var results = new List<Artists>();

            var searchItem = spotify.SpotifyWebApi.SearchItems(searchInput, SearchType.Artist);

            if (searchItem.Artists != null && searchItem.Artists.Total > 0)
            {
                searchItem.Artists.Items.ForEach(artist => results.Add(new Artists{ Name = artist.Name,
                                                                                    Image = artist.Images.Count != 0 ? artist.Images.Last().Url : null,
                                                                                    Href = artist.Href}));
            }

            return Json(results);
        }

        public IActionResult LikeArtist(string href, string image, string name)
        {
            if (!IsLoggedIn()) return RedirectToAction(nameof(UsersController.Login), "Users");

            Artists artistToLike = null;
            try
            {
                artistToLike = context.Artists.First(artists => artists.Href == href);
            }
            catch (Exception)
            {
                // ignored
            }

            if (artistToLike == null)
            {
                artistToLike = new Artists {Href = href, Image = image, Name = name};
                context.Artists.Add(artistToLike);
                context.SaveChanges();
            }

            context.LikedArtists.Add(new LikedArtists {ArtistId = artistToLike.ArtistId, UserId = HttpContext.Session.GetInt32("UserId").Value });

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                // ignored
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult PassArtist(string href, string image, string name)
        {
            if (!IsLoggedIn()) return RedirectToAction(nameof(UsersController.Login), "Users");

            Artists artistToPass = null;
            try
            {
                artistToPass = context.Artists.First(artists => artists.Href == href);
            }
            catch (Exception)
            {
                // ignored
            }

            if (artistToPass == null)
            {
                artistToPass = new Artists {Href = href, Image = image, Name = name};
                context.Artists.Add(artistToPass);
                context.SaveChanges();
            }

            context.PassedArtists.Add(new PassedArtists {ArtistId = artistToPass.ArtistId, UserId = HttpContext.Session.GetInt32("UserId").Value });

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                // ignored
            }

            return RedirectToAction(nameof(Index));
        }

        private ArtistMapViewModel CreateArtistMapViewModel()
        {
            var userId = HttpContext.Session.GetInt32("UserId").Value;
            var likedArtists = context.Users.Include(u => u.LikedArtists).ThenInclude(artists => artists.Artist).First(user => user.UserId == userId).LikedArtists;
            var passedArtists = context.Users.Include(u => u.PassedArtists).ThenInclude(artists => artists.Artist).First(user => user.UserId == userId).PassedArtists
                                       .Select(artist => artist.Artist.Href).ToHashSet();

            var artistsToDisplay = likedArtists.Select(l => l.Artist).Select(likedArtist => new ArtistNode(likedArtist, true)).ToList();
            var edgeTuples = new List<(int from, int to)>();

            foreach (var likedArtist in likedArtists.Select(l => l.Artist))
            {
                var relatedArtists = spotify.SpotifyWebApi.GetRelatedArtists(likedArtist.Href.Split('/').Last());

                var relatedAdded = 0;
                foreach (var relatedArtist in relatedArtists.Artists)
                {
                    if (passedArtists.Contains(relatedArtist.Href)) continue;

                    if (!artistsToDisplay.Select(artist => artist.Href).Contains(relatedArtist.Href))
                    {
                        if (relatedAdded < 5)
                        {
                            artistsToDisplay.Add(new ArtistNode(relatedArtist, false));
                            relatedAdded++;

                            edgeTuples.Add((artistsToDisplay.FindIndex(node => node.Href == likedArtist.Href) + 1,
                                            artistsToDisplay.FindIndex(node => node.Href == relatedArtist.Href) + 1));
                        }
                    }
                    else if (likedArtists.Select(artist => artist.Artist.Href).Contains(relatedArtist.Href))
                    {
                        if (edgeTuples.Select(tuple => tuple.to)
                                      .Contains(artistsToDisplay.FindIndex(node => node.Href == relatedArtist.Href) + 1)) continue;

                        if (edgeTuples
                           .Contains((artistsToDisplay.FindIndex(node => node.Href == relatedArtist.Href) + 1,
                                      artistsToDisplay.FindIndex(node => node.Href == likedArtist.Href) + 1))) continue;

                        edgeTuples.Add((artistsToDisplay.FindIndex(node => node.Href == likedArtist.Href) + 1,
                                        artistsToDisplay.FindIndex(node => node.Href == relatedArtist.Href) + 1));
                    }
                }
            }

            return new ArtistMapViewModel(artistsToDisplay, edgeTuples);
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue;
        }
    }
}