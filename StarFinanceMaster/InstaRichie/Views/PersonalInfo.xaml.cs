using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfo : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfo()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();
        }

        public void Results()
        {
            //conn.DropTable<PersonalInfoDB>();
            conn.CreateTable<PersonalInfoDB>();
            var query1 = conn.Table<PersonalInfoDB>();
            PersonalInfoView.ItemsSource = query1.ToList();
            PersonalInfoView1.ItemsSource = query1.ToList();
        }

        private async void AddPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstNameTxtBx.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Oops..!");
                    await dialog.ShowAsync();

                }
                else if (FirstNameTxtBx.Text.ToString() == "AccountName" || FirstNameTxtBx.Text.ToString() == "InitialAmount")
                {
                    MessageDialog variableerror = new MessageDialog("You cannot use this name", "Oops..!");
                }
                else
                {   // Inserts the data
                    conn.Insert(new PersonalInfoDB()
                    {
                        FirstName = FirstNameTxtBx.Text,
                        LastName = LastNameTxtBx.Text,
                        DOB = DOBTxtBx.Text,
                        Gender = GenderTxtBx.Text,
                        EmailAddress = EmailTxtBx.Text,
                        MobilePhone = PhoneNumberTxtBx.Text
                    });
                    Results();
                }
            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid entry", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Account Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Account will delete all transactions of this account", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    int AccountsLabel = ((PersonalInfoDB)PersonalInfoView.SelectedItem).PersonalID;
                    var querydel = conn.Query<PersonalInfoDB>("DELETE FROM PersonalInfoDB WHERE PersonalID='" + AccountsLabel + "'");
                    Results();
                    
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int num = pagecontent.SelectedIndex;
            if(num==0)
            {
                pageFooterEdit.Visibility = Visibility.Collapsed;
                pageFooter.Visibility = Visibility.Visible;
            }
            else
            {
                pageFooter.Visibility = Visibility.Collapsed;
                pageFooterEdit.Visibility = Visibility.Visible;
            }
        }

        private void PersonalInfoView1_ItemClick(object sender, ItemClickEventArgs e)
        {
            FirstNameTxtBxUpdate.Text = ((PersonalInfoDB)e.ClickedItem).FirstName;
            LastNameTxtBxUpdate.Text = ((PersonalInfoDB)e.ClickedItem).LastName;
            DOBTxtBxUpdate.Text = ((PersonalInfoDB)e.ClickedItem).DOB;
            GenderTxtBxUpdate.Text = ((PersonalInfoDB)e.ClickedItem).Gender;
            EmailTxtBxUpdate.Text = ((PersonalInfoDB)e.ClickedItem).EmailAddress;
            PhoneNumberTxtBxUpdate.Text = ((PersonalInfoDB)e.ClickedItem).MobilePhone;
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int PersonaalIDLabel = ((PersonalInfoDB)PersonalInfoView1.SelectedItem).PersonalID;
                var query1 = conn.Query<PersonalInfoDB>($"Update PersonalInfoDB set FirstName= '{FirstNameTxtBxUpdate.Text}', LastName = '{LastNameTxtBxUpdate.Text}', DOB='{DOBTxtBxUpdate.Text}', Gender='{GenderTxtBxUpdate.Text}', EmailAddress='{EmailTxtBxUpdate.Text}', MobilePhone='{PhoneNumberTxtBxUpdate.Text}' WHERE PersonalID= '{PersonaalIDLabel}'");
                Results();
            }
            catch(NullReferenceException)
            {
                MessageDialog ClearDialog = new MessageDialog("Please select the item to update", "Oops...!");
                await ClearDialog.ShowAsync();
            

             }
        }
    }
}
