using System.Collections.Generic;

namespace DocumentPortal.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }

        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
