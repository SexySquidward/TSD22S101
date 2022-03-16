
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
    public sealed partial class ContactDetailsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ContactDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();
        }

        public void Results()
        {
            //conn.DropTable<ContactDetail>();
            conn.CreateTable<ContactDetail>();
            var query1 = conn.Table<ContactDetail>();
            ContactDetailsView.ItemsSource = query1.ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        public async void AddContactDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_FirstName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }else if(_LastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }else if (_CompanyName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Company Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }else if (_MobilePhone.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Mobile Phone not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else
                {
                    conn.CreateTable<ContactDetail>();
                    conn.Insert(new ContactDetail
                    {
                        FirstName = _FirstName.Text.ToString(),
                        LastName = _LastName.Text.ToString(),
                        CompanyName = _CompanyName.Text.ToString(),
                        MobilePhone = _MobilePhone.Text.ToString().Replace(" ","")
                    }) ;
                    // Creating table
                    Results();
                    resetField();
                }
            }
            catch (Exception ex)
            {
                if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Phone Number already exist, Try Different Mobile Number", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
            }
        }

        public async void DeleteContactDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((ContactDetail)ContactDetailsView.SelectedItem).MobilePhone;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else
                {
                    conn.CreateTable<ContactDetail>();
                    var query1 = conn.Table<ContactDetail>();
                    var query3 = conn.Query<ContactDetail>("DELETE FROM ContactDetail WHERE MobilePhone ='" + AccSelection + "'");
                    ContactDetailsView.ItemsSource = query1.ToList();
                }
            }
            catch (Exception)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
                return;
            }
        }

        public void resetField()
        {
            _CompanyName.Text = "";
            _FirstName.Text = "";
            _LastName.Text = "";
            _MobilePhone.Text = "";
        }

    }
}
