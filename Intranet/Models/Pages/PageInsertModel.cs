using Extranet.Models;

namespace Intranet.Models.Pages
{
    public class PageInsertModel : BaseModel
    {
        public string Name { get; set; }
        public string HTML { get; set; }
    }

    public class PageEditModel : PageInsertModel
    {
    }
}
