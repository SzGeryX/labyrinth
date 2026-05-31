using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Controls;

namespace game
{
    enum Directions
    {
        North,
        South,
        West,
        East
    }


    class Tile
    {
        public char Icon { get; }
        public bool IsDiscovered { get; set; }
        public bool IsUsable { get => Icon != ' '; }

        public Label? Label;

        public List<Directions> Directions;


        public Tile(char icon)
        {
            this.Icon = icon == '.' ? ' ' : icon;
            this.Label = null;
            this.Directions = calculateDirections();

            IsDiscovered = false;

        }
        private List<Directions> calculateDirections() {
            switch (Icon) 
            {
                    case '█':
                    case '╬':
                        return new List<Directions>() { game.Directions.North, game.Directions.South, game.Directions.West, game.Directions.East };
                    case '═':
                        return new List<Directions>() { game.Directions.West, game.Directions.East };
                    case '╦':
                        return new List<Directions>() { game.Directions.South, game.Directions.West, game.Directions.East };
                    case '╩':
                        return new List<Directions>() { game.Directions.North, game.Directions.West, game.Directions.East };
                    case '║': 
                        return new List<Directions>() { game.Directions.North, game.Directions.South};
                    case '╣': 
                        return new List<Directions>() { game.Directions.North, game.Directions.South, game.Directions.West};
                    case '╠': 
                        return new List<Directions>() { game.Directions.North, game.Directions.South, game.Directions.East};
                    case '╗': 
                        return new List<Directions>() { game.Directions.South, game.Directions.West};
                    case '╝': 
                        return new List<Directions>() { game.Directions.North, game.Directions.West};
                    case '╚': 
                        return new List<Directions>() { game.Directions.North, game.Directions.East};
                    case '╔': 
                        return new List<Directions>() { game.Directions.South, game.Directions.East};

            }

            return new List<Directions>();
        }
    }
}
