﻿using Microsoft.EntityFrameworkCore;
using PaperTradingApi.Data;
using PaperTradingApi.Models;

namespace PaperTradingApi.Entities
{
    public class UsersRepository:IUsersRepository
    {
        private readonly PersonDbContext _db;
        public UsersRepository(PersonDbContext db)
        {
            _db = db;
        }

        public async Task<UserDetails?> AddFunds(string Name, decimal Amount)
        {
            UserDetails? user = await _db.UserDetail.FindAsync(Name.ToLower());
            if (user == null)
            {
                return null;
            }
            user.CurrentMoney += Amount;
            user.AllTimeMoney += Amount;
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<UserDetails> AddUser(UserDetails user)
        {
            user.UserName = user.UserName.ToLower();
            _db.UserDetail.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<UserDetails?> AlterUserMoney(string Name, decimal Amount)
        {
            UserDetails? user = await _db.UserDetail.FindAsync(Name.ToLower());
            if (user == null)
            {
                return null;
            }
            user.CurrentMoney -= Amount;
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<UserOrders> CreateNewOrder(UserOrders order)
        {
            _db.UserOrder.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<List<StockDetails>> GetAllStocks(string Name)
        {
            List<StockDetails> stocks = await _db.StockDetail.Where(temp => temp.UserName == Name.ToLower()).ToListAsync();
            return stocks;
        }

        public async Task<UserDetails?> GetUser(string Name)
        {
            UserDetails? user = await _db.UserDetail.FindAsync(Name.ToLower());
            return user;
        }

        public async Task<List<UserOrders>> GetUserHistory(string Name)
        {
            List<UserOrders> history = await _db.UserOrder.Where(temp => temp.UserName.ToLower() == Name.ToLower()).OrderBy(temp => temp.Timestamp).ToListAsync();
            return history;
        }

        public async Task<UserOrders?> GetUserOrder(string Name, DateTime timestamp)
        {
            UserOrders? userOrder = await _db.UserOrder.Include(temp => temp.User).FirstOrDefaultAsync(temp => temp.UserName.ToLower() == Name.ToLower() && temp.Timestamp == timestamp);
            return userOrder;
        }

        public async Task<StockDetails?> GetUserStock(string Name, string Stock)
        {
            return await _db.StockDetail.FirstOrDefaultAsync(temp => temp.UserName.ToLower() == Name.ToLower() && temp.StockTicker.ToLower() == Stock.ToLower());
        }
    }
}
