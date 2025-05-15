using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class StockRepository(ApplicationDBContext dBContext) : IStockRepository
    {
        private readonly ApplicationDBContext _context = dBContext;

        public Task<List<Stock>> GetAllAsync() 
        {
            return _context.Stock.Include(s => s.Comments).ToListAsync();
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