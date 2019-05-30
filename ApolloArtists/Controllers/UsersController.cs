using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using ApolloArtists.Models;
using Microsoft.AspNetCore.Http;

namespace ApolloArtists.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApolloArtistsContext context;

        public UsersController(ApolloArtistsContext context)
        {
            this.context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Login(Users users)
        {
            try
            {
                var user = context.Users.First(u => u.Email == users.Email);

                if (ValidatePassword(user.Password, users.Password))
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToAction(nameof(ArtistMapController.Index), "ArtistMap");
                }

                HttpContext.Session.SetString("LoginMessage", "Invalid password.");
            }
            catch (Exception)
            {
                HttpContext.Session.SetString("LoginMessage", "Email does not exist.");
            }

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Users users)
        {
            if (!context.Users.Any(u => u.Email == users.Email))
            {
                users.Password = EncryptPassword(users.Password);
                context.Users.Add(users);
                context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", users.UserId);
            }
            else
            {
                HttpContext.Session.SetString("LoginMessage", "Email already taken.");
                return View();
            }

            return RedirectToAction(nameof(ArtistMapController.Index), "ArtistMap");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private string EncryptPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        private bool ValidatePassword(string storedPassword, string enteredPassword)
        {
            byte[] storedPasswordBytes = Convert.FromBase64String(storedPassword);

            byte[] salt = new byte[16];
            Array.Copy(storedPasswordBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] enteredPasswordBytes = pbkdf2.GetBytes(20);

            bool match = true;
            for (int i = 0; match && i < enteredPasswordBytes.Length; i++)
            {
                if (storedPasswordBytes[i + salt.Length] != enteredPasswordBytes[i])
                {
                    match = false;
                }
            }

            return match;
        }
    }
}
