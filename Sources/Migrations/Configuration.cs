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
            base.Seed(context);
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "ContractorFilesBrowser.Data.SeederFiles.Countries.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                    //csvReader.Configuration.WillThrowOnMissingField = false;
                    var countries = csvReader.GetRecords<Country>().ToArray();
                    context.Countries.AddOrUpdate(countries);
                }
            }

            resourceName = "ContractorFilesBrowser.Data.SeederFiles.Provinces.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                    //csvReader.Configuration. = false;
                    //while (csvReader.Read())
                    {
                        //var provinceState = csvReader.GetRecord<Province>();
                        //var countryCode = csvReader.GetField<string>("CountryCode");
                        //context.Provinces.AddOrUpdate(p => p.Code, provinceState);
                    }
                    var provinces = csvReader.GetRecords<Province>().ToArray();
                    context.Provinces.AddOrUpdate(provinces);
                }
            }

            context.SaveChanges();
        }
    }
}
