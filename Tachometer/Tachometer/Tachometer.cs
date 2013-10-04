using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Tachometer
{
    public class Tachometer
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font fontSmall = Resources.GetFont(Resources.FontResources.small);
        static Font font7barPBd20 = Resources.GetFont(Resources.FontResources._7barPBd20);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        //static int distanceDial = 0;
        //static int distanceNumber = 0;

        static int displayMode = DISPLAY_MODE_BLACK_KMH;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        static bool showDisk = false;

        static string kmhMph = "";

        const int SHOW_DIGITAL_SECOND = 10;

        const int DEGREE_METER = 240;
        const int MIN_RED = 9;
        const int DIAL_UNIT = 4;

        const int LENGTH_HOUR_HAND = 38;
        const int LENGTH_HOUR_HAND_TAIL = 5;
        const int THICKNESS_HOUR_HAND = 5;


        const int MARGIN_DISK_EDGE = 3;
        const int MARGIN_RED_ZONE_DISK = 3;
        const int THICKNESS_RED_ZONE = 8;
        const int MARGIN_DIAL_RED_ZONE = 3;
        const int MARGIN_HOUR_NUMBER_DIAL = 7;

        const int MARGIN_RPM_EDGE_X = 0;
        const int MARGIN_RPM_EDGE_Y = 30;

        const int MARGIN_KMH_MPH_EDGE_X = 35;
        const int MARGIN_KMH_MPH_EDGE_Y = 20;

        const int MARGIN_MINUTE_EDGE_X = 14;
        const int MARGIN_MINUTE_EDGE_Y = 29;

        const int MARGINE_MINUTE_FRAME = 2;
        const int WIDTH_FRAME = 44;
        const int HEIGHT_FRAME = 26;
               

        const int LENGTH_DIAL_LONG = 5;
        const int THICKNESS_DIAL_LONG = 2;
        const int LENGTH_DIAL_SHORT = 1;
        const int THICKNESS_DIAL_SHORT = 2;

        const int RADIUS_OUTER_PIN = 2;
        const int RADIUS_INNER_PIN = 1;


        const int MAX_DISPLAY_MODE = 7;

        const int DISPLAY_MODE_BLACK_KMH = 0;
        const int DISPLAY_MODE_BLACK_MPH = 1;
        const int DISPLAY_MODE_BLACK_KMH_DISK = 2;
        const int DISPLAY_MODE_BLACK_MPH_DISK = 3;
        const int DISPLAY_MODE_WHITE_KMH = 4;
        const int DISPLAY_MODE_WHITE_MPH = 5;
        const int DISPLAY_MODE_WHITE_KMH_DISK = 6;
        const int DISPLAY_MODE_WHITE_MPH_DISK = 7;

        const string LABEL_RPM_01 = "x1000";
        const string LABEL_RPM_02 = "rpm";
        const string LABEL_KMH = "km/h";
        const string LABEL_MPH = "MPH";


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            colorForeground = new Color();
            colorBackground = new Color();


            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            //distanceDial = screenCenterX - (MARGIN_CIRCLE_EDGE + MARGIN_RED_ZONE_OUTER_CIRCLE + MARGIN_DIAL_INNER_CIRCLE);
            //distanceNumber = distanceDial-(LENGTH_DIAL_LONG + MARGIN_HOUR_NUMBER_DIAL);

            displayMode = DISPLAY_MODE_BLACK_KMH;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

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

            if (showDigital == false)
            {

                currentTime = DateTime.Now;

                degreeH = ((((DEGREE_METER / 12) * ((currentTime.Hour % 12))) + 180) + ((DEGREE_METER / 12) * currentTime.Minute / 60)) % 360;

                _display.Clear();

                if (showDisk != true)
                {
                    _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE, screenCenterY - MARGIN_DISK_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }
                else
                {
                    _display.DrawRectangle(colorForeground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE, screenCenterY - MARGIN_DISK_EDGE, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                }


                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK), screenCenterY - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK), colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK + THICKNESS_RED_ZONE), screenCenterY - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK + THICKNESS_RED_ZONE), colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _azmdrawing.DrawAngledLine(_display, colorBackground, 1, (((DEGREE_METER / 12) * MIN_RED) + 180) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK + THICKNESS_RED_ZONE), THICKNESS_RED_ZONE);
                _azmdrawing.DrawAngledLine(_display, colorBackground, 1, (((DEGREE_METER / 12) * 12) + 180) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK + THICKNESS_RED_ZONE), THICKNESS_RED_ZONE);

                _point = _azmdrawing.FindPointDegreeDistance((((DEGREE_METER / 12) * 12) + 180) % 360 + 2, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK) - 2);
                //_display.SetPixel(_point.X, _point.Y, colorBackground);
                _azmdrawing.FillArea(_display, colorBackground, _point.X, _point.Y);


                for (int i = 0; i < (MIN_RED * DIAL_UNIT); i++)
                {

                    if (i % DIAL_UNIT == 0)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 2, (((DEGREE_METER / 12 / DIAL_UNIT) * i) + 180) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK + THICKNESS_RED_ZONE), THICKNESS_RED_ZONE - 1);
                    }
                    else
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 2, (((DEGREE_METER / 12 / DIAL_UNIT) * i) + 180) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK) - 2, 1);
                    }

                }

                for (int i = (MIN_RED * DIAL_UNIT) + 1; i < (12 * DIAL_UNIT); i++)
                {

                    if (i % DIAL_UNIT == 0)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorBackground, 2, (((DEGREE_METER / 12 / DIAL_UNIT) * i) + 180) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK + THICKNESS_RED_ZONE), THICKNESS_RED_ZONE);
                    }
                    else
                    {
                        _azmdrawing.DrawAngledLine(_display, colorBackground, 2, (((DEGREE_METER / 12 / DIAL_UNIT) * i) + 180) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK) - 2, 1);
                    }


                }

                for (int i = 0; i <= 12; i++)
                {
                    _azmdrawing.DrawStringByDegreeDistance(_display, colorForeground, fontSmall, i.ToString(), (((DEGREE_METER / 12) * i) + 180) % 360,screenCenterX - (MARGIN_DISK_EDGE + MARGIN_RED_ZONE_DISK + THICKNESS_RED_ZONE + MARGIN_HOUR_NUMBER_DIAL));

                }

                _azmdrawing.DrawStringAligned(_display, colorForeground, fontSmall, LABEL_RPM_01, AZMDrawing.ALIGN_CENTER, MARGIN_RPM_EDGE_X, AZMDrawing.VALIGN_TOP, MARGIN_RPM_EDGE_Y);
                _azmdrawing.DrawStringAligned(_display, colorForeground, fontSmall, LABEL_RPM_02, AZMDrawing.ALIGN_CENTER, MARGIN_RPM_EDGE_X, AZMDrawing.VALIGN_TOP, MARGIN_RPM_EDGE_Y + (fontSmall.Height / 2) + 1);

                _azmdrawing.DrawStringAligned(_display, colorForeground, fontSmall, kmhMph, AZMDrawing.ALIGN_RIGHT, MARGIN_KMH_MPH_EDGE_X, AZMDrawing.VALIGN_BOTTOM, MARGIN_KMH_MPH_EDGE_Y);

                _point = _azmdrawing.FindPointDegreeDistance((degreeH + 180) % 360, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 1);

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_OUTER_PIN, RADIUS_OUTER_PIN, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_INNER_PIN, RADIUS_INNER_PIN, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.DrawRectangle(colorForeground, 1, screenWidth - MARGIN_MINUTE_EDGE_X - WIDTH_FRAME, screenHeight - MARGIN_MINUTE_EDGE_Y - HEIGHT_FRAME - MARGINE_MINUTE_FRAME, WIDTH_FRAME, HEIGHT_FRAME, 3, 3, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                _azmdrawing.FillArea(_display, colorForeground, screenWidth - MARGIN_MINUTE_EDGE_X - WIDTH_FRAME + 2, screenHeight - MARGIN_MINUTE_EDGE_Y - HEIGHT_FRAME - MARGINE_MINUTE_FRAME + 2);

                _azmdrawing.DrawStringAligned(_display, colorBackground, font7barPBd20, "0" + currentTime.Minute.ToString("D2"), AZMDrawing.ALIGN_RIGHT, MARGIN_MINUTE_EDGE_X, AZMDrawing.VALIGN_BOTTOM, MARGIN_MINUTE_EDGE_Y);

                _display.Flush();

            }

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
                        SetDisplayMode(displayMode);
                    }
                    else
                    {
                        showDigital = false;
                    }

                    UpdateTime(null);

                }
                else if (button == Buttons.MiddleRight)
                {
                    if (showDigital != true)
                    {
                        showDigital = true;
                        showDigitalCounter = 0;
                        UpdateTimeDigital(null);
                        _updateClockTimerDigital = new Timer(UpdateTimeDigital, null, dueTimeDigital, periodDigital);
                    }
                    else
                    {
                        showDigital = false;
                        UpdateTime(null);
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
                        SetDisplayMode(displayMode);
                    }
                    else
                    {
                        showDigital = false;
                    }

                    UpdateTime(null);

                }
            }

        }

        private static void SetDisplayMode(int displayMode)
        {

            switch (displayMode)
            {

                case DISPLAY_MODE_BLACK_KMH:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    kmhMph = LABEL_KMH;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_BLACK_MPH:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    kmhMph = LABEL_MPH;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_BLACK_KMH_DISK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    kmhMph = LABEL_KMH;
                    showDisk = true;

                    break;

                case DISPLAY_MODE_BLACK_MPH_DISK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    kmhMph = LABEL_MPH;
                    showDisk = true;

                    break;

                case DISPLAY_MODE_WHITE_KMH:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    kmhMph = LABEL_KMH;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_WHITE_MPH:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    kmhMph = LABEL_MPH;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_WHITE_KMH_DISK:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    kmhMph = LABEL_KMH;
                    showDisk = true;

                    break;

                case DISPLAY_MODE_WHITE_MPH_DISK:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    kmhMph = LABEL_MPH;
                    showDisk = true;

                    break;

            }

        }

        private static void UpdateTimeDigital(object state)
        {

            currentTime = DateTime.Now;


            if (showDigital == false || showDigitalCounter > SHOW_DIGITAL_SECOND)
            {
                showDigital = false;
                UpdateTime(null);
                _updateClockTimerDigital.Dispose();
            }
            else
            {
                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                _display.Flush();
                showDigitalCounter++;
            }

        }

    }

}

