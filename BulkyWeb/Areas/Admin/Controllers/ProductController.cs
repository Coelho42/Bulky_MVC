using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    // Defines the authorization so the user can't acess the data pages with the url
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();   
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
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
            if(id == null || id == 0)
            {
                // Create
                return View(productVM);
            }
            else
            {
                // Update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"ProductImages");
                return View(productVM);
            }
        }
        [HttpPost]
        // IForm file for file upload
        public IActionResult Upsert(ProductVM obj, List<IFormFile> files)
        {
            // Verifies if the obj provided is valid and checks for the model validation
            if (ModelState.IsValid)
            {
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }
                _unitOfWork.Save(); // Saves the obj into the database

                // Code to add an image to the wwwroot folder
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + obj.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if(!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageURL = @"\" + productPath + @"\" + fileName,
                            ProductId = obj.Product.Id,
                        };
                        if(obj.Product.ProductImages == null)
                        {
                            obj.Product.ProductImages = new List<ProductImage>();
                      
                        }
                        obj.Product.ProductImages.Add(productImage);
                    }
                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();
                }
                TempData["success"] = "Product created/updated successfully"; // TempData used to show a message after a CRUD operation is sucessful
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                        Text = u.Name,
                        Value = u.Id.ToString()
                });
                return View(obj);
            }
        }

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if(imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageURL))
                {
                    // delete the old image
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        imageToBeDeleted.ImageURL.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }
            return RedirectToAction(nameof(Upsert), new {id = productId});
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return Json(new {data = objProductList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productTobeDeleted = _unitOfWork.Product.Get(u=> u.Id == id);
            if(productTobeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (!Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }

            _unitOfWork.Product.Remove(productTobeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
