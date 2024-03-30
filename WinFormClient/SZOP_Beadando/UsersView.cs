using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SZOP_Beadando
{
    public partial class UsersView : Form
    {
        private Login login;
        private BookingsView bookingsView;
        private string un;
        private string pw;
        private int izAdmin;
        private int curid;
        private int clientChangeCnt;
        private int serverChangeCnt;

        public UsersView(Login login, string username, string password, int adm)
        {
            this.login = login;
            this.un = username;
            this.pw = password;
            this.izAdmin = adm;
            curid = getUserId();

            clientChangeCnt = 0;
            serverChangeCnt = ChangeCount.getChangeCount(un, pw);
            //MessageBox.Show("Client chg cnt: " + clientChangeCnt + "\nserverchangecnt: " + serverChangeCnt  );
            if (serverChangeCnt == -1) 
            {
                MessageBox.Show("Change count error, please refresh manually or restart the server!");
            }

            InitializeComponent();
            InitializeDGUsers();
            RefreshDGBUsers();
            btnBack.Hide();

            lblWelcome.Text = un;
        }

        public UsersView(BookingsView bookingsView, Login login, string username, string password, int adm)
        {
            this.bookingsView = bookingsView;
            this.login = login;
            this.un = username;
            this.pw = password;
            this.izAdmin = adm;
            curid = getUserId();
            if (curid == -1)
            {
                MessageBox.Show("Invalid login!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            clientChangeCnt = 0;
            serverChangeCnt = ChangeCount.getChangeCount(un, pw);
            if (serverChangeCnt == -1)
            {
                MessageBox.Show("Change count error, please refresh manually or restart the server!");
            }

            InitializeComponent();
            InitializeDGUsers();
            RefreshDGBUsers();
            btnInsert.Hide();
            btnDelete.Hide();
            tbId.Enabled = false;
            tbIsAdmin.Enabled = false;
            btnUpdate.Location = new Point(535, 270);

            lblWelcome.Text = un;
        }

        public void InitializeDGUsers()
        {
            dgUsers.Columns.Add("id", "Id");                dgUsers.Columns["id"].Width = 50;
            dgUsers.Columns.Add("username", "Username");    dgUsers.Columns["username"].Width = 125;
            dgUsers.Columns.Add("password", "Password");    dgUsers.Columns["password"].Width = 135;
            dgUsers.Columns.Add("isAdmin", "IsAdmin");      dgUsers.Columns["isAdmin"].Width = 85;
        }

        public void RefreshDGBUsers()
        {
            tbPassword.Clear();
            RestClient userClient = new RestClient("http://localhost:3000/api/users");

            if (izAdmin == 0)
                {
                    userClient = new RestClient("http://localhost:3000/api/user");
                }

            RestRequest req = new RestRequest();
            if (izAdmin == 0)
            {
                req.AddParameter("id", curid.ToString());
            }          
            try
            {
                RestResponse res = userClient.Get(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ResponseUsers r = userClient.Deserialize<ResponseUsers>(res).Data;
                    if (r.Error != 0)
                    {
                        MessageBox.Show(r.Message);
                    }
                    else
                    {
                        dgUsers.Rows.Clear();
                        foreach (User u in r.Users)
                        {
                            dgUsers.Rows.Add(
                                    u.Id,
                                    u.Username,
                                    u.Password,
                                    u.IsAdmin
                                );
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //clearContent();
        }

        private int getUserId()
        {
            int uid = -1;
            RestClient userClient = new RestClient("http://127.0.0.1:3000/api/usersid");
            RestRequest req = new RestRequest();
            req.AddParameter("username", un);
            req.AddParameter("password", pw);
            try
            {
                RestResponse res = userClient.Get(req);
                ResponseUsers r = userClient.Deserialize<ResponseUsers>(res).Data;
                //MessageBox.Show(r.ID.ToString());
                //MessageBox.Show((r.ID is int).ToString());
                uid = r.ID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return uid;
        }

        //public int getChangeCount()
        //{
        //    int changeCount = -1;
        //    RestClient userClient = new RestClient("http://127.0.0.1:3000/api/cnt");
        //    RestRequest req = new RestRequest();
        //    req.AddParameter("username", un);
        //    req.AddParameter("password", pw);
        //    try
        //    {
        //        RestResponse res = userClient.Get(req);
        //        ResponseUsers r = userClient.Deserialize<ResponseUsers>(res).Data;
        //        //MessageBox.Show(r.ID.ToString());
        //        //MessageBox.Show((r.ID is int).ToString());
        //        changeCount = r.ID;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    return changeCount;
        //}

        private void btnExit_Click(object sender, EventArgs e)
        {
            login.Show();
            this.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            bookingsView.Show();
            this.Close();
        }

        private void dgUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dgUsers.CurrentRow.Selected = true;
                tbId.Text = dgUsers.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                tbUsername.Text = dgUsers.Rows[e.RowIndex].Cells["Username"].Value.ToString();
                //tbPassword.Text = dgUsers.Rows[e.RowIndex].Cells["Password"].Value.ToString();
                tbIsAdmin.Text = dgUsers.Rows[e.RowIndex].Cells["IsAdmin"].Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void clearContent()
        {
            tbId.Clear();
            tbUsername.Clear();
            tbPassword.Clear();
            tbIsAdmin.Clear();
            tbId.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (Int32.Parse(tbId.Text) == curid)
            {
                MessageBox.Show("Admins cannot delete themselves!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RestClient userClient = new RestClient("http://localhost:3000/api/users/" + tbId.Text);
            RestRequest req = new RestRequest();
            req.AddParameter("username", un);
            req.AddParameter("password", pw);

            try
            {
                RestResponse res = userClient.Delete(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription);
                }
                else
                {
                    ResponseBookings r = userClient.Deserialize<ResponseBookings>(res).Data;
                    if (r.Error == 0)
                    {
                        RefreshDGBUsers();
                    }
                    MessageBox.Show(r.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            clearContent();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (tbIsAdmin.Text.Length == 0) 
            {
                tbIsAdmin.Text = "0";
            }

            RestClient userClient = new RestClient("http://localhost:3000/api/users");
            RestRequest req = new RestRequest();
            req.AddBody(new
            {
                username = un,
                password = pw,
                user = tbUsername.Text,
                pwd = tbPassword.Text,
                isAdmin = tbIsAdmin.Text,
            });

            try
            {
                RestResponse res = userClient.Post(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ResponseBookings r = userClient.Deserialize<ResponseBookings>(res).Data;
                    if (r.Error == 0)
                    {
                        RefreshDGBUsers();
                    }
                    MessageBox.Show(r.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            clearContent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            RestClient userClient = new RestClient("http://localhost:3000/api/users");
            if (izAdmin == 0)
            {
                userClient = new RestClient("http://localhost:3000/api/user");
            }

            RestRequest req = new RestRequest();

            req.AddBody(new
            {
                username = un,
                password = pw,
                id = tbId.Text,
                user = tbUsername.Text,
                pwd = tbPassword.Text,
                isAdmin = tbIsAdmin.Text
            });

            try
            {
                RestResponse res = userClient.Put(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ResponseBookings r = userClient.Deserialize<ResponseBookings>(res).Data;
                    if (r.Error == 0)
                    {
                        RefreshDGBUsers();
                    }
                    MessageBox.Show(r.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            clearContent();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (tbPassword.PasswordChar == '*')
            {
                tbPassword.PasswordChar = '\0';
            }
            else
            {
                tbPassword.PasswordChar = '*';
            }
        }

        private void refreshBy(string param)
        {
            RestClient usersClient = new RestClient();
            if (param.Equals("id"))
            {
                usersClient = new RestClient("http://localhost:3000/api/users/id/" + tbId.Text);
            }
            else if (param.Equals("name"))
            {
                usersClient = new RestClient("http://localhost:3000/api/users/user/" + tbUsername.Text);
            }
            else
            {
                usersClient = new RestClient("http://localhost:3000/api/users/isAdmin/" + tbIsAdmin.Text);
            }


            RestRequest req = new RestRequest();

            try
            {
                RestResponse res = usersClient.Get(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ResponseUsers r = usersClient.Deserialize<ResponseUsers>(res).Data;
                    if (r.Error != 0)
                    {
                        MessageBox.Show(r.Message);
                    }
                    else
                    {
                        dgUsers.Rows.Clear();
                        foreach (User u in r.Users)
                        {
                            dgUsers.Rows.Add(
                                u.Id,
                                u.Username,
                                u.Password,
                                u.IsAdmin
                                );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbId_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbId.Text.Length == 0)
            {
                RefreshDGBUsers();
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                refreshBy("id");
            }
        }

        private void tbUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbUsername.Text.Length == 0)
            {
                RefreshDGBUsers();
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                refreshBy("name");
            }
        }

        private void tbIsAdmin_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbIsAdmin.Text.Length == 0)
            {
                RefreshDGBUsers();
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                refreshBy("isAdmin");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDGBUsers();
        }

        private void lblClear_Click(object sender, EventArgs e)
        {
            clearContent();
        }

        private void UserViewTimer_Tick(object sender, EventArgs e)
        {
            serverChangeCnt = ChangeCount.getChangeCount(un, pw);
            if (clientChangeCnt != serverChangeCnt) 
            {
                RefreshDGBUsers();
                clientChangeCnt = serverChangeCnt;
                //MessageBox.Show("Change counter has been reset!");
            }
        }

        private void UsersView_FormClosing(object sender, FormClosingEventArgs e)
        {
            //login.Show();
        }
    }
}
