using System.ComponentModel;

namespace ContractorFilesBrowser.Model
{
    //!Klasa przechowująca dane dotyczące kontrahentów wyświetlane w programie.
    public class ContractorDataView
    {
        [DisplayName("ID Kontrahenta")]
        //!Identyfikator kontrahenta.
        public int ContractorId { get; set; } 

        [DisplayName("Imię i nazwisko")]
        //!Imię i nazwisko kontrahenta.
        public string Name { get; set; } 

        [DisplayName("Firma")]
        //!Nazwa firmy kontrahenta.
        public string CompanyName { get; set; }

        [DisplayName("NIP")]
        //!NIP kontrahenta.
        public string NipCode { get; set; } 

        [DisplayName("Adres email")]
        //!Adres email kontrahenta.
        public string EmailAddress { get; set; } 

        [DisplayName("Numer telefonu")]
        //!Numer telefonu kontrahenta.
        public string PhoneNumber { get; set; } 

        [DisplayName("Ulica")]
        //!Ulica adresu kontrahenta.
        public string Street { get; set; } 

        [DisplayName("Miejscowość")]
        //!Miejscowość adresu kontrahenta.
        public string City { get; set; } 

        [DisplayName("Kod pocztowy")]
        //!Kod pocztowy kontrahenta.
        public string ZipCode { get; set; } 

        [DisplayName("Województwo/prowincja")]
        //!Województwo/prowincja kontrahenta.
        public string ProvinceName { get; set; } 

        [DisplayName("Kraj")]
        //!Kraj kontrahenta.
        public string CountryName { get; set; } 

        [DisplayName("Typ kontrahenta")]
        //!Typ kontrahenta.
        public ContractorType ContractorType { get; set; } 
    }
}
