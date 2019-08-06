/* CLASS NAME: MenuPage
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * MenuPage contains the underlying code for the main menu of the Space Collection app.
 * Menu options are included for Space Ops and the Toybox which contains unlockable
 * games. Links to the high score page and credits are also on this page.
 * 
 * The menu page also loads the scores files to pass as arguments to the high score page.
 * 
 */

#region Namespaces Used
using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Windows.Storage;

#endregion

namespace Project
{
    public sealed partial class MenuPage : Page
    {
        #region Instance Variables
        private MediaElement meBlip;
        private MediaElement meBgMenu;

        private List<Score> bScores;
        private List<Score> mScores;
        private List<Score> hScores;

        private int[] iUnlocks;
        #endregion

        #region Constructor

        public MenuPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            meBgMenu = new MediaElement
            {
                Source = new Uri("ms-appx:///Assets/Forboding_Resonance.mp3"),
                AudioCategory = AudioCategory.GameMedia,
                IsLooping = true
            };

            meBlip = new MediaElement
            {
                AutoPlay = false,
                Source = new Uri("ms-appx:///Assets/Blip.mp3"),
                AudioCategory = AudioCategory.GameEffects
            };
            grdMenuGrid.Children.Add(meBgMenu);
            grdMenuGrid.Children.Add(meBlip);

            bScores = new List<Score>();
            mScores = new List<Score>();
            hScores = new List<Score>();

            iUnlocks = new int[3];
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsBackPressed;
            Frame.LostFocus += Frame_LostFocus;

            await loadScoreList();
            pgbLoading.Visibility = Visibility.Collapsed;
            tbkLoading.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsBackPressed;
            meBgMenu.Stop();
        }

        private void HardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            meBlip.Play();
            e.Handled = true;
            Frame.GoBack();
        }

        private void Frame_LostFocus(object sender, RoutedEventArgs e)
        {
            meBgMenu.Pause();
            Frame.GotFocus += Frame_GotFocus;
        }

        private void Frame_GotFocus(object sender, RoutedEventArgs e)
        {
            meBgMenu.Play();
        }

        private void btnBeginner_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();
            // Go to the game in beginner mode
            Frame.Navigate(typeof(GamePage), btnBeginner.Tag);
        }

        private void btnMedium_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();
            // Go to the game in medium mode
            Frame.Navigate(typeof(GamePage), btnMedium.Tag);
        }

        private void btnHard_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();
            // Go to the game in hard mode
            Frame.Navigate(typeof(GamePage), btnHard.Tag);
        }

        private async void hypCredits_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();
            string strCredits = string.Format(
                "Backgound free to use aquired from:\n{0}\n\nAll sounds free to use aquired from:\n{1}\n\n{2}\n\n{3}\n\n{4}\n\n{5}\n\nTesting by Connor",
                "https://www.youtube.com/watch?v=KRhLxmzqhO4", "https://www.youtube.com/audiolibrary/soundeffects",
                "Voyetra Digital Sound Gallery Volume 1 CD-ROM",
                "http://soundbible.com/143-Astronaut-Breathing.html", "https://www.freesound.org/people/plasterbrain/sounds/351810/"
                , "https://www.freesound.org/people/myfox14/sounds/382310/");
            MessageDialog msgCredits = new MessageDialog(strCredits);
            msgCredits.Commands.Add(new UICommand("Close"));
            await msgCredits.ShowAsync();
            meBlip.Play();
        }

        private void hypScores_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();
            Frame.Navigate(typeof(ScorePage));
        }

        private void btnToyBox_Click(object sender, RoutedEventArgs e)
        {
            meBlip.Play();
            Frame.Navigate(typeof(ToyPage), iUnlocks);
        }

        #endregion

        private async Task loadScoreList()
        {
            try
            {
                pgbLoading.Visibility = Visibility.Visible;
                tbkLoading.Visibility = Visibility.Visible;

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Score>));

                Stream myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("bScores.dat");
                bScores = jsonSerializer.ReadObject(myStream) as List<Score>;
                iUnlocks[0] = bScores.Count;
                myStream.Dispose();

                myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("mScores.dat");
                mScores = jsonSerializer.ReadObject(myStream) as List<Score>;
                iUnlocks[1] = mScores.Count;
                myStream.Dispose();

                myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("hScores.dat");
                hScores = jsonSerializer.ReadObject(myStream) as List<Score>;
                iUnlocks[2] = hScores.Count;
                myStream.Dispose();
            }
            catch (FileNotFoundException)
            {
                tbkLoading.Text = "Creating score files";
                string[] fileName = { "bScores.dat", "mScores.dat", "hScores.dat" };
                foreach (string file in fileName)
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Score>));
                    using (Stream stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                        file, CreationCollisionOption.OpenIfExists))
                    {
                        jsonSerializer.WriteObject(stream, new List<Score>());
                    }
                }
            }
        }
    }
}
