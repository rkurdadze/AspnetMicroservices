﻿using System;
using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
	public class DuscountRepository : IDiscountRepository
	{

        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection connection;

        public DuscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            Coupon coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("select * from Coupon where ProductName = @ProductName", new { ProductName = productName });

            if (coupon == null)
                return new Coupon { ProductName = "No Discount", Description = "No Discount Desc", Amount = 0 };
            return coupon;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var affected = await connection.ExecuteAsync("insert into Coupon (ProductName, Description, Amount) Values (@ProductName, @Description, @Amount)", new { @ProductName = coupon.ProductName, @Description = coupon.Description, @Amount = coupon.Amount });

            if (affected == 0)
                return false;

            return true;

        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var affected = await connection.ExecuteAsync("Update Coupon set ProductName=@ProductName, Description=@Description, Amount=@Amount where Id=@id", new { @ProductName = coupon.ProductName, @Description = coupon.Description, @Amount = coupon.Amount, @Id = coupon.Id });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var affected = await connection.ExecuteAsync("Delete Coupon where ProductName = @ProductName", new { ProductName = productName });

            if (affected == 0)
                return false;

            return true;
        }

        

        
    }
}

