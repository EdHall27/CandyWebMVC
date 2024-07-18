using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CandyWebMVC.Data;
using CandyWebMVC.Models;
using CandyWebMVC.Models.ViewModel;
using CandyWebMVC.Helper;

namespace CandyWebMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Context _context;

        public ProductsController(Context context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var isAdmin = User.Claims.Any(c => c.Type == "IsAdmin" && c.Value == "true");
            ViewBag.IsAdmin = isAdmin;

            return _context.Products != null ?
                            View(await _context.Products.ToListAsync()) :
                            Problem("Entity set 'Context.Products' is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ImageViewModel imageViewModel)
        {
            if (ModelState.IsValid)
            {
                string imagePath = "";

                if (imageViewModel.ImageFile != null)
                {
                    // Save the image to the server's file system and get the path
                    ImageHelper imageHelper = new();
                    imagePath = imageHelper.SaveImageAndGetPath(imageViewModel.ImageFile);
                }

                var entity = new Products
                {
                    Name = imageViewModel.Name,
                    Price = imageViewModel.Price,
                    Description = imageViewModel.Description,
                    ImagePath = imagePath
                };

                _context.Products.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Or any other action
            }

            return View(imageViewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,Stock")] Products product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productToUpdate = await _context.Products.FindAsync(id);
                    if (productToUpdate == null)
                    {
                        return NotFound();
                    }

                    productToUpdate.Name = product.Name;
                    productToUpdate.Price = product.Price;
                    productToUpdate.Description = product.Description;
                    productToUpdate.Stock = product.Stock;

                    // Handle image upload
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files.FirstOrDefault();
                        if (file != null && file.Length > 0)
                        {
                            ImageHelper imageHelper = new();
                            productToUpdate.ImagePath = imageHelper.SaveImageAndGetPath(file);
                        }
                    }

                    _context.Update(productToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'Context.Products'  is null.");
            }
            var products = await _context.Products.FindAsync(id);
            if (products != null)
            {
                _context.Products.Remove(products);
            }

            // Delete the image file from the server if it exists
            if (!string.IsNullOrEmpty(products?.ImagePath))
            {
                ImageHelper imageHelper = new();
                imageHelper.DeleteImageFile(products.ImagePath);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
