using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser + "," + SD.AdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private int PageSize = 3;
        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int productPage=1,string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                Appointments = new List<Appointments>()
            };
            StringBuilder param = new StringBuilder();
            param.Append("/Admin/Appointments?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);

            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);

            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);

            }
            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);

            }
            appointmentVM.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();

            if (User.IsInRole(SD.AdminEndUser))
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
            }
            if (searchName != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();

                }
                catch (Exception ex)
                {

                }
            }
            var count = appointmentVM.Appointments.Count;
            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(p=>p.AppointmentDate).Skip((productPage-1)*PageSize).Take(PageSize).ToList();
            appointmentVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };
            
            return View(appointmentVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductID
                                                      where a.AppointmentID == id
                                                      select p).Include("ProductTypes");
            AppointmentsDetailsViewModel ObjappointmentsVM = new AppointmentsDetailsViewModel()
            {
                appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUser.ToList(),
                products = productList.ToList()

            };
            return View(ObjappointmentsVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentsDetailsViewModel ObjAppointmentsVM)
        {
            if (ModelState.IsValid)
            {

                ObjAppointmentsVM.appointment.AppointmentDate = ObjAppointmentsVM.appointment.AppointmentDate
                    .AddHours(ObjAppointmentsVM.appointment.AppointmentTime.Hour)
                    .AddMinutes(ObjAppointmentsVM.appointment.AppointmentTime.Minute);
                var appointmentFromDb = _db.Appointments.Where(a => a.Id == ObjAppointmentsVM.appointment.Id).FirstOrDefault();
                appointmentFromDb.CustomerName = ObjAppointmentsVM.appointment.CustomerName;
                appointmentFromDb.CustomerEmail = ObjAppointmentsVM.appointment.CustomerEmail;
                appointmentFromDb.CustomerPhoneNumber = ObjAppointmentsVM.appointment.CustomerPhoneNumber;
                appointmentFromDb.isConfirmed = ObjAppointmentsVM.appointment.isConfirmed;
                appointmentFromDb.AppointmentDate = ObjAppointmentsVM.appointment.AppointmentDate;
                if (User.IsInRole(SD.SuperAdminEndUser))
                {
                    appointmentFromDb.SalesPersonId = ObjAppointmentsVM.appointment.SalesPersonId;
                }
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(ObjAppointmentsVM);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductID
                                                      where a.AppointmentID == id
                                                      select p).Include("ProductTypes");
            AppointmentsDetailsViewModel ObjappointmentsVM = new AppointmentsDetailsViewModel()
            {
                appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUser.ToList(),
                products = productList.ToList()

            };
            return View(ObjappointmentsVM);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductID
                                                      where a.AppointmentID == id
                                                      select p).Include("ProductTypes");
            AppointmentsDetailsViewModel ObjappointmentsVM = new AppointmentsDetailsViewModel()
            {
                appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUser.ToList(),
                products = productList.ToList()

            };
            return View(ObjappointmentsVM);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) 
        {
            var appointment = await _db.Appointments.FindAsync(id);
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        
        } 


    }
}