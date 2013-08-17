using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace DigiAna
{
    public class DigiAna
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font font7barPBdDigiAna48 = Resources.GetFont(Resources.FontResources._7barPBdDigiAna48);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeClock = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int clockX = 0;
        static int clockY = 0;
        
        static int displayMode = DISPLAY_MODE_DIGI_ANA_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int CLOCK_WIDTH = 27;
        const int CLOCK_HEIGHT = 27;

        const int MARGIN_HAND_CLOCK = 4;
        const int MARGIN_CLOCK_CLOCK = 2;
        const int MARGIN_DOT_CLOCK = 3;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_DIGI_ANA_BLACK = 0;
        const int DISPLAY_MODE_ANA_DIGI_BLACK = 1;
        const int DISPLAY_MODE_DIGI_ANA_WHITE = 2;
        const int DISPLAY_MODE_ANA_DIGI_WHITE = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            clockY = screenCenterY - (CLOCK_HEIGHT / 2);

            displayMode = DISPLAY_MODE_DIGI_ANA_BLACK;
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

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (displayMode == DISPLAY_MODE_DIGI_ANA_BLACK || displayMode == DISPLAY_MODE_DIGI_ANA_WHITE)
                {

                    clockX = (screenWidth - ((CLOCK_WIDTH * 4) + (MARGIN_CLOCK_CLOCK * 2) + (MARGIN_DOT_CLOCK * 2) + font7barPBdDigiAna48.CharWidth(':'))) / 2;
                    _display.DrawRectangle(colorForeground, 1, clockX, clockY, CLOCK_WIDTH, CLOCK_HEIGHT, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    degreeClock = _azmdrawing.HourToAngle((currentTime.Hour % 12) / 10 * 10, 0);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeClock, clockX + (CLOCK_WIDTH / 2), clockY + (CLOCK_WIDTH / 2), 0, (CLOCK_WIDTH / 2) - MARGIN_HAND_CLOCK, 0);  

                    clockX = (screenWidth - ((CLOCK_WIDTH * 4) + (MARGIN_CLOCK_CLOCK * 2) + (MARGIN_DOT_CLOCK * 2) + font7barPBdDigiAna48.CharWidth(':'))) / 2 + (CLOCK_WIDTH + MARGIN_CLOCK_CLOCK);                    
                    _display.DrawRectangle(colorForeground, 1, clockX, clockY, CLOCK_WIDTH, CLOCK_HEIGHT, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    degreeClock = _azmdrawing.HourToAngle((currentTime.Hour % 12) % 10, 0);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeClock, clockX + (CLOCK_WIDTH / 2), clockY + (CLOCK_WIDTH / 2), 0, (CLOCK_WIDTH / 2) - MARGIN_HAND_CLOCK, 0);  

                    clockX = (screenWidth - ((CLOCK_WIDTH * 4) + (MARGIN_CLOCK_CLOCK * 2) + (MARGIN_DOT_CLOCK * 2) + font7barPBdDigiAna48.CharWidth(':'))) / 2 + font7barPBdDigiAna48.CharWidth(':') + (MARGIN_DOT_CLOCK * 2) + ((CLOCK_WIDTH * 2) + MARGIN_CLOCK_CLOCK);                    
                    _display.DrawRectangle(colorForeground, 1, clockX, clockY, CLOCK_WIDTH, CLOCK_HEIGHT, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    degreeClock = _azmdrawing.MinuteToAngle(currentTime.Minute / 10 * 10);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeClock, clockX + (CLOCK_WIDTH / 2), clockY + (CLOCK_WIDTH / 2), 0, (CLOCK_WIDTH / 2) - MARGIN_HAND_CLOCK, 0);  

                    clockX = (screenWidth - ((CLOCK_WIDTH * 4) + (MARGIN_CLOCK_CLOCK * 2) + (MARGIN_DOT_CLOCK * 2) + font7barPBdDigiAna48.CharWidth(':'))) / 2 + font7barPBdDigiAna48.CharWidth(':') + (MARGIN_DOT_CLOCK * 2) + ((CLOCK_WIDTH * 3) + (MARGIN_CLOCK_CLOCK * 2));
                    _display.DrawRectangle(colorForeground, 1, clockX, clockY, CLOCK_WIDTH, CLOCK_HEIGHT, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    degreeClock = _azmdrawing.MinuteToAngle(currentTime.Minute % 10);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeClock, clockX + (CLOCK_WIDTH / 2), clockY + (CLOCK_WIDTH / 2), 0, (CLOCK_WIDTH / 2) - MARGIN_HAND_CLOCK, 0);  
                    
                    if (currentTime.Second % 2 == 0)
                    {
                        _display.DrawText(":", font7barPBdDigiAna48, colorForeground, screenCenterX - (font7barPBdDigiAna48.CharWidth(':') / 2), screenCenterY - (font7barPBdDigiAna48.Height / 2));
                    }
                    else
                    {
                        _display.DrawText(";", font7barPBdDigiAna48, colorForeground, screenCenterX - (font7barPBdDigiAna48.CharWidth(';') / 2), screenCenterY - (font7barPBdDigiAna48.Height / 2));
                    }

                
                
                
                }
                else if (displayMode == DISPLAY_MODE_ANA_DIGI_BLACK || displayMode == DISPLAY_MODE_ANA_DIGI_WHITE)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, 40);
                    _azmdrawing.DrawStringAngled(_display, colorForeground, font7barPBdDigiAna48, degreeH, _point.X, _point.Y, "0");

                    _point = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, 40);
                    _azmdrawing.DrawStringAngled(_display, colorForeground, font7barPBdDigiAna48, degreeM, _point.X, _point.Y, "1");
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

                    case DISPLAY_MODE_DIGI_ANA_BLACK:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ANA_DIGI_BLACK:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_DIGI_ANA_WHITE:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ANA_DIGI_WHITE:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                }

            }

        }


    }

}

