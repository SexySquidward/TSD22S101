
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
            Results(ContactDetailsView);
        }

        public void Results(ListView lv)
        {
            //conn.DropTable<ContactDetail>();
            conn.CreateTable<ContactDetail>();
            var query1 = conn.Table<ContactDetail>();
            lv.ItemsSource = query1.ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results(ContactDetailsView);
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
                }
                else if (_LastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else if (_CompanyName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Company Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else if (_MobilePhone.Text.ToString() == "")
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
                        MobilePhone = _MobilePhone.Text.ToString().Replace(" ", "")
                    });
                    // Creating table
                    Results(ContactDetailsView);
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

        public async void SaveContactDetails_Click(object sender, RoutedEventArgs e)
        {
            int ContactSelection = ((ContactDetail)UpdateDetailsView.SelectedItem).ContactID;
            try
            {
                if (_FirstNameUpdate.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else if (_LastNameUpdate.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else if (_CompanyNameUpdate.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Company Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else if (_MobilePhoneUpdate.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Mobile Phone not entered", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else
                {
                    var query_update = conn.Query<ContactDetail>("UPDATE ContactDetail SET FirstName = '" + _FirstNameUpdate.Text 
                        + "', LastName='" + _LastNameUpdate.Text + "', CompanyName = '" + _CompanyNameUpdate.Text + "', MobilePhone = '" + _MobilePhoneUpdate.Text +
                        "' WHERE ContactID ='" + ContactSelection + "'");
                    if (query_update.Count == 0)
                    {
                        MessageDialog dialog = new MessageDialog("Contact Updated Sucessfully!");
                        await dialog.ShowAsync();
                        Results(UpdateDetailsView);
                        resetAndHideUpdateField();
                    }
                    else
                    {
                        MessageDialog dialog = new MessageDialog("Somthing Went Wrong, Please Try Again");
                        await dialog.ShowAsync();
                        return;
                    }
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

        public void CancelContactUpdate_Click(object sender, RoutedEventArgs e)
        {
            resetAndHideUpdateField();
            Results(UpdateDetailsView);
        }

        public void resetAndHideUpdateField()
        {
            _FirstNameUpdate.Text = "";
            _LastNameUpdate.Text = "";
            _CompanyNameUpdate.Text = "";
            _MobilePhoneUpdate.Text = "";
            EditContactStackPanel.Visibility = Visibility.Collapsed;
            Grid.SetRow(UpdateDetailsView, 0);
            SaveChangeBarBtn.IsEnabled = false;
        }

        public async void DeleteContactDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ContactSelection = ((ContactDetail)ContactDetailsView.SelectedItem).ContactID;
                if (ContactSelection.Equals(null))
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else
                {
                    conn.CreateTable<ContactDetail>();
                    var query1 = conn.Table<ContactDetail>();
                    var query3 = conn.Query<ContactDetail>("DELETE FROM ContactDetail WHERE ContactID ='" + ContactSelection + "'");
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

        private void contactDetailsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int currentPage = ContactDetailsPivot.SelectedIndex;

            if (currentPage == 0)
            {
                EditContactStackPanel.Visibility = Visibility.Collapsed;
                Grid.SetRow(UpdateDetailsView, 0);
                pageFooter.Visibility = Visibility.Visible;
                pageFooterUpdate.Visibility = Visibility.Collapsed;
                Results(ContactDetailsView);
            }
            else
            {
                pageFooterUpdate.Visibility = Visibility.Visible;
                pageFooter.Visibility = Visibility.Collapsed;
                Results(UpdateDetailsView);
                SaveChangeBarBtn.IsEnabled = false;
            }
        }

        private async void EditContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int ContactSelection = ((ContactDetail)UpdateDetailsView.SelectedItem).ContactID;
                if (ContactSelection.Equals(null))
                {
                    MessageDialog dialog = new MessageDialog("Select a Contact First", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                else
                {
                    SaveChangeBarBtn.IsEnabled = true;
                    _FirstNameUpdate.Text = ((ContactDetail)UpdateDetailsView.SelectedItem).FirstName;
                    _LastNameUpdate.Text = ((ContactDetail)UpdateDetailsView.SelectedItem).LastName;
                    _CompanyNameUpdate.Text = ((ContactDetail)UpdateDetailsView.SelectedItem).CompanyName;
                    _MobilePhoneUpdate.Text = ((ContactDetail)UpdateDetailsView.SelectedItem).MobilePhone;
                    EditContactStackPanel.Visibility = Visibility.Visible;
                    Grid.SetRow(UpdateDetailsView, 1);
                }
            }
            catch (Exception)
            {
                MessageDialog dialog = new MessageDialog("Select a Contact First", "Oops..!");
                await dialog.ShowAsync();
                return;
            }
        }
    }
}
