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
            //FormClosing += (sender, args) => { DeleteSaves(); };
            FormClosing += (sender, args) =>
            {
                DeleteSaves();
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
                button.Enabled = false;
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
            tableLayout.Controls.Add(loadingTextLabel, 1, savesCount);
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
            int i = 0;
            for (; await sqlReader.ReadAsync() && i < savesCount; ++i)
            {
                var id = Convert.ToString(sqlReader["Id"]);
                var fieldState = Convert.ToString(sqlReader["FieldState"]);
                var mode = Convert.ToString(sqlReader["GameMode"]);
                var turn = Convert.ToString(sqlReader["CurrentTurn"]);
                var date = Convert.ToString(sqlReader["Date"]);
                saves.Add(new SaveData(id, fieldState, mode, turn, date));
                int x = i;// var needed to escape closure
                int saveId = saves[i].Id;
                if (fieldState != null && mode != null && turn != null && date != null)
                {
                    saveLabels[i].Text = GetSaveInfo(saves[i]);
                    loadButtons[i].Enabled = true;
                    loadButtons[i].Click += (sender, args) => { SelectedSave = saves[x]; };
                    loadButtons[i].Click += (sender, args) => { Close(); };
                    deleteButtons[i].Enabled = true;
                    deleteButtons[i].Click += (sender, args) =>
                    {
                        loadButtons[x].Enabled = false;
                        saveLabels[x].Text = "";
                        deleteButtons[x].Enabled = false;
                        deletingSaveNumbers.Add(saveId);
                    };
                }
                saveButtons[i].Enabled = true;
                saveButtons[i].Text = "Rewrite";
                saveButtons[i].Click += (sender, args) => { RewriteSave(saveId, SelectedSave); };
                saveButtons[i].Click += (sender, args) =>
                {
                    saveLabels[x].Text = GetSaveInfo(SelectedSave);
                };
            }
            AddEmptySaveButtons(i);
        }

        private void AddEmptySaveButtons(int index)
        {
            for (int i = index; i < savesCount; ++i)
            {
                int x = i;// var needed to escape closure
                saveButtons[i].Enabled = true;
                if (x != 0)
                    saveButtons[i].Click += (sender, args) =>
                    {
                        saves.Add(SelectedSave);
                        SelectedSave = new SaveData(SelectedSave.Field,
                            SelectedSave.GameMode, SelectedSave.CurrentTurn, saves[saves.Count - 1].Id + 1);
                    };
                saveButtons[i].Click += (sender, args) =>
                {
                    saveLabels[x].Text = GetSaveInfo(SelectedSave);
                };
                saveButtons[i].Click += (sender, args) => { WriteNewSave(SelectedSave); };
            }
        }

        private async void WriteNewSave(SaveData save) 
        {
            loadingTextLabel.Text = "Saving in progress, please wait";
            var command = new SqlCommand("INSERT INTO [GameSaves] (CurrentTurn, Date, FieldState, GameMode)VALUES(@CurrentTurn, @Date, @FieldState, @GameMode)", sqlConnection);
            command.Parameters.AddWithValue("CurrentTurn", save.GetCurrentTurnString());
            command.Parameters.AddWithValue("Date", save.Date);
            command.Parameters.AddWithValue("GameMode", save.GetGameModeString());
            command.Parameters.AddWithValue("FieldState", save.GetFieldString());
            await command.ExecuteNonQueryAsync();
            loadingTextLabel.Text = "";
        }

        private async void RewriteSave(int id, SaveData save)
        {
            loadingTextLabel.Text = "Saving in progress, please wait";
            var command = new SqlCommand("UPDATE [GameSaves] SET [CurrentTurn]=@CurrentTurn, [Date]=@Date, [FieldState]=@FieldState, [GameMode]=@GameMode WHERE [Id]=@Id", sqlConnection);
            command.Parameters.AddWithValue("CurrentTurn", save.GetCurrentTurnString());
            command.Parameters.AddWithValue("Date", save.Date);
            command.Parameters.AddWithValue("GameMode", save.GetGameModeString());
            command.Parameters.AddWithValue("FieldState", save.GetFieldString());
            command.Parameters.AddWithValue("Id", id);
            if (deletingSaveNumbers.Contains(id)) 
                deletingSaveNumbers.Remove(id);
            await command.ExecuteNonQueryAsync();
            loadingTextLabel.Text = "";
        }

        private void DeleteSaves() 
        {
            foreach (var id in deletingSaveNumbers) 
            {
                DeleteSave(id);
            }
        }

        private void DeleteSave(int id)
        {
            var command = new SqlCommand("DELETE FROM [GameSaves] WHERE [Id] = @Id", sqlConnection);
            command.Parameters.AddWithValue("Id", id);
            command.ExecuteNonQuery();
        }

        private string GetSaveInfo(SaveData save) 
        {
            return save.Id.ToString() + " " + save.GetGameModeString() + 
                " cur. turn: " + save.GetCurrentTurnString() + " " + save.Date;
        }
    }
}
