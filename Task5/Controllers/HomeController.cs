using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;
using System.Reflection;
using System.Text;
using CsvHelper;
using Microsoft.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Task5.Controllers
{
    public class HomeController : Controller
    {
        public static string spanishAlphabet = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZ0123456789";
        public static string italianAlphabet = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZzÀàÈèÉéÌìÒòÙù0123456789";
        public static string germanAlphabet = "AÄBCDDEFGHIİJJKLMNOÖPQRSTUÜVWXYZaäbcddefghii̇jjklmnoöpqrstuüvwxyz0123456789";
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public static List<User> users = new List<User>();
        public static int lastId = 0;
        public static int count = 20;
        public static int currentPage = 1;
        public static int seed = 0;
        public static string locale = "it";
        public static double errorsCount = 0;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        //TODO: filters and pagination
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Options(string appSeed, string region, string errors)
        {
            users = new List<User>();
            count = 20;
            lastId = 0;
            currentPage = 1;
            seed = Int32.Parse(appSeed);
            locale = region;
            errorsCount = double.Parse(errors.Replace('.',','));
            return View("Users");
        }

        public IActionResult Users()
        {
            return View();
        }
        private const int BATCH_SIZE = 20;
        [HttpPost]
        public IActionResult _Users(string sortOrder, string searchString, int firstItem = 0)
        {
            seed += currentPage;
            users.AddRange(UsersGenerator());
            var model = users.Skip(firstItem).Take(BATCH_SIZE).ToList();
            return PartialView(model);
        }
        public IList<User> UsersGenerator()
        {
            var users = new List<User>();
            Random random = new Random(seed);
            var FirstNames = (from firstName in _context.FirstNames
                    where firstName.Locale == locale
                        select  firstName).ToList();
            var LastNames = (from lastName in _context.LastNames
                where lastName.Locale == locale
                select lastName).ToList();
            var Addresses = (from address in _context.Addresses
                where address.Locale == locale
                select address).ToList();
            var PhoneNumbers = (from phoneNumber in _context.PhoneNumbers
                where phoneNumber.Locale == locale
                select phoneNumber).ToList();
            for (int i = 0; i < count; i++)
            {
                Guid guid = Guid.NewGuid();
                var user = new User();
                lastId += 1;
                user.Index = lastId;
                user.UserId = guid.ToString();
                user.Address = Addresses[random.Next(0, Addresses.Count())].FullAddress;
                user.PhoneNumber = PhoneNumbers[random.Next(0, PhoneNumbers.Count())].Number;
                user.FullName = FirstNames[random.Next(0, FirstNames.Count())].Name
                                + " " + LastNames[random.Next(0, LastNames.Count())].SecondName;
                if (errorsCount > 0)
                {
                    _logger.LogInformation("error count{errorCount}:", errorsCount);
                    GenerateErrors(user);
                }
                users.Add(user);
            }
            count = 10;
            currentPage++;
            return users;
        }

        public void GenerateErrors(User user)
        {
            var errors = Math.Truncate(errorsCount + new Random().NextDouble());
            _logger.LogInformation("error count{errorCount}:", errors);
            Random rnd = new Random();
            var num = rnd.NextDouble();
            if (num <= 0.33)
            {
                AddSymbolError(errors, user);
            }
            else if(num <= 0.66)
            {
                RemoveSymbolError(errors, user);
            }
            else
            {
                ChangeSymbolsError(errors, user);
            }
        }

        private void ChangeSymbolsError(double errors, User user)
        {
            Random rnd = new Random();
            int pos;
            char tmp, tmp1;
            for (int i = 0; i < errors; i++)
            {
                pos = rnd.Next(0, user.UserId.Length - 2);
                tmp = user.UserId[pos];
                tmp1 = user.UserId[pos + 1];
                user.UserId = user.UserId.Remove(pos, 2);
                user.UserId = user.UserId.Insert(pos, tmp1.ToString());
                user.UserId = user.UserId.Insert(pos + 1, tmp.ToString());
                pos = rnd.Next(0, user.Address.Length - 2);
                tmp = user.Address[pos];
                tmp1 = user.Address[pos + 1];
                user.Address = user.Address.Remove(pos, 2);
                user.Address = user.Address.Insert(pos, tmp1.ToString());
                user.Address = user.Address.Insert(pos + 1, tmp.ToString());
                pos = rnd.Next(0, user.PhoneNumber.Length - 2);
                tmp = user.PhoneNumber[pos];
                tmp1 = user.PhoneNumber[pos + 1];
                user.PhoneNumber = user.PhoneNumber.Remove(pos, 2);
                user.PhoneNumber = user.PhoneNumber.Insert(pos, tmp1.ToString());
                user.PhoneNumber = user.PhoneNumber.Insert(pos + 1, tmp.ToString());
                pos = rnd.Next(0, user.FullName.Length - 2);
                tmp = user.FullName[pos];
                tmp1 = user.FullName[pos + 1];
                user.FullName = user.FullName.Remove(pos, 2);
                user.FullName = user.FullName.Insert(pos, tmp1.ToString());
                user.FullName = user.FullName.Insert(pos + 1, tmp.ToString());
                }
        }

        private void RemoveSymbolError(double errors, User user)
        {
            try
            {
                Random rnd = new Random();
                for (int i = 0; i < errors; i++)
                {
                    if (user.UserId.Length <= 5 || user.Address.Length <= 5 || user.PhoneNumber.Length <= 5 ||
                        user.FullName.Length <= 5)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    else
                    {
                        user.UserId = user.UserId.Remove(rnd.Next(0, rnd.Next(0, user.UserId.Length - 1)), 1);
                        user.Address = user.Address.Remove(rnd.Next(0, rnd.Next(0, user.Address.Length - 1)), 1);
                        user.PhoneNumber = user.PhoneNumber.Remove(rnd.Next(0, rnd.Next(0, user.PhoneNumber.Length - 1)), 1);
                        user.FullName = user.FullName.Remove(rnd.Next(0, rnd.Next(0, user.FullName.Length - 1)), 1);
                    }
                       
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                AddSymbolError(errors, user);
            }
        }

        private void AddSymbolError(double errors, User user)
        {
            Random rnd = new Random();
            string symbols;
            if (locale.Equals("it"))
            {
                symbols = italianAlphabet;
            } else if (locale.Equals("es"))
            {
                symbols = spanishAlphabet;
            }
            else
            {
                symbols = germanAlphabet;
            }
            for (int i = 0; i < errors; i++)
            {
                if (user.UserId.Length >= 40 || user.Address.Length >= 150 || user.PhoneNumber.Length >= 20 ||
                    user.FullName.Length >= 40)
                {

                }
                else
                {
                    user.UserId = user.UserId.Insert(rnd.Next(0, user.UserId.Length - 1), symbols[rnd.Next(0, symbols.Length - 1)].ToString());
                    user.Address = user.Address.Insert(rnd.Next(0, user.Address.Length - 1), symbols[rnd.Next(0, symbols.Length - 1)].ToString());
                    user.PhoneNumber = user.PhoneNumber.Insert(rnd.Next(0, user.PhoneNumber.Length - 1), symbols[rnd.Next(0, symbols.Length - 1)].ToString());
                    user.FullName = user.FullName.Insert(rnd.Next(0, user.FullName.Length - 1), symbols[rnd.Next(0, symbols.Length - 1)].ToString());
                }
            }
        }

        public IActionResult DownloadCSV()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Index, UserId, Address, PhoneNumber, FullName");
            foreach (var user in users)
            {
                sb.AppendLine($"{user.Index},{user.UserId},{user.Address.Replace(',','.')},{user.PhoneNumber},{user.FullName}");
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "users.csv");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}