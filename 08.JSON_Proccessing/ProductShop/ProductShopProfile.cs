using AutoMapper;
using ProductShop.Models;
using ProductShop.Models.DataTransferObjects;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserInputModel, User>();
            this.CreateMap<ProductInputModel, Product>();
        }
    }
}
