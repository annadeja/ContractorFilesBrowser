using System;

namespace ContractorFilesBrowser.Model
{
    //!Klasa reprezentująca tablicę faktur w bazie danych, wykorzystana do wygenerowania tablicy w ramach metody Code First.
    //Rodzaje danych zostały luźno bazowane na systemie Comarch e-Sklep.
    public class ContractorOrder
    {
        //!Identyfikator faktury.
        public int ContractorOrderId { get; set; }

        //!Cena netto.
        public decimal BasePrice { get; set; }

        //!Waluta transakcji.
        public string Currency { get; set; }

        //!Stawka VAT.
        public decimal TaxRate { get; set; }

        //!Data wystawienia faktury.
        public DateTime IssueDate { get; set; }

        //!Data zakupu/sprzedaży.
        public DateTime SaleDate { get; set; }

        //!Termin płatności za fakturę.
        public DateTime PaymentDueDate { get; set; }

        //!Opis transakcji.
        public string Description { get; set; }

        //!Typ transakcji.
        public OrderType OrderType { get; set; }

        //!Identyfikator kontrahenta widniejącego na fakturze.
        public int ContractorId { get; set; } 

        public Contractor Contractor { get; set; } //Słowo kluczowe "virtual" zapewnia lazy loading.
    }
}