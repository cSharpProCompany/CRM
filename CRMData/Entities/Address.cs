﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.DAL.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }

        [Column("DAddressTypeId")]
        public int? AddressTypeId { get; set; }
        public virtual DAddressType AddressType { get; set; }

	    public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
