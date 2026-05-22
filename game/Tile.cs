using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace game
{
    class Tile
    {
        public char Icon { get; }
        public bool IsDiscovered { get; set; }

        public Tile(char icon)
        {
            this.Icon = icon == '.' ? ' ' : icon;
            IsDiscovered = false;
        }
    }
}
