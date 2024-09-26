using Microsoft.AspNetCore.Mvc;
using Shopping.Web.Services.IService;

namespace Shopping.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICatalogService _catalogService;

        public ProductController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _catalogService.GetAllProductAsync();
            return View(products);
        }
    }
}
