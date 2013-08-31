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
        static Bitmap _innerRing;
        static Bitmap _iris;

        static Bitmap _bmpwork;

        
        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static Timer _updateClockTimerDial;
        static TimeSpan dueTimeDial;
        static TimeSpan periodDial;

        static Timer _updateClockTimerIris;
        static TimeSpan dueTimeIris;
        static TimeSpan periodIris;

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
        static int dialWaitCounter = 0;

        static bool closeIris = false;
        static int irisCounter = 0;
        static int irisWaitCounter = 0;

        static int irisCenterX = 0;
        static int irisCenterY = 0;

        static bool incomingWormhole = false;

        const int SHOW_DIGITAL_SECOND = 10;

        const int ROUND_RIGHT = 0;
        const int ROUND_LEFT = 1;

        const int MARGIN_RIM_EDGE = 3;
        const int RIM_THICKNESS = 3;

        const int CHEVRON_LENGTH = 10;
        const int CHEVRON_THICKNESS = 6;
        const int INNER_CHEVRON_LENGTH = 9;
        const int INNER_CHEVRON_THICKNESS = 5;
        const int LOCKED_CHEVRON_LENGTH = 7;
        const int LOCKED_CHEVRON_THICKNESS = 4;

        const int HOLE_RADIUS = 46;
        const int HOLE_RIM_THICKNESS = 2;
        const int MARGIN_HOLE_RIM = 1;

        const int DIAL_INTERVAL = 50;
        const int DIAL_DEGREE = 20;
        const int DIAL_WAIT = 30;
        const int LOCK_INTERVAL_BEFORE = 40;
        const int LOCK_INTERVAL_AFTER = 20;

        const int IRIS_WIDTH = 93;
        const int IRIS_HEIGHT = 93;

        const int IRIS_INTERVAL = 150;
        const int IRIS_WAIT = 10;
        const int IRIS_WAIT2 = 10;
        const int IRIS_DEGREE = 5;
        const int MAX_IRIS_COUNTER = 15;
        const int RADIUS_IRIS_ANIMATION = 3;
        const int MARGIN_IRIS_WAIT = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _innerRing = new Bitmap(Resources.GetBytes(Resources.BinaryResources.InnerRing), Bitmap.BitmapImageType.Gif);
            _iris = new Bitmap(Resources.GetBytes(Resources.BinaryResources.Iris), Bitmap.BitmapImageType.Gif);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            gateRingWidth = _innerRing.Width;
            gateRingHeight = _innerRing.Height;

            irisCenterX = IRIS_WIDTH / 2;
            irisCenterY = IRIS_HEIGHT / 2;

            _bmpwork = new Bitmap(IRIS_WIDTH, IRIS_HEIGHT);

            colorForeground = Color.White;
            colorBackground = Color.Black;

            showDigital = false;
            showAnimation = false;

            degreeGateRing = 0;
            roundDirection = ROUND_RIGHT;

            closeIris = false;
            
            incomingWormhole = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            oldH = currentTime.Hour;

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            dueTimeDial = new TimeSpan(0, 0, 0, 0, 0);
            periodDial = new TimeSpan(0, 0, 0, 0, DIAL_INTERVAL);

            dueTimeIris = new TimeSpan(0, 0, 0, 0, 0);
            periodIris = new TimeSpan(0, 0, 0, 0, IRIS_INTERVAL);

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

                if (showAnimation == false)
                {
                    showAnimation = true;
                    hourCounter = -1;
                    lockCounter = 0;
                    dialWaitCounter = 0;
                    incomingWormhole = true;
                    UpdateTimeDial(null);
                    _updateClockTimerDial = new Timer(UpdateTimeDial, null, dueTimeDial, periodDial);
                }

            }
            else if (showDigital == false && showAnimation == false)
            {

                DrawStarGate();

                for (int i = 0; i <= currentTime.Hour % 12; i++)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, LOCKED_CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, LOCKED_CHEVRON_LENGTH, 5);

                }

                for (int i = (currentTime.Hour % 12) + 1; i < 12; i++)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, INNER_CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, INNER_CHEVRON_LENGTH, 5);

                }

                if (closeIris != true)
                {
                    _azmdrawing.DrawStringCentered(_display, colorForeground, fontAncientGModern24, screenCenterX, screenCenterY, currentTime.Minute.ToString("D2"));
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
                _display.DrawImage((screenWidth - gateRingWidth) / 2 + 1, (screenHeight - gateRingHeight) / 2 + 1, _innerRing, 0, 0, gateRingWidth, gateRingHeight);
            }
            else
            {
                _display.RotateImage(degreeGateRing, (screenWidth - gateRingWidth) / 2 + 1, (screenHeight - gateRingHeight) / 2 + 1, _innerRing, 0, 0, gateRingWidth, gateRingHeight, 255);
            }

            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE, screenCenterY - MARGIN_RIM_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 0);
            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_RIM_EDGE + RIM_THICKNESS), screenCenterY - (MARGIN_RIM_EDGE + RIM_THICKNESS), colorForeground, 0, 0, colorForeground, 0, 0, 0);
            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, HOLE_RADIUS + 1, HOLE_RADIUS + 1, colorForeground, 0, 0, colorForeground, 0, 0, 0);

            for (int i = 0; i < 360; i++)
            {
                if (i % 3 == 0)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_RIM_EDGE + RIM_THICKNESS + 1), RIM_THICKNESS + 1);
                }
            }

            _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1, screenCenterY - MARGIN_RIM_EDGE + 1, colorBackground, 0, 0, colorBackground, 0, 0, 0);
            _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_RIM_EDGE + RIM_THICKNESS) - 1, screenCenterY - (MARGIN_RIM_EDGE + RIM_THICKNESS) - 1, colorBackground, 0, 0, colorBackground, 0, 0, 0);

            if (closeIris == true)
            {
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, HOLE_RADIUS, HOLE_RADIUS, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _azmdrawing.DrawImageTransparently(_iris, 0, 0, _display, screenCenterX - irisCenterX, screenCenterY - irisCenterY, IRIS_WIDTH, IRIS_HEIGHT, colorBackground);
            }
            else
            {
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, HOLE_RADIUS, HOLE_RADIUS, colorBackground, 0, 0, colorBackground, 0, 0, 255);
            }

            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE, screenCenterY - MARGIN_RIM_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 0);

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

                        showAnimation = true;
                        irisCounter = 0;
                        irisWaitCounter = 0;
                        UpdateTimeIris(null);
                        _updateClockTimerIris = new Timer(UpdateTimeIris, null, dueTimeIris, periodIris);

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
                        dialWaitCounter = 0;
                        incomingWormhole = false;
                        UpdateTimeDial(null);
                        _updateClockTimerDial = new Timer(UpdateTimeDial, null, dueTimeDial, periodDial);
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

        static void UpdateTimeDial(object state)
        {

            currentTime = DateTime.Now;

            if (dialWaitCounter <= DIAL_WAIT)
            {

                if (dialWaitCounter == 0)
                {

                    DrawStarGate();

                    for (int i = 0; i < 12; i++)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                        _azmdrawing.DrawAngledLine(_display, colorBackground, INNER_CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, INNER_CHEVRON_LENGTH, 5);
                    }

                    _display.Flush();

                }
                
                ++ dialWaitCounter;

            }
            else if (lockCounter == 0)
            {

                if (roundDirection == ROUND_RIGHT)
                {
                    degreeGateRing += DIAL_DEGREE;
                }
                else
                {
                    degreeGateRing -= DIAL_DEGREE;
                }

                degreeGateRing = (degreeGateRing + 360) % 360;

                DrawStarGate();

                for (int i = 0; i <= hourCounter % 12; i++)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, LOCKED_CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, LOCKED_CHEVRON_LENGTH, 5);

                }

                for (int i = hourCounter + 1; i < 12; i++)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 0, CHEVRON_LENGTH, 5);
                    _azmdrawing.DrawAngledLine(_display, colorBackground, INNER_CHEVRON_THICKNESS, ((30 * i) + 180) % 360, _point.X, _point.Y, 1, INNER_CHEVRON_LENGTH, 5);

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
                _azmdrawing.DrawAngledLine(_display, colorForeground, LOCKED_CHEVRON_THICKNESS, ((30 * hourCounter) + 180) % 360, _point.X, _point.Y, 1, LOCKED_CHEVRON_LENGTH, 5);
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

                    if (incomingWormhole == true && closeIris == true)
                    {
                        irisCounter = 0;
                        irisWaitCounter = 0;
                        UpdateTimeIris(null);
                        _updateClockTimerIris = new Timer(UpdateTimeIris, null, dueTimeIris, periodIris);
                        _updateClockTimerDial.Dispose();
                    }
                    else
                    {
                        showAnimation = false;
                        UpdateTime(null);
                        _updateClockTimerDial.Dispose();
                    }

                }

            }

        }

        static void UpdateTimeIris(object state)
        {

            if (irisCounter < MAX_IRIS_COUNTER)
            {
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, HOLE_RADIUS - 1, HOLE_RADIUS - 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                if ((IRIS_DEGREE * irisCounter) % 360 == 0)
                {
                    _azmdrawing.DrawImageTransparently(_iris, 0, 0, _display, screenCenterX - irisCenterX, screenCenterY - irisCenterY, IRIS_WIDTH, IRIS_HEIGHT, colorBackground);
                }
                else
                {
                    _bmpwork.DrawRectangle(colorForeground, 1, 0, 0, IRIS_WIDTH, IRIS_HEIGHT, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _bmpwork.RotateImage((360 - (IRIS_DEGREE * irisCounter)) % 360, 0, 0, _iris, 0, 0, IRIS_WIDTH, IRIS_HEIGHT, 255);
                    _azmdrawing.DrawImageTransparently(_bmpwork, 0, 0, _display, screenCenterX - irisCenterX, screenCenterY - irisCenterY, IRIS_WIDTH, IRIS_HEIGHT, colorBackground);
                }


                if (closeIris == false)
                {
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, (RADIUS_IRIS_ANIMATION * (MAX_IRIS_COUNTER - irisCounter)), (RADIUS_IRIS_ANIMATION * (MAX_IRIS_COUNTER - irisCounter)), colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, (RADIUS_IRIS_ANIMATION * (MAX_IRIS_COUNTER - irisCounter)) , (RADIUS_IRIS_ANIMATION * (MAX_IRIS_COUNTER - irisCounter)), colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }
                else
                {
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_IRIS_ANIMATION * irisCounter, RADIUS_IRIS_ANIMATION * irisCounter, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_IRIS_ANIMATION * irisCounter , RADIUS_IRIS_ANIMATION * irisCounter, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, HOLE_RADIUS, HOLE_RADIUS, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, HOLE_RADIUS + 1, HOLE_RADIUS + 1, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                _display.Flush();

                ++irisCounter;

            }
            else if (MAX_IRIS_COUNTER <= irisCounter)
            {

                if (irisWaitCounter == 0)
                {

                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, HOLE_RADIUS, HOLE_RADIUS, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _azmdrawing.DrawImageTransparently(_iris, 0, 0, _display, screenCenterX - irisCenterX, screenCenterY - irisCenterY, IRIS_WIDTH, IRIS_HEIGHT, colorBackground);

                    if (closeIris == true)
                    {
                        _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, irisCenterX - MARGIN_IRIS_WAIT, irisCenterX - MARGIN_IRIS_WAIT, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, irisCenterX - MARGIN_IRIS_WAIT, irisCenterX - MARGIN_IRIS_WAIT, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    }
                    else
                    {
                        closeIris = true;
                        showAnimation = false;
                        UpdateTime(null);
                        _updateClockTimerIris.Dispose();
                    }

                    _display.Flush();

                }
                else if (irisWaitCounter == IRIS_WAIT)
                {
 
                    if (closeIris == true)
                    {
                        _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, HOLE_RADIUS + 1, HOLE_RADIUS + 1, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                        _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, HOLE_RADIUS, HOLE_RADIUS, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        _display.Flush();
                    }
                    else
                    {
                    }

                }
                else if (IRIS_WAIT + IRIS_WAIT2 <= irisWaitCounter)
                {

                    if (closeIris == true)
                    {
                        closeIris = false;
                    }
                    else
                    {
                        closeIris = true;
                    }

                    showAnimation = false;
                    UpdateTime(null);
                    _updateClockTimerIris.Dispose();

                }

                ++irisWaitCounter;

            }

        }

    }

}

