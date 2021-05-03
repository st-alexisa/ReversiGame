using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReversiGame
{
    public partial class FieldForm : Form
    {
        public FieldForm()
        {
            InitializeComponent();
            var game = new Game();
            int fieldSize = game.field.Size;
            var fieldButtons = GetFieldButtons(game);

            var tableLayout = new TableLayoutPanel();
            tableLayout.RowStyles.Clear();
            for (int i = 0; i < fieldSize; ++i)
            {
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            }
            tableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            for (int i = 0; i < fieldSize; ++i)
                for (int j = 0; j < fieldSize; ++j)
                    tableLayout.Controls.Add(fieldButtons[i, j], j, i);
            tableLayout.Dock = DockStyle.Fill;
            Controls.Add(tableLayout);
        }

        private Button[,] GetFieldButtons(Game game)
        {
            int fieldSize = game.field.Size;
            var buttons = new Button[fieldSize, fieldSize];
            for (int i = 0; i < fieldSize; ++i)
            {
                for (int j = 0; j < fieldSize; ++j)
                {
                    var button = new Button { Dock = DockStyle.Fill };
                    UpdateButton(game, button, i, j);
                    int x = i; // variables needed to avoid closure
                    int y = j;
                    button.Click += (sender, args) => game.MakeMove(x, y);
                    button.Click += (sender, args) => UpdateButtons(game, buttons);
                    buttons[i, j] = button;
                }
            }
            return buttons;
        }

        private void UpdateButtons(Game game, Button[,] buttons)
        {
            int size = game.field.Size;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    UpdateButton(game, buttons[i, j], i, j);
                }
            }
        }

        private void UpdateButton(Game game, Button button, int i, int j)
        {
            button.BackColor = game.field[i, j];
            if (game.IsAvailableMove(i, j))
            {
                button.BackColor = Color.LightGreen;
                button.Enabled = true;
            }
        }
    }
}
