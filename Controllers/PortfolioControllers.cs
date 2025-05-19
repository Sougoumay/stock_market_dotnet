using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioReposiotry;

        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepository, IPortfolioRepository portfolioRepository)
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portfolioReposiotry = portfolioRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioReposiotry.GetUserPortfolio(appUser);
            return Ok(userPortfolio);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var stock = await _stockRepository.GetBySymbolAsync(symbol);

            if (stock == null) return BadRequest("Cannot find stock with symbol " + symbol);

            var userPortfolio = await _portfolioReposiotry.GetUserPortfolio(appUser);

            if (userPortfolio.Any(p => p.Symbol.ToLower() == symbol.ToLower())) return BadRequest("The stock exist in the portfolio");

            Portfolio createdPortfolio = await _portfolioReposiotry.CreateAsync(appUser.Id, stock.Id);

            if (createdPortfolio == null)
            {
                return StatusCode(500, "Could not create portfolio");
            }

            return StatusCode(201, "Portfolio created successfully");
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var userPortfolio = await _portfolioReposiotry.GetUserPortfolio(appUser);

            var filteredStock = userPortfolio.Where(p => p.Symbol.ToLower() == symbol.ToLower());
            if (filteredStock.Count() == 1)
            {
                await _portfolioReposiotry.DeletePortfolioAsync(appUser, symbol);
            }
            else
            {
                return NotFound("Stock not in your portfolio");
            }

            return Ok();
        }
    }
}