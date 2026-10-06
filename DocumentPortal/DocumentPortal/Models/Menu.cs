using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentPortal.Models
{
    public class Menu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Menu name is required.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Order must be a number greater than or equal to 0.")]
        public int Order { get; set; }

        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
