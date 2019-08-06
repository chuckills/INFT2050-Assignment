/* CLASS NAME: ScorePage
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * ScorePage contains the underlying code to show the top ten list of
 * high scores for each difficulty in the Space Ops game. It shows
 * the number of questions asked, questions answered, overall accuracy
 * and a calculated score.
 * 
 */

#region Namespaces Used

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endregion

namespace Project
{
    public sealed partial class ScorePage : Page
    {
        #region Instance Variables
        private List<Score> bScores;
        private List<Score> mScores;
        private List<Score> hScores;
        private MediaElement meBlip;
        private MediaElement meBgScore;
        #endregion

        #region Constructor
        public ScorePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            bScores = new List<Score>();
            mScores = new List<Score>();
            hScores = new List<Score>();
            
            meBgScore = new MediaElement
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
            grdScoreGrid.Children.Add(meBgScore);
            grdScoreGrid.Children.Add(meBlip);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsBackPressed;
            await loadScoreList();
        }

        /// <summary>
        ///     Invoked when the page is no longer active in the frame
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsBackPressed;
        }

        /// <summary>
        ///     Invoked when the back button is pressed
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
        ///     Invoked when the reset button is tapped. Removes all high scores from the lists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnReset_Tapped(object sender, TappedRoutedEventArgs e)
        {
            meBlip.Play();
            string[] fileName = { "bScores.dat", "mScores.dat", "hScores.dat" };
            foreach (string file in fileName)
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Score>));
                using (Stream stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                    file, CreationCollisionOption.ReplaceExisting))
                {
                    jsonSerializer.WriteObject(stream, new List<Score>());
                }
            }
            Frame.GoBack();
        }
        #endregion

        /// <summary>
        ///     Loads the high score files for each difficulty and stores the values in List objects
        /// </summary>
        /// <returns></returns>
        private async Task loadScoreList()
        {
            int limit;
            string header = string.Format("{0,4}{1,9}{2,7}{3,10}{4,7}\n", "Rank", "Correct", "Asked", "Accuracy", "Score");

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Score>));
            try
            {
                Stream myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("bScores.dat");
                bScores = jsonSerializer.ReadObject(myStream) as List<Score>;
                myStream.Dispose();

                bList.Text += header;
                if (bScores.Count < 10)
                    limit = bScores.Count;
                else
                    limit = 10;
                for (int i = 0; i < limit; i++)
                {
                    bList.Text += string.Format("\n{0,4}{1,9:N0}{2,7:N0}{3,10:P1}{4,7:F1}", i + 1,
                        bScores[i].TotalCorrect, bScores[i].TotalQuestions, bScores[i].TotalAccuracy,
                        bScores[i].calcScore());
                }
            }
            catch (FileNotFoundException)
            {
                bList.Text = "No beginner scores have been saved!";
            }
            finally
            {
                if (bScores.Count == 0)
                    bList.Text = "No beginner scores have been saved!";
            }

            try
            {
                Stream myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("mScores.dat");
                mScores = jsonSerializer.ReadObject(myStream) as List<Score>;
                myStream.Dispose();

                mList.Text += header;
                if (mScores.Count < 10)
                    limit = mScores.Count;
                else
                    limit = 10;
                for (int i = 0; i < limit; i++)
                {
                    mList.Text += string.Format("\n{0,4}{1,9:N0}{2,7:N0}{3,10:P1}{4,7:F1}", i + 1, mScores[i].TotalCorrect, mScores[i].TotalQuestions, mScores[i].TotalAccuracy, mScores[i].calcScore());
                }
            }
            catch (FileNotFoundException)
            {
                mList.Text = "No medium scores have been saved!";
            }
            finally
            {
                if (mScores.Count == 0)
                    mList.Text = "No beginner scores have been saved!";
            }

            try
            {
                Stream myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("hScores.dat");
                hScores = jsonSerializer.ReadObject(myStream) as List<Score>;
                myStream.Dispose();
                
                hList.Text += header;
                if (hScores.Count < 10)
                    limit = hScores.Count;
                else
                    limit = 10;
                for (int i = 0; i < limit; i++)
                {
                    hList.Text += string.Format("\n{0,4}{1,9:N0}{2,7:N0}{3,10:P1}{4,7:F1}", i + 1, hScores[i].TotalCorrect, hScores[i].TotalQuestions, hScores[i].TotalAccuracy, hScores[i].calcScore());
                }
            }
            catch (FileNotFoundException)
            {
                hList.Text = "No hard scores have been saved!";
            }
            finally
            {
                if (hScores.Count == 0)
                    hList.Text = "No beginner scores have been saved!";
            }
        }
    }
}
