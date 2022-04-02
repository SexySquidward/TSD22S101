using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StartFinance.Models;
using SQLite.Net;
using System.Diagnostics;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public AppointmentPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Appointment>();
           
            var query = conn.Table<Appointment>();
            AppointmentList1.ItemsSource = query.ToList();
            AppointmentList.ItemsSource = query.ToList();
            

        } 
        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int num = pagecontent.SelectedIndex;
            if (num == 0)
            {
                DeletePageFooter.Visibility = Visibility.Collapsed;
                pageFooter.Visibility = Visibility.Visible;
            }
            else
            {
                pageFooter.Visibility = Visibility.Collapsed;
                DeletePageFooter.Visibility = Visibility.Visible;
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
             try
            {
                if (EventName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("EventName was not entered", "Oops...!");
                    await dialog.ShowAsync();
                }
                else if (EventLocation.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Location was not entered", "Oops...!");
                    await dialog.ShowAsync();
                }
                else if (EventDate.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Date was not entered", "Oops...!");
                    await dialog.ShowAsync();
                }
                else if (EventStartTime.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Start Time was not entered", "Oops...!");
                    await dialog.ShowAsync();
                }
                else if (EventEndTime.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("End Time was not entered", "Oops...!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new Appointment()
                    {
                        EventName = EventName.Text,
                        Location = EventLocation.Text,
                        EventDate = EventDate.Text,
                        StartTime = EventStartTime.Text,
                        EndTime = EventEndTime.Text
                    });
                    Results();
                }
            }catch(Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter in all the fields or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Event already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Appointment will cancel it. Are you sure?", "Important");
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
                    int AppointmentIdLabel = ((Appointment)AppointmentList1.SelectedItem).AppointmentID;
                    var querydel = conn.Query<Appointment>("DELETE FROM Appointment WHERE AppointmentID='" + AppointmentIdLabel + "'");
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

        private void AppointmentList_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            EventNameUpdate.Text = ((Appointment)e.ClickedItem).EventName;
            EventLocationUpdate.Text =((Appointment)e.ClickedItem).Location;
            EventDateUpdate.Text = ((Appointment)e.ClickedItem).EventDate;
            EventStartTimeUpdate.Text = ((Appointment)e.ClickedItem).StartTime;
            EventEndTimeUpdate.Text = ((Appointment)e.ClickedItem).EndTime;
        }

        private async void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                int AppointmentIdLabel = ((Appointment)AppointmentList.SelectedItem).AppointmentID;
                var querydel = conn.Query<Appointment>($"UPDATE Appointment SET EventName = '{EventNameUpdate.Text}', Location = '{EventLocationUpdate.Text}', EventDate = '{EventDateUpdate.Text}', StartTime = '{EventStartTimeUpdate.Text}', EndTime = '{EventEndTimeUpdate.Text}' WHERE AppointmentID= '{AppointmentIdLabel}'");
                Results();
            }
            catch (NullReferenceException)
            {
                MessageDialog ClearDialog = new MessageDialog("Please select the item to Update", "Oops..!");
                await ClearDialog.ShowAsync();
            }
        }
    }
}
