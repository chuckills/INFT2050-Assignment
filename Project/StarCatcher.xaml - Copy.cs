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
using Windows.UI.Xaml.Media.Imaging;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Phone.UI.Input;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StarCatcher : Page
    {
        private Random rand;
        private List<Image> lstStar;
        private List<animation> lstMovement;
        private int numSpawn;
        private Rect bounds;
        private double iPosition;
        private int iScore;
        private Accelerometer myAccelerometer;
        private double roll;
        private DispatcherTimer dptTimer;
        private int iLose;
        private bool bStarId;

        #region Main Page
        public StarCatcher()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            rand = new Random();
            numSpawn = 1;
            iPosition = 150;
            iScore = 0;
            bStarId = true;
            iLose = 0;
            bounds = Window.Current.Bounds;
            lstStar = new List<Image>();
            lstMovement = new List<animation>();

            // createStar();
            myAccelerometer = Accelerometer.GetDefault();
            roll = 0.0;
            dptTimer = new DispatcherTimer();
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



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        #endregion
        #region On Navitaged To
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            CompositionTarget.Rendering += updateStars;
            // CompositionTarget.Rendering += this.createStar;
            dptTimer.Tick += DptTimer_Tick;
            dptTimer.Interval = TimeSpan.FromMilliseconds(1000);
            dptTimer.Start();
            tbkScore.Text = string.Format("Score: {0,4:###0}", iScore);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            CompositionTarget.Rendering -= updateStars;
            dptTimer.Stop();
            dptTimer.Tick -= DptTimer_Tick;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();
        }

        private void DptTimer_Tick(object sender, object e)
        {
            if (bStarId)
            {
                createStar();
            }
        }
        #endregion
        #region Create Star
        public void createStar()
        {

            //numSpawn = rand.Next(3, 10);

            for (int i = 0; i < numSpawn; i++)
            {
                int xPos = rand.Next((int)bounds.Width);
                int yPos = 20;
                int xSpeed = 2 + rand.Next(2);
                int ySpeed = 2 + rand.Next(2);
                animation movementSpeed = new animation(xPos, yPos, xSpeed, ySpeed);
                lstMovement.Add(movementSpeed);


                Image imStar = new Image();
                imStar.Source = new BitmapImage(new Uri("ms-appx:///Assets/Star.png"));
                imStar.SetValue(Canvas.LeftProperty, xPos);
                imStar.SetValue(Canvas.TopProperty, yPos);
                canGameBoard.Children.Add(imStar);
                lstStar.Add(imStar);
            }

        }


        #endregion

        #region update Stars
        private void updateStars(object sender, object e)
        {

            for (int i = 0; i < lstStar.Count; i++)
            {
                //lstMovement[i].UpdatePosition((int)bounds.Width, (int)bounds.Height);

                Canvas.SetTop(lstStar[i], lstMovement[i].Ypos);
                lstMovement[i].Ypos += 3;



                if (lstMovement[i].Ypos >= 466 && lstMovement[i].Ypos <= 669 && lstMovement[i].Xpos >= iPosition && lstMovement[i].Xpos <= (iPosition + 100))
                {
                    canGameBoard.Children.Remove(lstStar[i]);
                    lstMovement.RemoveAt(i);
                    lstStar.RemoveAt(i);
                    iScore += 1;
                    tbkScore.Text = string.Format("Score: {0,4:###0}", iScore);
                }

                if (lstMovement[i].Ypos > bounds.Height-40)
                {
                    canGameBoard.Children.Remove(lstStar[i]);
                    lstMovement.RemoveAt(i);
                    lstStar.RemoveAt(i);
                    iLose += 1;
                    if (iLose >= 5)
                    {
                        tbxLose.Visibility = Visibility.Visible;
                        tbxLose.Text = "You Lose! \n" + "Your score was " + iScore + ".";
                        bStarId = false;
                        while (i < lstStar.Count)
                        {
                            canGameBoard.Children.Remove(lstStar[i]);
                            //lstMovement.RemoveAt(i);
                            //lstStar.RemoveAt(i);
                            i++;
                        }
                        lstStar.Clear();
                        lstMovement.Clear();

                        btnReset.Visibility = Visibility.Visible;

                    }


                }

            }
        }

        private void moveCatcher()
        {

            Canvas.SetLeft(catcher, (iPosition += 0.1 * roll));

            // Traverse the left border
            if (Canvas.GetLeft(catcher) < -100)
            {
                Canvas.SetLeft(catcher, bounds.Width + 100);
                iPosition = bounds.Width + 100;
            }

            // Traverse the right border
            if (Canvas.GetLeft(catcher) > bounds.Width + 100)
            {
                Canvas.SetLeft(catcher, -100);
                iPosition = -100;
            }


        }
        #endregion



        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            iScore = 0;
            tbxLose.Visibility = Visibility.Collapsed;
            bStarId = true;
            btnReset.Visibility = Visibility.Collapsed;
            iLose = 0;
            tbkScore.Text = string.Format("Score: {0,4:###0}", iScore);

            Frame.Navigate(typeof(StarCatcher));
            Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
        }

        private async void MyAccelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                roll = args.Reading.AccelerationX * 100;

                moveCatcher();

            });
        }
    }

    #region  Animation Class
    public class animation
    {
        private int xpos;
        private int ypos;
        private int xspeed;
        private int yspeed;

        #region Int properties
        public int Xpos
        {
            get { return xpos; }
            set { if (value >= 0) xpos = value; }
        }

        public int Ypos
        {
            get { return ypos; }
            set { if (value >= 0) ypos = value; }
        }

        public int Xspeed
        {
            get { return xspeed; }
            set { xspeed = value; }
        }

        public int Yspeed
        {
            get { return yspeed; }
            set { yspeed = value; }
        }
        #endregion

        public animation(int xStart, int yStart, int xSpeed, int ySpeed)
        {
            SetPosition(xStart, yStart);
            SetSpeed(xSpeed, Yspeed);
        }

        public void SetPosition(int xStart, int yStart)
        {
            Xpos = xStart;
            Ypos = yStart;
        }

        public void SetSpeed(int xSpeed, int ySpeed)
        {
            Xspeed = xSpeed;
            Yspeed = ySpeed;
        }

        public double Distance(double xOther, double yOther)
        {
            double distance = (xpos - xOther) * (xpos - xOther);
            distance += (ypos - yOther) * (ypos - yOther);
            return Math.Sqrt(distance);
        }

        public void UpdatePosition(int width, int height)
        {
            xpos += xspeed;
            if (xspeed > 0 && xpos > width)
            {
                //   xspeed *= -1;
            }
            else if (xspeed < 0 && xpos < 0)
            {
                //   xspeed *= -1;
            }

            ypos += yspeed;
            if (yspeed > 0 && ypos < height)
            {
                yspeed += 1;
            }
            else if (yspeed < 0 && ypos < 0)
            {
                //   yspeed *= -1;
            }
        }
    }
    #endregion
}
