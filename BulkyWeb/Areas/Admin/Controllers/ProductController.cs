using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();   
            return View(objProductList);
        }

        public IActionResult Create()
        {
            // Key value
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM obj)
        {
            // Verifies if the obj provided is valid and checks for the model validation
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj.Product); // Keeps track of what needs to be added
                _unitOfWork.Save(); // Saves the obj into the database
                TempData["success"] = "Product created successfully"; // TempData used to show a message after a CRUD operation is sucessful
                return RedirectToAction("Index", "Product");
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
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? productFromDb = _db.Products.FirstOrDefault(u => u.Id == id);
            //Product? productFromDb = _db.Products.Where(u => u.Id == id).FirstOrDefault();
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            // Verifies if the obj provided is valid and checks for the model validation
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj); // Keeps track of what needs to be added
                _unitOfWork.Save(); // Saves the obj into the database
                TempData["success"] = "Product created successfully"; TempData["success"] = "Product updated successfully"; // TempData used to show a message after a CRUD operation is sucessful
                return RedirectToAction("Index", "Product");
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
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? productFromDb = _db.Products.FirstOrDefault(u => u.Id == id);
            //Product? productFromDb = _db.Products.Where(u => u.Id == id).FirstOrDefault();
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj); // Keeps track of what needs to be deleted
            _unitOfWork.Save(); // Deletes the obj from the database
            TempData["success"] = "Product created successfully"; TempData["success"] = "Product deleted successfully"; // TempData used to show a message after a CRUD operation is sucessful
            return RedirectToAction("Index", "Product");
        }
    }
}
