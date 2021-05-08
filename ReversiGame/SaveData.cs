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
    }
}
