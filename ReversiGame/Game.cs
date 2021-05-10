using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReversiGame
{
    public class Game
    {
        #region IncludedTypes
        public class MadeAMoveArgs : EventArgs
        {
            public readonly Color PlayerColor;
            public readonly int X;
            public readonly int Y;
            public MadeAMoveArgs(Color color, int x, int y) 
            {
                this.PlayerColor = color;
                X = x;
                Y = y;
            }
            public override string ToString() 
            {
                return String.Format("({0}, {1})", X, Y);
            }
        }
        public class Cell 
        {
            public readonly Color Color;
            public readonly int X;
            public readonly int Y;
            public bool AvailableToMove;
            public Cell(Color color, int x, int y, bool available) 
            {
                Color = color;
                X = x;
                Y = y;
                AvailableToMove = available;
            }
        } 
        public enum Turn
        {
            Black,
            White
        }
        public enum GameMode 
        {
            Single,
            MultiPlayer
        }
        #endregion
        
        public Color CurrentTurnColor
        {
            get
            {
                if (CurrentTurn == Turn.Black)
                    return Color.Black;
                return Color.White;
            }
        }
        public readonly GameMode gameMode;
        public readonly int FieldSize;

        public static readonly Color Player1 = Color.White;
        public static readonly Color Player2 = Color.Black;

        private readonly Field field;
        public Turn CurrentTurn { get; private set; }
        public event EventHandler<MadeAMoveArgs> MadeAMove;
        public event EventHandler ChangedMovingSide;
        public event EventHandler NoAvailableMoves;

        public IEnumerable<Cell> GetFieldCells() 
        {
            for (int i = 0; i < FieldSize; ++i)
                for (int j = 0; j < FieldSize; ++j)
                    yield return new Cell(field[i, j], i, j, IsAvailableMove(i,j));
        }

        public Game(Field field, Turn currentTurn, GameMode gameMode = GameMode.Single)
        {
            this.field = field;
            this.FieldSize = field.Size;
            this.CurrentTurn = currentTurn; 
            this.gameMode = gameMode;
        }

        public Game(Field field) 
        {
            this.field = field;
            this.FieldSize = field.Size;
            this.CurrentTurn = Turn.White;
            this.gameMode = GameMode.Single;
        }

        public SaveData GetSave() 
        {
            return new SaveData(field, gameMode, CurrentTurn);
        }

        public Color GetCellCondition(int x, int y)
        {
            return field[x, y];
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
            MadeAMove.Invoke(this, new MadeAMoveArgs(CurrentTurnColor, x, y));
            if (!GetFieldCells().Where(cell => cell.AvailableToMove).Any())
            {
                ChangeMovingSide();
                NoAvailableMoves.Invoke(this, EventArgs.Empty);
            }
        }

        public void MakeMachineMove() 
        {
            if (gameMode == GameMode.Single && CurrentTurnColor == Player2)
            {
                var point = GetFieldCells()
                    .Where(cell => cell.AvailableToMove)
                    .OrderBy(cell => Math.Abs(cell.X - cell.Y))
                    .First();
                Thread.Sleep(1000);
                var x = point.X;
                var y = point.Y;
                field[x, y] = CurrentTurnColor;
                MoveInAllDirections(x, y);
                ChangeMovingSide();
                MadeAMove.Invoke(this, new MadeAMoveArgs(CurrentTurnColor, x, y));
            }
        }
        
        private void ChangeMovingSide()
        {
            if (CurrentTurn == Turn.Black)
                CurrentTurn = Turn.White;
            else CurrentTurn = Turn.Black;
            ChangedMovingSide.Invoke(this, EventArgs.Empty);
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
