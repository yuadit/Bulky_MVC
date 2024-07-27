using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        Company = new CompanyRepository(_db);
    }

    public ICompanyRepository Company { get; }


    public ICategoryRepository Category { get; }
    public IProductRepository Product { get; }

    public void Save()
    {
        _db.SaveChanges();
    }
}