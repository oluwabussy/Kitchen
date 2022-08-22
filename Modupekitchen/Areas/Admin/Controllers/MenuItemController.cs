using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modupekitchen.Data;
using Modupekitchen.Models.ViewModel;
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
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnviroment;        

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }


        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnviroment)
        {
            _db = db;
            _hostingEnviroment = hostingEnviroment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category,
                MenuItem = new Models.MenuItem()
            };
        }
        public async Task<IActionResult> Index()
        {
            var MenuItems = await _db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync();
            return View(MenuItems);
        }


        //GET - CREATE 
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }


        //POST CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();
            // work on the image saving section

            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDB = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if(files.Count() > 0)
            {
                // file was uploaded

                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using(var filesStream = new FileStream(Path.Combine(uploads,MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDB.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                // file not uploaded, so use default image
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDB.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - EDIT 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }


        //POST EDIT
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

           
            // work on the image saving section

            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDB = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count() > 0)
            {
                // new image has been uploaded

                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagepath = Path.Combine(webRootPath, menuItemFromDB.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDB.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }
            menuItemFromDB.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDB.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDB.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDB.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFromDB.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDB.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;


            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }
        
        
        
        
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            _db.MenuItem.Remove(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
