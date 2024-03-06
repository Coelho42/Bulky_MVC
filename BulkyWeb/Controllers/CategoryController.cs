using Bulky.DataAccess.Data;
using Bulky.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bulky.DataAccess.Migrations
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj); // Keeps track of what needs to be added
                _db.SaveChanges(); // Saves the obj into the database
                TempData["success"] = "Category created successfully"; // TempData used to show a message after a CRUD operation is sucessful
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0) 
            {
                return NotFound();
            }
            // Some ways of get data with the id when editing 
            Category? categoryFromDb = _db.Categories.Find(id);
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
                _db.Categories.Update(obj); // Keeps track of what needs to be added
                _db.SaveChanges(); // Saves the obj into the database
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
            Category? categoryFromDb = _db.Categories.Find(id);
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
            Category obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj); // Keeps track of what needs to be deleted
            _db.SaveChanges(); // Deletes the obj from the database
            TempData["success"] = "Category created successfully"; TempData["success"] = "Category deleted successfully"; // TempData used to show a message after a CRUD operation is sucessful
            return RedirectToAction("Index", "Category");
        }
    }
}
