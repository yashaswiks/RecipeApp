using RecipeApp.DapperDataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Business.Repository.IRepository;

public interface ICategoriesRepository
{
    Task<int?> CreateNewCategoryAsync(string newCategory);

    Task<Categories> GetRecordDetailsByNameAsync(string categoryName);

    Task<List<Categories>> GetAllAsync();

    Task<Categories> GetByIdAsync(int id);
}