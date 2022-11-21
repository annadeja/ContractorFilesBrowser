using ContractorFilesBrowser.Helpers;

namespace ContractorFilesBrowser.Model
{
    //!Klasa reprezentująca tablicę kontrahentów w bazie danych, wykorzystana do wygenerowania tablicy w ramach metody Code First.
    //Rodzaje danych zostały luźno bazowane na systemie Comarch e-Sklep.
    public class Contractor
    {
        //!Identyfikator kontrahenta.
        public int ContractorId { get; set; }

        //Dane osobowe.

        //!Imię i nazwisko kontrahenta.
        public string Name { get; set; }

        //!Adres email kontrahenta.
        public string EmailAddress { get; set; }

        //!Numer telefonu kontrahenta.
        public string PhoneNumber { get; set; }

        //Dane adresowe.

        //!Ulica adresu kontrahenta.
        public string Street { get; set; }

        //!Miejscowość adresu kontrahenta.
        public string City { get; set; }

        //!Kod pocztowy kontrahenta.
        public string ZipCode { get; set; }

        //!Identyfikator województwa/prowincji kontrahenta.
        public int ProvinceId { get; set; } 
        public virtual Province Province { get; set; } //Słowo kluczowe "virtual" zapewnia lazy loading.

        //Dane dotyczące firmy.

        //!Nazwa firmy kontrahenta.
        public string CompanyName { get; set; }

        //!NIP kontrahenta.
        public string NipCode { get; set; }

        //!Typ kontrahenta.
        public ContractorType ContractorType { get; set; }

        //!Faktury kontahenta. Zapewnia relację One To Many.
        private readonly ObservableListSource<ContractorOrder> orders = new ObservableListSource<ContractorOrder>(); 
        public virtual ObservableListSource<ContractorOrder> Orders { get { return orders; } } //Słowo kluczowe "virtual" zapewnia lazy loading.

    }
}
