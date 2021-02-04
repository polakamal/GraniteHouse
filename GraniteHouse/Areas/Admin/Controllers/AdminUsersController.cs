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
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.ApplicationUser.ToList());
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length==0)
            {
                return NotFound();
            }
            var userFromDb = await _db.ApplicationUser.FindAsync(id);
            if (userFromDb == null)
            {
                NotFound();
            }
            return View(userFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(String id ,   ApplicationUser applicationUser)
        {
             if(id != applicationUser.Id)
            {
                NotFound();

            }
            if (ModelState.IsValid) 
            {
                ApplicationUser userFromDb = _db.ApplicationUser.Where(u => u.Id == id).FirstOrDefault();
                userFromDb.Name = applicationUser.Name;
                userFromDb.PhoneNumber = applicationUser.PhoneNumber;
                _db.SaveChanges();
               return RedirectToAction(nameof(Index));
            }
            return View(applicationUser);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }
            var userFromDb = await _db.ApplicationUser.FindAsync(id);
            if (userFromDb == null)
            {
                NotFound();
            }
            return View(userFromDb);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(String id)
        {
           
            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = _db.ApplicationUser.Where(u => u.Id == id).FirstOrDefault();
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(id);
        }


    }
}
