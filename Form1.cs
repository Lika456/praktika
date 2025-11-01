using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // строка подключения
        string connectionString =
            @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Производство.mdb;";

        // сохранённая роль и ID пользователя
        private readonly string _role;
        private readonly int _userId;

        // конструкторы
        public Form1() : this("user", 0) { }

        public Form1(string role, int userId)
        {
            InitializeComponent();
            _role = role;
            _userId = userId;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Учёт транзакций";
        }

        // при загрузке формы блокируем редактирование у user
        private void Form1_Load(object sender, EventArgs e)
        {
            bool isAdmin = !string.IsNullOrWhiteSpace(_role) &&
                           _role.Equals("admin", StringComparison.OrdinalIgnoreCase);

            // запретить редактирование для пользователей
            dataGridView1.ReadOnly = !isAdmin;
            dataGridView1.AllowUserToAddRows = isAdmin;
            dataGridView1.AllowUserToDeleteRows = isAdmin;
            dataGridView1.EditMode = isAdmin
                ? DataGridViewEditMode.EditOnKeystrokeOrF2
                : DataGridViewEditMode.EditProgrammatically;

            // скрыть кнопку сохранения, если не админ
            buttonSave1.Visible = isAdmin;
        }

        // ====== кнопка 1: Показать данные ======
        private void button1_Click(object sender, EventArgs e)
        {
            bool isAdmin = _role.Equals("admin", StringComparison.OrdinalIgnoreCase);
            string sql;

            if (isAdmin)
            {
                // админ видит все транзакции с логинами пользователей
                sql = @"SELECT t.TranzactionID, t.CategoryID, t.UserID,  u.Login, c.CategoryName, t.TransDate, 
                               t.Amount, t.Discription
                        FROM ((Транзакции AS t
                               INNER JOIN Пользователи AS u ON t.UserID = u.UserID)
                               INNER JOIN Категории AS c ON t.CategoryID = c.CategoryID);";
            }
            else
            {
                // обычный пользователь видит только свои
                sql = $@"SELECT t.TranzactionID, c.CategoryName, t.TransDate, 
                                t.Amount, t.Discription
                         FROM (Транзакции AS t
                               INNER JOIN Категории AS c ON t.CategoryID = c.CategoryID)
                         WHERE t.UserID = {_userId};";
            }

            // загрузка данных
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        // ====== кнопка 2: Сохранить изменения (только для admin) ======
        private void button2_Click(object sender, EventArgs e)
        {
            if (!_role.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Изменять данные может только администратор.",
                                "Доступ запрещён",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string select = "SELECT * FROM [Транзакции]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(select, conn);
                    OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);

                    DataTable dt = (DataTable)dataGridView1.DataSource;
                    adapter.Update(dt);

                    MessageBox.Show("Изменения сохранены!", "БД",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
    }
}