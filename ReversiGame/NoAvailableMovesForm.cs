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
    public partial class NoAvailableMovesForm : Form
    {
        public NoAvailableMovesForm()
        {
            InitializeComponent();
            Height = 220;
            Width = 470;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            var table = new TableLayoutPanel() { Dock = DockStyle.Fill };
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            var infoLabel = new Label()
            {
                Text = "There is no available moves\nMove passed to another player",
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            var okButton = new Button()
            {
                Text = "Ok",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            okButton.Click += (sender, args) => Close();
            table.Controls.Add(infoLabel, 1, 1);
            table.Controls.Add(okButton, 1, 2);
            table.Controls.Add(new Label(), 2, 3);
            Controls.Add(table);
        }
    }
}
