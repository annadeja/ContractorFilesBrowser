namespace ContractorFilesBrowser.Model
{
    //!Klasa reprezentująca tablicę województw/prowincji w bazie danych, wykorzystana do wygenerowania tablicy w ramach metody Code First.
    public class Province
    {

        //!Identyfikator prowincji.
        public int ProvinceId { get; set; }

        //!Nazwa prowincji.
        public string ProvinceName { get; set; }

        //!Identyfikator kraju do jakiego przynależy prowincja.
        public int CountryId { get; set; } 
        public virtual Country Country { get; set; } //Słowo kluczowe "virtual" zapewnia lazy loading.
    }
}