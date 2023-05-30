using Core.DataAccess;
using Core.DataAccess.EntityFramework;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfProductDal : EfGenericRepository<Product, Context>, IProductDal
    {

        public List<ProductDto> GetProductDetails()
        {
            using (Context context = new Context())
            {
                var query = from p in context.Products
                            join c in context.Categories
                            on p.CategoryId equals c.CategoryId
                            select new ProductDto
                            {
                                ProductId = p.ProductId,
                                CategoryName = c.CategoryName,
                                UnitsInStock = p.UnitsInStock,
                                ProductName = p.ProductName
                            };

                return query.ToList(); 
            }
        }
    }
}
