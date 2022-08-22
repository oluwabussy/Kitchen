using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modupekitchen.Data;
using Modupekitchen.Models;
using Modupekitchen.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modupekitchen.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnviroment;


        [BindProperty]
        public Coupon_t coupons { get; set; }

        public CouponController(ApplicationDbContext db, IWebHostEnvironment hostingEnviroment)
        {
            _db = db;
            _hostingEnviroment = hostingEnviroment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon_t.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Createpost()
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupons.Picture = p1;
                }

                _db.Coupon_t.Update(coupons);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            //Byte[] Picture = await _db.Coupon_t
            //                     .Where(p => p.Picture == coupons.Picture)
            //                     .Select(p => p.Picture)
            //                     .FirstOrDefaultAsync();

            //return Ok(Convert.ToBase64String(Picture));

            var couponFromDB = await _db.Coupon_t.FirstOrDefaultAsync(m => m.Id == id);
            if (couponFromDB == null)
            {
                return NotFound();
            }

            


            return View(couponFromDB);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editpost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var couponFromDB = await _db.Coupon_t.Where(m=>m.Id ==id ).SingleOrDefaultAsync();

            if (couponFromDB == null)
            {
                return NotFound();
            }

            var files = HttpContext.Request.Form.Files;
            if (files.Count() > 0)
            {
                byte[] p1 = null;
                using (var fs1 = files[0].OpenReadStream())
                {
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                }
                coupons.Picture = p1;
            }


              couponFromDB.Picture =coupons.Picture;
              couponFromDB.Name = coupons.Name;
              couponFromDB.CouponType = coupons.CouponType;
              couponFromDB.Discount = coupons.Discount;
              couponFromDB.IsActive = coupons.IsActive;
              couponFromDB.MinimumAmount = coupons.MinimumAmount;

            await _db.SaveChangesAsync();
            


            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponFromDB = await _db.Coupon_t.FirstOrDefaultAsync(m => m.Id == id);
            if (couponFromDB == null)
            {
                return NotFound();
            }

            return View(couponFromDB);
        }
        
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponFromDB = await _db.Coupon_t.FirstOrDefaultAsync(m => m.Id == id);
            if (couponFromDB == null)
            {
                return NotFound();
            }

            return View(couponFromDB);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponFromDB = await _db.Coupon_t.Where(m => m.Id == id).SingleOrDefaultAsync();
            if (couponFromDB != null)
            {
                if (ModelState.IsValid)
                {
                    _db.Remove(couponFromDB);

                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            return View();

        }

    }
}
