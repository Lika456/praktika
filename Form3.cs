using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        string connectionString =
            @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Производство.mdb;";

        public Form3()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Регистрация";

            textBoxPassword.UseSystemPasswordChar = true;
            textBoxConfirm.UseSystemPasswordChar = true;
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string pass = textBoxPassword.Text.Trim();
            string confirm = textBoxConfirm.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string check = "SELECT COUNT(*) FROM [Пользователи] WHERE [Login]=@login";
                    OleDbCommand checkCmd = new OleDbCommand(check, conn);
                    checkCmd.Parameters.AddWithValue("@login", login);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        MessageBox.Show("Такой логин уже существует!", "Регистрация",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string insert = "INSERT INTO [Пользователи] ([Login], [Password], [Role]) " +
                                    "VALUES (@login, @pass, 'user')";
                    OleDbCommand cmd = new OleDbCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@pass", pass);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Регистрация успешна! Теперь войдите.",
                                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
    }
}