﻿using AutoMapper;
using CRM.Models;
using CRM.Services;
using CRMData.Adapters;
using CRMData.Contexts;
using CRMData.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult Index()
        {
            List<CustomerViewModel> customers;
            using (BaseContext context = ContextFactory.SingleContextFactory.Get<BaseContext>())
            {
                customers = Mapper.Map<List<Customer>, List<CustomerViewModel>>(context
                    .Customers
                    .Include("Phones")
                    .ToList());
            }
            return View(customers);
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model)
        {
            model.TableName = "Customers";

            if (!ModelState.IsValid)
            {
                return Json(new { status = "error", message = "Model is not valid!" });
            }

            var customerAdapter = new CustomerAdapter();

            var result = customerAdapter.GetCustomersByFilter(
                model.Field,
                model.SearchValue,
                model.OrderField,
                model.OrderDirection.Equals("ASC"));

            var items = Mapper.Map<List<Customer>, List<CustomerViewModel>>(result);
            return PartialView("_CustomersPagePartial", items);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            CustomerViewModel customer;
            using (BaseContext context = ContextFactory.SingleContextFactory.Get<BaseContext>())
            {
;               Customer customerDb = context.Customers
                    .Include(e => e.Phones)
                    .Include(e => e.Addresses)
                    .Include(e => e.Lead)
                    .FirstOrDefault(c => c.Id == id);

                customer = Mapper.Map<Customer, CustomerViewModel>(customerDb);

                List<Note> customerNote = context.Notes.Where(n => n.LeadId == customerDb.Lead.Id).ToList();
                customerNote.AddRange(context.Notes.Where(n => n.CustomerId == customerDb.Id).ToList());
                customer.Notes = Mapper.Map<List<Note>,List<NoteViewModel>>(customerNote);

                customer.Address = Mapper.Map<List<Address>, List<AddressViewModel>>((List<Address>)customerDb.Addresses);

                if (customer.Address.Count == 0)
                {
                    customer.Address.Add(new AddressViewModel());
                }
            }
            if (customer != null)
            {
                return View(customer);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(CustomerViewModel customer, List<string> note)
        {
            if (ModelState.IsValid)
            {
                using (BaseContext context = ContextFactory.SingleContextFactory.Get<BaseContext>())
                {
                    Customer customerToEdit = context.Customers
                        .Include(e => e.Phones)
                        .FirstOrDefault(c => c.Id == customer.Id);

                    if (customerToEdit != null)
                    {
                        customerToEdit.Title = customer.Title;
                        customerToEdit.Email = customer.Email;
                        customerToEdit.FirstName = customer.FirstName;
                        customerToEdit.LastName = customer.LastName;
                        customerToEdit.Phones
                            .FirstOrDefault()
                            .PhoneNumber = customer.Phones.FirstOrDefault().PhoneNumber;


                        List<Address> addresses = Mapper.Map<List<Address>>(customer.Address);

                        var ids = addresses.Select(a => a.Id).ToList();

                        var currnetAddr = context.Addresses.Where(e => ids.Contains(e.Id)).ToList();

                        if(currnetAddr.Count != 0)
                        {
                            currnetAddr.ForEach(e =>
                            {
                                var item = addresses.FirstOrDefault(a => a.Id == e.Id);

                                e.Line1 = item.Line1;
                                e.Line2 = item.Line2;
                                e.PostCode = item.PostCode;
                                e.Town = item.Town;
                                e.Country = item.Country;
                                e.County = item.County;
                                e.AddressTypeId = item.AddressTypeId;
                                e.AddressType = item.AddressType;
                            });
                        }
                        else
                        {
                            addresses.ForEach(e => customerToEdit.Addresses.Add(e));
                        }
                    }

                    if (customer.Notes.Any())
                    {
                        foreach (NoteViewModel reNewNote in customer.Notes)
                        {
                            Note oldNote = context.Notes.FirstOrDefault(e => e.Id == reNewNote.Id);
                            if(oldNote != null)
                                oldNote.Text = reNewNote.Text;
                        }
                    }

                    if(note != null && note.Any())
                    {
                        context.Notes.AddRange(note.Select(e => new Note() { Text = e, CustomerId = customer.Id }));
                    }

                    context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SendMessage(int id, string message)
        {
            var leadEmail = "";
            var currentUserEmail = User.Identity.Name.Split('|')[1];
            using (BaseContext context = ContextFactory.SingleContextFactory.Get<BaseContext>())
            {
                Customer customer = context.Customers.FirstOrDefault(c => c.Id == id);
                if (customer != null)
                {
                    leadEmail = customer?.Email;
                }
            }
            if (string.IsNullOrEmpty(leadEmail))
            {
                return Json(new { status = "error" });
            }
            EmailService.SendEmail(leadEmail, "Test Message!", message);
            return Json(new { status = "success" });
        }
    }
}