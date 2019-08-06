/* CLASS NAME: MrSpaceman
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * MrSpaceman is the underlying code for the game page MrSpaceman.xaml
 * 
 * The spaceman is controlled by tilting the device and the aim is to
 * collect the oxygen tanks placed on th screen at random for as long
 * as possible.
 * 
 * It is hoped that this game element would aid in the development of
 * a young player's concentration and hand/eye co-ordination and attention.
 */

#region Namespaces Used
using System;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Phone.Devices.Notification;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.UI.Xaml.Input;
#endregion

namespace Project
{
    public sealed partial class MrSpaceman : Page
    {
        #region Instance Variables
        private Accelerometer myAccelerometer;
        private double pitch;
        private double roll;
        private int manAngle;
        private int tankAngle;
        private int shipAngle;
        private Rect rctBounds;
        private DispatcherTimer dptO2Timer;
        private DispatcherTimer dptRescueTimer;
        private DateTime dtmRescueTime;
        private Image imgO2Tank;
        private int iO2Helper;
        private Random myRandom;
        private VibrationDevice vbVibrate;
        private int iVibrateCounter;
        #endregion

        #region Constructor
        public MrSpaceman()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            // Instantiate instance variables
            myAccelerometer = Accelerometer.GetDefault();
            pitch = 0.0;
            roll = 0.0;
            manAngle = 0;
            tankAngle = 0;
            shipAngle = 0;
            rctBounds = Window.Current.Bounds;
            dptO2Timer = new DispatcherTimer();
            dptRescueTimer = new DispatcherTimer();
            dtmRescueTime = new DateTime();
            imgO2Tank = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/O2tank.png")),
                Visibility = Visibility.Collapsed,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            iO2Helper = 0;
            myRandom = new Random();
            vbVibrate = VibrationDevice.GetDefault();
            iVibrateCounter = 0;

            // Set the report interval of the accelerometer
            if (myAccelerometer != null)
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

        #region Event Handlers
        /// <summary>
        ///     Invoked when the page is about to load
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsBackPressed;
        }
        
        /// <summary>
        ///     Invoked when the page is about to unload
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsBackPressed;
            dptO2Timer.Stop();
            dptO2Timer.Tick -= DptO2Timer_Tick;
            dptRescueTimer.Tick += DptRescueTimer_Tick;
            Frame.Tapped -= Frame_Tapped;
        }
        
        /// <summary>
        ///     Invoked when the device back button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            meBlip.Play();
            Frame.GoBack();
        }
        
        /// <summary>
        ///     Invoked when the page is fully loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Creates random coordinates to spawn a tank
            double xCoord = myRandom.NextDouble() * rctBounds.Width - 16;
            double yCoord = myRandom.NextDouble() * rctBounds.Height - 16;

            // Set the coordinates and place the tank on the canvas
            Canvas.SetLeft(imgO2Tank, xCoord);
            Canvas.SetTop(imgO2Tank, yCoord);
            Canvas.SetLeft(elpO2Helper, xCoord);
            Canvas.SetTop(elpO2Helper, yCoord);

            cnvHappyCanvas.Children.Add(imgO2Tank);
            imgO2Tank.Visibility = Visibility.Visible;
            elpO2Helper.Visibility = Visibility.Visible;

            // Start oxygen depletion
            dptO2Timer.Tick += DptO2Timer_Tick;
            dptO2Timer.Interval = TimeSpan.FromMilliseconds(10);
            Frame.IsTapEnabled = false;
            dptO2Timer.Start();

            // Countdown to Rescue
            dptRescueTimer.Tick += DptRescueTimer_Tick;
            dptRescueTimer.Interval = TimeSpan.FromMilliseconds(10);
            dptRescueTimer.Start();
            Canvas.SetTop(imgSpaceship, 0);
            dtmRescueTime = DateTime.Now.AddSeconds(90);
        }

        private void DptRescueTimer_Tick(object sender, object e)
        {
            if ((dtmRescueTime - DateTime.Now).TotalMilliseconds <= 0)
            {
                dptO2Timer.Stop();
                rescue();
            }
            else
            {
                Canvas.SetTop(imgSpaceship, Canvas.GetTop(imgSpaceship) + 0.2);
                imgSpaceship.Width += 0.05;
                imgSpaceship.Height += 0.05;
                meBackground.Volume += 0.001;
                Canvas.SetLeft(imgSpaceship, rctBounds.Width / 2 - imgSpaceship.Width / 2);
                tbkTimer.Text = string.Format("Time to Rescue {0:mm\\:ss\\.f}", dtmRescueTime - DateTime.Now);
            }
        }

        /// <summary>
        ///     Invoked each time the Timer tick interval has expired
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DptO2Timer_Tick(object sender, object e)
        {
            // Check if the oxygen has run out
            if (pgbO2Supply.Value > pgbO2Supply.Minimum)
            {
                // Rotate the oxygen tank and decrement
                RotateTransform rtRotateTank = new RotateTransform();
                imgO2Tank.RenderTransform = rtRotateTank;
                tankAngle += 2;
                rtRotateTank.Angle = tankAngle;
                pgbO2Supply.Value -= 0.9;
                iO2Helper += 1;

                // Changes the colour of the O2 meter and begins vibrating the device at intervals
                if (pgbO2Supply.Value < 200)
                {
                    pgbO2Supply.Foreground = new SolidColorBrush(Colors.Red);
                    iVibrateCounter += 10;
                    if (iVibrateCounter % 250 == 0)
                        vbVibrate.Vibrate(TimeSpan.FromMilliseconds(250));
                }
                else
                {
                    iVibrateCounter = 0;
                    pgbO2Supply.Foreground = new SolidColorBrush(Colors.Indigo);
                }

                if (iO2Helper == 5)
                {
                    elpO2Helper.Visibility = Visibility.Collapsed;
                }

                //------------------------------------------------------------------------------------------------------------
                // Some help from this source https://developer.mozilla.org/en-US/docs/Games/Techniques/2D_collision_detection

                // Get the current positions of the objects' centres
                Point mrSpacemanCentre = new Point(Canvas.GetLeft(imgMrSpaceman) + 40, Canvas.GetTop(imgMrSpaceman) + 40);
                Point o2TankCentre = new Point(Canvas.GetLeft(imgO2Tank) + 16, Canvas.GetTop(imgO2Tank) + 16);
                
                // Calculate the distance between their centres
                double distance = Math.Sqrt(Math.Pow(o2TankCentre.X - mrSpacemanCentre.X, 2) +
                                            Math.Pow(o2TankCentre.Y - mrSpacemanCentre.Y, 2));
                
                // Check if the tank was collected
                if (distance < 25)
                {
                    // Add oxygen and spawn a new tank
                    pgbO2Supply.Value += 20;

                    meOxygen.Play();
                    meBlip.Play();
                    moveTank();
                }
                //------------------------------------------------------------------------------------------------------------
            }
            else
            {
                imgO2Tank.Visibility = Visibility.Collapsed;
                elpO2Helper.Visibility = Visibility.Collapsed;

                // Oxygen has run out, get the current vector of the spaceman
                double endPitch = pitch;
                double endRoll = roll;

                // Enable the screen for tap input and display message
                if (!Frame.IsTapEnabled)
                {
                    meOxygen.Stop();
                    meOxygen.Source = new Uri("ms-appx:///Assets/PHAZER.mp3");
                    tbkEndMessage.Text = "Oxygen Depleted! Tap the Screen.";
                    tbkEndMessage.Visibility = Visibility.Visible;
                    Frame.IsTapEnabled = true;
                    Frame.Tapped += Frame_Tapped;
                }

                // Disable tilt input
                myAccelerometer.ReadingChanged -= MyAccelerometer_ReadingChanged;

                // Continue the current vector of the spaceman.
                Canvas.SetLeft(imgMrSpaceman, Canvas.GetLeft(imgMrSpaceman) + 0.1 * endRoll);
                Canvas.SetTop(imgMrSpaceman, Canvas.GetTop(imgMrSpaceman) - 0.1 * endPitch);
                dptRescueTimer.Stop();
            }
        }
        
        /// <summary>
        ///     Invoked after the screen is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frame_Tapped(object sender, TappedRoutedEventArgs e)
        {
            dptO2Timer.Stop();
            dptO2Timer.Tick -= DptO2Timer_Tick;
            gameOver();
        }
        
        /// <summary>
        ///     Invoked when the device is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void MyAccelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Get the current accelerometer reading
                pitch = args.Reading.AccelerationY * 100;
                roll = args.Reading.AccelerationX * 100;

                // Calculate the spaceman trajectory
                moveMrSpaceman();
            });
        }
        #endregion

        #region Helper Functions
        ///<summary>
        ///     Moves and spins the spaceman on the screen corresponding to the amount of
        ///     tilt the player is providing
        /// </summary>
        private void moveMrSpaceman()
        {
            // Rotate MrSpaceman
            RotateTransform rtRotate = new RotateTransform();
            imgMrSpaceman.RenderTransform = rtRotate;
            manAngle = (int)(manAngle + (pitch + roll) / 4);
            rtRotate.Angle = manAngle;

            // Traverse the left and right borders
            if (Canvas.GetLeft(imgMrSpaceman) < -50)
            {
                Canvas.SetLeft(imgMrSpaceman, rctBounds.Width + 50);
            }
            
            if (Canvas.GetLeft(imgMrSpaceman) > rctBounds.Width + 50)
            {
                Canvas.SetLeft(imgMrSpaceman, -50);
            }
            
            // Set the X velocity
            Canvas.SetLeft(imgMrSpaceman, Canvas.GetLeft(imgMrSpaceman) + 0.1 * roll);

            // Traverse the bottom and top borders
            if (Canvas.GetTop(imgMrSpaceman) < -50)
            {
                Canvas.SetTop(imgMrSpaceman, rctBounds.Height + 50);
            }

            if (Canvas.GetTop(imgMrSpaceman) > rctBounds.Height + 50)
            {
                Canvas.SetTop(imgMrSpaceman, -50);
            }

            // Set the Y velocity
            Canvas.SetTop(imgMrSpaceman, Canvas.GetTop(imgMrSpaceman) - 0.1 * pitch);

            // Rotates the spaceship to point at the spaceman
            Point pMrSpacemanCentre = new Point(Canvas.GetLeft(imgMrSpaceman) + 40, Canvas.GetTop(imgMrSpaceman) + 40);
            Point pSpaceshipCentre = new Point(Canvas.GetLeft(imgSpaceship) + imgSpaceship.Width / 2, 
                Canvas.GetTop(imgSpaceship) + imgSpaceship.Height / 2);

            RotateTransform rtRotateShip = new RotateTransform();
            imgSpaceship.RenderTransform = rtRotateShip;

            double distance = Math.Sqrt(Math.Pow(pMrSpacemanCentre.X - pSpaceshipCentre.X, 2) + Math.Pow(pMrSpacemanCentre.Y - pSpaceshipCentre.Y, 2));

            shipAngle = (int)(180 * Math.Asin((pMrSpacemanCentre.Y - pSpaceshipCentre.Y) / distance) / Math.PI) - 90;

            if (pMrSpacemanCentre.X < pSpaceshipCentre.X)
            {
                if (shipAngle >= 0)
                {
                    shipAngle = -270 + (int) (180 * Math.Asin((pMrSpacemanCentre.Y - pSpaceshipCentre.Y) / distance) / Math.PI);
                }
                else if (shipAngle < 0)
                {
                    shipAngle =  90 - (int)(180 * Math.Asin((pMrSpacemanCentre.Y - pSpaceshipCentre.Y) / distance) / Math.PI);
                }
            }
            
            rtRotateShip.Angle = shipAngle;
        }

        ///<summary>
        ///     When the spaceman touches an oxygen tank it respawns in another location
        ///</summary>
        private void moveTank()
        {
            iO2Helper = 0;
            // Get new tank coordinates
            imgO2Tank.Visibility = Visibility.Collapsed;

            double xCoord = myRandom.NextDouble() * rctBounds.Width;
            double yCoord = myRandom.NextDouble() * rctBounds.Height;

            // Keeps the spawned oxygen tank from spawning off the screen
            if (xCoord + 20 >= rctBounds.Width)
            {
                xCoord = rctBounds.Width - 20;
            }
            else if (xCoord <= 20)
            {
                xCoord = 20;
            }
            if (yCoord + 45 >= rctBounds.Height)
            {
                yCoord = rctBounds.Height - 45;
            }
            else if (yCoord <= 20)
            {
                yCoord = 20;
            }

            // Spawn a new tank
            Canvas.SetLeft(imgO2Tank, xCoord);
            Canvas.SetTop(imgO2Tank, yCoord);
            Canvas.SetLeft(elpO2Helper, xCoord);
            Canvas.SetTop(elpO2Helper, yCoord);

            elpO2Helper.Visibility = Visibility.Visible;
            imgO2Tank.Visibility = Visibility.Visible;
        }

        private void rescue()
        {
            Point mrSpacemanCentre = new Point(Canvas.GetLeft(imgMrSpaceman) + 40, Canvas.GetTop(imgMrSpaceman) + 40);
            Point spaceshipCentre = new Point(Canvas.GetLeft(imgSpaceship) + imgSpaceship.Width / 2, Canvas.GetTop(imgSpaceship) + imgSpaceship.Height / 2);
            
            // Calculate the distance between their centres
            double distance = Math.Sqrt(Math.Pow(spaceshipCentre.X - mrSpacemanCentre.X, 2) +
                                        Math.Pow(spaceshipCentre.Y - mrSpacemanCentre.Y, 2));

            tbkEndMessage.Text = "Help is here! Get to the spaceship!";
            tbkEndMessage.Visibility = Visibility.Visible;
            imgO2Tank.Visibility = Visibility.Collapsed;
            elpO2Helper.Visibility = Visibility.Collapsed;;

            // Reached the spaceship
            if (distance < 35)
            {
                dptRescueTimer.Stop();
                if (!Frame.IsTapEnabled)
                {
                    meOxygen.Stop();
                    meOxygen.Source = new Uri("ms-appx:///Assets/Explosion_Hiss_Bop_Bang.mp3");
                    tbkEndMessage.Text = "You got rescued! Tap the screen.";
                    imgMrSpaceman.Visibility = Visibility.Collapsed;
                    Frame.IsTapEnabled = true;
                    Frame.Tapped += Frame_Tapped;
                    meBackground.IsMuted = true;
                }
                myAccelerometer.ReadingChanged -= MyAccelerometer_ReadingChanged;
            }
        }

        /// <summary>
        ///     Asks the player if they want to restart or quit.
        /// </summary>
        private async void gameOver()
        {
            // Player touched the screen, Message Dialogue appears for user choice
            MessageDialog msgRestart = new MessageDialog("");

            msgRestart.Commands.Add(new UICommand("Restart"));
            msgRestart.Commands.Add(new UICommand("Quit"));
            msgRestart.Title = "Game Over";

            // Wait for user choice
            IUICommand choice = await msgRestart.ShowAsync();
            meBlip.Play();
            // Restart the game
            if (choice == msgRestart.Commands[0])
            {
                Frame.Navigate(typeof(MrSpaceman));
                Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
            }

            // Quit to the menu screen
            if (choice == msgRestart.Commands[1])
            {
                Frame.GoBack();
            }
        }
        #endregion
    }
}

