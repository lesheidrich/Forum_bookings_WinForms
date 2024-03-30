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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SZOP_Beadando
{
    public partial class BookingsView : Form
    {
        RestClient bookingsClient = new RestClient("http://localhost:3000/api/bookings/");
        private Login login;
        private string un;
        private string pw;
        private int adm;
        private int clientChangeCnt;
        private int serverChangeCnt;

        public BookingsView(Login login, string username, string password, int adm)
        {
            this.login = login;
            this.un = username;
            this.pw = password;
            this.adm = adm;

            clientChangeCnt = 0;
            serverChangeCnt = ChangeCount.getChangeCount(un, pw);
            if (serverChangeCnt == -1)
            {
                MessageBox.Show("Change count error, please refresh manually or restart the server!");
            }

            InitializeComponent();
            InitializeDGBookings();
            RefreshDGBookings();

            lblWelcome.Text = un;
        }

        public void InitializeDGBookings()
        {
            dgBookings.Columns.Add("id", "Id");                     dgBookings.Columns["id"].Width = 23;
            dgBookings.Columns.Add("name", "Name");                 dgBookings.Columns["name"].Width = 98; 
            dgBookings.Columns.Add("phone", "Phone");               dgBookings.Columns["phone"].Width = 88; 
            dgBookings.Columns.Add("email", "Email");               dgBookings.Columns["email"].Width = 159; 
            dgBookings.Columns.Add("bookingDate", "Booking Date");  dgBookings.Columns["bookingDate"].Width = 80; 
        }

        public void RefreshDGBookings()
        {
            RestClient bookingsClient = new RestClient("http://localhost:3000/api/bookings");
            RestRequest req = new RestRequest();

            try
            {
                RestResponse res = bookingsClient.Get(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ResponseBookings r = bookingsClient.Deserialize<ResponseBookings>(res).Data;
                    if (r.Error != 0)
                    {
                        MessageBox.Show(r.Message);
                    }
                    else
                    {
                        dgBookings.Rows.Clear();
                        foreach (Booking b in r.Bookings)
                        {
                            dgBookings.Rows.Add(
                                b.Id,
                                b.Name,
                                b.Phone,
                                b.Email,
                                b.BookingDate
                                );
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearContent()
        {
            tbId.Clear();
            tbName.Clear();
            tbPhone.Clear();
            tbEmail.Clear();
            tbBookingDate.Clear();
            tbId.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            login.Show();
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDGBookings();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            bookingsClient = new RestClient("http://localhost:3000/api/bookings/" + tbId.Text);
            RestRequest req = new RestRequest();
            req.AddParameter("username", un);
            req.AddParameter("password", pw);
            //req.AddBody(
            //    new
            //    {
            //        username = un,
            //        password = pw,
            //        //id = int.Parse(tbId.Text)
            //    }
            //);
            try
            {
                RestResponse res = bookingsClient.Delete(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription);
                }
                else
                {
                    ResponseBookings r = bookingsClient.Deserialize<ResponseBookings>(res).Data;
                    if (r.Error == 0)
                    {
                        RefreshDGBookings();
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
            RestRequest req = new RestRequest();
            req.AddBody(new
            {
                username = un,
                password = pw,
                name = tbName.Text,
                phone = tbPhone.Text,
                email = tbEmail.Text,
                bookingDate = tbBookingDate.Text
            });

            try
            {
                RestResponse res = bookingsClient.Post(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ResponseBookings r = bookingsClient.Deserialize<ResponseBookings>(res).Data;
                    if (r.Error == 0)
                    {
                        RefreshDGBookings();
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
            RestRequest req = new RestRequest();
            req.AddBody(new
            {
                username = un,
                password = pw,
                id = tbId.Text,
                name = tbName.Text,
                phone = tbPhone.Text,
                email = tbEmail.Text,
                bookingDate = tbBookingDate.Text
            });

            try
            {
                RestResponse res = bookingsClient.Put(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ResponseBookings r = bookingsClient.Deserialize<ResponseBookings>(res).Data;
                    if (r.Error == 0)
                    {
                        RefreshDGBookings();
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

        private void dgBookings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dgBookings.CurrentRow.Selected = true;
                tbId.Text = dgBookings.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                tbName.Text = dgBookings.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                tbPhone.Text = dgBookings.Rows[e.RowIndex].Cells["Phone"].Value.ToString();
                try
                {
                    tbEmail.Text = dgBookings.Rows[e.RowIndex].Cells["Email"].Value.ToString();
                }
                catch (NullReferenceException)
                {
                    tbEmail.Text = "";
                }

                tbBookingDate.Text = dgBookings.Rows[e.RowIndex].Cells["BookingDate"].Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void refreshBy(string param)
        {
            RestClient bookingsClient = new RestClient();
            if (param.Equals("id"))
            {
                bookingsClient = new RestClient("http://localhost:3000/api/bookings/id/" + tbId.Text);
            }
            else if (param.Equals("name"))
            {
                bookingsClient = new RestClient("http://localhost:3000/api/bookings/name/" + tbName.Text);
            }
            else
            {
                bookingsClient = new RestClient("http://localhost:3000/api/bookings/bookingDate/" + tbBookingDate.Text);
            }

            
            RestRequest req = new RestRequest();

            try
            {
                RestResponse res = bookingsClient.Get(req);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(res.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ResponseBookings r = bookingsClient.Deserialize<ResponseBookings>(res).Data;
                    if (r.Error != 0)
                    {
                        MessageBox.Show(r.Message);
                    }
                    else
                    {
                        dgBookings.Rows.Clear();
                        foreach (Booking b in r.Bookings)
                        {
                            dgBookings.Rows.Add(
                                b.Id,
                                b.Name,
                                b.Phone,
                                b.Email,
                                b.BookingDate
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

        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Back || char.IsLetterOrDigit((char)e.KeyCode))
            //{
            //    refreshByName();
            //}
            if (tbName.Text.Length == 0)
            {
                RefreshDGBookings();
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                refreshBy("name");
            }
        }

        private void tbId_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbId.Text.Length == 0)
            {
                RefreshDGBookings();
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                refreshBy("id");
            }
        }

        private void tbBookingDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbBookingDate.Text.Length == 0)
            {
                RefreshDGBookings();
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                refreshBy("date");        
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            UsersView userView = new UsersView(this, login, un, pw, adm);
            userView.Show();
            this.Hide();
        }

        private void lblClear_Click(object sender, EventArgs e)
        {
            clearContent();
        }

        private void BookingsViewTimer_Tick(object sender, EventArgs e)
        {
            serverChangeCnt = ChangeCount.getChangeCount(un, pw);
            if (clientChangeCnt != serverChangeCnt)
            {
                RefreshDGBookings();
                clientChangeCnt = serverChangeCnt;
                //MessageBox.Show("Change counter has been reset!");
            }

        }

        private void BookingsView_FormClosing(object sender, FormClosingEventArgs e)
        {
            login.Show();
        }
    }//end Bookingsview form
}
