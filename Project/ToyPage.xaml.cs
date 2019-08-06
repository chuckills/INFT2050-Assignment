/* CLASS NAME: ToyPage
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * ToyPage contains the menu buttons for unlockable reward games.
 * 
 * Each button unlocks with the completion of 10 games of Space Ops
 * on the corresponding difficulty.
 * 
 */

#region Namespaces Used

using System;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;

#endregion

namespace Project
{
    public sealed partial class ToyPage : Page
    {
        private int[] iUnlocks;

        public ToyPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            
            iUnlocks = new int[3];
        }

        #region Event Handlers
        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        ///     Unlocks the reward buttons acheived
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsBackPressed;
            
            if (e.Parameter != null)
                iUnlocks = (int[])e.Parameter;

            // Unlocks after 10 plays of Space Ops on Beginner level
            if (iUnlocks[0] == 10)
            {
                btnMrSpaceman.IsEnabled = true;
                lblSpaceman.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblSpaceman.Text = string.Format("Play {0} more times on Beginner Mode", 10 - iUnlocks[0]);
            }

            // Unlocks after 10 plays of Space Ops on Medium level
            if (iUnlocks[1] == 10)
            {
                btnStarCatch.IsEnabled = true;
                lblStarCatch.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblStarCatch.Text = string.Format("Play {0} more times on Medium Mode", 10 - iUnlocks[1]);
            }

            // Unlocks after 10 plays of Space Ops on Hard level
            if (iUnlocks[2] == 10)
            {
                button.IsEnabled = true;
                lblButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblButton.Text = string.Format("Play {0} more times on Hard Mode", 10 - iUnlocks[2]);
            }
        }

        /// <summary>
        ///     Invoked when the page is no longer active in the fram
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsBackPressed;
        }

        /// <summary>
        ///     Invoked when the back button is pressed on the device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            meBlip.Play();
            e.Handled = true;
            Frame.GoBack();
        }

        /// <summary>
        ///     Invoked when Mr Spaceman button is tapped. Navigates to Mr Spaceman game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMrSpaceman_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();
            Frame.Navigate(typeof(MrSpaceman));
        }

        /// <summary>
        ///     Invoked when Star Catcher button is tapped. Navigates to Star Catcher game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStarCatch_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();
            Frame.Navigate(typeof(StarCatcher));
        }
        #endregion
    }
}
