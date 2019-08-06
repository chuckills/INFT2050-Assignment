/* CLASS NAME: GamePage
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * GamePage contains the underlying code to run the math game SpaceOps.
 * 
 * An equation is displayed on the sceen with the operator covered and
 * the player must guess the correct operator using the touch controls.
 * 
 * It is hoped that this game element would aid in the development of
 * the young player's arithmetic abilities and speed.
 * 
 */

#region Namespaces Used
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.Phone.Devices.Notification;
using Windows.Storage;
using System.Runtime.Serialization.Json;
using Windows.UI.Core;
using Windows.UI.Text;
#endregion

namespace Project
{
    public sealed partial class GamePage : Page
    {
        #region Instance Variables
        private Uri uBlip;
        private Uri uCount;
        private Uri uLaser;
        private Uri uLevelUp;
        private Uri uLevelDown;
        private Uri uBgGame;
        private Question qQuestion;
        private DispatcherTimer dptTimer;
        private DateTime dtmFinishTime;
        private GameDifficulty gDifficulty;
        private VibrationDevice vbVibrate;
        private Score sPlayerScore;
        private List<Score> lScoreList;
        private PointerPoint ppPointerPress;
        private PointerPoint ppPointerRelease;
        private bool bAddCorrect;
        private bool bSubtractCorrect;
        private bool bMultiplyCorrect;
        private bool bDivideCorrect;
        private bool bQuestionAttempted;
        private bool bCountdownCompleted;
        #endregion

        #region Constructor
        public GamePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            
            uBlip = new Uri("ms-appx:///Assets/Blip.mp3");
            uCount = new Uri("ms-appx:///Assets/ALARMCLK.mp3");
            uLaser = new Uri("ms-appx:///Assets/Laser_Gun.mp3");
            uLevelUp = new Uri("ms-appx:///Assets/Explosion_Hiss_Bop_Bang.mp3");
            uLevelDown = new Uri("ms-appx:///Assets/PHAZER.mp3");

            qQuestion = null;
            dptTimer = new DispatcherTimer();
            dtmFinishTime = new DateTime();
            gDifficulty = null;
            vbVibrate = VibrationDevice.GetDefault();
            sPlayerScore = null;
            lScoreList = new List<Score>();
            ppPointerPress = null;
            ppPointerRelease = null;
            bAddCorrect = false;
            bSubtractCorrect = false;
            bMultiplyCorrect = false;
            bDivideCorrect = false;
            bQuestionAttempted = false;
            bCountdownCompleted = false;
        }
        #endregion
        
        #region Event Handlers
        ///<summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        ///</summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsBackPressed;

            Frame.PointerPressed += Frame_PointerPressed;
            Frame.PointerReleased += Frame_PointerReleased;

            tbkAdd.Tapped += TbkAdd_Tapped;
            tbkSubtract.Tapped += TbkSubtract_Tapped;
            tbkMultiply.Tapped += TbkMultiply_Tapped;
            tbkDivide.Tapped += TbkDivide_Tapped;
            
            // Instantiates the selected game difficulty
            gDifficulty = new GameDifficulty(e.Parameter.ToString()[0]);

            sPlayerScore = new Score();

            // Begin the game
            makeGame();
        }
        
        /// <summary>
        ///     Invoked when the page is about to leave the frame
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsBackPressed;
            
            Frame.PointerPressed -= Frame_PointerPressed;
            Frame.PointerReleased -= Frame_PointerReleased;
            
            tbkAdd.Tapped -= TbkAdd_Tapped;
            tbkSubtract.Tapped -= TbkSubtract_Tapped;
            tbkMultiply.Tapped -= TbkMultiply_Tapped;
            tbkDivide.Tapped -= TbkDivide_Tapped;
            playSound(uBlip);
        }

        /// <summary>
        ///     Invoked when the phone's back button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (bCountdownCompleted)
            {
                dptTimer.Stop();
                dptTimer.Tick -= DptTimer_Tick;
                Frame.GoBack();
            }
        }
        
        /// <summary>
        ///     Invoked at every interval of the DispatcherTimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DptTimer_Tick(object sender, object e)
        {
            tbkTimer.Text = string.Format("{0:mm\\:ss\\.f}", dtmFinishTime - DateTime.Now);
            if ((dtmFinishTime - DateTime.Now).TotalMilliseconds <= 0)
            {
                dptTimer.Stop();
                gameOver();
            }
        }

        #endregion
       
        #region Control events

        /// <summary>
        /// Invoked when the player starts touching the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frame_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Gets the coordinates of the point on the screen that was touched
            ppPointerPress = e.GetCurrentPoint(Frame);
        }
        
        /// <summary>
        /// Invoked when the player stops touching the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frame_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Gets the coordinates of the point where the player stopped touching the screen
            ppPointerRelease = e.GetCurrentPoint(Frame);

            // Calculate the total movement in the X and Y directions
            double dGetHorizontal = ppPointerRelease.Position.X - ppPointerPress.Position.X;
            double dGetVertical = ppPointerRelease.Position.Y - ppPointerPress.Position.Y;

            // Determine if the swipe was predominantly horizontal
            if (Math.Abs(dGetHorizontal) - Math.Abs(dGetVertical) >= 80)
            {
                if (dGetHorizontal > 0)
                {
                    // Swipe is in the direction of the multiply symbol
                    performMultiply();
                }
                else
                {
                    // Swipe is in the direction of the divide symbol
                    performDivide();
                }
            }
            else
            {
                // Determine if the swipe was predominantly vertical
                if (Math.Abs(dGetVertical) - Math.Abs(dGetHorizontal) >= 80)
                {
                    if (dGetVertical < 0)
                    {
                        // Swipe is in the direction of the add symbol
                        performAdd();
                    }
                    else
                    {
                        // Swipe is in the direction of the subtract symbol
                        performSubtract();
                    }
                }
            }
        }
        
        /// <summary>
        /// Invoked when the player taps the add symbol
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbkAdd_Tapped(object sender, TappedRoutedEventArgs e)
        {
            performAdd();
        }
        
        /// <summary>
        /// Invoked when the player taps the subtract symbol
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbkSubtract_Tapped(object sender, TappedRoutedEventArgs e)
        {
            performSubtract();
        }
        
        /// <summary>
        /// Invoked when the player taps the multiply symbol
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbkMultiply_Tapped(object sender, TappedRoutedEventArgs e)
        {
            performMultiply();
        }
        
        /// <summary>
        /// Invoked when the player taps the divide symbol
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbkDivide_Tapped(object sender, TappedRoutedEventArgs e)
        {
            performDivide();
        }

        #endregion

        #region Setup Game
        /// <summary>
        ///     Loads the previous high score list
        /// </summary>
        /// <returns></returns>
        private async Task loadScoreList()
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------------
            // Some help from this source https://channel9.msdn.com/Series/Windows-Phone-8-1-Development-for-Absolute-Beginners/Part-22-Storing-and-Retrieving-Serialized-Data

            string fnScoresFile = gDifficulty.DifficultyCode + "Scores.dat";
            try
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Score>));
                Stream myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fnScoresFile);
                lScoreList = jsonSerializer.ReadObject(myStream) as List<Score>;
                myStream.Dispose();
            }
            catch (FileNotFoundException)
            { }

            //-----------------------------------------------------------------------------------------------------------------------------------------------
        }

        /// <summary>
        ///     Saves the current high score list
        /// </summary>
        /// <returns></returns>
        private async Task saveScoreList()
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------------
            // Some help from this source https://channel9.msdn.com/Series/Windows-Phone-8-1-Development-for-Absolute-Beginners/Part-22-Storing-and-Retrieving-Serialized-Data

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Score>));
            using (Stream stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
            gDifficulty.DifficultyCode+"Scores.dat", CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, lScoreList);
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------
        }
        
        /// <summary>
        ///     Sets the game loop in motion
        /// </summary>
        private async void makeGame()
        {
            // Lock the game page to prevent invalid input
            Frame.IsEnabled = false;

            // Make the game elements invisible
            makeControlsInvisible();

            tbkCountdown.Visibility = Visibility.Visible;
           
            // Countdown the game start
            await startCountdown();
            meBackground.AutoPlay = true;
            meBackground.Play();

            // Hide the countdown box
            tbkCountdown.Visibility = Visibility.Collapsed;
            bCountdownCompleted = true;

            // Enable the input
            Frame.IsEnabled = true;

            // Make the screen elements visible
            makeControlsVisible();
            
            // Set the score display at 0/0 and timer display to empty 
            tbkScore.Text = string.Format("Score: {0,3:##0} / {1:##0}", sPlayerScore.TotalCorrect, sPlayerScore.TotalQuestions);
            tbkTimer.Text = "";

            // Start the game timer
            startTimer();

            // Generate the first question
            makeQuestion();
        }
        
        /// <summary>
        ///     Shows all necessary game elements on the screen
        /// </summary>
        private void makeControlsVisible()
        {
            tbkTimer.Visibility = Visibility.Visible;
            tbkScore.Visibility = Visibility.Visible;
            tbkQuestion.Visibility = Visibility.Visible;
            tbkAdd.Visibility = Visibility.Visible;
            tbkSubtract.Visibility = Visibility.Visible;
            tbkMultiply.Visibility = Visibility.Visible;
            tbkDivide.Visibility = Visibility.Visible;
            tbkSwipeHori.Visibility = Visibility.Visible;
            tbkSwipeVert.Visibility = Visibility.Visible;
        }
        
        /// <summary>
        ///     Hides all game elements from view
        /// </summary>
        private void makeControlsInvisible()
        {
            tbkTimer.Visibility = Visibility.Collapsed;
            tbkScore.Visibility = Visibility.Collapsed;
            tbkQuestion.Visibility = Visibility.Collapsed;
            tbkAdd.Visibility = Visibility.Collapsed;
            tbkSubtract.Visibility = Visibility.Collapsed;
            tbkMultiply.Visibility = Visibility.Collapsed;
            tbkDivide.Visibility = Visibility.Collapsed;
            tbkSwipeHori.Visibility = Visibility.Collapsed;
            tbkSwipeVert.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Timers
        /// <summary>
        ///     Displays a countdown to game starting at 3
        /// </summary>
        /// <returns></returns>
        private async Task startCountdown()
        {
            await Task.Delay(500);
            tbkCountdown.Visibility = Visibility.Visible;
            for (int i = 3; i > 0; i--)
            {
                playSound(uCount);
                tbkCountdown.Text = string.Format("{0}", i);
                await Task.Delay(1000);
            }
            playSound(uCount);
        }
        
        /// <summary>
        ///     Displays a game timer starting at one minute
        /// </summary>
        private void startTimer()
        {
            // Set the timer tick interval at 100 milliseconds
            dptTimer.Tick += DptTimer_Tick;
            dptTimer.Interval = TimeSpan.FromMilliseconds(100);

            // Select timer length
            switch (gDifficulty.DifficultyCode)
            {
                case 'b':
                    dtmFinishTime = DateTime.Now.AddSeconds(90);
                    break;
                case 'm':
                    dtmFinishTime = DateTime.Now.AddSeconds(75);
                    break;
                case 'h':
                    dtmFinishTime = DateTime.Now.AddSeconds(60);
                    break;
            }
            dptTimer.Start();
        }
        #endregion

        #region makeQuestion
        /// <summary>
        ///     Builds a number sentence with random operands and a random cOperatorerator 
        ///     which the player has to guess
        /// </summary>
        private void makeQuestion()
        {
            bQuestionAttempted = false;
            int[] iAddRange = gDifficulty.AddRange;
            int[] iSubtractRange = gDifficulty.SubtractRange;
            int[] iMultiplyRange = gDifficulty.MultiplyRange;
            int[] iDivideRange = gDifficulty.DivideRange;

            SolidColorBrush scbWhite = new SolidColorBrush(Colors.White);

            tbkAdd.Foreground = scbWhite;
            tbkSubtract.Foreground = scbWhite;
            tbkMultiply.Foreground = scbWhite;
            tbkDivide.Foreground = scbWhite;

            tbkAdd.FontWeight = FontWeights.Normal;
            tbkSubtract.FontWeight = FontWeights.Normal;
            tbkMultiply.FontWeight = FontWeights.Normal;
            tbkDivide.FontWeight = FontWeights.Normal;

            tbkQuestion.Visibility = Visibility.Visible;

            // Make a new random number generator
            Random myRandom = new Random();

            // Set to full Opacity
            tbkQuestion.Opacity = 1.0;

            // Choose a random operator
            switch (myRandom.Next(1, 5))
            {
                // Addition with the operand range specified by gDifficulty
                case 1:
                    qQuestion = new Question('+', iAddRange);
                    sPlayerScore.AddQuestions++;
                    break;

                // Subtraction with the operand range specified by gDifficulty
                case 2:
                    qQuestion = new Question('-', iSubtractRange);
                    sPlayerScore.SubtractQuestions++;
                    break;

                // Multiplication with the operand range specified by gDifficulty
                case 3:
                    qQuestion = new Question('×', iMultiplyRange);
                    sPlayerScore.MultiplyQuestions++;
                    break;

                // Division with the operand range specified by gDifficulty
                case 4:
                    qQuestion = new Question('÷', iDivideRange);
                    sPlayerScore.DivideQuestions++;
                    break;
            }

            Frame.IsEnabled = true;
            // Show the question
            tbkQuestion.Text = string.Format("{0} ☺ {1} = {2}", qQuestion.Operand1, qQuestion.Operand2, qQuestion.Answer);
        }
        #endregion

        #region Perform Operations
        /// <summary>
        ///     Checks if the player's anser of addition is correct
        /// </summary>
        private void performAdd()
        {
            if (qQuestion.Operand1 + qQuestion.Operand2 == qQuestion.Answer)
            {
                // Sometimes the number sentence for add and multiply is the same so this makes the display consistant with input
                if (qQuestion.Operator == '×')
                {
                    sPlayerScore.MultiplyQuestions--;
                    sPlayerScore.AddQuestions++;
                    qQuestion.Operator = '+';
                }

                // Adds to the added correctly counter
                sPlayerScore.AddCorrect++;

                // Checks whether the difficulty should be increased for addition operations
                if (bAddCorrect && sPlayerScore.AddAccuracy > 0.9)
                {
                    gDifficulty.increaseAdd();
                    playSound(uLevelUp);
                }
                else
                {
                    playSound(uLaser);
                }
                tbkAdd.Foreground = new SolidColorBrush(Colors.PaleGreen);
                bAddCorrect = true;
                showTick();
            }
            else
            {
                decreaseDifficulty();
                tbkAdd.Foreground = new SolidColorBrush(Colors.Maroon);
                bAddCorrect = false;
                showCross();
            }
            tbkAdd.FontWeight = FontWeights.Bold;
            bQuestionAttempted = true;
        }

        /// <summary>
        ///     Checks if the player's anser of suctraction is correct
        /// </summary>
        private void performSubtract()
        {
            if (qQuestion.Operand1 - qQuestion.Operand2 == qQuestion.Answer)
            {
                // Sometimes the number sentence for subtract and divide is the same so this makes the display consistant with input
                if (qQuestion.Operator == '÷')
                {
                    sPlayerScore.DivideQuestions--;
                    sPlayerScore.SubtractQuestions++;
                    qQuestion.Operator = '-';
                }

                // Adds to the subtracted correctly counter
                sPlayerScore.SubtractCorrect++;

                // Checks whether the difficulty should be increased for subtraction operations
                if (bSubtractCorrect && sPlayerScore.SubtractAccuracy > 0.9)
                {
                    gDifficulty.increaseSubtract();
                    playSound(uLevelUp);
                }
                else
                {
                    playSound(uLaser);
                }
                tbkSubtract.Foreground = new SolidColorBrush(Colors.PaleGreen);
                bSubtractCorrect = true;
                showTick();
            }
            else
            {
                decreaseDifficulty();
                tbkSubtract.Foreground = new SolidColorBrush(Colors.Maroon);
                bSubtractCorrect = false;
                showCross();
            }
            tbkSubtract.FontWeight = FontWeights.Bold;
            bQuestionAttempted = true;
        }

        /// <summary>
        ///     Checks if the player's answer of multiplication is correct
        /// </summary>
        private void performMultiply()
        {
            if (qQuestion.Operand1 * qQuestion.Operand2 == qQuestion.Answer)
            {
                // Sometimes the number sentence for multiply and divide is the same so this makes the display consistant with input
                if (qQuestion.Operator == '÷')
                {
                    sPlayerScore.DivideQuestions--;
                    sPlayerScore.MultiplyQuestions++;
                    qQuestion.Operator = '×';
                }

                // Sometimes the number sentence for multiply and add is the same so this makes the display consistant with input
                if (qQuestion.Operator == '+')
                {
                    sPlayerScore.AddQuestions--;
                    sPlayerScore.MultiplyQuestions++;
                    qQuestion.Operator = '×';
                }

                // Adds to the multiplied correctly counter
                sPlayerScore.MultiplyCorrect++;

                // Checks whether the difficulty should be increased for multiply operations
                if (bMultiplyCorrect && sPlayerScore.MultiplyAccuracy > 0.9)
                {
                    gDifficulty.increaseMultiply();
                    playSound(uLevelUp);
                }
                else
                {
                    playSound(uLaser);
                }
                tbkMultiply.Foreground = new SolidColorBrush(Colors.PaleGreen);
                bMultiplyCorrect = true;
                showTick();
            }
            else
            {
                decreaseDifficulty();
                tbkMultiply.Foreground = new SolidColorBrush(Colors.Maroon);
                bMultiplyCorrect = false;
                showCross();
            }
            tbkMultiply.FontWeight = FontWeights.Bold;
            bQuestionAttempted = true;
        }

        /// <summary>
        ///     Checks if the players answer of division is correct
        /// </summary>
        private void performDivide()
        {
            if (qQuestion.Operand1 / qQuestion.Operand2 == qQuestion.Answer)
            {
                // Sometimes the number sentence for multiply and divide is the same so this makes the display consistant with input
                if (qQuestion.Operator == '×')
                {
                    sPlayerScore.MultiplyQuestions--;
                    sPlayerScore.DivideQuestions++;
                    qQuestion.Operator = '÷';
                }

                // Sometimes the number sentence for subtract and divide is the same so this makes the display consistant with input
                if (qQuestion.Operator == '-')
                {
                    sPlayerScore.SubtractQuestions--;
                    sPlayerScore.DivideQuestions++;
                    qQuestion.Operator = '÷';
                }

                // Adds to the divided correctly counter
                sPlayerScore.DivideCorrect++;

                // Checks whether the difficulty should be increased for divide operations
                if (bDivideCorrect && sPlayerScore.DivideAccuracy > 0.9)
                {
                    gDifficulty.increaseDivide();
                    playSound(uLevelUp);
                }
                else
                {
                    playSound(uLaser);
                }
                tbkDivide.Foreground = new SolidColorBrush(Colors.PaleGreen);
                bDivideCorrect = true;
                showTick();
            }
            else
            {
                decreaseDifficulty();
                tbkDivide.Foreground = new SolidColorBrush(Colors.Maroon);
                bDivideCorrect = false;
                showCross();
            }
            tbkDivide.FontWeight = FontWeights.Bold;
            bQuestionAttempted = true;
        }
        
        /// <summary>
        ///     Helper method to decide which aspect of difficulty to decrease
        /// </summary>
        private void decreaseDifficulty()
        {
            switch (qQuestion.Operator)
            {
                case '+':
                    // Checks whether the difficulty should be decreased for addition operations
                    if (sPlayerScore.AddQuestions > 1 && !bAddCorrect && sPlayerScore.AddAccuracy < 0.6)
                    {
                        gDifficulty.decreaseAdd();
                        playSound(uLevelDown);
                    }
                    else
                    {
                        playSound(uLaser);
                    }
                    break;
                case '-':
                    // Checks whether the difficulty should be decreased for subtraction operations
                    if (sPlayerScore.SubtractQuestions > 1 && !bSubtractCorrect && sPlayerScore.SubtractAccuracy < 0.6)
                    {
                        gDifficulty.decreaseSubtract();
                        playSound(uLevelDown);
                    }
                    else
                    {
                        playSound(uLaser);
                    }
                    break;
                case '×':
                    // Checks whether the difficulty should be decreased for multiply operations
                    if (sPlayerScore.MultiplyQuestions > 1 && !bMultiplyCorrect && sPlayerScore.MultiplyAccuracy < 0.6)
                    {
                        gDifficulty.decreaseMultiply();
                        playSound(uLevelDown);
                    }
                    else
                    {
                        playSound(uLaser);
                    }
                    break;
                case '÷':
                    // Checks whether the difficulty should be decreased for divide operations
                    if (sPlayerScore.DivideQuestions > 1 && !bDivideCorrect && sPlayerScore.DivideAccuracy < 0.6)
                    {
                        gDifficulty.decreaseDivide();
                        playSound(uLevelDown);
                    }
                    else
                    {
                        playSound(uLaser);
                    }
                    break;
            }
        }
        #endregion

        #region Display Results
        /// <summary>
        ///     Occurs when the player gets the answer correct
        /// </summary>
        private async void showTick()
        {
            Frame.IsEnabled = false;

            // Update the score display
            tbkScore.Text = string.Format("Score: {0,3:##0} / {1:##0}", sPlayerScore.TotalCorrect, sPlayerScore.TotalQuestions);

            if ((dtmFinishTime-DateTime.Now).TotalMilliseconds > 0)
            {
                // Add bonus time
                dtmFinishTime = dtmFinishTime.AddSeconds(2);

                // Show the correct operator
                tbkQuestion.Text = string.Format("{0} {1} {2} = {3}", qQuestion.Operand1, qQuestion.Operator, qQuestion.Operand2, qQuestion.Answer);
                
                tbkSwipeHori.Visibility = Visibility.Collapsed;
                tbkSwipeVert.Visibility = Visibility.Collapsed;

                // Show a tick image
                imgTick.Opacity = 1.0;

                // Fade the tick and the question
                for (int i = 100; i >= 0; i--)
                {
                    tbkQuestion.Opacity = i / 100.0;
                    imgTick.Opacity = i / 20.0;
                    await Task.Delay(1);
                }

                tbkSwipeHori.Visibility = Visibility.Visible;
                tbkSwipeVert.Visibility = Visibility.Visible;

                // Make a new question
                if ((dtmFinishTime - DateTime.Now).TotalMilliseconds > 99)
                {
                    makeQuestion();
                }
            }
        }
        
        /// <summary>
        ///     Occurs whe the player gets the answer incorrect
        /// </summary>
        private async void showCross()
        {
            Frame.IsEnabled = false;

            // Update the score display
            tbkScore.Text = string.Format("Score: {0,3:##0} / {1:##0}", sPlayerScore.TotalCorrect, sPlayerScore.TotalQuestions);

            if ((dtmFinishTime - DateTime.Now).TotalMilliseconds > 0)
            {
                // Show the correct operator
                tbkQuestion.Text = string.Format("{0} {1} {2} = {3}", qQuestion.Operand1, qQuestion.Operator, qQuestion.Operand2, qQuestion.Answer);

                tbkSwipeHori.Visibility = Visibility.Collapsed;
                tbkSwipeVert.Visibility = Visibility.Collapsed;

                // Show a cross image
                imgCross.Opacity = 1.0;

                vbVibrate.Vibrate(TimeSpan.FromMilliseconds(250));

                // Fade the cross and the question
                for (int i = 100; i >= 0; i--)
                {
                    tbkQuestion.Opacity = i / 100.0;
                    imgCross.Opacity = i / 20.0;
                    await Task.Delay(1);
                }

                tbkSwipeHori.Visibility = Visibility.Visible;
                tbkSwipeVert.Visibility = Visibility.Visible;

                // Make a new question
                if ((dtmFinishTime - DateTime.Now).TotalMilliseconds > 99)
                {
                    makeQuestion();
                }
            }
        }

        /// <summary>
        ///     The timer has run out and the current game is over
        /// </summary>
        private async void gameOver()
        {
            // Stop the background sounds
            meBackground.IsMuted = true;

            await loadScoreList();

            Frame.IsEnabled = false;

            // Checks if the current question was attempted to show the correct score
            if (!bQuestionAttempted)
            {
                switch (qQuestion.Operator)
                {
                    case '+':
                        sPlayerScore.AddQuestions--;
                        break;
                    case '-':
                        sPlayerScore.SubtractQuestions--;
                        break;
                    case '×':
                        sPlayerScore.MultiplyQuestions--;
                        break;
                    case '÷':
                        sPlayerScore.DivideQuestions--;
                        break;
                }
            }
            else
            {
                switch (qQuestion.Operator)
                {
                    case '+':
                        sPlayerScore.AddQuestions++;
                        break;
                    case '-':
                        sPlayerScore.SubtractQuestions++;
                        break;
                    case '×':
                        sPlayerScore.MultiplyQuestions++;
                        break;
                    case '÷':
                        sPlayerScore.DivideQuestions++;
                        break;
                }
            }

            dptTimer.Tick -= DptTimer_Tick;
            

            makeControlsInvisible();

            // Add the score to the score list
            lScoreList.Add(sPlayerScore);
            lScoreList.Sort(new ScoreComparer());
            if (lScoreList.Count > 10)
            {
                lScoreList.RemoveRange(10, 1);
            }

            await saveScoreList();

            Frame.IsEnabled = true;
            // Timer finished, Message Dialogue appears for user choice
            MessageDialog msgRestart = new MessageDialog(sPlayerScore + "Restart or Quit?");

            msgRestart.Commands.Add(new UICommand("Restart"));
            msgRestart.Commands.Add(new UICommand("Quit"));
            msgRestart.Title = "Game Over";
            

            // Wait for user choice
            IUICommand choice = await msgRestart.ShowAsync();
            playSound(uBlip);

            // Restart the game
            if (choice == msgRestart.Commands[0])
            {
                Frame.Navigate(typeof(GamePage), gDifficulty.DifficultyCode);
                Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
            }

            // Quit to the menu screen
            if (choice == msgRestart.Commands[1])
            {
                Frame.IsEnabled = true;
                Frame.GoBack();
            }
        }

        #endregion

        /// <summary>
        ///     Overcomes a MediaElement limitation on Windows Phone 8.1
        /// </summary>
        /// <param name="sound"></param>
        private async void playSound(Uri sound)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (meSharedMediaElement_1.CurrentState == MediaElementState.Playing)
                {
                    if (meSharedMediaElement_2.CurrentState == MediaElementState.Playing)
                    {
                        meSharedMediaElement_3.Source = sound;
                    }
                    else
                    {
                        meSharedMediaElement_2.Source = sound;
                    }
                }
                else
                {
                    meSharedMediaElement_1.Source = sound;
                }
            });
        }
    }
}
