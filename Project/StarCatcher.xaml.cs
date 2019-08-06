/* CLASS NAME: Star Catcher
 * AUTHOR: Wade Carmichael
 * STUDENT NUMBER: c3259655
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * Star Catcher is a game that uses tilt controls. It allows the player to roll the phone to the left or right which controls the catcher.
 * Stars spawn at the top of the screen and fall toward the bottom. The player must use the catcher to catch the stars. Yellow stars are worth 1 point while
 * Red stars are worth 5 points. These points contribute to your overall score. If you miss 5 stars then the game is over.
 * 
 * It is hoped that this game element would aid in the development of
 * a young player's concentration and hand/eye co-ordination and attention.
 * 
 * All sounds are copyright free and were sourced from:
 * https://www.freesound.org/people/plasterbrain/sounds/351810/
 * https://www.freesound.org/people/myfox14/sounds/382310/
 * 
 * 
        ** Parts of this file were adapted from the week two turorial from INFT2050 in MobileGame.zip **
 */




#region Namespaces
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Devices.Sensors;
using Windows.Phone.UI.Input;
using Windows.UI.Core;

#endregion


namespace Project
{

    public sealed partial class StarCatcher : Page
    {
        #region Instance Variables
        private Random rand = new Random(); //A Random number generator
        private List<Image> lstStar; // A list that holds all the yellow stars
        private List<animation> lstMovement; // A list that defines the position of the Yellow Stars
        private List<Image> lstStar2; // A list that holds all the Red stars
        private List<animation> lstMovement2; // A list that defines the position of the Red Stars
        private int numSpawn = 1; // Decides how many stars are spawned.
        private Rect bounds; // Will be used to get the width and height of the screen
        private double iPosition = 150; // Initial position of the catcher
        private int iScore = 0; // The int used to keep track of the games score
        private Accelerometer myAccelerometer; // Creates an accelerometer 
        private double roll; // Used to keep track of the roll variable from the accelerometer
        private DispatcherTimer dptTimer; // Used to time the spawning of stars
        private int iLose; // Used to determine when a player loses the game
        private bool bStarId = true; // Used to determine when to spawn stars.
        private Image imStar; // Star Image
        private Image imStar2; // Star Image
        private int starAngle = 0; // The angle at which the star rotates
        private Uri starSound; // Used to create an uri for the starCapture sound
        private Uri uBlip;
        private Uri endGame;
        #endregion
       
        #region Constructor
        public StarCatcher() //Intialises all the starting variables
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            bounds = Window.Current.Bounds; //Assigns the width and height of the screen to bounds
            lstStar = new List<Image>();
            lstMovement = new List<animation>();
            lstStar2 = new List<Image>();
            lstMovement2 = new List<animation>();
            starSound = new Uri("ms-appx:///Assets/shootingStarMini.WAV");
            uBlip = new Uri("ms-appx:///Assets/Blip.mp3");
            endGame = new Uri("ms-appx:///Assets/gameOver.WAV");

            myAccelerometer = Accelerometer.GetDefault();
            roll = 0.0; // Sets the initial value or roll to 0;
            dptTimer = new DispatcherTimer();
            if (myAccelerometer != null) //Checks to see if phone has an accelerometer
            {
                uint reportInterval;
                if (myAccelerometer.MinimumReportInterval > 10)
                {
                    reportInterval = myAccelerometer.MinimumReportInterval;
                }
                else
                {
                    reportInterval = 10;
                }
                myAccelerometer.ReportInterval = reportInterval;

                // Create the accelerometer event handler
                myAccelerometer.ReadingChanged += MyAccelerometer_ReadingChanged;
            }



        }



        #endregion
       
        #region On Navitaged To and From
        protected override void OnNavigatedTo(NavigationEventArgs e) //When page is navigated to
        {

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            CompositionTarget.Rendering += this.updateStars; //Constantly updates position of stars
            Canvas.SetTop(catcher, bounds.Height - 110);
            tbkScore.Text = string.Format("Score: {0,4:###0}", iScore);
            dptTimer.Tick += DptTimer_Tick; // Creates event handler
            dptTimer.Interval = TimeSpan.FromMilliseconds(1000); //Sets time span
            dptTimer.Start(); // Starts timer. Timer ticks every second.

        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            playSound(uBlip);
            e.Handled = true;
            Frame.GoBack();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) //When leaving the page
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            CompositionTarget.Rendering -= updateStars;
            dptTimer.Stop();
            dptTimer.Tick -= DptTimer_Tick;
            myAccelerometer.ReadingChanged -= MyAccelerometer_ReadingChanged;
        }
        #endregion
        
        #region Timer
        private void DptTimer_Tick(object sender, object e) // Timer that ticks every second.
        {
            if (bStarId) // Yellow stars are continuously spawned
            {
                createYellowStar();
            }

            if (bStarId && iScore >= 10) // When score is equal to or greater than 10, Red Stars are spawned
            {
                createRedStar();
            }

        }
        #endregion
        
        #region Create Stars
        // ** Parts of this method were adapted from the week two turorial from INFT2050 in MobileGame.zip **
        public void createYellowStar() //The method for creating the Yellow stars
        {


            for (int i = 0; i < numSpawn; i++)
            {
                int xPos = rand.Next((int)bounds.Width - 46); //Assigns random x coordinate from the width of the screen
                int yPos = 20; // Sets the starting y coordinate as 20
                animation position = new animation(xPos, yPos); //Assigns x and y to object of animation class
                lstMovement.Add(position); // adds object to list

                //Creates a new star image, sets the renderTransformOrgin so that it can rotate, sets the starting position and finally adds to canvas and list.
                imStar = new Image();
                imStar.Source = new BitmapImage(new Uri("ms-appx:///Assets/Star.png"));
                imStar.RenderTransformOrigin = new Point(0.5, 0.5);
                imStar.SetValue(Canvas.LeftProperty, xPos);
                imStar.SetValue(Canvas.TopProperty, yPos);
                canGameStars.Children.Add(imStar);
                lstStar.Add(imStar);
            }

        }

        //Same as createYellowStar
        public void createRedStar()
        {


            for (int i = 0; i < numSpawn; i++)
            {
                int xPos = rand.Next((int)bounds.Width - 46);
                int yPos = 20;
                animation movementSpeed2 = new animation(xPos, yPos);
                lstMovement2.Add(movementSpeed2);


                imStar2 = new Image();
                imStar2.Source = new BitmapImage(new Uri("ms-appx:///Assets/Star2.png"));
                imStar2.SetValue(Canvas.LeftProperty, xPos);
                imStar2.SetValue(Canvas.TopProperty, yPos);
                canGameStars.Children.Add(imStar2);
                lstStar2.Add(imStar2);
            }

        }





        #endregion 
        
        #region update Stars
        
        // ** Parts of this section were adapted from the week two turorial from INFT2050 in MobileGame.zip**

        //This is the method that continually updates the stars positions and where majority of the game logic is housed.
        private void updateStars(object sender, object e)
        {
            //For loop continues as long as there are stars in the list
            for (int i = 0; i < lstStar.Count; i++)
            {
                //Spawns the star, moves the star down by 3 pixels each cycle and creates an object of RotateTransform.
                Canvas.SetTop(lstStar[i], lstMovement[i].Ypos);
                lstMovement[i].Ypos += 3;
                RotateTransform rtRotate = new RotateTransform();

                //Sets the rotation of the star and continually roates the star.
                starAngle += 1;
                rtRotate.Angle = starAngle;
                lstStar[i].RenderTransform = rtRotate;

                //If each collides with the catcher then the star is removed from the image list and movement list as well as the canvas. Score is increased by 1.
                //Score textbox is updated and a sound is played.
                if (lstMovement[i].Ypos >= bounds.Height - 110 && lstMovement[i].Ypos <= bounds.Height - 10 && lstMovement[i].Xpos >= iPosition && lstMovement[i].Xpos <= (iPosition + 100))
                {
                    canGameStars.Children.Remove(lstStar[i]);
                    lstMovement.RemoveAt(i);
                    lstStar.RemoveAt(i);
                    iScore += 1;
                    tbkScore.Text = string.Format("Score: {0,4:###0}", iScore);
                    playSound(starSound);
                }
                // If player fails to catch as star then the star is removed from both lists and canvas and the iLose variable is increased by 1.
                try
                {
                    if (lstMovement[i].Ypos > bounds.Height + 10)
                    {
                        canGameStars.Children.Remove(lstStar[i]);
                        lstMovement.RemoveAt(i);
                        lstStar.RemoveAt(i);
                        iLose += 1;
                        //If the iLose variable is greater than or equal to 5, then the game is over. All the stars are cleared, the final score is displayed
                        //and the reset button appears.
                        if (iLose >= 5)
                        {
                            end();

                        }


                    }
                }
                catch{ }

            }

            //This is same as the above code, however the lists are differernt and the red stars move at double the speed.
            for (int i = 0; i < lstStar2.Count; i++)
            {

                Canvas.SetTop(lstStar2[i], lstMovement2[i].Ypos);
                lstMovement2[i].Ypos += 6;
                RotateTransform rtRotate = new RotateTransform();

                starAngle += 1;
                rtRotate.Angle = starAngle;
                lstStar2[i].RenderTransform = rtRotate;



                if (lstMovement2[i].Ypos >= bounds.Height - 110 && lstMovement2[i].Ypos <= bounds.Height - 10 && lstMovement2[i].Xpos >= iPosition && lstMovement2[i].Xpos <= (iPosition + 100))
                {
                    canGameStars.Children.Remove(lstStar2[i]);
                    lstMovement2.RemoveAt(i);
                    lstStar2.RemoveAt(i);
                    iScore += 5;
                    tbkScore.Text = string.Format("Score: {0,4:###0}", iScore);
                    playSound(starSound);
                }

                try
                {
                    if (lstMovement2[i].Ypos >= bounds.Height + 10)
                    {
                        canGameStars.Children.Remove(lstStar2[i]);
                        lstMovement2.RemoveAt(i);
                        lstStar2.RemoveAt(i);
                        iLose += 1;

                        if (iLose >= 5)
                        {
                            end();
                        }


                    }
                }
                catch { }

            }
        }

        public void end()
        {
            tbkLose.Visibility = Visibility.Visible;
            tbkLose.Text = "Five stars got away! \n\n" + "Your score was " + iScore + "." + "\n\nPress Retry to play again.";
            playSound(endGame);
            bStarId = false;
            lstMovement.Clear();
            lstStar.Clear();
            lstMovement2.Clear();
            lstStar2.Clear();
            tbkScore.Visibility = Visibility.Collapsed;
            catcher.Visibility = Visibility.Collapsed;
            btnReset.Visibility = Visibility.Visible;
            canGameStars.Children.Clear();
        }
        #endregion
        
        #region Move Catcher
        //This code is responsible for moving the catcher by using the accelerometer.
        private void moveCatcher()
        {
            //Sets the postion of the catcher and updates the iPosition.
            Canvas.SetLeft(catcher, (iPosition += 0.1 * roll));

            // If the catcher leaves the screen on the left it is placed on the right.
            if (Canvas.GetLeft(catcher) < -100)
            {
                Canvas.SetLeft(catcher, bounds.Width + 100);
                iPosition = bounds.Width + 100;
            }

            // If the catcher leaves the screen on the right is is placed on the left.
            if (Canvas.GetLeft(catcher) > bounds.Width + 100)
            {
                Canvas.SetLeft(catcher, -100);
                iPosition = -100;
            }


        }

        //Uses the accelerometer to update the roll and moves the catcher.
        private async void MyAccelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                roll = args.Reading.AccelerationX * 100;

                moveCatcher();

            });
        }
        #endregion
        
        #region Reset Button
        //When the button is pressed all variables are reset. 
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {

            Frame.Navigate(typeof(StarCatcher));
            Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);

        }
        #endregion
        
        #region Sound organiser
        //Used to play multiple sounds at once. Up to 3 sounds can be played at once.
        //It checks if a media element is being played. If media element 1 is being used, use media element 2 etc.
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
        #endregion
    }

    #region  animation class

    // ** This class was adapted from the week two turorial from INFT2050 in MobileGame.zip**

    //This class is used to store the variables of the stars.
    public class animation
    {
        private int xpos;
        private int ypos;

        #region Int properties
        //Get and set methods.
        public int Xpos
        {
            get
            {
                return xpos;
            }
            set
            {
                if (value >= 0)
                {
                    xpos = value;
                }
            }
        }
        public int Ypos
        {
            get
            {
                return ypos;
            }
            set
            {
                if (value >= 0)
                {
                    ypos = value;
                }
            }
        }

        #endregion
        public animation(int xStart, int yStart)
        {
            SetPosition(xStart, yStart);
        }

        public void SetPosition(int xStart, int yStart)
        {
            Xpos = xStart;
            Ypos = yStart;
        }



    }
    #endregion
}
