using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("/api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;

        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            var commentDTO = comments.Select(c => c.ToCommentDTO());
            return Ok(commentDTO);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var commentModel = await _commentRepository.GetByIdAsync(id);

            if(commentModel == null ) {
                return NotFound();
            }

            return Ok(commentModel.ToCommentDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequestDTO requestDTO)
        {
            if(!await _stockRepository.ExistStock(requestDTO.StockId)) 
            {
                return BadRequest("Stcok doesn't exists");
            }

            var commentModel = requestDTO.ToCommentFromRequestDTO();
            await _commentRepository.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new {Id = commentModel.Id}, commentModel.ToCommentDTO());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDTO requestDTO)
        {
            if(id != requestDTO.Id) 
            {
                return BadRequest();
            }

            var commentModel = await _commentRepository.UpdateAsync(id, requestDTO);

            if(commentModel == null)
            {
                return NotFound();
            }

            return Ok(commentModel.ToCommentDTO());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var commentModel = await _commentRepository.DeleteAsync(id);

            if(commentModel == null) {
                return NotFound();
            }

            return NoContent();
        }
    }
}