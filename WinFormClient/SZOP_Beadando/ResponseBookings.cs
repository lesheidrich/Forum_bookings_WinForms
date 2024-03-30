using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZOP_Beadando
{
    public class ResponseBookings
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

        private List<Booking> bookings;

        public List<Booking> Bookings
        {
            get { return bookings; }
            set { bookings = value; }
        }



    }
}
