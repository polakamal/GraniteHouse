using GraniteHouse.Data;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Areas.Admin.Controllers
{
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
        
    }
}
