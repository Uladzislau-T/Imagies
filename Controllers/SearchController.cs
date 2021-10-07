using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_commerceFirstFull.Models;
using E_commerceFirstFull.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using E_commerceFirstFull.Models.ViewModels;


namespace E_commerceFirstFull.Controllers
{
    public class SearchController : Controller
    {
        private IProductRepository<Product> repository;
        private ProductService productService;
        private int totalItems = 0;

        public SearchController(IProductRepository<Product> productRepository, ProductService productService)
        {
            this.productService = productService;
            this.repository = productRepository;
        }        

        public IActionResult Index(string searchQuery, RequestParameters request)
        {
            productService.SearchQueryCached = searchQuery;
            var selectedProducts = repository.GetByTitle(searchQuery);

            if (productService.SelectedCategoriesCached.Count == 0)
                totalItems = selectedProducts.Count();

            selectedProducts = selectedProducts.Where(x => productService.SelectedCategoriesCached
                                               .All(x.Features.Contains));

            if (productService.SelectedCategoriesCached.Count != 0)
                totalItems = selectedProducts.Count();

            selectedProducts = selectedProducts.Skip((request.PageNumber - 1) * request.PageSize)
                                               .Take(request.PageSize);

            productService.SortProduct(ref selectedProducts, request.SortOrder);

            ProductModel productModel = new()
            {
                CurrentPage = request.PageNumber,
                TotalItems = totalItems,
                SortMethod = productService.SortObjectCached,
                Products = selectedProducts.ToList()
            };

            return View(productModel);
        }

        public IActionResult FilterProducts(RequestParameters request)
        {
            var selectedProducts = repository.GetByTitle(productService.SearchQueryCached);            

            productService.GetListOfCategories(request.Genre, request.Features, request.Platform);

            if (productService.SelectedCategoriesCached.Count == 0)
                totalItems = selectedProducts.Count();

            selectedProducts = selectedProducts.Where(x => productService.SelectedCategoriesCached
                                               .All(x.Features.Contains));

            if (productService.SelectedCategoriesCached.Count != 0)
                totalItems = selectedProducts.Count();

            selectedProducts = selectedProducts.Skip((request.PageNumber - 1) * request.PageSize)
                                               .Take(request.PageSize);

            productService.SortProduct(ref selectedProducts, request.SortOrder);

            ProductModel productModel = new()
            {
                CurrentPage = request.PageNumber,
                TotalItems = totalItems,
                SortMethod = productService.SortObjectCached,
                Products = selectedProducts.ToList()
            };

            return View("Index", productModel);
        }

        public IActionResult ProductPage()
        {
            return View();
        }
    }
}
