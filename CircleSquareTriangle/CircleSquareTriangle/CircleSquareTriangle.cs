using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace CircleSquareTriangle
{
    public class CircleSquareTriangle
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontIPAexGothic08Bold = Resources.GetFont(Resources.FontResources.IPAexGothic08Bold);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

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

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int ringX = 0;
        static int ringY = 0;
        static int ringRadius = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_CIRCLE_EDGE = 3;
        const int MARGIN_SQUARE_CIRCLE = 2;
        const int MARGIN_TRIANGLE_SQUARE = 2;
        const int MARGIN_DOT = 8;
        const int DISTANCE_TRIANGLE = 18;
        const int THICKNESS_CIRCLE = 13;
        const int THICKNESS_SQUARE = 16;
        const int RADIUS_DOT = 2;

        const int MAX_DISPLAY_MODE = 1;

        const int DISPLAY_MODE_WHITE = 0;
        const int DISPLAY_MODE_BLACK = 1;


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

            displayMode = DISPLAY_MODE_WHITE;
            colorForeground = Color.White;
            colorBackground = Color.Black;

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

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                ringRadius = screenCenterX - MARGIN_CIRCLE_EDGE;
                ringX = screenCenterX;
                ringY = screenCenterY;

                _display.DrawEllipse(colorForeground, 1, ringX, ringY, ringRadius, ringRadius, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, ringX, ringY, ringRadius - THICKNESS_CIRCLE, ringRadius - THICKNESS_CIRCLE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                for (int i = 1; i <= 12; i++)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(((30 * i) - degreeH + 360) % 360, ringX, ringY, ringRadius - (THICKNESS_CIRCLE / 2));
                    _azmdrawing.DrawStringAngled(_display, colorBackground, fontIPAexGothic08Bold, ((30 * i) - degreeH + 360) % 360, _point.X, _point.Y, i.ToString());
                }

                _corner0 = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45)) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE));
                _corner1 = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45) + 90) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE));
                _corner2 = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45) + 180) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE));
                _corner3 = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45) + 270) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE));

                _display.DrawLine(colorForeground, 1, _corner0.X, _corner0.Y, _corner1.X, _corner1.Y);
                _display.DrawLine(colorForeground, 1, _corner1.X, _corner1.Y, _corner2.X, _corner2.Y);
                _display.DrawLine(colorForeground, 1, _corner2.X, _corner2.Y, _corner3.X, _corner3.Y);
                _display.DrawLine(colorForeground, 1, _corner3.X, _corner3.Y, _corner0.X, _corner0.Y);

                _corner0 = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45)) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE + THICKNESS_SQUARE));
                _corner1 = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45) + 90) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE + THICKNESS_SQUARE));
                _corner2 = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45) + 180) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE + THICKNESS_SQUARE));
                _corner3 = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45) + 270) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE + THICKNESS_SQUARE));

                _display.DrawLine(colorForeground, 1, _corner0.X, _corner0.Y, _corner1.X, _corner1.Y);
                _display.DrawLine(colorForeground, 1, _corner1.X, _corner1.Y, _corner2.X, _corner2.Y);
                _display.DrawLine(colorForeground, 1, _corner2.X, _corner2.Y, _corner3.X, _corner3.Y);
                _display.DrawLine(colorForeground, 1, _corner3.X, _corner3.Y, _corner0.X, _corner0.Y);

                _point = _azmdrawing.FindPointDegreeDistance((360 - (degreeM - 45)) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_SQUARE_CIRCLE) - 3);
                _azmdrawing.FillArea(_display, colorForeground, _point.X, _point.Y);

                for (int i = 0; i < 12; i++)
                {

                    if (i % 3 == 0)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance((360 - ((30 * i) + degreeM)) % 360, screenCenterX, screenCenterY, DISTANCE_TRIANGLE + MARGIN_TRIANGLE_SQUARE + (THICKNESS_SQUARE / 2) - 1);
                        if (i == 0)
                        {
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontIPAexGothic08Bold, (360 - ((30 * i) + degreeM)) % 360, _point.X, _point.Y, "00");
                        }
                        else
                        {
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontIPAexGothic08Bold, (360 - ((30 * i) + degreeM)) % 360, _point.X, _point.Y, (60 - (5 * i)).ToString("D2"));
                        }
                    }
                    else
                    {
                        _point = _azmdrawing.FindPointDegreeDistance((360 - ((30 * i) + degreeM)) % 360, screenCenterX, screenCenterY, DISTANCE_TRIANGLE + MARGIN_TRIANGLE_SQUARE + (THICKNESS_SQUARE / 2) + 4);
                        //_display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, 1, 1, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        _azmdrawing.DrawAngledLine(_display, colorBackground, 2, 0, _point.X, _point.Y, 0, 1);
                    }
                
                }

                _corner0 = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, DISTANCE_TRIANGLE);
                _corner1 = _azmdrawing.FindPointDegreeDistance((degreeS + 360 + 120) % 360, screenCenterX, screenCenterY, DISTANCE_TRIANGLE);
                _corner2 = _azmdrawing.FindPointDegreeDistance((degreeS + 360 + 240) % 360, screenCenterX, screenCenterY, DISTANCE_TRIANGLE);

                _display.DrawLine(colorForeground, 1, _corner0.X, _corner0.Y, _corner1.X, _corner1.Y);
                _display.DrawLine(colorForeground, 1, _corner1.X, _corner1.Y, _corner2.X, _corner2.Y);
                _display.DrawLine(colorForeground, 1, _corner2.X, _corner2.Y, _corner0.X, _corner0.Y);

                _azmdrawing.FillArea(_display, colorForeground, screenCenterX, screenCenterY);

                _point = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, DISTANCE_TRIANGLE - MARGIN_DOT);
                _display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, RADIUS_DOT, RADIUS_DOT, colorBackground, 0, 0, colorBackground, 0, 0, 255);

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

                    case DISPLAY_MODE_WHITE:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                }

            }

        }


    }

}

