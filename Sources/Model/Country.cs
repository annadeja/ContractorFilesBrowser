using ContractorFilesBrowser.Helpers;

namespace ContractorFilesBrowser.Model
{
    //!Klasa reprezentująca tablicę krajów w bazie danych, wykorzystana do wygenerowania tablicy w ramach metody Code First.
    //Kraje zaprezentowano w formie tablicy bazy danych w celu ułatwienia oraz uregulowania procesu wprowadzania danych.
    //Korzystający z aplikacji mogą nie mieć na celu obsługę kontrahentów ze wszystkich możliwych krajów, nadając sens ograniczeniu ich liczby.
    //Kraje podane w ramach tej tabeli stanowią jedynie przykład na potrzeby zadania.
    public class Country
    {
        //!Identyfikator kraju.
        public int CountryId { get; set; }

        //!Nazwa kraju.
        public string CountryName { get; set; } 

        //!Prowincje kraju. Zapewnia relację One To Many.
        private readonly ObservableListSource<Province> provinces = new ObservableListSource<Province>(); 
        public virtual ObservableListSource<Province> Provinces { get { return provinces; } } //Słowo kluczowe "virtual" zapewnia lazy loading.

        //!Kontrahenci w danym kraju. Zapewnia relację One To Many.
        private readonly ObservableListSource<Contractor> contractors = new ObservableListSource<Contractor>(); 
        public virtual ObservableListSource<Contractor> Contractors { get { return contractors; } } //Słowo kluczowe "virtual" zapewnia lazy loading.
    }
}