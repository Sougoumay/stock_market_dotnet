using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class CommentRepository(ApplicationDBContext dBContext) : ICommentRepository
    {
        private readonly ApplicationDBContext _context = dBContext;

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.Include(c => c.Appuser).ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments.Include(c => c.Appuser).FirstOrDefaultAsync( c => c.Id == id);
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();

            return commentModel; 
        }

        public async Task<Comment> UpdateAsync(int id, UpdateCommentRequestDTO requestDTO)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            
            if(commentModel == null) {
                return null;
            }

            commentModel.Content = requestDTO.Content;
            commentModel.Title = requestDTO.Title;

            await _context.SaveChangesAsync();

            return commentModel;

        }

        public async Task<Comment> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

            if(commentModel == null) 
            {
                return null;
            }

            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();

            return commentModel;
        }
    }
}