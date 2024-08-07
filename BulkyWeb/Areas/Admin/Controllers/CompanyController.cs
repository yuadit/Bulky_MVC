using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public ActionResult Index()
    {
        var objCompanyList = _unitOfWork.Company.GetAll().ToList();
        return View(objCompanyList);
    }

    public IActionResult Upsert(int? id)
    {
        var company = new Company();
        if (id == null || id == 0) return View(company);

        //update
        company = _unitOfWork.Company.Get(u => u.Id == id);
        return View(company);
    }

    [HttpPost]
    public IActionResult Upsert(Company company)
    {
        if (!ModelState.IsValid) return View(company);

        if (company.Id == 0)
            _unitOfWork.Company.Add(company);
        else
            _unitOfWork.Company.Update(company);

        _unitOfWork.Save();
        return RedirectToAction("Index");
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(int id)
    {
        var objCompanyList = _unitOfWork.Company.GetAll();
        return Json(new { data = objCompanyList });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
        if (companyToBeDeleted == null) return Json(new { success = false, message = "Error while deleting" });

        _unitOfWork.Company.Remove(companyToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}