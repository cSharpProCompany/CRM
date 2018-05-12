﻿using CRMData.Contexts;
using CRMData.Entities;
using System;

namespace CRMData.Repository
{
	public interface IUnitOfWork : IDisposable
	{
		BaseContext Context { get; }
		IGenericRepository<Lead> LeadsRepository { get; }
		IGenericRepository<Note> NotesRepository { get; }
		IGenericRepository<Address> AddressRepository { get; }
		IGenericRepository<DAddressType> DAddressTypesRepository { get; }
		IGenericRepository<DPhoneType> DPhonesTypesRepository { get; }
		IGenericRepository<DUserType> DUserTypesRepository { get; }
		IGenericRepository<Email> EmailsRepository { get; }
		IGenericRepository<LeadConvertedLog> LeadsConvertedLogsRepositoryRepository { get; }
		IGenericRepository<Phone> PhonesRepository { get; }
		IGenericRepository<User> UsersRepository { get; }
		IGenericRepository<Call> CallsRepository { get; }
		void Save();
		void Dispose(bool disposing);
	}
}