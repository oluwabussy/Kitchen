using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Modupekitchen.Data;
using Modupekitchen.Models;
using Modupekitchen.Models.ViewModel;
using Modupekitchen.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modupekitchen.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }


        //get index

        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }

        //get Create

        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                //assigned the property values
                CategoryList = await _db.Category.ToListAsync(), // fetch all Categories in DB
                SubCategory = new Models.SubCategory(), // use to create new SubCategory
                //retreving the list of a string only
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
            };
            return View(model);
        }

        //post - Create

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubcategoryExist = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubcategoryExist.Count() > 0)
                {
                    //error

                    StatusMessage = "Error: sub category exist under " + doesSubcategoryExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage= StatusMessage
            };
            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> sub_Categories = new List<SubCategory>();
            sub_Categories = await (from subCategory in _db.SubCategory
                                    where subCategory.CategoryId == id
                              select subCategory).ToListAsync();

            return Json(new SelectList(sub_Categories, "Id", "Name"));
        }






        //GET EDIT

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                //assigned the property values
                CategoryList = await _db.Category.ToListAsync(), // fetch all Categories in DB
                SubCategory = subCategory,
                //retreving the list of a string only
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
            };
            return View(model);
        }

        //post - Create

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubcategoryExist = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubcategoryExist.Count() > 0)
                {
                    //error

                    StatusMessage = "Error: sub category exist under " + doesSubcategoryExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    var subCatFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCatFromDb.Name = model.SubCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            modelVM.SubCategory.Id = id;
            return View(modelVM);
        }

        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.SubCategory.FindAsync(id);
            if (category == null)
            {
                return NotFound();

            }
            return View(category);
        }



        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subcategory = await _db.SubCategory.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();

            }
            return View(subcategory);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int? id)
        {

            var subcategory = await _db.SubCategory.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }
            _db.SubCategory.Remove(subcategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
