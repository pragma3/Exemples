using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ActionFilterMVC.Models
{
    public class Client
    {
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public string ClientNumber { get; set; }
        public int ClientID { get; set; }
        [DataType(DataType.Text)]
        public string StreetAddress { get; set; }
        [DataType(DataType.Text)]
        public string PostCode { get; set; }
        [DataType(DataType.Text)]
        public string State { get; set; }
        [DataType(DataType.Text)]
        public string Country { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}