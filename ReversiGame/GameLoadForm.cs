using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReversiGame
{
    public partial class GameLoadForm : Form
    {
        readonly static int savesCount = 10;
        SqlConnection sqlConnection;
        readonly TableLayoutPanel tableLayout;
        readonly List<Label> saveLabels;
        readonly List<SaveData> saves;
        readonly List<Button> loadButtons;
        public SaveData SelectedSave { get; private set; }

        public GameLoadForm()
        {
            InitializeComponent();
            saves = new List<SaveData>();
            saveLabels = new List<Label>();
            loadButtons = new List<Button>();
            tableLayout = GetTableLayoutPanel();
            AddSaveLabels();
            AddLoadButtons();
            Controls.Add(tableLayout);

            Load += new System.EventHandler(GameLoadForm_Load);
            FormClosing += (sender, args) => 
            {
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();
            };
        }

        private void AddSaveLabels()
        {
            for (int i = 0; i < savesCount; ++i) 
            {
                var label = new Label() { Dock = DockStyle.Fill};
                saveLabels.Add(label);
                tableLayout.Controls.Add(label, 0, i);
            }
        }
        
        private void AddLoadButtons() 
        {
            for (int i = 0; i < savesCount; ++i) 
            {
                var button = new Button() { Text = "Load", Dock = DockStyle.Fill};
                button.Enabled = false;
                loadButtons.Add(button);
                tableLayout.Controls.Add(button, 1, i);
            }
        }

        private TableLayoutPanel GetTableLayoutPanel() 
        {
            var table = new TableLayoutPanel()
            {
                BackColor = Color.LightBlue,
                Dock = DockStyle.Fill,
            };
            for (int i = 0; i < 10; ++i)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            }
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            return table;
        }

        private async void GameLoadForm_Load(object sender, EventArgs e) 
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Repositories\ReversiGame\ReversiGame\GameSaves.mdf;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();
            SqlDataReader sqlReader = null;
            var command = new SqlCommand("SELECT * FROM [GameSaves]", sqlConnection);
            try 
            {
                sqlReader = await command.ExecuteReaderAsync();
                for (int i = 0; await sqlReader.ReadAsync() && i < savesCount; ++i) 
                {
                    saves.Add(new SaveData(Convert.ToString(sqlReader["Id"]),
                        Convert.ToString(sqlReader["FieldState"]),
                        Convert.ToString(sqlReader["GameMode"]),
                        Convert.ToString(sqlReader["CurrentTurn"]),
                        Convert.ToString(sqlReader["Date"])));
                    saveLabels[i].Text = saves[i].Id.ToString() + "th save   " + saves[i].Date;
                    int x = i;// var needed to escape closure
                    loadButtons[i].Enabled = true;
                    loadButtons[i].Click += (sender, args) => { SelectedSave = saves[x]; };
                    loadButtons[i].Click += (sender, args) => { Close(); };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally 
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }
        }
    }
}
