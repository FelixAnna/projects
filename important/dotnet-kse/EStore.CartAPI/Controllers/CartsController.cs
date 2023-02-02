﻿using EStore.Common.Models;
using EStore.SharedServices.Carts.Contracts;
using EStore.SharedServices.Carts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EStore.ProductAPI.Controllers
{
    [Authorize(Policy = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartsService cartService;

        public CartsController(ICartsService cartService)
        {
            this.cartService = cartService;
        }

        [HttpGet]
        public async Task<GetCartResponse> GetAsync()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value!;
            var cart = await cartService.GetAsync(userId);
            return cart;
        }

        [HttpPost("{cartId}/items/add")]
        public async Task<int> AddProductsAsync(int cartId, [FromBody] CartItemModel[] cartItems)
        {
            if (!await cartService.ExistsAsync(cartId))
            {
                return 0;
            }

            var count = await cartService.AddProductsAsync(cartId, cartItems);
            return count;
        }

        [HttpPost("{cartId}/items/remove")]
        public async Task<int> RemoveProductsAsync(int cartId,[FromBody] Guid[] cartItemIds)
        {
            if (!await cartService.ExistsAsync(cartId))
            {
                return 0;
            }

            var count = await cartService.RemoveProductsAsync(cartId, cartItemIds);
            return count;
        }

        [HttpPost("{cartId}/items/update")]
        public async Task<bool> UpdateCartItemAsync(int cartId, CartItemModel cartItem)
        {
            if (!await cartService.ExistsAsync(cartId))
            {
                return false;
            }

            var success = await cartService.UpdateCartItemAsync(cartItem);
            return success;
        }
    }
}
