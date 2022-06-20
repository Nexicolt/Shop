using System.ComponentModel.DataAnnotations;

namespace Extranet.Models
{
    public class LoginModel : BaseModel
    {
        [Microsoft.Build.Framework.Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Microsoft.Build.Framework.Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }
    }
}
