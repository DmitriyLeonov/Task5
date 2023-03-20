using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Models;

namespace Task5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        //TODO: filters and pagination
        public IActionResult Index()
        {
            int count = 20, seed = 4;
            List<User> users = new List<User>();
            string locale = "it";
            users.AddRange(UsersGenerator(count, locale, seed));
            return View(users);
        }

        public IList<User> UsersGenerator(int count, string locale, int seed)
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
                user.Index = i + 1;
                user.UserId = guid.ToString();
                user.Address = Addresses[random.Next(0, Addresses.Count())].FullAddress;
                user.PhoneNumber = PhoneNumbers[random.Next(0, PhoneNumbers.Count())].Number;
                user.FullName = FirstNames[random.Next(0, FirstNames.Count())].Name
                                + " " + LastNames[random.Next(0, LastNames.Count())].SecondName;
                users.Add(user);
            }
            return users;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}