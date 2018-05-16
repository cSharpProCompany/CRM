﻿using CRM.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
	public class CreateLeadViewModel : IUser
	{
		public int Id { get; set; }

		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		public string Name
		{
			get => $"{FirstName} {LastName}";
			set
			{
				var name = value.Split(' ');
				FirstName = name[0];
				LastName = name[1];
			}
		}

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Display(Name = "Phone Number")]
		public string Phone { get; set; }

		[Display(Name = "Message")]
		public string Message { get; set; }


	}
}