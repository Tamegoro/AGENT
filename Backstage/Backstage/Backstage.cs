using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Backstage
{
    public class Backstage
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontsmalNumberBold = Resources.GetFont(Resources.FontResources.smalNumberBold);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;
        static AGENT.AZMutil.Point _corner0;
        static AGENT.AZMutil.Point _corner1;
        static AGENT.AZMutil.Point _corner2;
        static AGENT.AZMutil.Point _corner3;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int degreeNum = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_EDGE_DISK = 1;
        const int MARGIN_CENTER_WINDOW = 3;

        const int RADIUS_OUTER_DISK = 36;
        const int RADIUS_INNER_DISK = 24;
        const int RADIUS_CENTER_HOLE = 12;

        const int WINDOW_WIDTH = 21;
        const int WINDOW_HEIGHT = 12;
        const int WINDOW_THICKNESS = 3;

        const int OFFSET_NUMBER = 0;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_TRANSPARENT = 1;
        const int DISPLAY_MODE_WHITE = 2;
        const int DISPLAY_MODE_WHITE_TRANSPARENT = 3;
        

        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();
            _corner0 = new AGENT.AZMutil.Point();
            _corner1 = new AGENT.AZMutil.Point();
            _corner2 = new AGENT.AZMutil.Point();
            _corner3 = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK;
            colorForeground = Color.Black;
            colorBackground = Color.White;

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

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

            _display.Clear();


            if (showDigital == false)
            {

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                if (displayMode == DISPLAY_MODE_BLACK || displayMode == DISPLAY_MODE_WHITE)
                {
                    _display.DrawRectangle(colorForeground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                    _point = _azmdrawing.FindPointDegreeDistance(315, screenCenterX, screenCenterY, MARGIN_CENTER_WINDOW + WINDOW_WIDTH);
                    _corner0 = _azmdrawing.FindPointDegreeDistance(315 - 90, _point.X, _point.Y, (WINDOW_HEIGHT / 2) + 1);
                    _corner1 = _azmdrawing.FindPointDegreeDistance(315 + 90, _point.X, _point.Y, (WINDOW_HEIGHT / 2) - 1);
                    _point = _azmdrawing.FindPointDegreeDistance(315, screenCenterX, screenCenterY, MARGIN_CENTER_WINDOW + 1);
                    _corner2 = _azmdrawing.FindPointDegreeDistance(315 - 90, _point.X, _point.Y, (WINDOW_HEIGHT / 2) + 1);
                    _corner3 = _azmdrawing.FindPointDegreeDistance(315 + 90, _point.X, _point.Y, (WINDOW_HEIGHT / 2) - 1);

                    _display.DrawLine(colorBackground, 1, _corner0.X, _corner0.Y, _corner2.X, _corner2.Y);
                    _display.DrawLine(colorBackground, 1, _corner2.X, _corner2.Y, _corner3.X, _corner3.Y);
                    _display.DrawLine(colorBackground, 1, _corner3.X, _corner3.Y, _corner1.X, _corner1.Y);
                    _display.DrawLine(colorBackground, 1, _corner1.X, _corner1.Y, _corner0.X, _corner0.Y);

                    _point = _azmdrawing.FindPointDegreeDistance(315, screenCenterX, screenCenterY, MARGIN_CENTER_WINDOW + (WINDOW_WIDTH / 2));
                    _azmdrawing.FillArea(_display, colorBackground, _point.X, _point.Y);

                    _point = _azmdrawing.FindPointDegreeDistance(135, screenCenterX, screenCenterY, MARGIN_CENTER_WINDOW + WINDOW_WIDTH);
                    _corner0 = _azmdrawing.FindPointDegreeDistance(135 - 90, _point.X, _point.Y, (WINDOW_HEIGHT / 2) - 1);
                    _corner1 = _azmdrawing.FindPointDegreeDistance(135 + 90, _point.X, _point.Y, (WINDOW_HEIGHT / 2) + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(135, screenCenterX, screenCenterY, MARGIN_CENTER_WINDOW + 1);
                    _corner2 = _azmdrawing.FindPointDegreeDistance(135 - 90, _point.X, _point.Y, (WINDOW_HEIGHT / 2) - 1);
                    _corner3 = _azmdrawing.FindPointDegreeDistance(135 + 90, _point.X, _point.Y, (WINDOW_HEIGHT / 2) + 1);

                    _display.DrawLine(colorBackground, 1, _corner0.X, _corner0.Y, _corner2.X, _corner2.Y);
                    _display.DrawLine(colorBackground, 1, _corner2.X, _corner2.Y, _corner3.X, _corner3.Y);
                    _display.DrawLine(colorBackground, 1, _corner3.X, _corner3.Y, _corner1.X, _corner1.Y);
                    _display.DrawLine(colorBackground, 1, _corner1.X, _corner1.Y, _corner0.X, _corner0.Y);

                    _point = _azmdrawing.FindPointDegreeDistance(135, screenCenterX, screenCenterY, MARGIN_CENTER_WINDOW + (WINDOW_WIDTH / 2));
                    _azmdrawing.FillArea(_display, colorBackground, _point.X, _point.Y);

                    _azmdrawing.DrawAngledLine(_display, colorBackground, 2, 45, screenCenterX, screenCenterY + 1, 3, 1);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, 2, 225, screenCenterX, screenCenterY + 1, 3, 1); 

                }
                else if (displayMode == DISPLAY_MODE_BLACK_TRANSPARENT || displayMode == DISPLAY_MODE_WHITE_TRANSPARENT)
                {
                    _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                }


                _display.DrawEllipse(colorForeground, 1, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, RADIUS_OUTER_DISK, RADIUS_OUTER_DISK, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                _display.DrawEllipse(colorForeground, 1, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, RADIUS_INNER_DISK, RADIUS_INNER_DISK, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                _display.DrawEllipse(colorForeground, 1, screenWidth - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), screenHeight - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), RADIUS_OUTER_DISK, RADIUS_OUTER_DISK, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                _display.DrawEllipse(colorForeground, 1, screenWidth - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), screenHeight - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), RADIUS_INNER_DISK, RADIUS_INNER_DISK, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                if (displayMode == DISPLAY_MODE_BLACK_TRANSPARENT || displayMode == DISPLAY_MODE_WHITE_TRANSPARENT)
                {
                    _display.DrawEllipse(colorForeground, 1, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, RADIUS_CENTER_HOLE, RADIUS_CENTER_HOLE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, screenWidth - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), screenHeight - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), RADIUS_CENTER_HOLE, RADIUS_CENTER_HOLE, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                    _azmdrawing.FillArea(_display, colorForeground, 0, 0);
                    _azmdrawing.FillArea(_display, colorForeground, screenWidth - 1, screenHeight - 1);
                    _display.DrawRectangle(colorForeground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                }
                    


                for (int i = 0; i < 3; i++)
                {

                    //degreeNum = 270 + 45 + (60 * i) - (60 * (currentTime.Minute % 10) / 10);
                    degreeNum = 90 + 45 + (120 * i);
                    _point = _azmdrawing.FindPointDegreeDistance(degreeNum % 360, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, RADIUS_INNER_DISK - (fontsmalNumberBold.Height / 2) - OFFSET_NUMBER);
                    _azmdrawing.DrawStringAngled(_display, colorForeground, fontsmalNumberBold, (degreeNum - 90) % 360, _point.X, _point.Y, ((((currentTime.Hour / 10 + i) % 10) + 3) % 3).ToString());

                }

                for (int i = 0; i < 10; i++)
                {

                    degreeNum = 90 + 45 + (36 * i);
                    _point = _azmdrawing.FindPointDegreeDistance(degreeNum % 360, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, MARGIN_EDGE_DISK + RADIUS_OUTER_DISK, RADIUS_INNER_DISK + (RADIUS_OUTER_DISK - RADIUS_INNER_DISK) - (fontsmalNumberBold.Height / 2) - OFFSET_NUMBER);
                    _azmdrawing.DrawStringAngled(_display, colorForeground, fontsmalNumberBold, (degreeNum - 90) % 360, _point.X, _point.Y, ((currentTime.Hour % 10 + i) % 10).ToString());

                }


                for (int i = 0; i < 6; i++)
                {

                    //degreeNum = 270 + 45 + (60 * i) - (60 * (currentTime.Minute % 10) / 10);
                    degreeNum = 270 + 45 + (60 * i);
                    _point = _azmdrawing.FindPointDegreeDistance(degreeNum % 360, screenWidth - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), screenHeight - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), RADIUS_INNER_DISK + (RADIUS_OUTER_DISK - RADIUS_INNER_DISK) - (fontsmalNumberBold.Height / 2) - OFFSET_NUMBER - 1);
                    _azmdrawing.DrawStringAngled(_display, colorForeground, fontsmalNumberBold, (degreeNum + 90) % 360, _point.X, _point.Y, ((((currentTime.Minute / 10 + i) % 10) + 6) % 6).ToString());

                }

                for (int i = 0; i < 10; i++)
                {

                    degreeNum = 270 + 45 + (36 * i);
                    _point = _azmdrawing.FindPointDegreeDistance(degreeNum % 360, screenWidth - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), screenHeight - (MARGIN_EDGE_DISK + RADIUS_OUTER_DISK), RADIUS_INNER_DISK - (fontsmalNumberBold.Height / 2) - OFFSET_NUMBER);
                    _azmdrawing.DrawStringAngled(_display, colorForeground, fontsmalNumberBold, (degreeNum + 90) % 360, _point.X, _point.Y, ((currentTime.Minute % 10 + i) % 10).ToString());

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
                else if (button == Buttons.MiddleRight)
                {
                    if (showDigital != true)
                    {
                        showDigital = true;
                        showDigitalCounter = 0;
                        UpdateTime(null);
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

                switch (displayMode)
                {

                    case DISPLAY_MODE_BLACK:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_TRANSPARENT:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_TRANSPARENT:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;


                }

            }

        }


    }

}

