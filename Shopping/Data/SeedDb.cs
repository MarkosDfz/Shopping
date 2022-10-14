using Shopping.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckCategoriesAsync();
            await CheckCountriesAsync();
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
