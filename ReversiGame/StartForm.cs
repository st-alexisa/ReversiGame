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
        private readonly TableLayoutPanel table;
        public StartForm()
        {
            InitializeComponent();
            Field = new Field();
            CurrentTurn = Game.Turn.White;
            var textLabel = new Label
            {
                Text = "Start",
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font(this.Font.FontFamily, 14, FontStyle.Regular),
                Dock = DockStyle.Fill,
            };
            var onePlayerButton = new Button
            {
                Text = "1 Player ",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 14, FontStyle.Regular),
                Dock = DockStyle.Fill,
                Enabled = false,
            };
            var twoPlayerButton = new Button
            {
                Text = "2 Players ",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 14, FontStyle.Regular),
                Dock = DockStyle.Fill
            };
            var newGameButton = new Button() 
            {
                Text = "New Game",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 14, FontStyle.Regular),
                Dock = DockStyle.Fill
            };
            var loadGameButton = new Button()
            {
                Text = "Load Game",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 14, FontStyle.Regular),
                Dock = DockStyle.Fill
            };
            table = GetTableLayoutPanel();
            table.Controls.Add(new Panel(), 0, 0);
            table.Controls.Add(textLabel, 1, 1);
            table.Controls.Add(newGameButton, 1, 2);
            table.Controls.Add(loadGameButton, 2, 2);
            table.Controls.Add(new Panel(), 3, 3);
            table.Dock = DockStyle.Fill;
            Controls.Add(table);

            newGameButton.Click += (sender, args) => 
            {
                textLabel.Text = "Select Game Mode: ";
                table.Controls.Remove(newGameButton);
                table.Controls.Remove(loadGameButton);
                table.Controls.Add(onePlayerButton, 1, 2);
                table.Controls.Add(twoPlayerButton, 2, 2);
            };
            loadGameButton.Click += (sender, args) =>
            {
                var loadForm = new GameLoadForm();
                loadForm.ShowDialog();
                GameMode = loadForm.SelectedSave.GameMode;
                Field = loadForm.SelectedSave.Field;
                CurrentTurn = loadForm.SelectedSave.CurrentTurn;
                Close();
            };
            onePlayerButton.Click += (sender, args) => { GameMode = Game.GameMode.Single; };
            twoPlayerButton.Click += (sender, args) => { GameMode = Game.GameMode.MultiPlayer; };
            onePlayerButton.Click += (sender, args) => Close();
            twoPlayerButton.Click += (sender, args) => Close();
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
    }
}
