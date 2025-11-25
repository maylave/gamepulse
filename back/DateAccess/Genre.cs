using System;
using System.Collections.Generic;
using System.Text;

namespace DateAccess
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Game> Games { get; } = new List<Game>();
    }
}
