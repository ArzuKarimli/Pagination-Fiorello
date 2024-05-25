
using Fiorello_Db.Data;
using Fiorello_Db.Models;
using Fiorello_Db.Services.Interfaces;
using Fiorello_Db.ViewModel;
using Fiorello_Db.ViewModel.Baskets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Fiorello_Db.Controllers
{
    public class HomeController : Controller
    {
       private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICarouselService _carouselService;
        private readonly IHttpContextAccessor _accessor;
        public HomeController( AppDbContext context,IProductService productService,ICategoryService categoryService,ICarouselService carouselService,IHttpContextAccessor accessor)
        {
            _context = context;
             _productService=productService;
            _categoryService=categoryService;
            _carouselService=carouselService;
            _accessor = accessor;
        }

        public async Task<IActionResult> Index()
        {
           
            List<Category> categories= await _categoryService.GetAllAsync();
            List<Product> products= await _productService.GetAllWithImagesAsync();
            List<Blog> blogs= await _context.Blogs.Where(m=>!m.SoftDeleted).Take(3).ToListAsync();
            List<CarouselImage> carouselImages = await _carouselService.GetAllAsync();

           

            HomeVM model = new()
            {
              Blogs = blogs,
              Products = products,
              Categories= categories,
              CarouselImages= carouselImages
            };
           
            
             return View(model);
        }

        [HttpPost]
     
        public async Task<IActionResult> AddProductToBasket(int? id)
        {
            if (id is null) return BadRequest();

            List<BasketVM> basketProducts = null;

            if (_accessor.HttpContext.Request.Cookies["basket"] is not null)
            {
                basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
            else
            {
                basketProducts= new List<BasketVM>();
            }

            var dbProduct = await _context.Products.FirstOrDefaultAsync(m=>m.Id==(int)id);


            var existProduct=basketProducts.FirstOrDefault(m => m.Id ==(int)id);
            if(existProduct is not null)
            {
                existProduct.Count++;
            }
            else
            {
                basketProducts.Add(new BasketVM
                {
                    Id = (int)id,
                    Count = 1,
                    Price=dbProduct.Price
                    
                });
            }
          


            _accessor.HttpContext.Response.Cookies.Append("basket",JsonConvert.SerializeObject(basketProducts));

            int count=basketProducts.Sum(m => m.Count);
            float total=basketProducts.Sum(m=>m.Count * m.Price);

            return Ok(new {count,total});

        }
    }
}