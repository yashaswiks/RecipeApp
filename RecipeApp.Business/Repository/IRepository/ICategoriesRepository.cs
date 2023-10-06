using RecipeApp.DapperDataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface ICategoriesRepository
{
    Task<int?> CreateNewCategoryAsync(string newCategory);

    Task<Categories> GetRecordDetailsByName(string categoryName);

    Task<List<Categories>> GetAll();

    Task<Categories> GetById(int id);
}