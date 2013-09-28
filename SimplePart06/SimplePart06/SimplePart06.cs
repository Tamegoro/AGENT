using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace SimplePart06
{
    public class SimplePart06
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;


        static DateTime currentTime;

        static Font fontSwis721BTNumbers20 = Resources.GetFont(Resources.FontResources.Swis721BTNumbers20);
        static Font fontSwis721BdOulBTNumbers20 = Resources.GetFont(Resources.FontResources.Swis721BdOulBTNumbers20);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;
        static Color colorDisk;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK_FRAHAND;

        static bool showDisk = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 40;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 5;

        const int LENGTH_MINUTE_HAND = 50;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 5;

        const int LENGTH_SECOND_HAND = 49;
        const int LENGTH_SECOND_HAND_TAIL = 9;
        const int THICKNESS_SECOND_HAND = 1;

        const int RADIUS_PIN_OUTER = 2;
        const int RADIUS_PIN_INNER = 1;

        const int MARGIN_DISK_EDGE = 3;
        const int THICKNESS_CIRCLE = 2;
        const int MARGIN_NUMBER_EDGE = 20;
        const int MOD_2CHAR_NUMBER = 7;
        

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK_FRAHAND = 0;
        const int DISPLAY_MODE_BLACK_FRAHAND_DISK = 1;
        const int DISPLAY_MODE_WHITE_FRAHAND = 2;
        const int DISPLAY_MODE_WHITE_FRAHAND_DISK = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_FRAHAND;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

            //dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            //period = new TimeSpan(0, 0, 1, 0, 0);

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

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (showDisk == true)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE, screenCenterY - MARGIN_DISK_EDGE, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                }
                else
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE, screenCenterY - MARGIN_DISK_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }

                if (showDisk == true)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE - 1, screenCenterY - MARGIN_DISK_EDGE - 1, colorDisk, 0, 0, colorDisk, 0, 0, 0);
                }
                else
                {
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE - 1, screenCenterY - MARGIN_DISK_EDGE - 1, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                }

                for (int i = 1; i <= 12; i++)
                {

                    if (i < 10)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE);
                    }
                    else
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MOD_2CHAR_NUMBER);
                    }

                    if (i % 2 == 0)
                    {
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontSwis721BdOulBTNumbers20, _point.X, _point.Y, i.ToString());
                    }
                    else
                    {
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontSwis721BTNumbers20, _point.X, _point.Y, i.ToString());
                    }

                }

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _point = _azmdrawing.FindPointDegreeDistance((degreeH + 180) % 360, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 6);

                _point = _azmdrawing.FindPointDegreeDistance((degreeM + 180) % 360, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, 6);

                _point = _azmdrawing.FindPointDegreeDistance((degreeS + 180) % 360, screenCenterX, screenCenterY, LENGTH_SECOND_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, _point.X, _point.Y, 0, LENGTH_SECOND_HAND + LENGTH_SECOND_HAND_TAIL);

                if (showDisk == true)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, RADIUS_PIN_OUTER, RADIUS_PIN_OUTER, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN_INNER, RADIUS_PIN_INNER, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    //_display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN_OUTER, RADIUS_PIN_OUTER, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    //_display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, RADIUS_PIN_INNER, RADIUS_PIN_INNER, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                }
                else
                {
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_PIN_OUTER, RADIUS_PIN_OUTER, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN_INNER, RADIUS_PIN_INNER, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    //_display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN_OUTER, RADIUS_PIN_OUTER, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    //_display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_PIN_INNER, RADIUS_PIN_INNER, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                }



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

                case DISPLAY_MODE_BLACK_FRAHAND:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    colorDisk = Color.Black;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_BLACK_FRAHAND_DISK:

                    colorForeground = Color.White;
                    colorBackground = Color.White;
                    colorDisk = Color.Black;
                    showDisk = true;

                    break;

                case DISPLAY_MODE_WHITE_FRAHAND:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    colorDisk = Color.White;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_WHITE_FRAHAND_DISK:

                    colorForeground = Color.Black;
                    colorBackground = Color.Black;
                    colorDisk = Color.White;
                    showDisk = true;

                    break;

            }

        }

    }

}

