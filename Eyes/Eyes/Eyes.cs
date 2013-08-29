using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Eyes
{
    public class Eyes
    {

        static Bitmap _display;
        static Timer _updateClockTimer;

        static DateTime currentTime;
        static TimeSpan dueTime;
        static TimeSpan period;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static Color colorBackground;
        static Color colorForeground;

        static int currentH = 0;
        static int currentM = 0;
        static int currentS = 0;

        static int degree = 0;
        static int distance = 0;

        static bool wakeup = false;

        const int LEFT_SIDE_EYE_CENTER_X = 33;
        const int LEFT_SIDE_EYE_CENTER_Y = 48;
        const int LEFT_SIDE_EYE_RADIUS_X = 28;
        const int LEFT_SIDE_EYE_RADIUS_Y = 35;

        const int RIGHT_SIDE_EYE_CENTER_X = 95;
        const int RIGHT_SIDE_EYE_CENTER_Y = 48;
        const int RIGHT_SIDE_EYE_RADIUS_X = 28;
        const int RIGHT_SIDE_EYE_RADIUS_Y = 35;

        const int IRIS_SIZE = 10;
        const int IRIS_OFFSET = 14;

        const int MOUTH_OPEN_CENTER_X = 64;
        const int MOUTH_OPEN_CENTER_Y = 65;
        const int MOUTH_OPEN_CUT_HEIGHT = 100;
        const int MOUTH_OPEN_CUT_MARGIN = 10;
        const int MOUTH_OPEN_RADIUS_X = 50;
        const int MOUTH_OPEN_RADIUS_Y = 50;

        const int MOUTH_CLOSE_CENTER_X = 64;
        const int MOUTH_CLOSE_CENTER_Y = 100;
        const int MOUTH_CLOSE_RADIUS_X = 30;
        const int MOUTH_CLOSE_RADIUS_Y = 10;

        const bool OPENED = true;
        const bool CLOSED = false;

        const bool LEFT_SIDE = true;
        const bool RIGHT_SIDE = false;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            currentTime = new DateTime();

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            colorBackground = Color.Black;
            colorForeground = Color.White;


            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period); // start our update timer

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        static void UpdateTime(object state)
        {

            currentTime = DateTime.Now;

            currentH = currentTime.Hour;
            currentM = currentTime.Minute;
            currentS = currentTime.Second;

            _display.Clear();

            _display.DrawRectangle(colorBackground, 1, 0, 0, _display.Width, _display.Height, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            if ((currentH >= 23 && wakeup == false) || (currentH < 7 && wakeup == false))
            {
                DrawMouth(CLOSED);
                DrawLeftEye(CLOSED);
                DrawRightEye(CLOSED);
            }
            else if (currentH % 12 != 0 && currentM != 0)
            {
                DrawMouth(OPENED);
                DrawLeftEye(OPENED);
                DrawRightEye(OPENED);
            }
            else if (currentH % 12 == 0 && currentM != 0)
            {
                DrawMouth(OPENED);
                DrawLeftEye(OPENED);
                //DrawLeftEye(CLOSED);
                DrawRightEye(OPENED);
            }
            else if (currentH % 12 != 0 && currentM == 0)
            {
                DrawMouth(OPENED);
                DrawLeftEye(OPENED);
                DrawRightEye(OPENED);
                //DrawRightEye(CLOSED);
            }
            else if (currentH % 12 == 0 && currentM == 0)
            {
                DrawMouth(OPENED);
                DrawLeftEye(OPENED);
                //DrawLeftEye(CLOSED);
                DrawRightEye(OPENED);
                //DrawRightEye(CLOSED);
            }


            if (wakeup == true)
            {
                wakeup = false;
            }


            _display.Flush();

        }

        private static void DrawLeftEye(bool OpenClose = OPENED)
        {

            if (OpenClose == OPENED)
            {
                _display.DrawEllipse(colorForeground, 1, LEFT_SIDE_EYE_CENTER_X, LEFT_SIDE_EYE_CENTER_Y, LEFT_SIDE_EYE_RADIUS_X, LEFT_SIDE_EYE_RADIUS_Y, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                //_display.DrawEllipse(colorBackground, 1, LEFT_SIDE_EYE_CENTER_X, LEFT_SIDE_EYE_CENTER_Y, 4, 4, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                DrawIris(LEFT_SIDE);
            }
            else
            {
                _display.DrawLine(colorForeground, 1, LEFT_SIDE_EYE_CENTER_X - LEFT_SIDE_EYE_RADIUS_X + 10, LEFT_SIDE_EYE_CENTER_Y, LEFT_SIDE_EYE_CENTER_X + LEFT_SIDE_EYE_RADIUS_X - 10, LEFT_SIDE_EYE_CENTER_Y);
                _display.DrawLine(colorForeground, 1, LEFT_SIDE_EYE_CENTER_X - LEFT_SIDE_EYE_RADIUS_X + 10, LEFT_SIDE_EYE_CENTER_Y + 1, LEFT_SIDE_EYE_CENTER_X + LEFT_SIDE_EYE_RADIUS_X - 10, LEFT_SIDE_EYE_CENTER_Y + 1);
                //_display.DrawLine(colorForeground, 1, LEFT_SIDE_EYE_CENTER_X - LEFT_SIDE_EYE_RADIUS_X + 10, LEFT_SIDE_EYE_CENTER_Y + 5, LEFT_SIDE_EYE_CENTER_X + LEFT_SIDE_EYE_RADIUS_X - 10, LEFT_SIDE_EYE_CENTER_Y);
                //_display.DrawLine(colorForeground, 1, LEFT_SIDE_EYE_CENTER_X - LEFT_SIDE_EYE_RADIUS_X + 10, LEFT_SIDE_EYE_CENTER_Y + 5 + 1, LEFT_SIDE_EYE_CENTER_X + LEFT_SIDE_EYE_RADIUS_X - 10, LEFT_SIDE_EYE_CENTER_Y + 1);
            }

        }


        private static void DrawRightEye(bool OpenClose = OPENED)
        {

            if (OpenClose == OPENED)
            {
                _display.DrawEllipse(colorForeground, 1, RIGHT_SIDE_EYE_CENTER_X, RIGHT_SIDE_EYE_CENTER_Y, RIGHT_SIDE_EYE_RADIUS_X, RIGHT_SIDE_EYE_RADIUS_Y, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                //_display.DrawEllipse(colorBackground, 1, RIGHT_SIDE_EYE_CENTER_X, RIGHT_SIDE_EYE_CENTER_Y, 4, 4, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                DrawIris(RIGHT_SIDE);
            }
            else
            {
                _display.DrawLine(colorForeground, 1, RIGHT_SIDE_EYE_CENTER_X - RIGHT_SIDE_EYE_RADIUS_X + 10, RIGHT_SIDE_EYE_CENTER_Y, RIGHT_SIDE_EYE_CENTER_X + RIGHT_SIDE_EYE_RADIUS_X - 10, RIGHT_SIDE_EYE_CENTER_Y);
                _display.DrawLine(colorForeground, 1, RIGHT_SIDE_EYE_CENTER_X - RIGHT_SIDE_EYE_RADIUS_X + 10, RIGHT_SIDE_EYE_CENTER_Y + 1, RIGHT_SIDE_EYE_CENTER_X + RIGHT_SIDE_EYE_RADIUS_X - 10, RIGHT_SIDE_EYE_CENTER_Y + 1);
                //_display.DrawLine(colorForeground, 1, RIGHT_SIDE_EYE_CENTER_X - RIGHT_SIDE_EYE_RADIUS_X + 10, RIGHT_SIDE_EYE_CENTER_Y, RIGHT_SIDE_EYE_CENTER_X + RIGHT_SIDE_EYE_RADIUS_X - 10, RIGHT_SIDE_EYE_CENTER_Y + 5);
                //_display.DrawLine(colorForeground, 1, RIGHT_SIDE_EYE_CENTER_X - RIGHT_SIDE_EYE_RADIUS_X + 10, RIGHT_SIDE_EYE_CENTER_Y + 1, RIGHT_SIDE_EYE_CENTER_X + RIGHT_SIDE_EYE_RADIUS_X - 10, RIGHT_SIDE_EYE_CENTER_Y + 5 + 1);
            }

        }


        private static void DrawMouth(bool OpenClose = OPENED)
        {

            if (OpenClose == OPENED)
            {

                _display.DrawEllipse(colorForeground, 1, MOUTH_OPEN_CENTER_X, MOUTH_OPEN_CENTER_Y, MOUTH_OPEN_RADIUS_X, MOUTH_OPEN_RADIUS_Y, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, MOUTH_OPEN_CENTER_X, MOUTH_OPEN_CENTER_Y, MOUTH_OPEN_RADIUS_X - 2, MOUTH_OPEN_RADIUS_Y - 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawRectangle(colorBackground, 1, 0, 0, _display.Width, MOUTH_OPEN_CUT_HEIGHT, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawRectangle(colorBackground, 1, 0, 0, MOUTH_OPEN_CUT_MARGIN, _display.Height, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawRectangle(colorBackground, 1, _display.Width - MOUTH_OPEN_CUT_MARGIN + 1, 0, MOUTH_OPEN_CUT_MARGIN, _display.Height, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            }
            else
            {
                _display.DrawLine(colorForeground, 1, MOUTH_CLOSE_CENTER_X - MOUTH_CLOSE_RADIUS_X, MOUTH_CLOSE_CENTER_Y, MOUTH_CLOSE_CENTER_X + MOUTH_CLOSE_RADIUS_X, MOUTH_CLOSE_CENTER_Y);
                _display.DrawLine(colorForeground, 1, MOUTH_CLOSE_CENTER_X - MOUTH_CLOSE_RADIUS_X, MOUTH_CLOSE_CENTER_Y + 1, MOUTH_CLOSE_CENTER_X + MOUTH_CLOSE_RADIUS_X, MOUTH_CLOSE_CENTER_Y + 1);
            }

        }


        private static void DrawIris(bool LeftRitgh = LEFT_SIDE)
        {

            //            const int LEFT_SIDE_EYE_RADIUS_X = 28;
            //            const int LEFT_SIDE_EYE_RADIUS_Y = 35;

            int x = 0;
            int y = 0;


            if (LeftRitgh == LEFT_SIDE)
            {
                x = LEFT_SIDE_EYE_CENTER_X;
                y = LEFT_SIDE_EYE_CENTER_Y;
                degree = _azmdrawing.HourToAngle(currentH, currentM);
            }
            else
            {
                x = RIGHT_SIDE_EYE_CENTER_X;
                y = RIGHT_SIDE_EYE_CENTER_Y;
                degree = _azmdrawing.MinuteToAngle(currentM);
            }

            if (0 <= degree && degree < 90)
            {
                distance = LEFT_SIDE_EYE_RADIUS_Y - ((LEFT_SIDE_EYE_RADIUS_Y - LEFT_SIDE_EYE_RADIUS_X) * (degree % 90) / 90) - IRIS_OFFSET;
            }
            else if (90 <= degree && degree < 180)
            {
                distance = LEFT_SIDE_EYE_RADIUS_X + ((LEFT_SIDE_EYE_RADIUS_Y - LEFT_SIDE_EYE_RADIUS_X) * (degree % 90) / 90) - IRIS_OFFSET;
            }
            else if (180 <= degree && degree < 270)
            {
                distance = LEFT_SIDE_EYE_RADIUS_Y - ((LEFT_SIDE_EYE_RADIUS_Y - LEFT_SIDE_EYE_RADIUS_X) * (degree % 90) / 90) - IRIS_OFFSET;
            }
            else if (270 <= degree && degree < 360)
            {
                distance = LEFT_SIDE_EYE_RADIUS_X + ((LEFT_SIDE_EYE_RADIUS_Y - LEFT_SIDE_EYE_RADIUS_X) * (degree % 90) / 90) - IRIS_OFFSET;
            }

            _point = _azmdrawing.FindPointDegreeDistance(degree, x, y, distance);

            _display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, IRIS_SIZE, IRIS_SIZE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

        }

        private static void LookStraight()
        {

            _display.DrawEllipse(colorForeground, 1, LEFT_SIDE_EYE_CENTER_X, LEFT_SIDE_EYE_CENTER_Y, LEFT_SIDE_EYE_RADIUS_X, LEFT_SIDE_EYE_RADIUS_Y, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            _display.DrawEllipse(colorBackground, 1, LEFT_SIDE_EYE_CENTER_X + 10, LEFT_SIDE_EYE_CENTER_Y + 0, IRIS_SIZE, IRIS_SIZE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _display.DrawEllipse(colorForeground, 1, RIGHT_SIDE_EYE_CENTER_X, RIGHT_SIDE_EYE_CENTER_Y, RIGHT_SIDE_EYE_RADIUS_X, RIGHT_SIDE_EYE_RADIUS_Y, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            _display.DrawEllipse(colorBackground, 1, RIGHT_SIDE_EYE_CENTER_X - 10, RIGHT_SIDE_EYE_CENTER_Y + 0, IRIS_SIZE, IRIS_SIZE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _display.Flush();

        }



        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.MiddleRight)
                {
                    if (currentH >= 23 || currentH < 7)
                    {
                        wakeup = true;
                        UpdateTime(null);
                    }
                    else
                    {
                        LookStraight();
                    }

                }

            }

        }


    }

}
