using ContractorFilesBrowser.Model;
using CsvHelper;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ContractorFilesBrowser.Data
{
    //!Klasa odpowiedzialna za inicjalizację bazy danych oraz zapełnienie jej przykładowymi danymi.
    public class ContractorDatabaseInitializer : CreateDatabaseIfNotExists<ContractorContext>
    {
        //!Metoda zapełniająca bazę danych przykładowymi danymi.
        protected override void Seed(ContractorContext context)
        {
            base.Seed(context);
            Assembly assembly = Assembly.GetExecutingAssembly();

            SeedCountries(assembly, context);
            SeedProvinces(assembly, context);
            context.SaveChanges();

            SeedContractors(assembly, context); 
            SeedOrders(assembly, context);
            context.SaveChanges();
        }

        //!Metoda wczytująca z pliku dane dotyczące krajów.
        private void SeedCountries(Assembly assembly, ContractorContext context)
        {
            string resourceName = "ContractorFilesBrowser.Data.SeederFiles.Countries.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                    var countries = csvReader.GetRecords<Country>().ToArray();
                    context.Countries.AddOrUpdate(countries);
                }
            }
        }

        //!Metoda wczytująca z pliku dane dotyczące województw/prowincji.
        private void SeedProvinces(Assembly assembly, ContractorContext context)
        {
            string resourceName = "ContractorFilesBrowser.Data.SeederFiles.Provinces.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csvReader.Read();
                    csvReader.ReadHeader();
                    while (csvReader.Read())
                    {
                        Province province = new Province
                        {
                            ProvinceName = csvReader.GetField<string>("ProvinceName"),
                            CountryId = csvReader.GetField<int>("CountryId")
                        };
                        context.Provinces.AddOrUpdate(province);
                    }
                }
            }
        }

        //!Metoda wczytująca z pliku dane dotyczące kontrahentów.
        private void SeedContractors(Assembly assembly, ContractorContext context)
        {
            string resourceName = "ContractorFilesBrowser.Data.SeederFiles.Contractors.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csvReader.Read();
                    csvReader.ReadHeader();
                    while (csvReader.Read())
                    {
                        Contractor contractor = new Contractor
                        {
                            Name = csvReader.GetField<string>("Name"),
                            EmailAddress = csvReader.GetField<string>("EmailAddress"),
                            PhoneNumber = csvReader.GetField<string>("PhoneNumber"),
                            Street = csvReader.GetField<string>("Street"),
                            City = csvReader.GetField<string>("City"),
                            ZipCode = csvReader.GetField<string>("ZipCode"),
                            ProvinceId = csvReader.GetField<int>("ProvinceId"),
                            CompanyName = csvReader.GetField<string>("CompanyName"),
                            NipCode = csvReader.GetField<string>("NipCode"),
                            ContractorType = csvReader.GetField<ContractorType>("ContractorType")
                        };
                        context.Contractors.AddOrUpdate(contractor);
                    }
                }
            }
        }

        //!Metoda wczytująca z pliku dane dotyczące faktur.
        private void SeedOrders(Assembly assembly, ContractorContext context)
        {
            string resourceName = "ContractorFilesBrowser.Data.SeederFiles.Orders.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csvReader.Read();
                    csvReader.ReadHeader();
                    while (csvReader.Read())
                    {
                        ContractorOrder order = new ContractorOrder
                        {
                            BasePrice = csvReader.GetField<decimal>("BasePrice"),
                            Currency = csvReader.GetField<string>("Currency"),
                            TaxRate = csvReader.GetField<decimal>("TaxRate"),
                            IssueDate = DateTime.Today,
                            SaleDate = DateTime.Today,
                            PaymentDueDate = DateTime.Today,
                            Description = csvReader.GetField<string>("Description"),
                            OrderType = csvReader.GetField<OrderType>("OrderType"),
                            ContractorId = csvReader.GetField<int>("ContractorId")
                        };
                        context.Orders.AddOrUpdate(order);
                    }
                }
            }
        }
    }
}
