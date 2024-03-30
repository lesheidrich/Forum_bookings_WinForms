using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;

namespace SZOP_Beadando
{
    public partial class Login : Form
    {
        RestClient loginClient = new RestClient("http://localhost:3000/api");
        public Login()
        {
            InitializeComponent();
        }

        private void clearContents()
        {
            tbUsername.Clear();
            tbPassword.Clear();
            tbUsername.Focus();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            RestRequest req = new RestRequest();
            req.AddParameter("username", tbUsername.Text);
            req.AddParameter("password", tbPassword.Text);

            try
            {
                RestResponse res = loginClient.Get(req);
                //res.StatusCode //visszaadja result http kodot
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "HTTP Request Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ResponseUsers r = loginClient.Deserialize<ResponseUsers>(res).Data;
                    if (r.Error == 1)
                    {
                        MessageBox.Show(r.Message, "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        //MessageBox.Show("Welcome " + tbUsername.Text + "\n" + r.Message);

                        if (r.IsAdmin == 0)
                        {
                            BookingsView bookingsView = new BookingsView(this, tbUsername.Text, tbPassword.Text, r.IsAdmin);
                            bookingsView.Show();
                        }
                        else
                        {
                            UsersView adminView = new UsersView(this, tbUsername.Text, tbPassword.Text, r.IsAdmin);
                            adminView.Show();
                        }
                        this.Hide();
                        clearContents();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }         
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearContents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                BtnLogin_Click(this, new EventArgs());
            }
        }

        private void tbUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(this, new EventArgs());
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (tbPassword.PasswordChar== '*')
            {
                tbPassword.PasswordChar = '\0';
            }
            else
            {
                tbPassword.PasswordChar = '*';
            }
            
        }
    }
}
