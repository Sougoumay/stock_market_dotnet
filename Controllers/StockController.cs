using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public StockController(ApplicationDBContext context) {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() 
        {
            var stocks = _context.Stock.ToList().Select(s => s.ToStockDTO());

            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) 
        {
            var stock = _context.Stock.Find(id);
            
            if(stock == null) {
                return NotFound();
            }

            return Ok(stock.ToStockDTO());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateStockRequestDTO stockDTO)
        {
            var stockModel = stockDTO.ToStockFromCreateStockDTO();
            _context.Stock.Add(stockModel);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id}, stockModel.ToStockDTO());
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateStockRequestDTO updateDTO) 
        {
            
            if (id != updateDTO.Id) {
                return BadRequest();
            }

            var stockModel = _context.Stock.FirstOrDefault(x => x.Id == id);

            if(stockModel == null) {
                return NotFound();
            }

            stockModel.CompanyName = updateDTO.CompanyName;
            stockModel.Industry = updateDTO.Industry;
            stockModel.LastDiv = updateDTO.LastDiv;
            stockModel.Purchase = updateDTO.Purchase;
            stockModel.MarketCap = updateDTO.MarketCap;
            stockModel.Symbol = updateDTO.Symbol;

            _context.SaveChanges();

            return Ok(stockModel.ToStockDTO());
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id) 
        {
            var stockModel = _context.Stock.FirstOrDefault(x => x.Id == id);

            if(stockModel == null) {
                return NotFound();
            }

            _context.Stock.Remove(stockModel);
            _context.SaveChanges();

            return NoContent();
        }

    }
}