using System.Data.Entity;
using ContractorFilesBrowser.Model;

namespace ContractorFilesBrowser.Data
{
    //!Klasa umożliwiająca komunikację z bazą danych.
    public class ContractorContext : DbContext
    {
        public ContractorContext()
        {
            Database.SetInitializer(new ContractorDatabaseInitializer());
        }

        //!Tablica przechowująca kontrahentów.
        public DbSet<Contractor> Contractors { get; set; }
        //!Tablica przechowująca faktury.
        public DbSet<ContractorOrder> Orders { get; set; }
        //!Tablica przechowująca kraje.
        public DbSet<Country> Countries { get; set; }
        //!Tablica przechowująca województwa/prowincje.
        public DbSet<Province> Provinces { get; set; }
    }
}