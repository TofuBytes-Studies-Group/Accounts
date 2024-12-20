﻿namespace Accounts.Core.Entities
{
    public class ContactInfo
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        
        public Guid AddressId { get; set; }

        public Address Address { get; set; }
    }
}