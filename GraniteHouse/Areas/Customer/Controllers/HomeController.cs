using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db,ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }
       
        public async Task<IActionResult> Index()
        {
            var ProductList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();
            return View(ProductList);
        }
        public async Task<IActionResult> Details(int id)
        {
            var Product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m => m.Id == id).FirstOrDefaultAsync();
            return View(Product);
        }
        [HttpPost,ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {

            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShopingCart");
            if(listShoppingCart == null) 
            {
                listShoppingCart = new List<int>();


            }
            listShoppingCart.Add(id);
            HttpContext.Session.Set("ssShopingCart", listShoppingCart);
            return RedirectToAction("Index", "Home",new {area = "Customer" });

        }
        public IActionResult Remove(int id ) 
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShopingCart");
            if (listShoppingCart.Count >0)
            {
                if (listShoppingCart.Contains(id))
                {
                    listShoppingCart.Remove(id);


                }
            }
            HttpContext.Session.Set("ssShopingCart", listShoppingCart);
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
