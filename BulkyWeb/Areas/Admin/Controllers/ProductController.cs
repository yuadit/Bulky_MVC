using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
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
        var objProductList = _unitOfWork.Product.GetAll("Category").ToList();
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
        productVm.Product = _unitOfWork.Product.Get(u => u.Id == id);
        return View(productVm);
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, IFormFile? file)
    {
        // if (obj.ListPrice > obj.Price || obj.ListPrice > obj.Price50 || obj.ListPrice > obj.Price100)
        //     ModelState.AddModelError("ListPrice", "List Price cannot be cheaper than other prices");
        if (!ModelState.IsValid)
        {
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(productVM);
        }

        var wwwRootPath = _webHostEnvironment.WebRootPath;
        if (file != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var productPath = Path.Combine(wwwRootPath, @"images\product");

            if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
            {
                //delete the old image
                var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
            }

            using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            productVM.Product.ImageUrl = @"\images\product\" + fileName;
        }
        else
        {
            if (productVM.Product.Id == 0) productVM.Product.ImageUrl = "";
        }

        if (productVM.Product.Id == 0)
            _unitOfWork.Product.Add(productVM.Product);
        else
            _unitOfWork.Product.Update(productVM.Product);

        _unitOfWork.Save();
        TempData["success"] = "Product created successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int? id)
    {
        if (id is null or 0) return NotFound();
        var productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        if (productFromDb == null) return NotFound();
        return View(productFromDb);
    }

    [HttpPost]
    [ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var obj = _unitOfWork.Product.Get(u => u.Id == id);
        if (obj == null) return NotFound();

        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "Product deleted successfully";
        return RedirectToAction("Index");
    }
}