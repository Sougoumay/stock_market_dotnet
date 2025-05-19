using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Models;

namespace api.Mappers
{
    public static class CommentMapper
    {
        public static CommentDTO ToCommentDTO(this Comment commentModel)
        {
            return new CommentDTO
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                StockId = commentModel.StockId,
                CreatedBy = commentModel.Appuser.UserName
            };
        }

        public static Comment ToCommentFromRequestDTO(this CreateCommentRequestDTO requestDTO)
        {
            return new Comment
            {
                Title = requestDTO.Title,
                Content = requestDTO.Content,
                StockId = requestDTO.StockId
            };
        }
    }
}