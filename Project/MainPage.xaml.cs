/* CLASS NAME: MainPage
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * MainPage contains the underlying code for the opening page
 * of the Space Collection app.
 * 
 * It has a single button control that leads to a menu page
 * 
 */

#region Namespaces Used

using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

#endregion

namespace Project
{
    public sealed partial class MainPage : Page
    {
        private MediaElement meBlip;

        #region Constructor
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            meBlip = new MediaElement
            {
                Volume = 1,
                AutoPlay = false,
                Source = new Uri("ms-appx:///Assets/Blip.mp3"),
                AudioCategory = AudioCategory.GameEffects
            };
            grdMainGrid.Children.Add(meBlip);
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StatusBar.GetForCurrentView().HideAsync();
            HardwareButtons.BackPressed += HardwareButtonsBackPressed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsBackPressed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Application.Current.Exit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();

            // Go to the menu
            Frame.Navigate(typeof(MenuPage));
        }
        #endregion
    }
}
