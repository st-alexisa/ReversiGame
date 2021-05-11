using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiGame
{
    public class SaveData
    {
        public readonly int Id;
        public readonly Field Field;
        public readonly Game.GameMode GameMode;
        public readonly Game.Turn CurrentTurn;
        public readonly string Date;

        public SaveData(Field field, Game.GameMode gameMode,
            Game.Turn currentTurn, int id = 0)
        {
            Id = id;
            Field = field;
            GameMode = gameMode;
            CurrentTurn = currentTurn;
            Date = "today";
        }

        public SaveData(string id, string fieldString, string gameMode, 
            string currentTurn, string date) 
        {
            Id = int.Parse(id);
            GameMode = (Game.GameMode)Enum.Parse(typeof(Game.GameMode), gameMode, true);
            CurrentTurn = (Game.Turn)Enum.Parse(typeof(Game.Turn), currentTurn, true);
            Date = date;
            Field = new Field();
            for (int i = 0; i < 64; ++i) 
            {
                Field[i / 8, i % 8] = fieldString[i] switch
                {
                    'W' => Color.White,
                    'B' => Color.Black,
                    _ => Field.EmptyColor,
                };
            }
        }

        public string GetFieldString() 
        {
            var str = new StringBuilder();
            for (int i = 0; i < Field.Size; ++i) 
            {
                for (int j = 0; j < Field.Size; ++j)
                {
                    if (Field[i, j] == Color.Black)
                        str.Append('B');
                    else if (Field[i, j] == Color.White)
                        str.Append('W');
                    else
                        str.Append('E');
                }
            }
            return str.ToString();
        }
        public string GetGameModeString() 
        {
            if (GameMode == Game.GameMode.MultiPlayer)
                return "MultiPlayer";
            return "Single";
        }
        public string GetCurrentTurnString() 
        {
            if (CurrentTurn == Game.Turn.Black)
                return "Black";
            return "White";
        }
    }
}
