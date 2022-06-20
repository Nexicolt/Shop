using System.ComponentModel.DataAnnotations;

namespace Intranet.Models.Categories
{
    public class CategoryInsertModel : BaseModel
    {
        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane")]
        public IFormFile Icon{ get; set; }
    }

    public class CategoryEditModel : CategoryInsertModel
    {
        //Przyz edycji, ikona nie jest już wymagana
        public new IFormFile? Icon { get; set; }
    }
}
