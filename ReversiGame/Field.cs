using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiGame
{
    public class Field
    {
        enum CellCondition
        {
            Empty,
            Black,
            White
        }

        private readonly CellCondition[,] field;
        public readonly int Size;
        public static Color EmptyColor = Color.Green;

        public Field() : this(8) { }

        Field(int size)
        {
            Size = size;
            field = new CellCondition[size, size];
            field[size / 2 - 1, size / 2 - 1] = CellCondition.Black;
            field[size / 2 - 1, size / 2] = CellCondition.White;
            field[size / 2, size / 2 - 1] = CellCondition.White;
            field[size / 2, size / 2] = CellCondition.Black;
        }

        public Color this[int x, int y]
        {
            get
            {
                if (field[x, y] == CellCondition.Black)
                    return Color.Black;
                if (field[x, y] == CellCondition.White)
                    return Color.White;
                return EmptyColor;
            }
            set
            {
                if (value == Color.Black)
                {
                    field[x, y] = CellCondition.Black;
                    return;
                }
                if (value == Color.White)
                {
                    field[x, y] = CellCondition.White;
                    return;
                }
                field[x, y] = CellCondition.Empty;
            }
        }
    }
}
