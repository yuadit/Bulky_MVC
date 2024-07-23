using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryController(ICategoryRepository _categoryRepo)
    {
        this._categoryRepo = _categoryRepo;
    }

    public ActionResult Index()
    {
        var objCategoryList = _categoryRepo.GetAll().ToList();
        return View(objCategoryList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
            ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
        if (obj.Name != null && obj.Name.ToLower() == "test") ModelState.AddModelError("", "Test is an invalid value");
        if (!ModelState.IsValid) return View();
        _categoryRepo.Add(obj);
        _categoryRepo.Save();
        TempData["success"] = "Category created successfully";
        // return RedirectToAction("Index","Category");
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0) return NotFound();

        var categoryFromDb = _categoryRepo.Get(u => u.Id == id);
        // var categoryFromDb = _db.Categories.FirstOrDefault(u => u.Id == id);
        // var categoryFromDb = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

        if (categoryFromDb == null) return NotFound();
        return View(categoryFromDb);
    }

    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (!ModelState.IsValid) return View();
        _categoryRepo.Update(obj);
        _categoryRepo.Save();
        TempData["success"] = "Category updated successfully";
        // return RedirectToAction("Index","Category");
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int? id)
    {
        if (id is null or 0) return NotFound();

        var categoryFromDb = _categoryRepo.Get(u=> u.Id == id);
        // var categoryFromDb = _db.Categories.FirstOrDefault(u => u.Id == id);
        // var categoryFromDb = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

        if (categoryFromDb == null) return NotFound();
        return View(categoryFromDb);
    }

    [HttpPost]
    [ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var obj = _categoryRepo.Get(u=> u.Id == id);
        if (obj == null) return NotFound();

        _categoryRepo.Remove(obj);
        _categoryRepo.Save();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}