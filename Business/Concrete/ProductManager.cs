using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IProductDal _productDal;
        //private readonly ICategoryService _categoryService;

        public ProductManager(IProductDal productDal, ICategoryService categoryService)
        {
            _productDal = productDal;
            //_categoryService = categoryService;

        }

        [ValidationAspect(typeof(ProductValidator))]
        public IResult Add(Product product)
        {
            //Aynı isimde ürün eklenemez
            //Eğer mevcut kategori sayısı 15'i geçtiyse sisteme yeni ürün eklenemez. ve 
            IResult result = BusinessRules.Run(CheckIfProductNameExists(product.ProductName),
                CheckIfProductCountOfCategoryCorrect(product.CategoryId));

            if (result != null)
            {
                return result;
            }

            _productDal.Add(product);

            return new SuccessResult(Messages.ProductAdded);



            //23:10 Dersteyiz
        }


        public IDataResult<List<Product>> GetAll()
        {
            var data = _productDal.GetAll();
            return new SuccessDataResult<List<Product>>(data, "basarili");
        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            var data = _productDal.GetAll(p => p.CategoryId == id);
            return new SuccessDataResult<List<Product>>(data, "basarili");
        }

        public IDataResult<Product> GetById(int productId)
        {
            var data = _productDal.Get(p => p.ProductId == productId);
            return new SuccessDataResult<Product>(data, "basarili");
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            var data = _productDal.GetAll(p => p.UnitPrice <= max && p.UnitPrice >= min);
            return new SuccessDataResult<List<Product>>(data, "basarili");
        }

        //public IDataResult<List<ProductDto>> GetProductDetails()
        //{
        //    return new SuccessDataResult<List<ProductDto>>(_productDal.GetProductDetails());
        //}

        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
        {
            //Select count(*) from products where categoryId=1
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;
            if (result >= 15)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }

        private IResult CheckIfProductNameExists(string productName)
        {
            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }

        //private IResult CheckIfCategoryLimitExceded()
        //{
        //    var result = _categoryService.GetAll();
        //    if (result.Data.Count > 15)
        //    {
        //        return new ErrorResult(Messages.CategoryLimitExceded);
        //    }

        //    return new SuccessResult();
        //}
    }
}
