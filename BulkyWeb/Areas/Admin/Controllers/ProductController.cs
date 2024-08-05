using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
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

    public ActionResult Index()
    {
        var objProductList = _unitOfWork.Product.GetAll(null,"Category").ToList();
        return View(objProductList);
    }

    public IActionResult Upsert(int? id)
    {
        var CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });
        // ViewBag.CategoryList = CategoryList;
        // ViewData["CategoryList"] = CategoryList;
        ProductVM productVm = new()
        {
            CategoryList = CategoryList,
            Product = new Product()
        };
        if (id == null || id == 0) return View(productVm);

        //update
        productVm.Product = _unitOfWork.Product.Get(u=>u.Id==id,includeProperties:"ProductImages");
        return View(productVm);
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
    {
        if (ModelState.IsValid)
        {
            if (productVM.Product.Id == 0)
                _unitOfWork.Product.Add(productVM.Product);
            else
                _unitOfWork.Product.Update(productVM.Product);

            _unitOfWork.Save();


            var wwwRootPath = _webHostEnvironment.WebRootPath;
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var productPath = @"images\products\product-" + productVM.Product.Id;
                    var finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductImage productImage = new()
                    {
                        ImageUrl = @"\" + productPath + @"\" + fileName,
                        ProductId = productVM.Product.Id
                    };

                    if (productVM.Product.ProductImages == null)
                        productVM.Product.ProductImages = new List<ProductImage>();

                    productVM.Product.ProductImages.Add(productImage);
                }

                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
            }


            TempData["success"] = "Product created/updated successfully";
            return RedirectToAction("Index");
        }

        productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });
        return View(productVM);
    }
    
    public IActionResult DeleteImage(int imageId) {
        var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
        int productId = imageToBeDeleted.ProductId;
        if (imageToBeDeleted != null) {
            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl)) {
                var oldImagePath =
                    Path.Combine(_webHostEnvironment.WebRootPath,
                        imageToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath)) {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.ProductImage.Remove(imageToBeDeleted);
            _unitOfWork.Save();

            TempData["success"] = "Deleted successfully";
        }

        return RedirectToAction(nameof(Upsert), new { id = productId });
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(int id)
    {
        var objProductList = _unitOfWork.Product.GetAll(null,"Category").ToList();
        return Json(new { data = objProductList });
    }
    
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var productToBeDeleted = _unitOfWork.Product.Get(u=> u.Id == id);
        if (productToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        
        // var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
        // if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
        
        _unitOfWork.Product.Remove(productToBeDeleted);
        _unitOfWork.Save();
        
        return Json(new { success = true, message = "Delete Successful" });
    }
    #endregion
}