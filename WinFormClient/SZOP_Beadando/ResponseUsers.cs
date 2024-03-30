using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZOP_Beadando
{
    public class ResponseUsers
    {

        private int error;

        public int Error
        {
            get { return error; }
            set { error = value; }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private int isAdmin;

        public int IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }

        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }


        private List<User> users;

        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }
    }
}


