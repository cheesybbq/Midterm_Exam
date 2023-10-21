using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Midterm_Exam.Models;

namespace Midterm_Exam.Controllers
{
    [RoutePrefix("Products")]
    public class ProductsController : ApiController
    {
        private MidtermEntities db = new MidtermEntities();

        [Route("GetProducts")]
        // GET: api/Products
        public IQueryable<Product> GetProduct()
        {
            return db.Product;
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        //GET By Product Code, returns a single product 
        [Route("Find/{productcode}/ProductCode")]
        public IHttpActionResult GetProductByProductCode(string productcode)
        {
            var products = db.Product.Where(m => m.ProductName.Contains(productcode));
            return Ok(products);
        }

        //GET By Category Code, returns a list of products 
        [Route("Find/{categorycode}/ProductName")]
        public IHttpActionResult GetProductByCategoryCode(string categorycode)
        {
            var products = db.Category.Where(m => m.CategoryCode.Contains(categorycode));
            return Ok(products);
        }

        //PUT Update by Product Code  
        [Route("{code}")]
        [Route("Edit/{code}")]
        public IHttpActionResult UpdateByProductCode(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);

        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Product.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
        }

        //POST Add new Product for existing Category
        [Route("")]
        [Route("Category/{category_id}/products")]
        public string AddProductForExistingCategory(Product product)
        {
            var newproduct = new Product
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Color = product.Color,
                Size = product.Size,
                Price = product.Price
            };
            db.Product.Add(newproduct);
            db.SaveChanges();
            return "Product Added!";
        }

        //POST Add new Product and a new Category at the same time  
        [Route("Category/{category_id}/Product/{product_id}")]
        public string AddNewProductAndNewCategory(Product product, Category category)
        {
            var newcategory = new Category
            {
                CategoryId = category.CategoryId,
                CategoryCode = category.CategoryCode,
            };
            db.Category.Add(newcategory);
            db.SaveChanges();

            var newproduct = new Product
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Color = product.Color,
                Size = product.Size,
                Price = product.Price
            };
            db.Product.Add(newproduct);
            db.SaveChanges();
            return "Product Added!";
        }

        // DELETE by Product Code
        [Route("{code}")]
        [Route("Delete/{code}")]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Product.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Product.Count(e => e.ProductId == id) > 0;
        }
    }
}