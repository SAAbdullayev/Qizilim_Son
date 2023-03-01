using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qizilim.az.Models.DataContext;
using Qizilim.az.Models.Entities.Membreship;
using Qizilim.az.Models.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Qizilim.az.Models.Entities.Products;

namespace Qizilim.az.Controllers
{
    
    public class HomePageController : Controller
    {
        private readonly QizilimDbContext db;
        private readonly IMediator mediator;
        private readonly UserManager<QizilimUser> userManager; 
        public HomePageController(QizilimDbContext db,
            UserManager<QizilimUser> userManager,
            IMediator mediator)
        {
            this.db = db;
            this.mediator = mediator;
            this.userManager = userManager;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name != null)
            {
                var userAbout = await userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.User = userAbout;

                if (userAbout.Status == null)
                {
                    return RedirectToAction("verifyRegister", "account");
                }
                else if (userAbout.Status == false)
                {
                    return RedirectToAction("rejectedRegister", "account");
                }
            }




            var model = new MainViewModel();
            model.Products = await db.Products
                .Where(cp => cp.DeletedById == null)
                .Include(cp => cp.ProductImages)
                .ThenInclude(cp => cp.Image)
                .Include(cp => cp.ProductEyar)
                .ThenInclude(cp => cp.Eyar)
                .Include(cp => cp.ProductColor)
                .ThenInclude(cp => cp.Color).ToListAsync();
            model.QizilimUser = db.Users
               .Where(cp => cp.shopName != null).ToList();
            model.Advertisement = db.Advertisement
                .Where(cp => cp.DeletedById == null).ToList();
            return View(model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> VipProducts()
        {
            if (User.Identity.Name != null)
            {
                var userAbout = await userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.User = userAbout;

                if (userAbout.Status == null)
                {
                    return RedirectToAction("verifyRegister", "account");
                }
                else if (userAbout.Status == false)
                {
                    return RedirectToAction("rejectedRegister", "account");
                }
            }

            var model = new MainViewModel();
            model.Products = await db.Products
               .Where(cp => cp.DeletedById == null && cp.PremiumProduct == true)
               .Include(cp => cp.ProductImages)
               .ThenInclude(cp => cp.Image)
               .Include(cp => cp.ProductEyar)
               .ThenInclude(cp => cp.Eyar)
               .Include(cp => cp.ProductColor)
               .ThenInclude(cp => cp.Color).ToListAsync();
            model.QizilimUser = db.Users
               .Where(cp => cp.shopName != null).ToList();
            model.Advertisement = db.Advertisement
                .Where(cp => cp.DeletedById == null).ToList();

            return View(model);
        }

        public async Task<IActionResult> Liked()
        {
            var userAbout = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.User = userAbout;


            if (userAbout.Status == null)
            {
                return RedirectToAction("verifyRegister", "account");
            }
            else if (userAbout.Status == false)
            {
                return RedirectToAction("rejectedRegister", "account");
            }

            var likedProducts = db.LikedProducts.Where(lp => lp.UserId == userAbout.Id).ToList();
            ViewBag.LikedProducts = likedProducts;
            
            var model = new MainViewModel();
            model.Products = await db.Products
               .Where(cp => cp.DeletedById == null)
               .Include(cp => cp.ProductImages)
               .ThenInclude(cp => cp.Image)
               .Include(cp => cp.ProductEyar)
               .ThenInclude(cp => cp.Eyar)
               .Include(cp => cp.ProductColor)
               .ThenInclude(cp => cp.Color)
               .Include(cp=>cp.LikedProducts).ToListAsync();
            model.QizilimUser = db.Users
               .Where(cp => cp.shopName != null).ToList();
            model.Advertisement = db.Advertisement
                .Where(cp => cp.DeletedById == null).ToList();
            return View(model);
        }


        public async Task<IActionResult> toLike(int productId)
        {
            var userAbout = await userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.User = userAbout;


            if (userAbout.Status == null)
            {
                return RedirectToAction("verifyRegister", "account");
            }
            else if (userAbout.Status == false)
            {
                return RedirectToAction("rejectedRegister", "account");
            }

            var likedProducts = db.LikedProducts.Where(lp => lp.UserId == userAbout.Id).ToList();

            foreach (var item in likedProducts)
            {
                if (item.ProductId == productId)
                {
                    var productliked = db.LikedProducts.FirstOrDefault(lp => lp.ProductId == productId);
                    db.LikedProducts.Remove(productliked);
                    await db.SaveChangesAsync();

                    return Json(new { status = 200 });
                }
            }

            await db.LikedProducts.AddAsync(new LikedProduct
            {
                UserId = userAbout.Id,
                ProductId = productId
            });

            await db.SaveChangesAsync();

            return Json(new { status = 200 });
        }

        
        [AllowAnonymous]
        public async Task<IActionResult> searchProduct(string? selectedTitle,
            bool? selectedDiamond, 
            string? selectedLoc, 
            bool? selectedDelivery,
            double[]? selectedColors, 
            double[]? selectedEyar,
            double? minWeight,
            double? maxWeight,
            double? minPrice,
            double? maxPrice)
        {
            if (User.Identity.Name != null)
            {
                var userAbout = await userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.User = userAbout;

                if (userAbout.Status == null)
                {
                    return RedirectToAction("verifyRegister", "account");
                }
                else if (userAbout.Status == false)
                {
                    return RedirectToAction("rejectedRegister", "account");
                }
            }
            var productFilter = await db.Products.Where(p => p.DeletedById == null).ToListAsync();
            var product = await db.Products.Where(p => p.DeletedById == null).ToListAsync();


            if (selectedTitle != null && selectedTitle != "Nov")
                productFilter = productFilter.Where(pf => pf.Nov == selectedTitle).ToList();

            if (selectedDiamond != null)
                productFilter = productFilter.Where(p => p.HasDiamond == selectedDiamond).ToList();

            if (selectedDelivery != null)
                productFilter = productFilter.Where(p => p.Delivery == selectedDelivery).ToList();

            if (minPrice != null)
                productFilter = productFilter.Where(p => p.Price >= minPrice).ToList();

            if (maxPrice != null)
                productFilter = productFilter.Where(p => p.Price <= maxPrice).ToList();

            if (minWeight != null)
                productFilter = productFilter.Where(p => p.Weight >= minWeight).ToList();

            if (maxWeight != null)
                productFilter = productFilter.Where(p => p.Weight <= maxWeight).ToList();

            if (selectedLoc != null && selectedTitle != "Ticarət Mərkəzi")
            {
                var users = db.Users.Where(u => u.shopLocation == selectedLoc).ToList();

                foreach (var item in users)
                {
                    productFilter = productFilter.Where(p => p.CreatedById <= item.Id).ToList();
                }
            }

            ViewBag.Shops = await db.Users.Where(u => u.shopName != null).ToListAsync();


            return View(productFilter);
        }

    }
}
