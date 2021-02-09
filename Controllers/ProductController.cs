﻿using liftoff_storefront.Data;
using liftoff_storefront.Models;
using liftoff_storefront.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liftoff_storefront.Controllers
{
    public class ProductController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private StorefrontDbContext context;
        public ProductController(StorefrontDbContext dbcontext, UserManager<IdentityUser> usermanager)
        {
            context = dbcontext;
            userManager = usermanager;
        }

        public IActionResult Index()
        {
            List<Product> products = context.Products.ToList();
            return View(products);
        }

        [HttpGet("/product/{id}")]
        public IActionResult ProductPage(int id)
        {
            Product product = context.Products.Find(id);
            ViewBag.desc = new HtmlString(product.Description);
            return View(product);
        }

        [HttpGet("/product/{id}/comments")]
        public IActionResult ViewComments(int id)
        {
            List<UserComment> comments = context.UserComments
                .Where(x => x.ProductId == id)
                .Include(x => x.Product)
                .Include(x => x.IdentityUser)
                .ToList();
            ViewBag.id = id;
            return View(comments);
        }

        [Authorize]
        [HttpGet("/product/{id}/comments/add")]
        public IActionResult AddComment(int id)
        {
            AddCommentViewModel viewmodel = new AddCommentViewModel { ProductId = id };
            return View(viewmodel);
        }

        [Authorize]
        [HttpPost("/product/{id}/comments/add")]
        public IActionResult AddComment(AddCommentViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                UserComment comment = new UserComment();
                comment.Content = viewmodel.Content;
                comment.ProductId = viewmodel.ProductId;
                comment.IdentityUserId = userManager.GetUserId(User);

                context.UserComments.Add(comment);
                context.SaveChanges();
                return Redirect("/home");
            }
            return View("AddComment", viewmodel);
        }
    }
}
