using api.Dtos.Stock;
using api.Models;

namespace api.Mappers
{
    public static class StockMappers
    {
        public static StockDTO ToStockDTO(this Stock stockModel) 
        {
            return new StockDTO {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap,
                Comments = stockModel.Comments.Select(c => c.ToCommentDTO()).ToList()
            };
        }

        public static Stock ToStockFromCreateStockDTO(this CreateStockRequestDTO stockRequestDTO) 
        {
            return new Stock 
            {
                Symbol = stockRequestDTO.Symbol,
                CompanyName = stockRequestDTO.CompanyName,
                Purchase = stockRequestDTO.Purchase,
                LastDiv = stockRequestDTO.LastDiv,
                Industry = stockRequestDTO.Industry,
                MarketCap = stockRequestDTO.MarketCap
            };
        }
    }
}