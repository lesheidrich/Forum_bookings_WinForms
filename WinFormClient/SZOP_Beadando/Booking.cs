using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZOP_Beadando
{
    public class Booking
    {
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string phone;

		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}

		private string email;

		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		private string bookingDate;

		public string BookingDate
		{
			get { return bookingDate; }
			set { bookingDate = value; }
		}
		public Booking(int id, string name, string phone, string email, string bookingDate)
		{
			this.id = id;
			this.name = name;
			this.phone = phone;
			this.email = email;
			this.bookingDate = bookingDate;
		}

	}
}
