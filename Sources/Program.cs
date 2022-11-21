using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;
using ContractorFilesBrowser.Model;
using ContractorFilesBrowser.View;
using CsvHelper;
using ContractorFilesBrowser.Data;

namespace ContractorFilesBrowser
{
    //!Klasa stanowiąca punkt wejściowy do programu.
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
            //using(ContractorContext context = new ContractorContext())
            //{

            //    Assembly assembly = Assembly.GetExecutingAssembly();
            //    string resourceName = "ContractorFilesBrowser.Data.SeederFiles.Countries.csv";
            //    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //    {
            //        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            //        {
            //            CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            //            //csvReader.Configuration.WillThrowOnMissingField = false;
            //            var countries = csvReader.GetRecords<Country>();//.ToList();
            //            context.Countries.AddOrUpdate(countries);
            //        }
            //    }
            //}
        }
    }
}