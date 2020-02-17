using Microsoft.EntityFrameworkCore;
using CoinWallet.DomainModel.DataModel;
using System;
using System.Diagnostics;

namespace CoinWallet.DomainModel
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		   : base(options)
		{
		}

		public DbSet<Wallet> Wallet { get; set; }
	}
}