using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]

    [Area("Admin")]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.specialTags.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags SpecialTags)
        {
            if (ModelState.IsValid)
            {
                _db.Add(SpecialTags);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(SpecialTags);
        }
        public async Task<IActionResult> Edit(int? id)
        {
        if(id == null)
            {
                return NotFound();
            }
            var specailTags = await _db.specialTags.FindAsync(id);
            if (specailTags == null) 
            {
                return NotFound();
            }
        return View(specailTags);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags specialTags)
        { 
        if(id != specialTags.Id) 
            {
                return NotFound();
            }

            if (ModelState.IsValid) 
            {
                 _db.Update(specialTags);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialTags);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var specialTags = await _db.specialTags.FindAsync(id);
            if (specialTags == null)
            {
                return NotFound();
            }
            return View(specialTags);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var specialTags = await _db.specialTags.FindAsync(id);
            if (specialTags == null)
            {
                return NotFound();
            }
            return View(specialTags);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var specialTags = await _db.specialTags.FindAsync(id);
            _db.specialTags.Remove(specialTags);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }



    }
}
