using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Football
{
    public class Football
    {

        static Bitmap _display;

        static Bitmap _background;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;


        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorHand;
        static Color colorPin;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int backgroundWidth = 0;
        static int backgroundHeight = 0;

        static int displayMode = DISPLAY_MODE_WHITE_FRAHAND;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 40;
        const int LENGTH_HOUR_HAND_TAIL = 15;
        const int THICKNESS_HOUR_HAND = 6;

        const int LENGTH_MINUTE_HAND = 60;
        const int LENGTH_MINUTE_HAND_TAIL = 15;
        const int THICKNESS_MINUTE_HAND = 6;

        const int RADIUS_PIN = 1;

        const int MAX_DISPLAY_MODE = 1;

        const int HAND_TYPE_FRA = 5;

        const int DISPLAY_MODE_WHITE_FRAHAND = 0;
        const int DISPLAY_MODE_BLACK_FRAHAND = 1;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _background = new Bitmap(Resources.GetBytes(Resources.BinaryResources.football), Bitmap.BitmapImageType.Gif);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            backgroundWidth = _background.Width;
            backgroundHeight = _background.Height;

            displayMode = DISPLAY_MODE_BLACK_FRAHAND;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

            //dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            //period = new TimeSpan(0, 0, 0, 1, 0);

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period);

            ButtonHelper.ButtonSetup = new Buttons[]
            {
                Buttons.BottomRight, Buttons.MiddleRight, Buttons.TopRight
            };

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        static void UpdateTime(object state)
        {

            currentTime = DateTime.Now;

            if (showDigital == false)
            {

                _display.Clear();

                _display.DrawImage(0, 0, _background, 0, 0, backgroundWidth, backgroundHeight);

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                _point = _azmdrawing.FindPointDegreeDistance((degreeM + 180) % 360, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorHand, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, 6);

                _point = _azmdrawing.FindPointDegreeDistance((degreeH + 180) % 360, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorHand, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 6);

                _display.DrawEllipse(colorPin, 1, screenCenterX, screenCenterY, RADIUS_PIN, RADIUS_PIN, colorPin, 0, 0, colorPin, 0, 0, 255);

            }
            else
            {

                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                ++showDigitalCounter;

                if (showDigitalCounter > SHOW_DIGITAL_SECOND)
                {
                    showDigital = false;
                    showDigitalCounter = 0;
                }

            }

            _display.Flush();

        }


        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.TopRight)
                {
                    if (showDigital == false)
                    {
                        if (showDigital == false)
                        {
                            --displayMode;
                            if (displayMode < 0)
                            {
                                displayMode = MAX_DISPLAY_MODE;
                            }
                        }
                        else
                        {
                            showDigital = false;
                        }
                    }
                    else
                    {
                        showDigital = false;
                    }
                }
                else if (button == Buttons.MiddleRight)
                {
                    if (showDigital != true)
                    {
                        showDigital = true;
                        showDigitalCounter = 0;
                    }
                    else
                    {
                        showDigital = false;
                    }
                }
                else if (button == Buttons.BottomRight)
                {
                    if (showDigital == false)
                    {
                        ++displayMode;
                        if (displayMode > MAX_DISPLAY_MODE)
                        {
                            displayMode = 0;
                        }
                    }
                    else
                    {
                        showDigital = false;
                    }
                }

                SetDisplayMode(displayMode);

                UpdateTime(null);

            }

        }

        private static void SetDisplayMode(int displayMode)
        {

            switch (displayMode)
            {

                case DISPLAY_MODE_WHITE_FRAHAND:

                    colorHand = Color.White;
                    colorPin = Color.White;

                    break;

                case DISPLAY_MODE_BLACK_FRAHAND:

                    colorHand = Color.Black;
                    colorPin = Color.Black;

                    break;

            }

        }

    }

}

