using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Triangle
{
    public class Triangle
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;


        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;
        static Color colorDisk;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static AGENT.AZMutil.Point _pointH;
        static AGENT.AZMutil.Point _pointM;
        static AGENT.AZMutil.Point _pointS;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDisk = false;
        static bool showDial = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int DISTANCE_HOUR = 24;
        const int DISTANCE_MINUTE = 39;
        const int DISTANCE_SECOND = 54;

        const int THICKNESS_LINE = 2;


        const int MARGIN_DISK_EDGE = 3;
        const int MARGIN_DIAL_DISK = 3;

        const int MAX_DISPLAY_MODE = 7;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_DIAL = 1;
        const int DISPLAY_MODE_BLACK_DISK = 2;
        const int DISPLAY_MODE_BLACK_DISK_DIAL = 3;
        const int DISPLAY_MODE_WHITE = 4;
        const int DISPLAY_MODE_WHITE_DIAL = 5;
        const int DISPLAY_MODE_WHITE_DISK = 6;
        const int DISPLAY_MODE_WHITE_DISK_DIAL = 7;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            _point = new AGENT.AZMutil.Point();

            _pointH = new AGENT.AZMutil.Point();
            _pointM = new AGENT.AZMutil.Point();
            _pointS = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

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

                if (showDial == true)
                {
                    for (int i = 1; i <= 12; i++)
                    {

                        if (1 <= i && i <= 8)
                        {
                            _azmdrawing.DrawAngledLine(_display, colorForeground, 2, 30 * i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_DIAL_DISK), 1);
                        }
                        else if (9 <= i && i <= 12)
                        {
                            _azmdrawing.DrawAngledLine(_display, colorForeground, 2, 30 * i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_DIAL_DISK) - 1, 1);
                        }

                    }
                }

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _pointH = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, DISTANCE_HOUR);
                _pointM = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, DISTANCE_MINUTE);
                _pointS = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, DISTANCE_SECOND);

                _display.DrawLine(colorForeground, 1, _pointH.X, _pointH.Y, _pointM.X, _pointM.Y);
                _display.DrawLine(colorForeground, 1, _pointH.X, _pointH.Y, _pointS.X, _pointS.Y);
                _display.DrawLine(colorForeground, 1, _pointM.X, _pointM.Y, _pointS.X, _pointS.Y);


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

                case DISPLAY_MODE_BLACK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    colorDisk = Color.Black;
                    showDial = false;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_BLACK_DISK:

                    colorForeground = Color.White;
                    colorBackground = Color.White;
                    colorDisk = Color.Black;
                    showDial = false;
                    showDisk = true;

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    colorDisk = Color.White;
                    showDial = false;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_WHITE_DISK:

                    colorForeground = Color.Black;
                    colorBackground = Color.Black;
                    colorDisk = Color.White;
                    showDial = false;
                    showDisk = true;

                    break;

                case DISPLAY_MODE_BLACK_DIAL:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    colorDisk = Color.Black;
                    showDial = true;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_BLACK_DISK_DIAL:

                    colorForeground = Color.White;
                    colorBackground = Color.White;
                    colorDisk = Color.Black;
                    showDial = true;
                    showDisk = true;

                    break;

                case DISPLAY_MODE_WHITE_DIAL:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    colorDisk = Color.White;
                    showDial = true;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_WHITE_DISK_DIAL:

                    colorForeground = Color.Black;
                    colorBackground = Color.Black;
                    colorDisk = Color.White;
                    showDial = true;
                    showDisk = true;

                    break;

            }

        }

    }

}

