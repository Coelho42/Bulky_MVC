using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    // Defines the authorization so the user can't acess the data pages with the url
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            // Custom validations
            if (obj.Name.Equals(obj.DisplayOrder.ToString()))
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the name");
            }
            if (obj.Name.ToLower().Equals("test"))
            {
                ModelState.AddModelError("name", "Test is an invalid value");
            }
            // Verifies if the obj provided is valid and checks for the model validation
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj); // Keeps track of what needs to be added
                _unitOfWork.Save(); // Saves the obj into the database
                TempData["success"] = "Category created successfully"; // TempData used to show a message after a CRUD operation is sucessful
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // Some ways of get data with the id when editing 
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromD1 = _db.Categories.FirstOrDefault(u => u.Id == id);
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            // Verifies if the obj provided is valid and checks for the model validation
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj); // Keeps track of what needs to be added
                _unitOfWork.Save(); // Saves the obj into the database
                TempData["success"] = "Category created successfully"; TempData["success"] = "Category updated successfully"; // TempData used to show a message after a CRUD operation is sucessful
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // Some ways of get data with the id when editing 
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromD1 = _db.Categories.FirstOrDefault(u => u.Id == id);
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj); // Keeps track of what needs to be deleted
            _unitOfWork.Save(); // Deletes the obj from the database
            TempData["success"] = "Category created successfully"; TempData["success"] = "Category deleted successfully"; // TempData used to show a message after a CRUD operation is sucessful
            return RedirectToAction("Index", "Category");
        }
    }
}
