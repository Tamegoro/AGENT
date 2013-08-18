using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;

namespace PeepWindow
{
    public class PeepWindow
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static AZMDrawing _azmdrawing;

        static Color colorForeground;
        static Color colorBackground;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int degreeH = 0;
        static int degreeM = 0;

        static int drawH = 0;
        static int drawM = 0;

        static int oldM = 0;

        static int displayMode = DISPLAY_MODE_WHITE_POIHAND;

        static int handType = 0;
        static bool showSecond = false;
        static bool showDate = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MAX_DISPLAY_MODE = 15;

        const int HAND_TYPE_SQUARE = 0;
        const int HAND_TYPE_POINT = 1;

        const int DISPLAY_MODE_WHITE_POIHAND = 0;
        const int DISPLAY_MODE_WHITE_POIHAND_SECOND = 1;
        const int DISPLAY_MODE_WHITE_POIHAND_DATE = 2;
        const int DISPLAY_MODE_WHITE_POIHAND_SECOND_DATE = 3;
        const int DISPLAY_MODE_WHITE_SQUHAND = 4;
        const int DISPLAY_MODE_WHITE_SQUHAND_SECOND = 5;
        const int DISPLAY_MODE_WHITE_SQUHAND_DATE = 6;
        const int DISPLAY_MODE_WHITE_SQUHAND_SECOND_DATE = 7;
        const int DISPLAY_MODE_BLACK_POIHAND = 8;
        const int DISPLAY_MODE_BLACK_POIHAND_SECOND = 9;
        const int DISPLAY_MODE_BLACK_POIHAND_DATE = 10;
        const int DISPLAY_MODE_BLACK_POIHAND_SECOND_DATE = 11;
        const int DISPLAY_MODE_BLACK_SQUHAND = 12;
        const int DISPLAY_MODE_BLACK_SQUHAND_SECOND = 13;
        const int DISPLAY_MODE_BLACK_SQUHAND_DATE = 14;
        const int DISPLAY_MODE_BLACK_SQUHAND_SECOND_DATE = 15;



        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            currentTime = new DateTime();

            colorForeground = new Color();
            colorBackground = new Color();

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            displayMode = DISPLAY_MODE_WHITE_POIHAND;
            handType = HAND_TYPE_POINT;
            showSecond = false;
            showDate = false;
            colorForeground = Color.Black;
            colorBackground = Color.White;

            oldM = -1;

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

            if (showDigital != true)
            {

                if (showSecond == true || oldM != currentTime.Minute)
                {

                    oldM = currentTime.Minute;

                    _display.Clear();

                    degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                    degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                    _display.DrawRectangle(colorForeground, 1, 0, 0, _display.Width, _display.Height, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                    _display.DrawRectangle(colorBackground, 1, 3, 3, 56, 56, 8, 8, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                    _azmdrawing.FillArea(_display, colorBackground, 30, 30);
                    _display.DrawRectangle(colorForeground, 1, 5, 5, 52, 52, 8, 8, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                    _display.DrawRectangle(colorBackground, 1, 58, 58, 67, 67, 8, 8, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                    _azmdrawing.FillArea(_display, colorBackground, 90, 90);
                    _display.DrawRectangle(colorForeground, 1, 60, 60, 63, 63, 8, 8, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                    _display.DrawEllipse(colorForeground, 1, 63, 63, 53, 53, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                    for (int i = 47; i <= 58; i++)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 2, 6 * i, 64, 64, 50, 1);
                    }

                    for (int i = 15; i <= 30; i++)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 2, 6 * i, 63, 63, 50, 1);
                    }

                    for (int i = -8; i <= 8; i++)
                    {

                        drawH = ((_azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute) / 6) + i + 60) % 60;
                        drawM = (currentTime.Minute + i + 60) % 60;

                        if (drawH % 5 == 0)
                        {

                            if (drawH / 5 == 0)
                            {
                                _azmdrawing.DrawStringByDegreeDistance(_display, colorForeground, fontsmall, "12", 270 + 45 + (6 * i), 41);
                            }
                            else
                            {
                                _azmdrawing.DrawStringByDegreeDistance(_display, colorForeground, fontsmall, (drawH / 5).ToString(), 270 + 45 + (6 * i), 41);
                            }
                        }

                        if (drawM % 5 == 0)
                        {

                            _azmdrawing.DrawStringByDegreeDistance(_display, colorForeground, fontsmall, drawM.ToString("D2"), 90 + 45 + (6 * i), 40);

                        }

                    }

                    for (int i = 0; i < 2; i++)
                    {
                        _display.DrawRectangle(colorBackground, 1, 3 + i, 3 + i, 56 - (i * 2), 56 - (i * 2), 8, 8, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                        _display.DrawRectangle(colorBackground, 1, 58 + i, 58 + i, 67 - (i * 2), 67 - (i * 2), 8, 8, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                    }

                    if (showSecond == true)
                    {
                        //_display.DrawEllipse(colorBackground, 1, 29, 93, 22, 22, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        //_display.DrawEllipse(colorForeground, 1, 29, 93, 19, 19, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                        _display.DrawRectangle(colorBackground, 1, 9, 73, 40, 40, 8, 8, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                        _azmdrawing.FillArea(_display, colorBackground, 20, 80);
                        _display.DrawRectangle(colorForeground, 1, 11, 75, 36, 36, 8, 8, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 2, _azmdrawing.SecondToAngle(currentTime.Second), 29, 93, 0, 15);
                        _display.DrawEllipse(colorForeground, 1, 29, 93, 2, 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                        _display.DrawEllipse(colorBackground, 1, 29, 93, 1, 1, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    }

                    if (showDate == true)
                    {
                        _display.DrawRectangle(colorBackground, 1, 82, 20, 23, 19, 3, 3, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                        _azmdrawing.FillArea(_display, colorBackground, 90, 25);
                        _display.DrawRectangle(colorForeground, 1, 84, 22, 19, 15, 3, 3, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                        _display.DrawText(currentTime.Day.ToString("D2"), fontsmall, colorForeground, 88, 23);
                    }

                    _azmdrawing.DrawAngledLine(_display, colorForeground, 4, 315, 58, 58, 0, 25, handType);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 4, 135, 59, 59, 0, 40, handType);

                    _display.DrawLine(colorBackground, 1, 58, 53, 63, 58);
                    _display.DrawLine(colorBackground, 1, 53, 58, 58, 63);

                    _display.DrawEllipse(colorBackground, 1, 58, 58, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, 58, 58, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                    _display.Flush();

                }

            }
            else
            {

                _display.Clear();

                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                ++showDigitalCounter;

                if (showDigitalCounter > SHOW_DIGITAL_SECOND)
                {
                    showDigital = false;
                    showDigitalCounter = 0;
                }

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

                    case DISPLAY_MODE_WHITE_POIHAND:

                        handType = HAND_TYPE_POINT;
                        showSecond = false;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND_DATE:

                        handType = HAND_TYPE_POINT;
                        showSecond = false;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND:

                        handType = HAND_TYPE_SQUARE;
                        showSecond = false;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_DATE:

                        handType = HAND_TYPE_SQUARE;
                        showSecond = false;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND:

                        handType = HAND_TYPE_POINT;
                        showSecond = false;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_DATE:

                        handType = HAND_TYPE_POINT;
                        showSecond = false;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND:

                        handType = HAND_TYPE_SQUARE;
                        showSecond = false;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_DATE:

                        handType = HAND_TYPE_SQUARE;
                        showSecond = false;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND_SECOND:

                        handType = HAND_TYPE_POINT;
                        showSecond = true;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND_SECOND_DATE:

                        handType = HAND_TYPE_POINT;
                        showSecond = true;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_SECOND:

                        handType = HAND_TYPE_SQUARE;
                        showSecond = true;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_SECOND_DATE:

                        handType = HAND_TYPE_SQUARE;
                        showSecond = true;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_SECOND:

                        handType = HAND_TYPE_POINT;
                        showSecond = true;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_SECOND_DATE:

                        handType = HAND_TYPE_POINT;
                        showSecond = true;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_SECOND:

                        handType = HAND_TYPE_SQUARE;
                        showSecond = true;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_SECOND_DATE:

                        handType = HAND_TYPE_SQUARE;
                        showSecond = true;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        oldM = -1;
                        UpdateTime(null);

                        break;

                }

            }

        }

    }

}

