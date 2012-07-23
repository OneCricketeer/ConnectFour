using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplications.Connect4
{
    class Piece
    {
        char letter;
        public Piece(char letter)
        {
            this.letter = letter;
        }

        public override string ToString()
        {
            return ""+this.letter;
        }
    }
}
