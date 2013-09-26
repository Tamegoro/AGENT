using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace DeathMarch
{
    public class DeathMarch
    {

        static Bitmap _display;

        static Bitmap _backgroundBlack;
        static Bitmap _backgroundWhite;
        
        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;


        static DateTime currentTime;

        static Font fontSmall = Resources.GetFont(Resources.FontResources.small);
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

        static int backgroundWidth = 0;
        static int backgroundHeight = 0;
        
        static int drawX = 0;
        static int drawY = 0;
        
        static int displayMode = DISPLAY_MODE_BLACK_FRAHAND;

        static int handType = 0;
        static bool showDisk = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int WORK_HOUR = 270;
        const int FREE_HOUR = 90;

        const int LENGTH_HOUR_HAND = 31;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 5;

        const int LENGTH_MINUTE_HAND = 41;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 5;

        const int LENGTH_SECOND_HAND = 40;
        const int LENGTH_SECOND_HAND_TAIL = 9;
        const int THICKNESS_SECOND_HAND = 1;

        const int RADIUS_PIN_OUTER = 2;
        const int RADIUS_PIN_INNER = 1;

        const int MARGIN_DIAL_EDGE = 5;
        const int MARGIN_DISK_EDGE = 3;
        const int THICKNESS_CIRCLE = 2;
        const int MARGIN_NUMBER_EDGE = 17;

        const int LENGTH_DIAL = 4;
        const int THICKNESS_DIAL = 2;

        const int MAX_DISPLAY_MODE = 3;

        const int HAND_TYPE_SQU = 0;
        const int HAND_TYPE_POI = 1;
        const int HAND_TYPE_FRA = 6;

        const int DISPLAY_MODE_BLACK_FRAHAND = 0;
        const int DISPLAY_MODE_BLACK_FRAHAND_DISK = 1;
        const int DISPLAY_MODE_WHITE_FRAHAND = 2;
        const int DISPLAY_MODE_WHITE_FRAHAND_DISK = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _backgroundBlack = new Bitmap(Resources.GetBytes(Resources.BinaryResources.ScullWhite), Bitmap.BitmapImageType.Gif);
            _backgroundWhite = new Bitmap(Resources.GetBytes(Resources.BinaryResources.ScullBlack), Bitmap.BitmapImageType.Gif);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            backgroundWidth = _backgroundBlack.Width;
            backgroundHeight = _backgroundBlack.Height;
            
            drawX = (screenWidth - backgroundWidth) / 2 + 1;
            drawY = (screenHeight - backgroundHeight) / 2 + 1;
            
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

                if (currentTime.Hour < 12)
                {

                    for (int i = 1; i <= 6; i++)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_DIAL, (FREE_HOUR / 6) * i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DIAL_EDGE + LENGTH_DIAL), LENGTH_DIAL);
                    }

                    for (int i = 7; i <= 12; i++)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_DIAL, (WORK_HOUR / 6) * (i - 6) + FREE_HOUR, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DIAL_EDGE + LENGTH_DIAL), LENGTH_DIAL);
                    }

                }
                else
                {

                    for (int i = 1; i <= 6; i++)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_DIAL, (WORK_HOUR / 6) * i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DIAL_EDGE + LENGTH_DIAL), LENGTH_DIAL);
                    }

                    for (int i = 7; i <= 12; i++)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_DIAL, (FREE_HOUR / 6) * (i - 6) + WORK_HOUR, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DIAL_EDGE + LENGTH_DIAL), LENGTH_DIAL);
                    }

                }


                if (showDisk == true)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE - 1, screenCenterY - MARGIN_DISK_EDGE - 1, colorDisk, 0, 0, colorDisk, 0, 0, 0);
                }
                else
                {
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE - 1, screenCenterY - MARGIN_DISK_EDGE - 1, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                }

                if (currentTime.Hour < 12)
                {

                    for (int i = 1; i <= 6; i++)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance((FREE_HOUR / 6) * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X, _point.Y, " " + i.ToString());
                    }

                    for (int i = 7; i <= 7; i++)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(((WORK_HOUR / 6) * (i - 6)) + FREE_HOUR, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X, _point.Y, " " + i.ToString());
                    }

                    for (int i = 8; i <= 12; i++)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(((WORK_HOUR / 6) * (i - 6)) + FREE_HOUR, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X, _point.Y, i.ToString());
                    }

                }
                else
                {

                    for (int i = 1; i <= 6; i++)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance((WORK_HOUR / 6) * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X, _point.Y, " " + i.ToString());
                    }

                    for (int i = 7; i <= 7; i++)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(((FREE_HOUR / 6) * (i - 6)) + WORK_HOUR, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X, _point.Y, " " + i.ToString());
                    }

                    for (int i = 8; i <= 12; i++)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(((FREE_HOUR / 6) * (i - 6)) + WORK_HOUR, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X, _point.Y, i.ToString());
                    }

                }

                if (colorForeground != Color.Black)
                {
                    _azmdrawing.DrawImageTransparently(_backgroundBlack, 0, 0, _display, drawX, drawY, backgroundWidth, backgroundHeight, colorForeground);
                }
                else
                {
                    _azmdrawing.DrawImageTransparently(_backgroundWhite, 0, 0, _display, drawX, drawY, backgroundWidth, backgroundHeight, colorForeground);
                }




                if (currentTime.Hour < 12)
                {

                    if (0 <= (currentTime.Hour % 12) && (currentTime.Hour % 12) <= 6)
                    {
                        degreeH = (FREE_HOUR * (currentTime.Hour % 12) / 6) + (FREE_HOUR * currentTime.Minute / 6 / 60);
                    }
                    else
                    {
                        degreeH = ((WORK_HOUR * ((currentTime.Hour % 12) - 6)) / 6 + FREE_HOUR) + (WORK_HOUR * currentTime.Minute / 6 / 60);
                    }

                    /*
                                        if (0 <= currentTime.Minute && currentTime.Minute <= 30)
                                        {
                                            degreeM = FREE_HOUR * currentTime.Minute / 30;
                                        }
                                        else
                                        {
                                            degreeM = (WORK_HOUR * (currentTime.Minute - 30)) / 30 + FREE_HOUR;
                                        }


                                        if (0 <= currentTime.Second && currentTime.Second <= 30)
                                        {
                                            degreeS = FREE_HOUR * currentTime.Second / 30;
                                        }
                                        else
                                        {
                                            degreeS = (WORK_HOUR * (currentTime.Second - 30)) / 30 + FREE_HOUR;
                                        }
                    */

                }
                else
                {

                    if (0 <= (currentTime.Hour % 12) && (currentTime.Hour % 12) <= 6)
                    {
                        degreeH = (WORK_HOUR * (currentTime.Hour % 12) / 6) + (WORK_HOUR * currentTime.Minute / 6 / 60);
                    }
                    else
                    {
                        degreeH = ((FREE_HOUR * ((currentTime.Hour % 12) - 6)) / 6 + WORK_HOUR) + (FREE_HOUR * currentTime.Minute / 6 / 60);
                    }

                    /*
                                        if (0 <= currentTime.Minute && currentTime.Minute <= 30)
                                        {
                                            degreeM = WORK_HOUR * currentTime.Minute / 30;
                                        }
                                        else
                                        {
                                            degreeM = (FREE_HOUR * (currentTime.Minute - 30)) / 30 + WORK_HOUR;
                                        }


                                        if (0 <= currentTime.Second && currentTime.Second <= 30)
                                        {
                                            degreeS = WORK_HOUR * currentTime.Second / 30;
                                        }
                                        else
                                        {
                                            degreeS = (FREE_HOUR * (currentTime.Second - 30)) / 30 + WORK_HOUR;
                                        }
                    */

                }

                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND, handType);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, (degreeH + 180) % 360, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_TAIL, handType);

                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND, handType);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, (degreeM + 180) % 360, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL, handType);

                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, screenCenterX, screenCenterY, 0, LENGTH_SECOND_HAND, handType);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, (degreeS + 180) % 360, screenCenterX, screenCenterY, 0, LENGTH_SECOND_HAND_TAIL, handType);


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
                    handType = HAND_TYPE_FRA;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_BLACK_FRAHAND_DISK:

                    colorForeground = Color.White;
                    colorBackground = Color.White;
                    colorDisk = Color.Black;
                    handType = HAND_TYPE_FRA;
                    showDisk = true;

                    break;

                case DISPLAY_MODE_WHITE_FRAHAND:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    colorDisk = Color.White;
                    handType = HAND_TYPE_FRA;
                    showDisk = false;

                    break;

                case DISPLAY_MODE_WHITE_FRAHAND_DISK:

                    colorForeground = Color.Black;
                    colorBackground = Color.Black;
                    colorDisk = Color.White;
                    handType = HAND_TYPE_FRA;
                    showDisk = true;

                    break;

            }

        }

    }

}

