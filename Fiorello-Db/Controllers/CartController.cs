using Fiorello_Db.Data;
using Fiorello_Db.Models;
using Fiorello_Db.ViewModel;
using Fiorello_Db.ViewModel.Baskets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;

namespace Fiorello_Db.Controllers
{
    public class CartController : Controller
    {
        public readonly AppDbContext _context;
        public readonly IHttpContextAccessor _accessor;

        public CartController(AppDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public async Task<IActionResult> Index()
        {

            List<BasketVM> basketProducts = new();

            if (_accessor.HttpContext.Request.Cookies["basket"] is not null)
            {
                basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
            

            var cartProducts = new List<CartVM>();

            foreach (var basketProduct in basketProducts)
            {
                var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(m => m.Id == basketProduct.Id);

                if (product != null)
                {
                    var mainImage = product.ProductImages.FirstOrDefault(m => m.IsMain == true).Name;


                    cartProducts.Add(new CartVM
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Image = mainImage,
                        Count = basketProduct.Count,
                        Price = basketProduct.Count * basketProduct.Price,
                        BasketTotalPrice = basketProducts.Sum(m => m.Count * m.Price)
                    });
                }
            }

            return View(cartProducts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            List<BasketVM> basketproducts = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            if (basketproducts == null) return NotFound();
            var product = basketproducts.FirstOrDefault(m => m.Id == id);
            basketproducts.Remove(product);
            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketproducts));
            return RedirectToAction(nameof(Index));

        }
        //public async Task<IActionResult> DeleteFromBasket(int? id)
        //{
        //    if (id is null) return BadRequest();


        //    List<BasketVM> basketProducts = new();

        //    if (_accessor.HttpContext.Request.Cookies["basket"] is not null)
        //    {
        //        basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
        //    }

        //    basketProducts = basketProducts.Where(m => m.Id != id).ToList();
        //    _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketProducts));
        //    int count = basketProducts.Sum(m => m.Count);
        //    float total = basketProducts.Sum(m => m.Count * m.Price);
        //    return Ok(new { count, total });



        //}
        [HttpPost]
        public async Task<IActionResult> IncrementCounterProduct(int? id)
        {
            List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            var product = basket.FirstOrDefault(m => m.Id == id);
            product.Count -= 1;
            if (product.Count == 0)
            {
                basket.Remove(product);
            }
            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            float totalPrice = basket.Sum(m => m.Count * m.Price);
            var count = product.Count;
            var price = product.Count * product.Price;

            return Ok(new { count, totalPrice, price });
        }

        [HttpPost]
        public async Task<IActionResult> ReductionCounterProduct(int? id)
        {
            List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            var product = basket.FirstOrDefault(m => m.Id == id);
            product.Count += 1;
            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            float totalPrice = basket.Sum(m => m.Count * m.Price);
            var count = product.Count;
            var price = product.Count * product.Price;
            return Ok(new { count, totalPrice, price });
        }


    }
}

