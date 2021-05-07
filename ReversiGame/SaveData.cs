using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiGame
{
    public struct SaveData
    {
        public readonly int Id;
        public readonly Field Field;
        public readonly Game.GameMode GameMode;
        public readonly Color CurrentTurn;
        public readonly string Date;
        public SaveData(string id, string fieldString, string gameMode, 
            string currentTurn, string date) 
        {
            Id = int.Parse(id);
            GameMode = (Game.GameMode)Enum.Parse(typeof(Game.GameMode), gameMode, true);
            if (currentTurn.ToLower() == "white")
                CurrentTurn = Color.White;
            else if (currentTurn.ToLower() == "black")
                CurrentTurn = Color.Black;
            else 
                throw new ArgumentException("incorrect input: Only Black or White is available");
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
