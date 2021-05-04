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
        public Game.GameMode gameMode;
        public StartForm()
        {
            InitializeComponent();

            var label = new Label
            {
                Text = "Select Game Mode: ",
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font(this.Font.FontFamily, 14, FontStyle.Regular),
                //BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                //BackColor = Color.AntiqueWhite
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
            //table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            table.Controls.Add(new Panel(), 0, 0);
            table.Controls.Add(label, 1, 1);
            table.Controls.Add(onePlayerButton, 1, 2);
            table.Controls.Add(twoPlayerButton, 2, 2);
            table.Controls.Add(new Panel(), 3, 3);
            table.Dock = DockStyle.Fill;
            Controls.Add(table);
            onePlayerButton.Click += (sender, args) => { gameMode = Game.GameMode.Single; };
            twoPlayerButton.Click += (sender, args) => { gameMode = Game.GameMode.MultiPlayer; };
            onePlayerButton.Click += (sender, args) => Close();
            twoPlayerButton.Click += (sender, args) => Close();
        }
    }
}
