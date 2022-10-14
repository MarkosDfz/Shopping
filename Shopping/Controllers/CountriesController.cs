using System.Linq;
using Shopping.Data;
using Shopping.Data.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using Shopping.Models;

namespace Shopping.Controllers
{
    public class CountriesController : Controller
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Country> countries = await _context.Countries
                                                           .Include(c => c.States)
                                                           .ToListAsync();

            return View(countries);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            Country country = await _context.Countries
                                            .Include(c => c.States)
                                            .ThenInclude(c => c.Cities)
                                            .FirstOrDefaultAsync(c => c.Id == id);

            if (country == null) return NotFound();

            return View(country);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Country country = new() { States = new List<State>() };

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException updateEx)
                {
                    if (updateEx.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, updateEx.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }
            }

            return View(country);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            Country country = await _context.Countries.FindAsync(id);

            if (country == null) return NotFound();

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Country country)
        {
            if (id != country.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException updateEx)
                {
                    if (updateEx.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, updateEx.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }
            }

            return View(country);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            Country country = await _context.Countries
                                            .Include(c => c.States)
                                            .FirstOrDefaultAsync(c => c.Id == id);
           
            if (country == null) return NotFound();

            return View(country);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Country country = await _context.Countries.FindAsync(id);
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AddState(int? id)
        {
            if (id == null) return NotFound();

            Country country = await _context.Countries.FindAsync(id);

            if (country == null) return NotFound();

            StateViewModel stateVM = new()
            {
                CountryId = country.Id
            };

            return View(stateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddState(StateViewModel stateVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    State state = new()
                    {
                        Name = stateVM.Name,
                        Cities = new List<City>(),
                        Country = await _context.Countries.FindAsync(stateVM.CountryId)                        
                    };

                    _context.Add(state);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { Id = stateVM.CountryId } );
                }
                catch (DbUpdateException updateEx)
                {
                    if (updateEx.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una provincia con el mismo nombre en este país");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, updateEx.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }
            }

            return View(stateVM);
        }

        [HttpGet]
        public async Task<IActionResult> EditState(int? id)
        {
            if (id == null) return NotFound();

            State state = await _context.States
                                        .Include(s => s.Country)
                                        .FirstOrDefaultAsync(s => s.Id == id);

            if (state == null) return NotFound();

            StateViewModel stateVM = new()
            {
                Id = state.Id,
                Name = state.Name,
                CountryId = state.Country.Id
            };

            return View(stateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditState(int id, StateViewModel stateVM)
        {
            if (id != stateVM.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    State state = new()
                    {
                        Id = stateVM.Id,
                        Name = stateVM.Name
                    };

                    _context.Update(state);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = stateVM.CountryId });
                }
                catch (DbUpdateException updateEx)
                {
                    if (updateEx.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una provincia con el mismo nombre en el país");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, updateEx.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }
            }

            return View(stateVM);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsState(int? id)
        {
            if (id == null) return NotFound();

            State state = await _context.States
                                        .Include(s => s.Country)
                                        .Include(s => s.Cities)
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (state == null) return NotFound();

            return View(state);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteState(int? id)
        {
            if (id == null) return NotFound();

            State state = await _context.States
                                        .Include(s => s.Country)
                                        .Include(s => s.Cities)
                                        .FirstOrDefaultAsync(s => s.Id == id);

            if (state == null) return NotFound();

            return View(state);
        }

        [HttpPost, ActionName("DeleteState")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteStateConfirmed(int id)
        {
            State state = await _context.States
                                        .Include(s => s.Country)
                                        .FirstOrDefaultAsync(s => s.Id == id);

            _context.Remove(state);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = state.Country.Id });
        }

        [HttpGet]
        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null) return NotFound();

            State state = await _context.States.FindAsync(id);

            if (state == null) return NotFound();

            CityViewModel cityVM = new()
            {
                StateId = state.Id
            };

            return View(cityVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(CityViewModel cityVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    City city = new City()
                    {
                        Name = cityVM.Name,
                        State = await _context.States.FindAsync(cityVM.StateId)
                    };

                    _context.Add(city);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(DetailsState), new { id = cityVM.StateId });
                }
                catch (DbUpdateException updateEx)
                {
                    if (updateEx.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una ciudad con el mismo nombre en esta Provincia");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, updateEx.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }
            }

            return View(cityVM);
        }

        [HttpGet]
        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null) return NotFound();

            City city = await _context.Cities
                                      .Include(c => c.State)
                                      .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null) return NotFound();

            CityViewModel cityVM = new()
            {
                Id = city.Id,
                Name = city.Name,
                StateId = city.State.Id
            };

            return View(cityVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditCity(CityViewModel cityVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    City city = new()
                    {
                        Id = cityVM.Id,
                        Name = cityVM.Name,
                    };

                    _context.Update(city);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(DetailsState), new { id = cityVM.StateId });
                }
                catch (DbUpdateException updateEx)
                {
                    if (updateEx.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una provincia con el mismo nombre en el país");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, updateEx.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }
            }

            return View(cityVM);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsCity(int? id)
        {
            if (id == null) return NotFound();

            City city = await _context.Cities
                                      .Include(c => c.State)
                                      .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null) return NotFound();

            return View(city);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null) return NotFound();

            City city = await _context.Cities
                                      .Include(c => c.State)
                                      .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null) return NotFound();

            return View(city);
        }

        [HttpPost, ActionName("DeleteCity")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteCityConfirmed(int id)
        {
            City city = await _context.Cities
                                      .Include(c => c.State)
                                      .FirstOrDefaultAsync(c => c.Id == id);

            _context.Remove(city);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DetailsState), new { id = city.State.Id });
        }
    }
}
