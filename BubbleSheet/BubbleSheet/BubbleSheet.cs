using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace BubbleSheet
{
    public class BubbleSheet
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontNumbers07 = Resources.GetFont(Resources.FontResources.Numbers07);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int gapX = 0;
        static int gapY = 0;

        static int margineX = 0;
        static int margineY = 0;

        static int h1 = 0;
        static int h2 = 0;
        static int m1 = 0;
        static int m2 = 0;
        static int s1 = 0;
        static int s2 = 0;

        static int h12h24 = 0;
        static int holeType = 0;

        static int displayMode = DISPLAY_MODE_BLACK_CIRCLE_24;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int H12 = 0;
        const int H24 = 1;

        const int HOLE_CIRCLE = 0;
        const int HOLE_SQUARE = 1;

        const int SIZE_HOLE = 11;

        const int MAX_DISPLAY_MODE = 7;

        const int DISPLAY_MODE_BLACK_CIRCLE_12 = 0;
        const int DISPLAY_MODE_BLACK_CIRCLE_24 = 1;
        const int DISPLAY_MODE_BLACK_SQUARE_12 = 2;
        const int DISPLAY_MODE_BLACK_SQUARE_24 = 3;
        const int DISPLAY_MODE_WHITE_CIRCLE_12 = 4;
        const int DISPLAY_MODE_WHITE_CIRCLE_24 = 5;
        const int DISPLAY_MODE_WHITE_SQUARE_12 = 6;
        const int DISPLAY_MODE_WHITE_SQUARE_24 = 7;


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

            gapX = (screenWidth - (SIZE_HOLE * 6)) / 5;
            gapY = (screenHeight - (SIZE_HOLE * 10)) / 9;

            margineX = (screenWidth - ((SIZE_HOLE * 6) + (gapX * 5))) / 2;
            margineY = (screenHeight - ((SIZE_HOLE * 10) + (gapY * 9))) / 2;

            displayMode = DISPLAY_MODE_BLACK_CIRCLE_12;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

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

                h1 = (currentTime.Hour % h12h24) / 10;
                h2 = (currentTime.Hour % h12h24) % 10;
                m1 = currentTime.Minute / 10;
                m2 = currentTime.Minute % 10;
                s1 = currentTime.Second / 10;
                s2 = currentTime.Second % 10;

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {

                        switch (holeType)
                        {

                            case HOLE_CIRCLE:

                                _display.DrawEllipse(colorForeground, 1, margineX + ((SIZE_HOLE) * (i + 1)) + (gapX * i) - (SIZE_HOLE / 2) - 1, margineY + ((SIZE_HOLE) * (j + 1)) + (gapY * j) - (SIZE_HOLE / 2) - 1, SIZE_HOLE / 2, SIZE_HOLE / 2, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                                break;


                            case HOLE_SQUARE:

                                _display.DrawRectangle(colorForeground, 1, margineX + ((SIZE_HOLE) * i) + (gapX * (i - 1)) + SIZE_HOLE + 1, margineY + ((SIZE_HOLE) * j) + (gapY * (j - 1)) + 2, SIZE_HOLE, SIZE_HOLE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                                break;

                        }

                        _display.DrawText(j.ToString(), fontNumbers07, colorForeground, margineX + ((SIZE_HOLE) * (i + 1)) + (gapX * i) - fontNumbers07.Height - 1, margineY + ((SIZE_HOLE) * j) + (gapY * (j - 1) + fontNumbers07.CharWidth('0')) - 1);

                    }

                }

                switch (holeType)
                {

                    case HOLE_CIRCLE:

                        _display.DrawEllipse(colorForeground, 1, margineX + ((SIZE_HOLE) * (0 + 1)) + (gapX * 0) - (SIZE_HOLE / 2) - 1, margineY + ((SIZE_HOLE) * (h1 + 1)) + (gapY * h1) - (SIZE_HOLE / 2) - 1, SIZE_HOLE / 2, SIZE_HOLE / 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, margineX + ((SIZE_HOLE) * (1 + 1)) + (gapX * 1) - (SIZE_HOLE / 2) - 1, margineY + ((SIZE_HOLE) * (h2 + 1)) + (gapY * h2) - (SIZE_HOLE / 2) - 1, SIZE_HOLE / 2, SIZE_HOLE / 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, margineX + ((SIZE_HOLE) * (2 + 1)) + (gapX * 2) - (SIZE_HOLE / 2) - 1, margineY + ((SIZE_HOLE) * (m1 + 1)) + (gapY * m1) - (SIZE_HOLE / 2) - 1, SIZE_HOLE / 2, SIZE_HOLE / 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, margineX + ((SIZE_HOLE) * (3 + 1)) + (gapX * 3) - (SIZE_HOLE / 2) - 1, margineY + ((SIZE_HOLE) * (m2 + 1)) + (gapY * m2) - (SIZE_HOLE / 2) - 1, SIZE_HOLE / 2, SIZE_HOLE / 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, margineX + ((SIZE_HOLE) * (4 + 1)) + (gapX * 4) - (SIZE_HOLE / 2) - 1, margineY + ((SIZE_HOLE) * (s1 + 1)) + (gapY * s1) - (SIZE_HOLE / 2) - 1, SIZE_HOLE / 2, SIZE_HOLE / 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, margineX + ((SIZE_HOLE) * (5 + 1)) + (gapX * 5) - (SIZE_HOLE / 2) - 1, margineY + ((SIZE_HOLE) * (s2 + 1)) + (gapY * s2) - (SIZE_HOLE / 2) - 1, SIZE_HOLE / 2, SIZE_HOLE / 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                        break;


                    case HOLE_SQUARE:

                        _display.DrawRectangle(colorForeground, 1, margineX + ((SIZE_HOLE) * 0) + (gapX * (0 - 1)) + SIZE_HOLE + 1, margineY + ((SIZE_HOLE) * h1) + (gapY * (h1 - 1)) + 2, SIZE_HOLE, SIZE_HOLE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawRectangle(colorForeground, 1, margineX + ((SIZE_HOLE) * 1) + (gapX * (1 - 1)) + SIZE_HOLE + 1, margineY + ((SIZE_HOLE) * h2) + (gapY * (h2 - 1)) + 2, SIZE_HOLE, SIZE_HOLE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawRectangle(colorForeground, 1, margineX + ((SIZE_HOLE) * 2) + (gapX * (2 - 1)) + SIZE_HOLE + 1, margineY + ((SIZE_HOLE) * m1) + (gapY * (m1 - 1)) + 2, SIZE_HOLE, SIZE_HOLE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawRectangle(colorForeground, 1, margineX + ((SIZE_HOLE) * 3) + (gapX * (3 - 1)) + SIZE_HOLE + 1, margineY + ((SIZE_HOLE) * m2) + (gapY * (m2 - 1)) + 2, SIZE_HOLE, SIZE_HOLE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawRectangle(colorForeground, 1, margineX + ((SIZE_HOLE) * 4) + (gapX * (4 - 1)) + SIZE_HOLE + 1, margineY + ((SIZE_HOLE) * s1) + (gapY * (s1 - 1)) + 2, SIZE_HOLE, SIZE_HOLE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawRectangle(colorForeground, 1, margineX + ((SIZE_HOLE) * 5) + (gapX * (5 - 1)) + SIZE_HOLE + 1, margineY + ((SIZE_HOLE) * s2) + (gapY * (s2 - 1)) + 2, SIZE_HOLE, SIZE_HOLE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                        break;

                }

                _display.DrawText(h1.ToString(), fontNumbers07, colorBackground, margineX + ((SIZE_HOLE) * (0 + 1)) + (gapX * 0) - fontNumbers07.Height - 1, margineY + ((SIZE_HOLE) * h1) + (gapY * (h1 - 1) + fontNumbers07.CharWidth('0')) - 1);
                _display.DrawText(h2.ToString(), fontNumbers07, colorBackground, margineX + ((SIZE_HOLE) * (1 + 1)) + (gapX * 1) - fontNumbers07.Height - 1, margineY + ((SIZE_HOLE) * h2) + (gapY * (h2 - 1) + fontNumbers07.CharWidth('0')) - 1);
                _display.DrawText(m1.ToString(), fontNumbers07, colorBackground, margineX + ((SIZE_HOLE) * (2 + 1)) + (gapX * 2) - fontNumbers07.Height - 1, margineY + ((SIZE_HOLE) * m1) + (gapY * (m1 - 1) + fontNumbers07.CharWidth('0')) - 1);
                _display.DrawText(m2.ToString(), fontNumbers07, colorBackground, margineX + ((SIZE_HOLE) * (3 + 1)) + (gapX * 3) - fontNumbers07.Height - 1, margineY + ((SIZE_HOLE) * m2) + (gapY * (m2 - 1) + fontNumbers07.CharWidth('0')) - 1);
                _display.DrawText(s1.ToString(), fontNumbers07, colorBackground, margineX + ((SIZE_HOLE) * (4 + 1)) + (gapX * 4) - fontNumbers07.Height - 1, margineY + ((SIZE_HOLE) * s1) + (gapY * (s1 - 1) + fontNumbers07.CharWidth('0')) - 1);
                _display.DrawText(s2.ToString(), fontNumbers07, colorBackground, margineX + ((SIZE_HOLE) * (5 + 1)) + (gapX * 5) - fontNumbers07.Height - 1, margineY + ((SIZE_HOLE) * s2) + (gapY * (s2 - 1) + fontNumbers07.CharWidth('0')) - 1);

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

                case DISPLAY_MODE_BLACK_CIRCLE_12:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    holeType = HOLE_CIRCLE;
                    h12h24 = 12;

                    break;

                case DISPLAY_MODE_BLACK_CIRCLE_24:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    holeType = HOLE_CIRCLE;
                    h12h24 = 24;

                    break;

                case DISPLAY_MODE_BLACK_SQUARE_12:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    holeType = HOLE_SQUARE;
                    h12h24 = 12;

                    break;

                case DISPLAY_MODE_BLACK_SQUARE_24:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    holeType = HOLE_SQUARE;
                    h12h24 = 24;

                    break;

                case DISPLAY_MODE_WHITE_CIRCLE_12:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    holeType = HOLE_CIRCLE;
                    h12h24 = 12;

                    break;

                case DISPLAY_MODE_WHITE_CIRCLE_24:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    holeType = HOLE_CIRCLE;
                    h12h24 = 24;

                    break;

                case DISPLAY_MODE_WHITE_SQUARE_12:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    holeType = HOLE_SQUARE;
                    h12h24 = 12;

                    break;

                case DISPLAY_MODE_WHITE_SQUARE_24:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    holeType = HOLE_SQUARE;
                    h12h24 = 24;

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

