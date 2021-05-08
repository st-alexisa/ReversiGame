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
    public partial class StartForm : Form
    {
        public Game.GameMode GameMode { get; private set; }
        public Field Field { get; private set; }
        public Game.Turn CurrentTurn { get; private set; }
        readonly TableLayoutPanel table;
        readonly Button onePlayerButton;
        readonly Button twoPlayerButton;
        readonly Button newGameButton;
        readonly Button loadGameButton;
        readonly Button backButton;
        readonly Label textLabel;

        public StartForm()
        {
            InitializeComponent();
            textLabel = CreateLabel("Start");
            onePlayerButton = CreateButton("1 Player ", false);
            twoPlayerButton = CreateButton("2 Players ");
            newGameButton = CreateButton("New Game");
            loadGameButton = CreateButton("Load Game");
            backButton = CreateButton("<- return", true, DockStyle.Left, 8);

            table = GetTableLayoutPanel();
            ShowStartScreen();
            table.Dock = DockStyle.Fill;
            Controls.Add(table);

            newGameButton.Click += (sender, args) => ShowSelectModeScreen();
            loadGameButton.Click += (sender, args) => ShowLoadForm();
            onePlayerButton.Click += (sender, args) => InitializeGameParameters(Game.GameMode.Single);
            twoPlayerButton.Click += (sender, args) => InitializeGameParameters(Game.GameMode.MultiPlayer);
            onePlayerButton.Click += (sender, args) => Close();
            twoPlayerButton.Click += (sender, args) => Close();
            backButton.Click += (sender, args) => ShowStartScreen();
        }

        private TableLayoutPanel GetTableLayoutPanel() 
        {
            var table = new TableLayoutPanel();
            table.RowStyles.Clear();
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            return table;
        }

        private void InitializeGameParameters(Game.GameMode gameMode, Game.Turn currentTurn = Game.Turn.White)
        {
            Field = new Field();
            CurrentTurn = currentTurn;
            GameMode = gameMode;
        }
        private Button CreateButton(string buttonText, bool enabled = true,
            DockStyle dockStyle = DockStyle.Fill, int fontSize = 14) 
        {
            return new Button
            {
                Text = buttonText,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, fontSize, FontStyle.Regular),
                Dock = dockStyle,
                Enabled = enabled,
            };
        }
        private Label CreateLabel(string text, 
            DockStyle dockStyle = DockStyle.Fill, int fontSize = 14) 
        {
            return new Label
            {
                Text = text,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font(this.Font.FontFamily, fontSize, FontStyle.Regular),
                Dock = dockStyle,
            };
        }
        private void ShowStartScreen()
        {
            textLabel.Text = "Start";
            table.Controls.Clear();
            table.Controls.Add(new Panel(), 0, 0);
            table.Controls.Add(textLabel, 1, 1);
            table.Controls.Add(newGameButton, 1, 2);
            table.Controls.Add(loadGameButton, 2, 2);
            table.Controls.Add(new Panel(), 3, 3);
        }
        private void ShowSelectModeScreen()
        {
            textLabel.Text = "Select Game Mode: ";
            table.Controls.Clear();
            table.Controls.Add(new Panel(), 0, 0);
            table.Controls.Add(textLabel, 1, 1);
            table.Controls.Add(onePlayerButton, 1, 2);
            table.Controls.Add(twoPlayerButton, 2, 2);
            table.Controls.Add(backButton, 0, 0);
            table.Controls.Add(new Panel(), 3, 3);
        }
        private void ShowLoadForm()
        {
            var loadForm = new GameLoadForm();
            loadForm.ShowDialog();
            if (loadForm.SelectedSave != null)
            {
                GameMode = loadForm.SelectedSave.GameMode;
                Field = loadForm.SelectedSave.Field;
                CurrentTurn = loadForm.SelectedSave.CurrentTurn;
                Close();
            }
        }
    }
}
