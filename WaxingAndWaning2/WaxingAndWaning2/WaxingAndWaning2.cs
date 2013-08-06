using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace WaxingAndWaning2
{
    public class WaxingAndWaning2
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        static int fillX = 0;
        static int fillY = 0;


        const int SHOW_DIGITAL_SECOND = 10;

        const int WIDTH_HOUR = 15;
        const int WIDTH_MINUTE = 15;
        const int MARGIN_OUTER = 3;
        const int MARGIN_INNER = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

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

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        static void UpdateTime(object state)
        {

            currentTime = DateTime.Now;

            _display.Clear();


            if (showDigital == false)
            {

                if (currentTime.Hour - 12 < 0)
                {
                    degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                    degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                    degreeS = _azmdrawing.SecondToAngle(currentTime.Second);
                }
                else
                {
                    degreeH = (360 - _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute)) % 360;
                    degreeM = (360 - _azmdrawing.MinuteToAngle(currentTime.Minute)) % 360;
                    degreeS = (360 - _azmdrawing.SecondToAngle(currentTime.Second)) % 360;
                }


                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterX, (screenWidth / 2) - MARGIN_OUTER, (screenHeight / 2) - MARGIN_OUTER, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterX, (screenWidth / 2) - MARGIN_OUTER - WIDTH_HOUR, (screenHeight / 2) - MARGIN_OUTER - WIDTH_HOUR, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterX, (screenHeight / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER, (screenHeight / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterX, (screenHeight / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER - WIDTH_MINUTE, (screenHeight / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER - WIDTH_MINUTE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterX, (screenHeight / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER - WIDTH_MINUTE - MARGIN_INNER, (screenHeight / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER - WIDTH_MINUTE - MARGIN_INNER, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                fillX = screenCenterX - 1;

                if (currentTime.Hour % 12 != 0)
                {
                    _azmdrawing.DrawAngledLine(_display, colorBackground, 1, 0, screenCenterX, screenCenterY, (screenWidth / 2) - MARGIN_OUTER - WIDTH_HOUR - 1, WIDTH_HOUR + 1);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, 1, degreeH, screenCenterX, screenCenterY, (screenWidth / 2) - MARGIN_OUTER - WIDTH_HOUR - 1, WIDTH_HOUR + 1);
                    fillY = MARGIN_OUTER + 1;
                    _azmdrawing.FillArea(_display, colorBackground, fillX, fillY);
                }

                if (currentTime.Minute % 60 != 0)
                {
                    _azmdrawing.DrawAngledLine(_display, colorBackground, 1, 0, screenCenterX, screenCenterY,  (screenWidth / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER - WIDTH_MINUTE - 1, WIDTH_MINUTE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, 1, degreeM, screenCenterX, screenCenterY, (screenWidth / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER - WIDTH_MINUTE - 1, WIDTH_MINUTE + 1);
                    fillY = MARGIN_OUTER + WIDTH_HOUR + MARGIN_INNER + 1;
                    _azmdrawing.FillArea(_display, colorBackground, fillX, fillY);
                }

                if (currentTime.Second % 60 != 0)
                {
                    _azmdrawing.DrawAngledLine(_display, colorBackground, 1, 0, screenCenterX, screenCenterY, 0, (screenWidth / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER - WIDTH_MINUTE - MARGIN_INNER + 1);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, 1, degreeS, screenCenterX, screenCenterY, 0, (screenWidth / 2) - MARGIN_OUTER - WIDTH_HOUR - MARGIN_INNER - WIDTH_MINUTE - MARGIN_INNER + 1);
                    fillY = MARGIN_OUTER + WIDTH_HOUR + MARGIN_INNER + WIDTH_MINUTE + MARGIN_INNER + 1;
                    _azmdrawing.FillArea(_display, colorBackground, fillX, fillY);
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
                if (button == Buttons.MiddleRight)
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

            }

        }


    }

}

