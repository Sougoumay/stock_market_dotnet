using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Comments")]
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int? StockId { get; set; } // La définition de la clé étrangère stockId

        public Stock? Stock { get; set; } // Définition de la property de navigation, ce qui permet d'accéder au Stock depuis le commentaire.

        public string AppUserId { get; set; }
        public AppUser Appuser { get; set; }
    }
}