using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookWeb.Models
{
	public class Transaction
	{
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Account Name")]
        public string AccountName { get; set; }

        [Required]
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
    }
}

