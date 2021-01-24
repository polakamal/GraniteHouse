using GraniteHouse.Data;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {

        private readonly ApplicationDbContext _db;
       private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductsViewModel productsVM { get; set; }
        public ProductsController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {

            _db = db;
            _hostingEnvironment = hostingEnvironment;
            productsVM = new ProductsViewModel()
            {

                ProductTypes = _db.productTypes.ToList(),
                SpecialTags = _db.specialTags.ToList(),
                Products = new Models.Products()


            };
        }
        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }

        public IActionResult Create() 
        {

            return View(productsVM);
        }
        //post action
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatPost() 
        {

            if (!ModelState.IsValid) 
            {

                return View(productsVM);
                
            }
            _db.Products.Add(productsVM.Products);
            await _db.SaveChangesAsync();
            //image is been saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var productsFromDb = _db.Products.Find(productsVM.Products.Id); 
            if(files.Count !=0)
            {
                //image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);
                using (var filestream = new FileStream(Path.Combine(uploads, productsVM.Products.Id + extension) ,FileMode.Create)) 
                {
                    files[0].CopyTo(filestream);
                 
                }
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsVM.Products.Id + extension;

            }
            else 
            {

                //when user not uploaded images
                var uploads = Path.Combine(webRootPath,SD.ImageFolder + @"\" + SD.DefaultProductImage );
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder+@"\" + productsVM.Products.Id + ".png");
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsVM.Products.Id + ".png";


            }
           await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        
        }
    
    }
}
