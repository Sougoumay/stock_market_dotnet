using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class StockRepository(ApplicationDBContext dBContext) : IStockRepository
    {
        private readonly ApplicationDBContext _context = dBContext;

        public async Task<List<Stock>> GetAllAsync(QueryObject query) 
        {
            var stocks = _context.Stock.Include(s => s.Comments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.ToLower().Contains(query.CompanyName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.ToLower().Contains(query.Symbol.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id) 
        {
            return await _context.Stock.Include(s => s.Comments).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Stock> CreateAsync(Stock stockModel) 
        {
            await _context.Stock.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDTO stockDTO)
        {
            var stockModel = await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);

            if(stockModel == null) {
                return null;
            }

            stockModel.CompanyName = stockDTO.CompanyName;
            stockModel.Industry = stockDTO.Industry;
            stockModel.LastDiv = stockDTO.LastDiv;
            stockModel.Purchase = stockDTO.Purchase;
            stockModel.MarketCap = stockDTO.MarketCap;
            stockModel.Symbol = stockDTO.Symbol;

            await _context.SaveChangesAsync();

            return stockModel;
        }
        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);

            if( stockModel == null)
            {
                return null;
            }

            _context.Stock.Remove(stockModel);
            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<bool> ExistStock(int id)
        {
            return await _context.Stock.AnyAsync(s => s.Id == id);
        }
    }
}