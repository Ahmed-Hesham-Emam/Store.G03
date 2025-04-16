﻿using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Services.Abstractions;
using Services.Specifications;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
    {
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
        {

        #region Get All Products
        public async Task<PaginationResponse<ProductResultDto>> GetAllProductsAsync(ProductSpecificationsParameters productSpecsParams)
            {
            var spec = new ProductWithBrandsAndTypesSpecifications(productSpecsParams);



            var products = await unitOfWork.GetRepository<Product, int>().GetAllAsync(spec);

            var specCount = new ProductWithCountSpecifications(productSpecsParams);

            var count = await unitOfWork.GetRepository<Product, int>().CountAsync(specCount);

            var result = mapper.Map<IEnumerable<ProductResultDto>>(products);
            return new PaginationResponse<ProductResultDto>(productSpecsParams.PageIndex, productSpecsParams.PageSize, count, result);
            }
        #endregion

        #region Get Product By Id
        public async Task<ProductResultDto?> GetProductByIdAsync(int id)
            {
            var spec = new ProductWithBrandsAndTypesSpecifications(id);

            var product = await unitOfWork.GetRepository<Product, int>().GetAsync(spec);

            if ( product is null ) return null;

            var result = mapper.Map<ProductResultDto>(product);
            return result;
            }
        #endregion

        #region Get All Brands
        public async Task<IEnumerable<BrandResultDto>> GetAllBrandsAsync()
            {
            var brands = await unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();
            var result = mapper.Map<IEnumerable<BrandResultDto>>(brands);
            return result;
            }
        #endregion

        #region Get All Types
        public async Task<IEnumerable<TypeResultDto>> GetAllTypesAsync()
            {
            var types = await unitOfWork.GetRepository<ProductType, int>().GetAllAsync();
            var result = mapper.Map<IEnumerable<TypeResultDto>>(types);
            return result;
            }
        #endregion


        }
    }
