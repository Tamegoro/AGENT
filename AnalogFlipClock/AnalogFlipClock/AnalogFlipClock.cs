using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;

namespace AnalogFlipClock
{
    public class AnalogFlipClock
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static Timer _updateClockTimerFlip;
        static TimeSpan dueTimeFlip;
        static TimeSpan periodFlip;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static Bitmap _bmpwork;

        static Color colorForeground;
        static Color colorBackground;

        static int degreeH = 0;
        static int degreeM = 0;

        static int currentH = 0;
        static int currentM = 0;
        static int oldH = 0;
        static int oldM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int panelWidth = 0;
        static int panelHeight = 0;

        static int panelCenterX = 0;

        const int MAX_FRAME = 12;

        static bool showFlip = false;
        static int showFlipCounter = 0;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        const int MARGIN_PANEL_EDGE = 3;
        const int MARGIN_PANEL_PANEL = 2;
        const int RADIUS_PANEL_CORNER = 6;

        const int RADIUS_CENTER_DOT = 7;

        const int MAX_DISPLAY_MODE = 1;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 35;
        const int THICKNESS_HOUR_HAND = 3;
        const int LENGTH_MINUTE_HAND = 50;
        const int THICKNESS_MINUTE_HAND = 3;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_WHITE = 1;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            currentTime = new DateTime();

            colorForeground = new Color();
            colorBackground = new Color();

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            panelWidth = screenWidth - (MARGIN_PANEL_EDGE * 2);
            panelHeight = (screenHeight - ((MARGIN_PANEL_EDGE * 2) + MARGIN_PANEL_PANEL)) / 2;

            panelCenterX = panelWidth / 2;

            _bmpwork = new Bitmap(panelWidth, panelHeight);

            displayMode = DISPLAY_MODE_WHITE;
            colorForeground = Color.Black;
            colorBackground = Color.White;

            currentTime = DateTime.Now;
            currentH = currentTime.Hour;
            currentM = currentTime.Minute;
            oldH = currentH;
            oldM = currentM;

            UpdateTime(null);


            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            dueTimeFlip = new TimeSpan(0, 0, 0, 0, 0);
            periodFlip = new TimeSpan(0, 0, 0, 0, 100);

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

            if (showDigital != true)
            {

                currentTime = DateTime.Now;
                currentH = currentTime.Hour;
                currentM = currentTime.Minute;


                if (currentM == oldM)
                {

                    _display.Clear();

                    _display.DrawRectangle(colorForeground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                    DrawClock(currentH, currentM, 0);
                    DrawClock(currentH, currentM, 1);

                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_CENTER_DOT, RADIUS_CENTER_DOT, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                    _display.Flush();

                }
                else
                {


                    //UpdateTimeFlip(null);
                    showFlip = true;
                    _updateClockTimerFlip = new Timer(UpdateTimeFlip, null, dueTimeFlip, periodFlip);

                }
            }


        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (showFlip != true)
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
                            UpdateTimeDigital(null);
                            _updateClockTimerDigital = new Timer(UpdateTimeDigital, null, dueTimeDigital, periodDigital);
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
                }

                switch (displayMode)
                {

                    case DISPLAY_MODE_BLACK:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                }

            }

        }


        static void UpdateTimeDigital(object state)
        {

            currentTime = DateTime.Now;

            if (showDigital == false || showDigitalCounter > SHOW_DIGITAL_SECOND)
            {
                showDigitalCounter = 0;
                showDigital = false;
                UpdateTime(null);
                _updateClockTimerDigital.Dispose();
            }
            else
            {
                _display.Clear();
                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                _display.Flush();
                showDigitalCounter++;
            }

        }
        static void UpdateTimeFlip(object state)
        {

            currentTime = DateTime.Now;
            currentH = currentTime.Hour;
            currentM = currentTime.Minute;

            if (showFlipCounter > MAX_FRAME)
            {

                showFlipCounter = 0;
                showFlip = false;

                oldH = currentH;
                oldM = currentM;

                UpdateTime(null);
                _updateClockTimerFlip.Dispose();

            }
            else
            {

                _display.Clear();

                _display.DrawRectangle(colorForeground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                if (showFlipCounter <= (MAX_FRAME / 2))
                {
                    DrawClock(currentH, currentM, 0);
                    DrawClock(oldH, oldM, 0, showFlipCounter);
                    DrawClock(oldH, oldM, 1);
                }
                else
                {
                    DrawClock(currentH, currentM, 0);
                    DrawClock(oldH, oldM, 1);
                    DrawClock(currentH, currentM, 1, (showFlipCounter - (MAX_FRAME / 2)));
                }

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_CENTER_DOT, RADIUS_CENTER_DOT, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.Flush();

            }

            showFlipCounter++;

        }

        static void DrawClock(int hour, int minute, int upperLower, int frame = 0)
        {

            int handCenterY;
            int marginPanelY;

            degreeH = _azmdrawing.HourToAngle(hour, minute);
            degreeM = _azmdrawing.MinuteToAngle(minute);

            if (frame == 0)
            {
                panelHeight = (screenHeight - ((MARGIN_PANEL_EDGE * 2) + MARGIN_PANEL_PANEL)) / 2;
            }
            else
            {
                if (upperLower == 0)
                {
                    panelHeight = ((screenHeight - ((MARGIN_PANEL_EDGE * 2) + MARGIN_PANEL_PANEL)) / 2) - ((screenHeight - ((MARGIN_PANEL_EDGE * 2) + MARGIN_PANEL_PANEL)) / 2) / (MAX_FRAME / 2) * (frame);
                }
                else
                {
                    panelHeight = ((screenHeight - ((MARGIN_PANEL_EDGE * 2) + MARGIN_PANEL_PANEL)) / 2) / (MAX_FRAME / 2) * frame;
                }
            }

            if (upperLower == 0)
            {
                handCenterY = panelHeight - 1;
                marginPanelY = screenCenterY - panelHeight - (MARGIN_PANEL_PANEL / 2);
            }
            else
            {
                handCenterY = 0;
                marginPanelY = screenCenterY + (MARGIN_PANEL_PANEL / 2);
            }

            _bmpwork.Clear();

            _bmpwork.DrawRectangle(colorBackground, 1, 0, 0, _bmpwork.Width, _bmpwork.Height, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _bmpwork.DrawRectangle(colorForeground, 1, -1, -1, panelWidth + 2, panelHeight + 2, RADIUS_PANEL_CORNER, RADIUS_PANEL_CORNER, colorForeground, 0, 0, colorForeground, 0, 0, 255);

            _azmdrawing.FillArea(_bmpwork, colorForeground, 0, 0);
            _azmdrawing.FillArea(_bmpwork, colorForeground, panelWidth - 1, 0);
            _azmdrawing.FillArea(_bmpwork, colorForeground, 0, panelHeight - 1);
            _azmdrawing.FillArea(_bmpwork, colorForeground, panelWidth - 1, panelHeight - 1);

            if (frame == 0)
            {
                _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_HOUR_HAND, degreeH, panelCenterX, handCenterY, 0, LENGTH_HOUR_HAND, 4);
                _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_MINUTE_HAND, degreeM, panelCenterX, handCenterY, 0, LENGTH_MINUTE_HAND, 4);
            }
            else
            {
                if (upperLower == 0)
                {
                    if (0 <= degreeH && degreeH <= 90)
                    {
                        degreeH = degreeH + ((90 - degreeH) / (MAX_FRAME / 2) * frame);
                        _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_HOUR_HAND, degreeH, panelCenterX, handCenterY, 0, LENGTH_HOUR_HAND, 4);
                    }
                    else if (270 <= degreeH && degreeH <= 360)
                    {
                        degreeH = degreeH - ((degreeH - 270) / (MAX_FRAME / 2) * frame);
                        _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_HOUR_HAND, degreeH, panelCenterX, handCenterY, 0, LENGTH_HOUR_HAND, 4);
                    }
                    if (0 <= degreeM && degreeM <= 90)
                    {
                        degreeM = degreeM + ((90 - degreeM) / (MAX_FRAME / 2) * frame);
                        _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_MINUTE_HAND, degreeM, panelCenterX, handCenterY, 0, LENGTH_MINUTE_HAND, 4);
                    }
                    else if (270 <= degreeM && degreeM <= 360)
                    {
                        degreeM = degreeM - ((degreeM - 270) / (MAX_FRAME / 2) * frame);
                        _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_MINUTE_HAND, degreeM, panelCenterX, handCenterY, 0, LENGTH_MINUTE_HAND, 4);
                    }
                }
                else
                {
                    if (90 <= degreeH && degreeH <= 180)
                    {
                        degreeH = 90 + ((degreeH - 90) / (MAX_FRAME / 2) * frame);
                        _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_HOUR_HAND, degreeH, panelCenterX, handCenterY, 0, LENGTH_HOUR_HAND, 4);
                    }
                    else if (180 < degreeH && degreeH <= 270)
                    {
                        degreeH = 270 - ((270 - degreeH) / (MAX_FRAME / 2) * frame);
                        _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_HOUR_HAND, degreeH, panelCenterX, handCenterY, 0, LENGTH_HOUR_HAND, 4);
                    }
                    if (90 <= degreeM && degreeM <= 180)
                    {
                        degreeM = 90 + ((degreeM - 90) / (MAX_FRAME / 2) * frame);
                        _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_MINUTE_HAND, degreeM, panelCenterX, handCenterY, 0, LENGTH_MINUTE_HAND, 4);
                    }
                    else if (180 < degreeM && degreeM <= 270)
                    {
                        degreeM = 270 - ((270 - degreeM) / (MAX_FRAME / 2) * frame);
                        _azmdrawing.DrawAngledLine(_bmpwork, colorForeground, THICKNESS_MINUTE_HAND, degreeM, panelCenterX, handCenterY, 0, LENGTH_MINUTE_HAND, 4);
                    }
                }
            }

            _display.DrawImage(MARGIN_PANEL_EDGE, marginPanelY, _bmpwork, 0, 0, panelWidth, panelHeight);

            if (frame != 0)
            {
                if (upperLower == 0)
                {
                    _display.DrawLine(colorForeground, 1, 0, marginPanelY - 1, screenWidth, marginPanelY - 1);
                }
                else
                {
                    _display.DrawLine(colorForeground, 1, 0, screenCenterY + panelHeight + 1, screenWidth, screenCenterY + panelHeight + 1);
                }
            }

        }

    }

}

