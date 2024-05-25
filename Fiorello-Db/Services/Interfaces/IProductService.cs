using Fiorello_Db.Areas.Admin.ViewModel.Product;
using Fiorello_Db.Models;


namespace Fiorello_Db.Services.Interfaces
{
    public interface IProductService
    { 
        Task<List<Product>> GetAllWithImagesAsync();
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        List<ProductVM> GetMappedDatas(List<Product> products);
        Task<List<Product>> GetAllPaginationAsync(int page, int take=4);
        Task<int> GetCountAsync();
    }
}
