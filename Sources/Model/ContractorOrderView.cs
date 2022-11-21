using System;
using System.ComponentModel;

namespace ContractorFilesBrowser.Model
{
    //!Klasa przechowująca dane dotyczące faktur wyświetlane w programie.
    public class ContractorOrderView
    {
        [DisplayName("ID faktury")]
        //!Identyfikator faktury.
        public int ContractorOrderId { get; set; }

        [DisplayName("Imię i nazwisko")]
        //!Imię i nazwisko kontrahenta.
        public string Name { get; set; } 

        [DisplayName("Firma")]
        //!Nazwa firmy kontrahenta.
        public string CompanyName { get; set; } 

        [DisplayName("Cena netto")]
        //!Cena netto.
        public decimal BasePrice { get; set; }

        [DisplayName("Waluta")]
        //!Waluta transakcji.
        public string Currency { get; set; } 

        [DisplayName("VAT")]
        //!Wartość VAT.
        public decimal Tax { get; set; } 

        [DisplayName("Stawka VAT")]
        //!Stawka VAT.
        public string TaxRate { get; set; } 

        [DisplayName("Cena brutto")]
        //!Cena brutto.
        public decimal PriceWithTax { get; set; } 

        [DisplayName("Opis")]
        //!Opis transakcji.
        public string Description { get; set; } 

        [DisplayName("Typ transakcji")]
        //!Typ transakcji.
        public OrderType OrderType { get; set; } 

        [DisplayName("Data wystawienia")]
        //!Data wystawienia faktury.
        public DateTime IssueDate { get; set; } 

        [DisplayName("Data zakupu lub sprzedaży")]
        //!Data zakupu/sprzedaży.
        public DateTime SaleDate { get; set; } 

        [DisplayName("Termin płatności")]
        //!Termin płatności za fakturę.
        public DateTime PaymentDueDate { get; set; } 
    }
}
