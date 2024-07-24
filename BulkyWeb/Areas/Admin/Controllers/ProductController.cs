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

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public ActionResult Index()
    {
        var objProductList = _unitOfWork.Product.GetAll().ToList();
        return View(objProductList);
    }

    public IActionResult Create()
    {
        IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
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
        return View(productVm);
    }

    [HttpPost]
    public IActionResult Create(ProductVM obj)
    {
        // if (obj.ListPrice > obj.Price || obj.ListPrice > obj.Price50 || obj.ListPrice > obj.Price100)
        //     ModelState.AddModelError("ListPrice", "List Price cannot be cheaper than other prices");
        if (!ModelState.IsValid)
        {
            obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }
        _unitOfWork.Product.Add(obj.Product);
        _unitOfWork.Save();
        TempData["success"] = "Product created successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0) return NotFound();
        var productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

        if (productFromDb == null) return NotFound();
        return View(productFromDb);
    }

    [HttpPost]
    public IActionResult Edit(Product obj)
    {
        // if (obj.ListPrice > obj.Price || obj.ListPrice > obj.Price50 || obj.ListPrice > obj.Price100)
        //     ModelState.AddModelError("ListPrice", "List Price cannot be cheaper than other prices");
        if (!ModelState.IsValid) return View();
        _unitOfWork.Product.Update(obj);
        _unitOfWork.Save();
        TempData["success"] = "Product updated successfully";
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