using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiGame
{
    class Game
    {
        private enum Turn
        {
            Black,
            White
        }

        public readonly Field field;
        private Turn currentTurn;

        public Color CurrentTurnColor
        {
            get
            {
                if (currentTurn == Turn.Black)
                    return Color.Black;
                return Color.White;
            }
        }

        public Game()
        {
            field = new Field();
            currentTurn = Turn.White;
        }

        public bool IsAvailableMove(int x, int y)
        {
            if (field[x, y] != Field.EmptyColor)
                return false;
            for (int dx = -1; dx <= 1; ++dx)
                for (int dy = -1; dy <= 1; ++dy)
                {
                    if (CheckDirection(x, y, dx, dy))
                        return true;
                }
            return false;
        }

        public void MakeMove(int x, int y)
        {
            field[x, y] = CurrentTurnColor;
            MoveInAllDirections(x, y);
            ChangeMovingSide();
        }

        private void ChangeMovingSide()
        {
            if (currentTurn == Turn.Black)
                currentTurn = Turn.White;
            else currentTurn = Turn.Black;
        }

        #region MoveMethods
        private void MoveInAllDirections(int x, int y)
        {
            for (int dx = -1; dx <= 1; ++dx)
                for (int dy = -1; dy <= 1; ++dy)
                    MoveInDirection(x, y, dx, dy);
        }

        private void MoveInDirection(int x, int y, int dx, int dy, bool recoloring = false)
        {
            if (dx == 0 && dy == 0)
                return;
            x += dx;
            y += dy;
            for (; x != field.Size && x != -1 && y != field.Size && y != -1; x += dx, y += dy)
            {
                if (field[x, y] == Field.EmptyColor)
                    return;
                if (field[x, y] == CurrentTurnColor)
                {
                    if (!recoloring)
                        MoveInDirection(x, y, -dx, -dy, true);
                    return;
                }
                if (recoloring)
                    field[x, y] = CurrentTurnColor;
            }
        }
        
        private bool CheckDirection(int x, int y, int dx, int dy)
        {
            if (dx == 0 && dy == 0)
                return false;
            x += dx;
            y += dy;
            for (int step = 1; x != field.Size && x != -1 && y != field.Size && y != -1;
                x += dx, y += dy, step++)
            {
                if (field[x, y] == Field.EmptyColor)
                    return false;
                if (field[x, y] == CurrentTurnColor)
                    return step > 1;
            }
            return false;
        }
        #endregion
    }
}
