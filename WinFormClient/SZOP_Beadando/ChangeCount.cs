using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SZOP_Beadando
{
    internal class ChangeCount
    {
        //private int changeCountId;

        //public int ChangeCountId
        //{
        //    get { return changeCountId; }
        //    set { changeCountId = value; }
        //}

        //public ChangeCount(string usn, string pwd)
        //{
        //    this.changeCountId = getChangeCount(usn, pwd);
        //}


        public static int getChangeCount(string username, string password)
        {
            int changeCount = -1;
            RestClient userClient = new RestClient("http://127.0.0.1:3000/api/cnt");
            RestRequest req = new RestRequest();
            req.AddParameter("username", username);
            req.AddParameter("password", password);
            try
            {
                RestResponse res = userClient.Get(req);
                ResponseUsers r = userClient.Deserialize<ResponseUsers>(res).Data;
                //MessageBox.Show(r.ID.ToString());
                //MessageBox.Show((r.ID is int).ToString());
                changeCount = r.ID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return changeCount;
        }
    }
}
