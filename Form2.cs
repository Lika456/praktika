using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class LoginForm : Form
    {
        string connectionString =
            @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Производство.mdb;";

        public LoginForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Авторизация";
        }

        // Вход
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT UserID, Role FROM [Пользователи] " +
                                   "WHERE [Login]=@login AND [Password]=@pass";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@login", textBoxLogin.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", textBoxPassword.Text.Trim());

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = Convert.ToInt32(reader["UserID"]);
                            string role = reader["Role"].ToString().Trim().ToLower();

                            MessageBox.Show($"Добро пожаловать, {textBoxLogin.Text}!",
                                            "Вход выполнен", MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);

                            this.Hide();
                            Form1 main = new Form1(role, userId);
                            main.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль!",
                                            "Ошибка", MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения: " + ex.Message);
                }
            }
        }

        // Регистрация
        private void buttonRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 reg = new Form3();
            reg.ShowDialog();
            this.Show();
        }
    }
}