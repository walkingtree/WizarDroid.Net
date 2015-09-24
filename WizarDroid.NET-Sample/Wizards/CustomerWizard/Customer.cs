using System;

namespace WizarDroid.NET_Sample.Wizards
{
    public class Customer
    {
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
    }

    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
