using Shopping.Data.Entities;
using Shopping.Enums;
using Shopping.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckCategoriesAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync();
        }

        private async Task CheckRolesAsync()
        {
            foreach (var item in Enum.GetValues(typeof(UserType)))
                await _userHelper.CheckRoleAsync(item.ToString());
        }

        private async Task CheckUserAsync()
        {
            User user = new()
            {
                Document  = "050354785",
                FirstName = "Marcos",
                LastName  = "Dfz",
                UserName  = "MarkosDfz",
                Email     = "asdsa@gmail.com",
                City      = _context.Cities.FirstOrDefault(), 
                Address   = "hogwarts",
                PhoneNumber = "0986987452",
                UserType  = UserType.Admin
            };

            User userDb = await _userHelper.GetUserAsync(user.Email);

            if (userDb == null)
            {
                await _userHelper.AddUserAsync(user, "dfz666");
                await _userHelper.AddUserToRoleAsync(user, UserType.Admin.ToString());
            }
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                string[] categories = { "Tecnología", "Ropa", "Calzado", "Accesorios", "Mascotas", "Deportes" };

                foreach (string category in categories)
                    _context.Categories.Add(new Category { Name = category });

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Ecuador",
                    States = new List<State>()
                    {
                        new State { Name = "Pichincha" },
                        new State {
                            Name = "Cotopaxi",
                            Cities = new List<City>
                            {
                                new City { Name = "Latacunga" },
                                new City { Name = "Salcedo" },
                            }
                        },
                    }
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}
