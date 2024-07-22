using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db;

    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }

    public ActionResult Index()
    {
        var objCategoryList = _db.Categories.ToList();
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
        _db.Categories.Add(obj);
        _db.SaveChanges();
        TempData["success"] = "Category created successfully";
        // return RedirectToAction("Index","Category");
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0) return NotFound();

        var categoryFromDb = _db.Categories.Find(id);
        // var categoryFromDb = _db.Categories.FirstOrDefault(u => u.Id == id);
        // var categoryFromDb = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

        if (categoryFromDb == null) return NotFound();
        return View(categoryFromDb);
    }

    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (!ModelState.IsValid) return View();
        _db.Categories.Update(obj);
        _db.SaveChanges();
        TempData["success"] = "Category updated successfully";
        // return RedirectToAction("Index","Category");
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int? id)
    {
        if (id is null or 0) return NotFound();

        var categoryFromDb = _db.Categories.Find(id);
        // var categoryFromDb = _db.Categories.FirstOrDefault(u => u.Id == id);
        // var categoryFromDb = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

        if (categoryFromDb == null) return NotFound();
        return View(categoryFromDb);
    }

    [HttpPost]
    [ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var obj = _db.Categories.Find(id);
        if (obj == null) return NotFound();

        _db.Categories.Remove(obj);
        _db.SaveChanges();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}