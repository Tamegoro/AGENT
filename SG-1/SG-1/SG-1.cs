using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace SG1
{
    public class SG1
    {

        static Bitmap _display;
        static Bitmap _gateRing;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static Timer _updateClockTimerAnimation;
        static TimeSpan dueTimeAnimation;
        static TimeSpan periodAnimation;

        static DateTime currentTime;

        static Font fontAncientGModern24 = Resources.GetFont(Resources.FontResources.AncientGModern24);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int oldH = 0;
        static int roundDirection = ROUND_RIGHT;


        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int gateRingWidth = 0;
        static int gateRingHeight = 0;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        static bool showAnimation = false;
        static int hourCounter = 0;
        static int degreeGateRing = 0;
        static int lockCounter = 0;

        static int fontType = FONT_ANCIENT;

        const int FONT_ANCIENT = 0;
        const int FONT_ARABIC = 1;

        const int SHOW_DIGITAL_SECOND = 10;

        const int ROUND_RIGHT = 0;
        const int ROUND_LEFT = 1;

        const int MARGIN_RIM_EDGE = 3;
        const int RIM_THICKNESS = 4;

        const int CHEVRON_LENGTH = 11;
        const int CHEVRON_THICKNESS = 6;

        const int HOLE_RADIUS = 40;
        const int HOLE_RIM_THICKNESS = 2;

        const int ROUND_INTERVAL = 100;
        const int ROUND_DEGREE = 20;
        const int LOCK_INTERVAL_BEFORE = 15;
        const int LOCK_INTERVAL_AFTER = 7;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _gateRing = new Bitmap(Resources.GetBytes(Resources.BinaryResources.GateRing), Bitmap.BitmapImageType.Gif);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            gateRingWidth = _gateRing.Width;
            gateRingHeight = _gateRing.Height;

            colorForeground = Color.White;
            colorBackground = Color.Black;

            fontType = FONT_ANCIENT;

            showDigital = false;
            showAnimation = false;

            degreeGateRing = 0;
            roundDirection = ROUND_RIGHT;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            oldH = currentTime.Hour;

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            dueTimeAnimation = new TimeSpan(0, 0, 0, 0, 0);
            periodAnimation = new TimeSpan(0, 0, 0, 0, ROUND_INTERVAL);

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

            if (oldH != currentTime.Hour)
            {

                oldH = currentTime.Hour;

                showAnimation = true;
                hourCounter = -1;
                lockCounter = 0;
                UpdateTimeAnimation(null);
                _updateClockTimerAnimation = new Timer(UpdateTimeAnimation, null, dueTimeAnimation, periodAnimation);

            }
            else if (showDigital == false && showAnimation == false)
            {

                DrawStarGate();

                for (int i = 0; i <= currentTime.Hour % 12; i++)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, CHEVRON_THICKNESS - 1, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, CHEVRON_LENGTH - 2, 5);

                }

                for (int i = (currentTime.Hour % 12) + 1; i < 12; i++)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, CHEVRON_THICKNESS - 1, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, CHEVRON_LENGTH - 2, 5);

                }

                if (fontType == FONT_ANCIENT)
                {
                    _azmdrawing.DrawStringCentered(_display, colorForeground, fontAncientGModern24, screenCenterX, screenCenterY, currentTime.Minute.ToString("D2"));
                }
                else if (fontType == FONT_ARABIC)
                {
                    _azmdrawing.DrawStringCentered(_display, colorForeground, font7barPBd24, screenCenterX, screenCenterY + 3, currentTime.Minute.ToString("D2"));
                }

                _display.Flush();

            }

        }

        private static void DrawStarGate()
        {
            _display.Clear();

            _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            if (degreeGateRing == 0)
            {
                _display.DrawImage((screenWidth - gateRingWidth) / 2 + 1, (screenHeight - gateRingHeight) / 2 + 1, _gateRing, 0, 0, gateRingWidth, gateRingHeight);
            }
            else
            {
                _display.RotateImage(degreeGateRing, (screenWidth - gateRingWidth) / 2 + 1, (screenHeight - gateRingHeight) / 2 + 1, _gateRing, 0, 0, gateRingWidth, gateRingHeight, 255);
            }

            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE, screenCenterY - MARGIN_RIM_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 0);
            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_RIM_EDGE + RIM_THICKNESS), screenCenterY - (MARGIN_RIM_EDGE + RIM_THICKNESS), colorForeground, 0, 0, colorForeground, 0, 0, 0);

            for (int i = 0; i < 360; i++)
            {
                if (i % 3 == 0)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_RIM_EDGE + RIM_THICKNESS + 1), RIM_THICKNESS + 2);
                }
            }

            _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_RIM_EDGE + RIM_THICKNESS) - 1, screenCenterY - (MARGIN_RIM_EDGE + RIM_THICKNESS) - 1, colorBackground, 0, 0, colorBackground, 0, 0, 0);

            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1, screenCenterY - MARGIN_RIM_EDGE + 1, colorForeground, 0, 0, colorForeground, 0, 0, 0);
            //_display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 2, screenCenterY - MARGIN_RIM_EDGE + 2, colorForeground, 0, 0, colorForeground, 0, 0, 0);


            for (int i = 0; i < 12; i++)
            {

                _azmdrawing.DrawAngledLine(_display, colorBackground, 6, (30 * i) + 15, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1, 5);

            }

            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE, screenCenterY - MARGIN_RIM_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 0);
            for (int i = 0; i <= HOLE_RIM_THICKNESS; i++)
            {
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, HOLE_RADIUS + i, HOLE_RADIUS + i, colorForeground, 0, 0, colorForeground, 0, 0, 0);
            }

        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.TopRight)
                {
                    if (showDigital == true && showAnimation == false)
                    {
                        showDigital = false;
                        UpdateTimeDigital(null);
                    }
                    else if (showAnimation == false)
                    {

                        if (fontType == FONT_ANCIENT)
                        {
                            fontType = FONT_ARABIC;
                        }
                        else
                        {
                            fontType = FONT_ANCIENT;
                        }

                        _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, HOLE_RADIUS - 1, HOLE_RADIUS - 1, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                        if (fontType == FONT_ANCIENT)
                        {
                            _azmdrawing.DrawStringCentered(_display, colorForeground, fontAncientGModern24, screenCenterX, screenCenterY, currentTime.Minute.ToString("D2"));
                        }
                        else if (fontType == FONT_ARABIC)
                        {
                            _azmdrawing.DrawStringCentered(_display, colorForeground, font7barPBd24, screenCenterX, screenCenterY + 3, currentTime.Minute.ToString("D2"));
                        }

                        _display.Flush();

                    }
                }
                else if (button == Buttons.MiddleRight)
                {
                    if (showDigital == true && showAnimation == false)
                    {
                        showDigital = false;
                        UpdateTimeDigital(null);
                    }
                    else if (showAnimation == false)
                    {
                        showDigital = true;
                        showDigitalCounter = 0;
                        UpdateTimeDigital(null);
                        _updateClockTimerDigital = new Timer(UpdateTimeDigital, null, dueTimeDigital, periodDigital);
                    }
                }
                else if (button == Buttons.BottomRight)
                {
                    if (showDigital == true && showAnimation == false)
                    {
                        showDigital = false;
                        UpdateTimeDigital(null);
                    }
                    else if (showAnimation == false)
                    {
                        showAnimation = true;
                        hourCounter = -1;
                        lockCounter = 0;
                        UpdateTimeAnimation(null);
                        _updateClockTimerAnimation = new Timer(UpdateTimeAnimation, null, dueTimeAnimation, periodAnimation);
                    }
                }

            }

        }

        static void UpdateTimeDigital(object state)
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

        static void UpdateTimeAnimation(object state)
        {

            currentTime = DateTime.Now;

            if (lockCounter == 0)
            {

                if (roundDirection == ROUND_RIGHT)
                {
                    degreeGateRing += ROUND_DEGREE;
                }
                else
                {
                    degreeGateRing -= ROUND_DEGREE;
                }

                degreeGateRing = (degreeGateRing + 360) % 360;

                DrawStarGate();

                for (int i = 0; i <= hourCounter % 12; i++)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, CHEVRON_THICKNESS - 1, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, CHEVRON_LENGTH - 2, 5);

                }

                for (int i = hourCounter + 1; i < 12; i++)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, CHEVRON_THICKNESS - 1, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, CHEVRON_LENGTH - 2, 5);

                }

                if (degreeGateRing <= 0 || 180 <= degreeGateRing)
                {
                    lockCounter = 1;
                    ++hourCounter;
                    Agent.Contrib.Hardware.Viberate.ViberateProvider.Current.Viberate(1);
                }

                _display.Flush();

            }
            else if (lockCounter < LOCK_INTERVAL_BEFORE)
            {
                ++lockCounter;
            }
            else if (lockCounter == LOCK_INTERVAL_BEFORE)
            {
                _point = _azmdrawing.FindPointDegreeDistance(30 * hourCounter, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                _azmdrawing.DrawAngledLine(_display, colorBackground, CHEVRON_THICKNESS, ((30 * hourCounter) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                _azmdrawing.DrawAngledLine(_display, colorForeground, CHEVRON_THICKNESS - 1, ((30 * hourCounter) + 180) % 360, _point.X, _point.Y, 1, CHEVRON_LENGTH - 2, 5);
                _display.Flush();
                ++lockCounter;
            }
            else if (lockCounter < LOCK_INTERVAL_BEFORE + LOCK_INTERVAL_AFTER)
            {
                ++lockCounter;
            }
            else if (LOCK_INTERVAL_BEFORE + LOCK_INTERVAL_AFTER <= lockCounter)
            {

                if (roundDirection == ROUND_RIGHT)
                {
                    roundDirection = ROUND_LEFT;
                }
                else
                {
                    roundDirection = ROUND_RIGHT;
                }

                lockCounter = 0;

                if (currentTime.Hour % 12 <= hourCounter)
                {
                    showAnimation = false;
                    UpdateTime(null);
                    _updateClockTimerAnimation.Dispose();
                }

            }

        }

    }

}

