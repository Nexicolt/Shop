using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Intranet.Models.Books
{
    public class BookInsertModel : BaseModel
    {
        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        public IFormFile Icon { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        public long? CategoryId { get; set; }
        public SelectList CateogoriesList { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Wartośc musi być większa od 0")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Wartośc musi być większa od 0")]
        public uint Pages { get; set; }


        public BookInsertModel()
        {
            CateogoriesList = new SelectList(new List<string>());
        }
    }

    public class BookEditModel : BookInsertModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "Wartośc musi być większa od -1")]
        public uint Discount { get; set; }
        public bool Available { get; set; }

        public new IFormFile? Icon { get; set; }
    }
}
