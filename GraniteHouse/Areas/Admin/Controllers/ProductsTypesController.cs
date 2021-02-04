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
    public class ProductsTypesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductsTypesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.productTypes.ToList());
        }

        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes ProductTypes) 
        {
            if (ModelState.IsValid) 
            {
                _db.Add(ProductTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            }
            return View(ProductTypes);
        }
        public async  Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var productType = await _db.productTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ProductTypes ProductTypes)
        {
            if (id != ProductTypes.Id) 
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.Update(ProductTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(ProductTypes);
        }
         public async  Task<IActionResult> Details(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var productType = await _db.productTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productType = await _db.productTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ProductTypes = await _db.productTypes.FindAsync(id);
            _db.productTypes.Remove(ProductTypes);
            
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

        }


    }
}
