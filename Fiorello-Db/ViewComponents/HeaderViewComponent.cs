using Fiorello_Db.Services.Interfaces;
using Fiorello_Db.ViewModel;
using Fiorello_Db.ViewModel.Baskets;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Fiorello_Db.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IBioService _bioService;
        private readonly IHttpContextAccessor _accessor;

        public HeaderViewComponent(IBioService bioService, IHttpContextAccessor accessor)
        {
            _bioService = bioService;
            _accessor = accessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string,string> settingDatas = await _bioService.GetAllAsync();
            List<BasketVM> basketProducts = new();

            if (_accessor.HttpContext.Request.Cookies["basket"] is not null)
            {
                basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
          

            HeaderVM response = new()
            {
                Settings = settingDatas,
                BasketCount = basketProducts.Sum(m => m.Count),
                BasketTotalPrice = basketProducts.Sum(m => m.Count*m.Price)
            };
            return await Task.FromResult(View(response));
        }
    }
}
