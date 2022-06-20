using System.ComponentModel.DataAnnotations;

namespace Data.Model.Base
{
    public class BaseEntity
    {
        [Key]
        public long? Id { get; set; }

        public bool IsLocked { get; set; }

        /// <summary>
        /// Bardzo prosty, ale skuteczny 'soft delete', bo od razu wskoczy data edycji i edytujący użytkownik
        /// </summary>
        public void Delete()
        {
            this.IsLocked = true;
        }

    }


}