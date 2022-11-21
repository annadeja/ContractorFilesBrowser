using System;
using System.Data;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Windows.Forms;
using ContractorFilesBrowser.Model;
using ContractorFilesBrowser.Data;

namespace ContractorFilesBrowser.View
{
    //!Klasa zarządzająca interakcjami użytkownika z interfejsem graficznym oraz bazą danych.
    //Możliwe byłoby także podzielenie tej klasy, jednakże nie jest ono w mojej opinii konieczne na potrzeby tego zadania.
    public partial class MainWindow : Form
    {
        //!Obiekt umożliwiający komunikację aplikacji z bazą danych.
        private ContractorContext context;
        //!Obecnie wybrany przez użytkownika rekord kontrahenta.
        private Contractor selectedContractor;
        //!Obecnie wybrany przez użytkownika rekord faktury.
        private ContractorOrder selectedOrder;
        //!Stała okreslająca maksymalną stawkę VAT dostępną do wyboru.
        private const decimal maxTaxRate = 0.5M;  //Wartość została dopasowana do najwyższego obecnego VAT na świecie - 50% obowiązującego w Bhutanie.

        public MainWindow()
        {
            InitializeComponent();
        }

        //!Metoda inicjująca elementy UI oraz pobierająca dane z bazy danych przy uruchomieniu programu.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            context = new ContractorContext(); //Utworzenie kontekstu do komunikacji z bazą danych.
            contractorDataGridView.AutoGenerateColumns = true;
            RefreshContractorDataView(); //Pierwsze wczytanie "perspektywy" z danymi kontrahentów.

            contractorOrderGridView.AutoGenerateColumns = true;
            RefreshContractorOrderView(); //Pierwsze wczytanie "perspektywy" z danymi faktur.

            //Inicjalizacja kontrolek dotyczących kontrahentów.
            context.Countries.Load();
            countryBindingSource.DataSource = context.Countries.Local.ToBindingList();
            countryComboBox.SelectedIndex = -1;
            typeComboBox.DataSource = Enum.GetValues(typeof(ContractorType));

            //Inicjalizacja kontrolek dotyczących faktur.
            Dictionary<string, decimal> taxRates = new Dictionary<string, decimal>();
            for (decimal i = 0.0M; i <= maxTaxRate; i+=0.01M) //Ze względu na ścisły format podawania stawek VAT, prosztszym jest pozwolenie użytkownikowi na wybór z listy.
                taxRates.Add(i * 100 + "%", i);
            taxRateComboBox.DataSource = new BindingSource(taxRates, null);
            taxRateComboBox.DisplayMember = "Key";
            taxRateComboBox.ValueMember = "Value";
            orderTypeComboBox.DataSource = Enum.GetValues(typeof(OrderType));
        }

        //!Metoda zapewniająca zamknięcie połączenia z bazą danych w momencie zakmnięcia programu.
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            context.Dispose();
        }

        //!Metoda zapewniająca reset UI do domyślnego stanu po zakończeniu określonej operacji.
        private void ResetInputs()
        {
            selectedContractor = null;
            selectedOrder = null;

            contractorCreateButton.Enabled = true;
            orderCreateButton.Enabled = true;

            contractorEditButton.Enabled = false;
            orderEditButton.Enabled = false;

            contractorDeleteButton.Enabled = false;
            orderDeleteButton.Enabled = false;

            contractorGroupBox.Enabled = false;
            orderGroupBox.Enabled = false;

            contractorDataGridView.ClearSelection();
            contractorOrderGridView.ClearSelection();

            EmptyInputs();
        }

        //!Funkcja czyszcząca wszelkie pola jakie użytkownik może wypełnić.
        private void EmptyInputs()
        {
            //Operacje te można wykonać także poprzez rekurencyjne wysukiwanie w kontrolce inputTabControl.
            foreach (Control control in contractorGroupBox.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.Text = String.Empty;
                        break;
                    case ComboBox comboBox:
                        comboBox.SelectedIndex = -1;
                        break;
                }
            }

            foreach (Control control in orderGroupBox.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.Text = String.Empty;
                        break;
                    case ComboBox comboBox:
                        comboBox.SelectedIndex = -1;
                        break;
                    case DateTimePicker dateTimePicker:
                        dateTimePicker.Value = DateTime.Today;
                        break;
                }
            }

        }

        #region Contractor controls.
        //!Metoda pobierająca informacje dotyczące kontrahentów do wyświetlenia w programie.
        //Tworzenie faktycznej perspektywy nie jest oficjalnie wspierane w metodzie Code First.
        //Ze względu na to oraz niewielki rozmiar wykorzystanej w programie bazy danych, skorzystano w zamian z zapytania LINQ do uzyskania porównywalnego rezultatu.
        private void RefreshContractorDataView()
        {
            var view = (from c in context.Contractors
                        join p in context.Provinces on c.ProvinceId equals p.ProvinceId
                        join co in context.Countries on p.CountryId equals co.CountryId
                        select new ContractorDataView()
                        {
                            ContractorId = c.ContractorId,
                            Name = c.Name,
                            CompanyName = c.CompanyName,
                            EmailAddress = c.EmailAddress,
                            PhoneNumber = c.PhoneNumber,
                            Street = c.Street,
                            City = c.City,
                            ZipCode = c.ZipCode,
                            NipCode = c.NipCode,
                            ProvinceName = p.ProvinceName,
                            CountryName = co.CountryName,
                            ContractorType = c.ContractorType
                        });

            contractorDataGridView.DataSource = view.ToList();
            contractorDataGridView.Refresh();
        }

        //!Metoda obsługująca wydarzenie wyboru lub deselekcji rekordu w tabeli kontrahentów.
        private void ContractorDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) //Kliknięcie prawym przyciskiem myszy doprowadza do deselekcji rekordu.
            {
                ResetInputs();
                RefreshContractorOrderView();
                return;
            }
            else if(e.Button == MouseButtons.Left) //Kliknięcie lewym przyciskiem myszy doprowadza do wyboru rekordu.
            {
                selectedContractor = context.Contractors.Find((int)contractorDataGridView.CurrentRow.Cells[0].Value); //Ekstrahuje obiekt z bazy danych na podstawie ContractorId.

                contractorEditButton.Enabled = true; //Umożliwia edycję lub usunięcie wybranego rekordu.
                contractorDeleteButton.Enabled = true;

                //Przepisanie danych do odpowiednich pól celem edycji lub skopiowania.
                nameTextBox.Text = selectedContractor.Name;
                companyNameTextBox.Text = selectedContractor.CompanyName;
                emailAddressTextBox.Text = selectedContractor.EmailAddress;
                phoneNumberTextBox.Text = selectedContractor.PhoneNumber;
                streetTextBox.Text = selectedContractor.Street;
                cityTextBox.Text = selectedContractor.City;
                zipCodeTextBox.Text = selectedContractor.ZipCode;
                countryComboBox.SelectedValue = context.Provinces.Find(selectedContractor.ProvinceId).CountryId;
                provinceComboBox.SelectedValue = selectedContractor.ProvinceId;
                nipCodeTextBox.Text = selectedContractor.NipCode;
                typeComboBox.SelectedItem = selectedContractor.ContractorType;

                RefreshContractorOrderView();
            }
        }

        //!Metoda zapewniająca wyświetlenie tylko tych województw/prowincji przynależących do wybranego kraju.
        private void CountryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (countryComboBox.SelectedIndex != -1) //Tj. kiedy selekcja nie jest pusta.
                try //Blok konieczny do wychwycenia błędu w komunikacji z zamkniętym kontekstem przy zamykaniu aplikacji.
                {
                    var provinces = (from p in context.Provinces where p.CountryId == (int) countryComboBox.SelectedValue select new { p.ProvinceId, p.ProvinceName });
                    provinceComboBox.DataSource = provinces.ToList();
                    provinceComboBox.ValueMember = "ProvinceId";
                    provinceComboBox.DisplayMember = "ProvinceName";
                    provinceComboBox.Enabled = true;

                }
                catch //Nie ma konieczności podejmowania działań w momencie zamknięcia aplikacji.
                {}
            else
                provinceComboBox.Enabled = false;
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Nowy rekord" dla kontrahentów.
        private void ContractorCreateButton_Click(object sender, EventArgs e)
        {
            selectedContractor = null; //Zapewnia, iż utworzony zostanie nowy obiekt.
            contractorEditButton.Enabled = false;
            contractorDeleteButton.Enabled = false;
            contractorGroupBox.Enabled = true;
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Edytuj rekord" dla kontrahentów.
        private void ContractorEditButton_Click(object sender, EventArgs e)
        {
            contractorCreateButton.Enabled = false;
            contractorDeleteButton.Enabled = false;
            contractorGroupBox.Enabled = true;
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Usuń rekord" dla kontrahentów.
        private void ContractorDeleteButton_Click(object sender, EventArgs e)
        {
            if (context.Orders.Where(o => o.ContractorId == selectedContractor.ContractorId).Count() > 0)
            {
                MessageBox.Show("Kontrahent ten posiada w bazie przypisane faktury. Nie może zostać usunięty.", "Błąd usuwania.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult result = MessageBox.Show("Czy na pewno chcesz trwale usunąć ten rekord?", "Usuń rekord.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Cancel)
                return;

            context.Contractors.Remove(selectedContractor);
            context.SaveChanges();
            RefreshContractorDataView();
            ResetInputs();
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Zatwierdź" podczas próby edycji lub dodania nowego kontrahenta.
        private void ContractorConfirmButton_Click(object sender, EventArgs e)
        {
            if (!ContractorDataCheck()) //Sprawdzenie poprawności danych.
            {
                MessageBox.Show("Wykryto nieprawdidłowe dane podczas wprowadzania rekordu. Proszę o poprawne uzupełnienie wszelkich pustych pól tekstowych oraz list rozwijanych.", 
                    "Nieprawidłowe dane.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Contractor newContractor = new Contractor 
            {
                Name = nameTextBox.Text,
                EmailAddress = emailAddressTextBox.Text,
                PhoneNumber = phoneNumberTextBox.Text,
                Street = streetTextBox.Text,
                City = cityTextBox.Text,
                ZipCode = zipCodeTextBox.Text,
                CompanyName = companyNameTextBox.Text,
                NipCode = nipCodeTextBox.Text,
                ProvinceId = (int) provinceComboBox.SelectedValue,
                ContractorType = (ContractorType) typeComboBox.SelectedValue
            };
            if (selectedContractor != null) //Sprawdzenie czy dokonywana jest edycja.
                newContractor.ContractorId = selectedContractor.ContractorId;

            context.Contractors.AddOrUpdate(newContractor);
            context.SaveChanges();
            RefreshContractorDataView();
            RefreshContractorOrderView();
            ResetInputs();
        }

        //!Metoda sprawdzająca czy wszystkie dane w polach kontrahenta zostały wprowadzone poprawnie.
        private bool ContractorDataCheck() 
        {
            foreach(Control control in contractorGroupBox.Controls)
            {
                switch(control)
                {
                    case TextBox textBox:
                        if (String.IsNullOrEmpty(textBox.Text))
                            return false;
                        break;
                    case ComboBox comboBox:
                        if (comboBox.SelectedIndex == -1)
                            return false;
                        break;
                }
            }
            return true;
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Odrzuć" podczas próby edycji lub dodania nowego kontrahenta.
        private void ContractorRejectButton_Click(object sender, EventArgs e)
        {
            ResetInputs();
        }
        #endregion

        #region Order controls.
        //!Metoda pobierająca informacje dotyczące faktur do wyświetlenia w programie.
        private void RefreshContractorOrderView()
        {
            List<int> selectedIds = new List<int>(); //Umożliwia wyświetlenie faktur dla wszystkich wybranych kontrahentów.
            foreach(DataGridViewRow row in contractorDataGridView.SelectedRows)
                selectedIds.Add((int)row.Cells[0].Value);

            if (selectedIds.Count == 0) //Wyświetla wszystkie faktury w razie braku wyboru kontrahenta.
                selectedIds = (from c in context.Contractors select new { c.ContractorId}).ToList().ConvertAll(x => x.ContractorId);

            var view = (from o in context.Orders
                        join c in context.Contractors on o.ContractorId equals c.ContractorId where selectedIds.Contains(o.ContractorId)
                        select new ContractorOrderView
                        {
                            ContractorOrderId = o.ContractorOrderId,
                            Name = c.Name,
                            CompanyName = c.CompanyName,
                            BasePrice = o.BasePrice,
                            Currency = o.Currency,
                            Tax = Math.Round((o.BasePrice * o.TaxRate), 2),
                            TaxRate = o.TaxRate * 100 + "%",
                            PriceWithTax = Math.Round((o.BasePrice + o.BasePrice * o.TaxRate), 2),
                            Description = o.Description,
                            OrderType = o.OrderType,
                            IssueDate = o.IssueDate,
                            SaleDate = o.SaleDate,
                            PaymentDueDate = o.PaymentDueDate,
                        });
            contractorOrderGridView.DataSource = view.ToList();
            contractorOrderGridView.Refresh();

        }

        //!Metoda obsługująca wydarzenie wyboru lub deselekcji rekordu w tabeli faktur.
        private void ContractorOrdersGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) //Kliknięcie prawym przyciskiem myszy doprowadza do deselekcji rekordu.
            {
                ResetInputsPreserveContractorSelection();
                return;
            }
            else if (e.Button == MouseButtons.Left) //Przepisanie danych do odpowiednich pól celem edycji lub skopiowania.
            {
                selectedOrder = context.Orders.Find((int)contractorOrderGridView.CurrentRow.Cells[0].Value); //Ekstrahuje obiekt z bazy danych na podstawie ContractorOrderId.

                orderEditButton.Enabled = true; //Umożliwia edycję lub usunięcie wybranego rekordu.
                orderDeleteButton.Enabled = true;

                basePriceTextBox.Text = selectedOrder.BasePrice.ToString();
                currencyTextBox.Text = selectedOrder.Currency;
                taxRateComboBox.SelectedValue = selectedOrder.TaxRate;
                descriptionTextBox.Text = selectedOrder.Description;
                orderTypeComboBox.SelectedItem = selectedOrder.OrderType;
                contractorIdTextBox.Text = selectedOrder.ContractorId.ToString();
                issueDatePicker.Value = selectedOrder.IssueDate;
                saleDatePicker.Value = selectedOrder.SaleDate;
                paymentDueDatePicker.Value = selectedOrder.PaymentDueDate;
            }
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Nowy rekord" dla faktur.
        private void OrderCreateButton_Click(object sender, EventArgs e)
        {
            selectedOrder = null; //Zapewnia, iż utworzony zostanie nowy obiekt.
            orderEditButton.Enabled = false;
            orderDeleteButton.Enabled = false;
            orderGroupBox.Enabled = true;
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Edytuj rekord" dla faktur.
        private void OrderEditButton_Click(object sender, EventArgs e)
        {
            orderCreateButton.Enabled = false;
            orderDeleteButton.Enabled = false;
            orderGroupBox.Enabled = true;
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Usuń rekord" dla faktur.
        private void OrderDeleteButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Czy na pewno chcesz trwale usunąć ten rekord?", "Usuń rekord.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Cancel)
                return;
            context.Orders.Remove(selectedOrder);
            context.SaveChanges();
            RefreshContractorOrderView();
            ResetInputsPreserveContractorSelection();
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Zatwierdź" podczas próby edycji lub dodania nowej faktury.
        private void OrderConfirmButton_Click(object sender, EventArgs e)
        {
            if (!OrderDataCheck())
            {
                MessageBox.Show("Wykryto nieprawdidłowe dane podczas wprowadzania rekordu. Proszę o poprawne zupełnienie wszelkich pustych pól tekstowych oraz list rozwijanych.",
                    "Nieprawidłowe dane.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            decimal convertedValue; //Funkcja Convert.ToDecimal() nie przyjmuje dodatkowych argumentów, dlatego też dla konieczne jest tutaj wykorzystanie Decimal.TryParse().
            Decimal.TryParse(basePriceTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out convertedValue); 
            ContractorOrder newOrder = new ContractorOrder
            {
                BasePrice = convertedValue,
                Currency = currencyTextBox.Text,
                TaxRate = Convert.ToDecimal(taxRateComboBox.SelectedValue),
                IssueDate = issueDatePicker.Value,
                SaleDate = saleDatePicker.Value,
                PaymentDueDate= paymentDueDatePicker.Value,
                Description = descriptionTextBox.Text,
                OrderType = (OrderType) orderTypeComboBox.SelectedValue,
                ContractorId = Convert.ToInt32(contractorIdTextBox.Text)
            };
            if (selectedOrder != null) //Sprawdzenie czy dokonywana jest edycja.
                newOrder.ContractorOrderId = selectedOrder.ContractorOrderId;

            context.Orders.AddOrUpdate(newOrder);
            context.SaveChanges();
            RefreshContractorOrderView();
            ResetInputsPreserveContractorSelection();
        }

        //!Metoda sprawdzająca czy wszystkie dane w polach faktury zostały wprowadzone poprawnie.
        private bool OrderDataCheck()
        {
            basePriceTextBox.Text = basePriceTextBox.Text.Replace(",", ".");
            int contractorId;
            //Sprawdzenie poprawności danych liczbowych.
            if (!int.TryParse(contractorIdTextBox.Text, out contractorId) || !Decimal.TryParse(basePriceTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _)) 
                //Nie można użyć operatora as na typie prostym, co powoduje konieczność takiej konstrukcji warunku.
                return false;
            if (context.Contractors.Find(contractorId) == null) //Sprawdzenie czy istnieje kontrahent o podanym identyfikatorze. 
                return false;

            foreach (Control control in orderGroupBox.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        if (String.IsNullOrEmpty(textBox.Text))
                            return false;
                        break;
                    case ComboBox comboBox:
                        if (comboBox.SelectedIndex == -1)
                            return false;
                        break;
                }
            }
            return true;
        }

        //!Metoda obsługująca wydarzenie naciśnięcia przycisku "Odrzuć" podczas próby edycji lub dodania nowej faktury.
        private void OrderRejectButton_Click(object sender, EventArgs e)
        {
            ResetInputsPreserveContractorSelection();
        }

        //!Metoda zapewniająca reset elementów UI do stanu domyślnego, zachowująca jednak zaznaczenie uprzednio wybranych kontrahentów w celu uzyskania tego samego widoku faktur.
        private void ResetInputsPreserveContractorSelection()
        {
            var selectedRows = contractorDataGridView.SelectedRows;
            ResetInputs();
            foreach (DataGridViewRow row in selectedRows)
                row.Selected = true;
        }
        #endregion
    }
}