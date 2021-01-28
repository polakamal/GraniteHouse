using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()

            };
        }
        public async Task<IActionResult> Index()
        {

            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShopingCart");
            if (listShoppingCart.Count >0) 
            {
                foreach (int cartItem in listShoppingCart) 
                {
                    Products prod = _db.Products.Include(p=> p.SpecialTags).Include(p=>p.ProductTypes).Where(p=> p.Id ==cartItem).FirstOrDefault();
                    ShoppingCartVM.Products.Add(prod);
                }
            
            }
            return View(ShoppingCartVM);
        }
        [HttpPost,ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost() 
        {
            List<int> listCartItem = HttpContext.Session.Get<List<int>>("ssShopingCart");
            ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate.AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour).AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);
            Appointments appointments = ShoppingCartVM.Appointments;
              _db.Appointments.Add(appointments);
              _db.SaveChanges();
            int appointmentId = appointments.Id;
            foreach(int productId in listCartItem) 
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentID = appointmentId,
                    ProductID = productId
                };
                _db.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);
            }
             _db.SaveChanges();
            listCartItem = new List<int>();
            HttpContext.Session.Set("ssShopingCart", listCartItem);
            return RedirectToAction("AppointmentConfirmation", "ShoppingCart" , new {id =appointmentId } );
        }
        public IActionResult Remove(int id)
        {
            List<int> listCartItem = HttpContext.Session.Get<List<int>>("ssShopingCart");
            if(listCartItem.Count >0) 
            {
                if (listCartItem.Contains(id)) 
                {

                    listCartItem.Remove(id);
                
                }
            }
            HttpContext.Session.Set("ssShopingCart", listCartItem);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult AppointmentConfirmation(int id)
        {
            ShoppingCartVM.Appointments = _db.Appointments.Where(a => a.Id == id).FirstOrDefault();
            List<ProductsSelectedForAppointment> objProdList = _db.ProductsSelectedForAppointment.Where(p=>p.AppointmentID == id).ToList();
            foreach (ProductsSelectedForAppointment prodAptObj in objProdList) 
            {
                ShoppingCartVM.Products.Add(  _db.Products.Include(p=>p.ProductTypes).Include(p=>p.SpecialTags).Where(p=>p.Id==prodAptObj.ProductID).FirstOrDefault() );
            
            }
            return View(ShoppingCartVM);
        }

    }
}
