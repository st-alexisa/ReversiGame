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
        readonly Game game;
        readonly int fieldSize;
        readonly Button[,] fieldButtons;
        readonly TableLayoutPanel tableLayout;

        public FieldForm(Field field, Game.Turn currentTurn, Game.GameMode gameMode = Game.GameMode.Single)
        {
            InitializeComponent();
            Height = Width;
            game = new Game(field, currentTurn, gameMode);
            fieldSize = game.FieldSize;
            fieldButtons = GetFieldButtons(game);
            game.MadeAMove += (sender, args) => UpdateButtons(game, fieldButtons);
            game.NoAvailableMoves += (sender, args) =>
            {
                MessageBox.Show("There is no available moves\nMove passed to another player", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            tableLayout = GetTableLayoutPanel(game, fieldButtons);
            Controls.Add(tableLayout);
            FormClosing += (sender, args) => ShowSaveScreen();
        }

        private TableLayoutPanel GetTableLayoutPanel(Game game, Button[,] fieldButtons) 
        {
            var tableLayout = new TableLayoutPanel();
            int fieldSize = game.FieldSize;
            tableLayout.RowStyles.Clear();
            for (int i = 0; i < fieldSize; ++i)
            {
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, ClientSize.Width / fieldSize));
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            }
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));

            //tableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            for (int i = 0; i < fieldSize; ++i)
                for (int j = 0; j < fieldSize; ++j)
                    tableLayout.Controls.Add(fieldButtons[i, j], j, i);

            AddLabels(tableLayout);
            tableLayout.Dock = DockStyle.Fill;
            return tableLayout;
        }

        private void ShowSaveScreen()
        {
            var result = MessageBox.Show("Save before exit", "", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                new GameLoadForm(game.GetSave()).ShowDialog();
        }

        #region LabelsAddingMethods
        private void AddLabels(TableLayoutPanel tableLayout) 
        {
            var gameModeLabel = new Label()
            {
                Text = game.gameMode.ToString(),
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Fill,
                BackColor = Color.AliceBlue,
            };
            tableLayout.Controls.Add(gameModeLabel, 0, fieldSize);
            AddCurrentPlayerLabel(tableLayout);
            AddLastMoveLabel(tableLayout);
        }

        private void AddCurrentPlayerLabel(TableLayoutPanel tableLayout) 
        {
            var currentPlayerTextLabel = new Label()
            {
                Text = "Current Turn:",
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Fill,
                BackColor = Color.AliceBlue
            };
            var currentPlayerLabel = GetCurrentPlayerLabel();
            game.ChangedMovingSide += (sender, args) =>
            {
                currentPlayerLabel.BackColor = game.CurrentTurnColor;
                if (game.CurrentTurnColor == Color.White)
                {
                    currentPlayerLabel.Text = "White";
                    currentPlayerLabel.ForeColor = Color.Black;
                }
                else
                {
                    currentPlayerLabel.Text = "Black";
                    currentPlayerLabel.ForeColor = Color.White;
                }
            };
            tableLayout.Controls.Add(currentPlayerTextLabel, 1, fieldSize);
            tableLayout.Controls.Add(currentPlayerLabel, 2, fieldSize);
        }

        private Label GetCurrentPlayerLabel()
        {
            string colorText;
            Color textColor;
            if (game.CurrentTurnColor == Color.Black)
            {
                colorText = "Black";
                textColor = Color.White;
            }
            else
            {
                colorText = "White";
                textColor = Color.Black;
            }
            var currentPlayerLabel = new Label()
            {
                Text = colorText,
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Fill,
                ForeColor = textColor,
                BackColor = game.CurrentTurnColor,
                BorderStyle = BorderStyle.FixedSingle,
            };
            return currentPlayerLabel;
        }

        private void AddLastMoveLabel(TableLayoutPanel tableLayout) 
        {
            var lastMoveTextLabel = new Label()
            {
                Text = "Last move:",
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Fill,
                BackColor = Color.AliceBlue,
            };
            var lastMoveLabel = new Label()
            {
                Text = "",
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Fill,
                BackColor = Color.AliceBlue,
                BorderStyle = BorderStyle.FixedSingle,
            };
            game.MadeAMove += (sender, args) =>
            {
                lastMoveLabel.Text = args.ToString();
            };
            tableLayout.Controls.Add(lastMoveTextLabel, 3, fieldSize);
            tableLayout.Controls.Add(lastMoveLabel, 4, fieldSize);
        }
        #endregion

        #region ButtonMethods
        private Button[,] GetFieldButtons(Game game)
        {
            var buttons = new Button[fieldSize, fieldSize];
            for (int i = 0; i < fieldSize; ++i)
            {
                for (int j = 0; j < fieldSize; ++j)
                {
                    var button = new Button { Dock = DockStyle.Fill };
                    UpdateButton(game, button, i, j);
                    int x = i; // variables needed to avoid closure
                    int y = j;
                    button.MouseHover += (sender, args) => 
                    {
                        var text = "empty";
                        if (game.GetCellCondition(x, y) == Color.White)
                            text = "white";
                        if (game.GetCellCondition(x, y) == Color.Black)
                            text = "black";
                        button.Text = text;
                        button.ForeColor = Color.Red;
                    };
                    button.Click += (sender, args) => game.MakeMove(x, y);
                    //button.Click += (sender, args) => UpdateButtons(game, buttons);
                    button.Click += (sender, args) => game.MakeMachineMove();
                    //button.Click += (sender, args) => UpdateButtons(game, buttons);
                    buttons[i, j] = button;
                }
            }
            return buttons;
        }

        private void UpdateButtons(Game game, Button[,] buttons)
        {
            foreach (var cell in game.GetFieldCells()) 
            {
                var button = buttons[cell.X, cell.Y];
                button.BackColor = cell.Color;
                if (cell.AvailableToMove) 
                {
                    button.BackColor = Color.LightGreen;
                    button.Enabled = true;
                }
                else
                {
                    button.Enabled = false;
                }
            }
        }

        private void UpdateButton(Game game, Button button, int i, int j)
        {
            button.BackColor = game.GetCellCondition(i, j);
            if (game.IsAvailableMove(i, j))
            {
                button.BackColor = Color.LightGreen;
                button.Enabled = true;
            }
            else 
            { 
                button.Enabled = false;
            }
        }
        #endregion
    }
}
