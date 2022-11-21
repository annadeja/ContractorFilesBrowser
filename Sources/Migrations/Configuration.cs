namespace ContractorFilesBrowser.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using CsvHelper;
    using System.IO;
    using System.Globalization;
    using System.Text;
    using ContractorFilesBrowser.Model;

    internal sealed class Configuration : DbMigrationsConfiguration<ContractorFilesBrowser.Data.ContractorContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "ContractorFilesBrowser.Data.ContractorContext";
        }

        protected override void Seed(ContractorFilesBrowser.Data.ContractorContext context)
        {
            
        }
    }
}
