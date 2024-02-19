﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.Registration
{
    public class RegisterCustomerEntity
    {
        public string FullName { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int InitialAmount { get; set; }
    }
}
