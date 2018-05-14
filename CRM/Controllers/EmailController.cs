﻿using AutoMapper;
using CRM.Attributes;
using CRM.DAL.Entities;
using CRM.DAL.Repository;
using CRM.Extentions;
using CRM.Interfaces;
using CRM.Models;
using CRM.Services;
using System;
using System.Web.Mvc;

namespace CRM.Controllers
{
	[Authenticate]
	public class EmailController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public EmailController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpPost]
		public ActionResult SendMessage(int id, string message)
		{
			IUser user = GetUserModel(id);

			if (string.IsNullOrEmpty(user.Email))
			{
				return Json(new { status = "error" });
			}
			if (user is LeadViewModel && user != null)
			{
				SetLeadOwner(user);
			}

			EmailService.SendEmail(new EmailViewModel(user.Email, "CRM Message!", message), user);

			return Json(new { status = "success" });
		}

		public ActionResult Conversation(int id)
		{
			IUser user = GetUserModel(id);
			var mailings = EmailService.LoadMails(user);

			ViewBag.Id = id;
			ViewBag.UserType = (user is LeadViewModel) ? "Lead" : "Customer";

			return View(mailings);
		}

		[HttpPost]
		public ActionResult GetEmails(int id, string type)
		{
			IUser user = GetUserModel(id, type);

			var mailings = EmailService.GetMailings(GetLastReceivedDate(user), user);

			return Json(new { mailings });
		}

		private void SetLeadOwner(IUser model)
		{
			Lead lead = _unitOfWork.LeadsRepository.FindById(model.Id);

			if (lead != null)
			{
				if (lead.LeadOwner == 0)
				{
					var userCreads = User.GetCurrentUserCreads();

					var currentUser = _unitOfWork.UsersRepository.FindBy(u => u.Email == userCreads.Email);

					lead.LeadOwner = currentUser.Id;
					_unitOfWork.Save();
				}
			}
		}

		private User GetCustomerModel(int id)
		{
			User customerModel = _unitOfWork.UsersRepository.FindById(id);

			return customerModel;
		}

		private Lead GetLeadModel(int id)
		{
			Lead leadModel = _unitOfWork.LeadsRepository.FindById(id);

			return leadModel;
		}

		private DateTime? GetLastReceivedDate(IUser user)
		{
			var predicate = user is LeadViewModel ? (Func<Email, bool>)(e => e.LeadId == user.Id) : e => e.UserId == user.Id;
			DateTime? date = _unitOfWork.EmailsRepository.FindBy(predicate).SentDate;

			return date;
		}

		private IUser GetUserModel(int id, string type = "")
		{
			IUser user;
			if (type == "Lead" || HttpContext.Request.UrlReferrer.AbsolutePath.Contains("Lead"))
			{
				Lead lead = GetLeadModel(id);
				user = Mapper.Map<Lead, LeadViewModel>(lead);
			}
			else
			{
				User customer = GetCustomerModel(id);
				user = Mapper.Map<User, UserViewModel>(customer);
			}
			return user;
		}
	}
}