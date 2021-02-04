using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.utility;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = SD.SuperAdminEndUser)]

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

        //GET : Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            productsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (productsVM.Products == null)
            {
                return NotFound();
            }

            return View(productsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
          
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _db.Products.Where(m => m.Id == productsVM.Products.Id).FirstOrDefault();
                if(files.Count >0 && files[0] != null) 
                {
                    //if user upload new image
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productFromDb.Image);
                    if (System.IO.File.Exists(Path.Combine(uploads, productsVM.Products.Id + extension_old)))
                        {

                        System.IO.File.Delete(Path.Combine(uploads, productsVM.Products.Id + extension_old));
                       
                       }
                    using (var filestream = new FileStream(Path.Combine(uploads, productsVM.Products.Id + extension_new), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);

                        } 
                        productsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + productsVM.Products.Id + extension_new;
                       
                       }
                if(productsVM.Products.Image != null) 
                {
                    productFromDb.Image = productsVM.Products.Image;
                }
                productFromDb.Name = productsVM.Products.Name;
                productFromDb.Price = productsVM.Products.Price;
                productFromDb.Available = productsVM.Products.Available;
                productFromDb.ShadeColor = productsVM.Products.ShadeColor;
                productFromDb.ProductTypeId = productsVM.Products.ProductTypeId;
                productFromDb.SpecialTagId = productsVM.Products.SpecialTagId;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productsVM);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            productsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (productsVM.Products == null)
            {
                return NotFound();
            }

            return View(productsVM);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            productsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (productsVM.Products == null)
            {
                return NotFound();
            }

            return View(productsVM);
        }
        [HttpPost ,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Products products = await _db.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();

            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(products.Image);
                if (System.IO.File.Exists(Path.Combine(uploads, products.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, products.Id + extension));
                }
                _db.Products.Remove(products);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } 

        }

    }
}
