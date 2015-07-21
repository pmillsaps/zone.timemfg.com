using System;
using Orchard.Data;
using Orchard.Data.Migration;
using Ventajou.ActiveDirectory.Models;

namespace Ventajou.ActiveDirectory
{
	public class Migrations : DataMigrationImpl
	{
		private readonly IRepository<SettingsRecord> _repository;

		public Migrations(IRepository<SettingsRecord> repository)
		{
			_repository = repository;
		}

		public int Create()
		{
			SchemaBuilder.CreateTable("DomainRecord", table => table
					.Column<int>("Id", column=>column.PrimaryKey().Identity())
					.Column<string>("Name", column => column.PrimaryKey().NotNull())
					.Column<string>("UserName")
					.Column<string>("Password")
				);

			SchemaBuilder.CreateTable("SettingsRecord", table => table
					.Column<int>("Id", column => column.PrimaryKey().Identity())
					.Column<string>("DefaultDomain")
				);

			if (_repository == null)
				throw new InvalidOperationException("Couldn't find settings repository.");

			_repository.Create(new SettingsRecord
			{
				DefaultDomain = null
			});

			return 1;
		}
	}
}