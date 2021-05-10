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
        enum Mode 
        {
            Save,
            Load
        }
        readonly static int savesCount = 10;
        SqlConnection sqlConnection;
        TableLayoutPanel tableLayout { get; set; }
        readonly List<Label> saveLabels;
        readonly List<SaveData> saves;
        readonly List<Button> loadButtons;
        readonly List<Button> saveButtons;
        readonly List<Button> deleteButtons;
        readonly HashSet<int> deletingSaveNumbers;
        readonly Label loadingTextLabel;
        readonly Mode mode;
        public SaveData SelectedSave { get; private set; }

        public GameLoadForm(SaveData save = null)
        {
            InitializeComponent();
            if (save != null)
                mode = Mode.Save;
            else mode = Mode.Load;
            SelectedSave = save;
            loadingTextLabel = new Label { Text = "save reading in progress.. please, wait", Dock = DockStyle.Fill };
            saves = new List<SaveData>();
            saveLabels = new List<Label>();
            loadButtons = new List<Button>();
            saveButtons = new List<Button>();
            deleteButtons = new List<Button>();
            deletingSaveNumbers = new HashSet<int>();
            CreateTableLayoutPanel();
            Controls.Add(tableLayout);

            Load += new System.EventHandler(GameLoadForm_Load);
            FormClosing += (sender, args) =>
            {
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();
            };
        }

        #region TableCreating
        private void AddSaveLabels()
        {
            for (int i = 0; i < savesCount; ++i) 
            {
                var label = new Label() { Dock = DockStyle.Fill};
                saveLabels.Add(label);
                tableLayout.Controls.Add(label, 1, i);
            }
        }
        private void AddSaveButtons()
        {
            for (int i = 0; i < savesCount; ++i)
            {
                var button = new Button() { Text = "Save", Dock = DockStyle.Fill };
                button.Enabled = true;
                saveButtons.Add(button);
                if(mode == Mode.Save)
                    tableLayout.Controls.Add(button, 2, i);
            }
        }
        private void AddLoadButtons() 
        {
            for (int i = 0; i < savesCount; ++i) 
            {
                var button = new Button() { Text = "Load", Dock = DockStyle.Fill};
                button.Enabled = false;
                loadButtons.Add(button);
                if (mode == Mode.Load)
                    tableLayout.Controls.Add(button, 2, i);
            }
        }
        private void AddDeleteButtons() 
        {
            for (int i = 0; i < savesCount; ++i)
            {
                var button = new Button() { Text = "Delete", Dock = DockStyle.Fill };
                button.Enabled = false;
                deleteButtons.Add(button);
                tableLayout.Controls.Add(button, 3, i);
            }
        }
        private void AddNumberLabels() 
        {
            for (int i = 0; i < savesCount; ++i) 
                tableLayout.Controls.Add(new Label 
                { Text = (i + 1).ToString(), Dock = DockStyle.Fill }, 0, i);
        }
        private void CreateTableLayoutPanel() 
        {
            tableLayout = new TableLayoutPanel()
            {
                BackColor = Color.LightBlue,
                Dock = DockStyle.Fill,
            };
            for (int i = 0; i < 10; ++i)
            {
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            }
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            tableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            AddNumberLabels();
            AddSaveLabels();
            AddSaveButtons();
            AddLoadButtons();
            AddDeleteButtons();
            tableLayout.Controls.Add(loadingTextLabel, 0, savesCount);
        }
        #endregion
        
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
                ReadDataBase(sqlReader);
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
            loadingTextLabel.Text = "";
        }

        private async void ReadDataBase(SqlDataReader sqlReader)
        {
            for (int i = 0; await sqlReader.ReadAsync() && i < savesCount; ++i)
            {
                saves.Add(new SaveData(Convert.ToString(sqlReader["Id"]),
                    Convert.ToString(sqlReader["FieldState"]),
                    Convert.ToString(sqlReader["GameMode"]),
                    Convert.ToString(sqlReader["CurrentTurn"]),
                    Convert.ToString(sqlReader["Date"])));
                saveLabels[i].Text = saves[i].GetGameModeString() + " cur. turn: " + saves[i].GetCurrentTurnString() +" "+ saves[i].Date;
                int x = i;// var needed to escape closure
                int id = saves[i].Id;
                loadButtons[i].Enabled = true;
                loadButtons[i].Click += (sender, args) => { SelectedSave = saves[x]; };
                loadButtons[i].Click += (sender, args) => { Close(); };
                saveButtons[i].Click += (sender, args) => { Save(SelectedSave, id); };
                saveButtons[i].Click += (sender, args) =>
                {
                    saveLabels[x].Text = SelectedSave.GetGameModeString() + " cur. turn: " + SelectedSave.GetCurrentTurnString()
                      + " " + SelectedSave.Date;
                };
                deleteButtons[i].Enabled = true;
                deleteButtons[i].Click += (sender, args) =>
                {
                    loadButtons[x].Enabled = false;
                    saveLabels[x].Text = "";
                    deleteButtons[x].Enabled = false;
                    deletingSaveNumbers.Add(x);
                };
            }
        }

        private async void Save(SaveData save, int id) 
        {
            var command = new SqlCommand("UPDATE [GameSaves] SET [CurrentTurn]=@CurrentTurn, [Date]=@Date, [FieldState]=@FieldState, [GameMode]=@GameMode WHERE [Id]=@Id", sqlConnection);
            command.Parameters.AddWithValue("CurrentTurn", save.GetCurrentTurnString());
            command.Parameters.AddWithValue("Date", save.Date);
            command.Parameters.AddWithValue("GameMode", save.GetGameModeString());
            command.Parameters.AddWithValue("FieldState", save.GetFieldString());
            command.Parameters.AddWithValue("Id", id);
            if (deletingSaveNumbers.Contains(id)) 
                deletingSaveNumbers.Remove(id);
            await command.ExecuteNonQueryAsync();
        }

        private async void DeleteSave(int number) 
        {
            var command = new SqlCommand("", sqlConnection);
            //deleting
            await command.ExecuteNonQueryAsync();
        }
    }
}
