using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Additions
{
    public class Additions
    {
        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontIPAexGothicNumberSymbol16 = Resources.GetFont(Resources.FontResources.IPAGothicFixedNumberSymbol16);

        static AZMDrawing _azmdrawing;

        static Color colorForeground;
        static Color colorBackground;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int currentH = 0;
        static int currentM = 0;
        static int currentS = 0;

        static int oldH = 0;
        static int oldM = 0;

        static int lH = 0;
        static int rH = 0;
        static int lM = 0;
        static int rM = 0;
        static int lS = 0;
        static int rS = 0;

        static Random rnd;

        const int QUESTION_X_H = 14;
        const int QUESTION_Y_H = 15;
        const int QUESTION_X_M = 14;
        const int QUESTION_Y_M = 52;
        const int QUESTION_X_S = 14;
        const int QUESTION_Y_S = 90;

        const int ANSWER_X_H = 102;
        const int ANSWER_Y_H = 33;
        const int ANSWER_X_M = 102;
        const int ANSWER_Y_M = 72;
        const int ANSWER_X_S = 102;
        const int ANSWER_Y_S = 104;


        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showAnswer = false;
        static int showAnswerCounter = 0;


        const int SHOW_ANSWER_SECOND = 10;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_WHITE = 1;

        const int MAX_DISPLAY_MODE = 1;



        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            currentTime = new DateTime();

            colorForeground = new Color();
            colorBackground = new Color();

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK;
            colorForeground = Color.White;
            colorBackground = Color.Black;

            oldH = -1;
            oldM = -1;

            showAnswer = false;

            currentTime = DateTime.Now;

            rnd = new Random(currentTime.Millisecond);

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

            currentH = currentTime.Hour;
            currentM = currentTime.Minute;
            currentS = currentTime.Second;

            _display.Clear();

            _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            if (oldH != currentH)
            {
                if (currentH != 0)
                {
                    lH = rnd.Next(currentH);
                    rH = currentH - lH;
                    oldH = currentH;
                }
                else
                {
                    lH = 0;
                    rH = 0;
                    oldH = currentH;
                }
            }

            if (oldM != currentM)
            {
                if (currentM != 0)
                {
                    lM = rnd.Next(currentM);
                    rM = currentM - lM;
                    oldM = currentM;
                }
                else
                {
                    lM = 0;
                    rM = 0;
                    oldM = currentM;
                }
            }

            if (currentS != 0)
            {
                lS = rnd.Next(currentS);
                rS = currentS - lS;
            }
            else
            {
                lS = 0;
                rS = 0;
            }
 

            _display.DrawText(lH.ToString("D2") + "+" + rH.ToString("D2") + "=", fontIPAexGothicNumberSymbol16, colorForeground, QUESTION_X_H, QUESTION_Y_H);
            _display.DrawText(lM.ToString("D2") + "+" + rM.ToString("D2") + "=", fontIPAexGothicNumberSymbol16, colorForeground, QUESTION_X_M, QUESTION_Y_M);
            _display.DrawText(lS.ToString("D2") + "+" + rS.ToString("D2") + "=", fontIPAexGothicNumberSymbol16, colorForeground, QUESTION_X_S, QUESTION_Y_S);

            if (showAnswer == true)
            {
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontIPAexGothicNumberSymbol16, 90, ANSWER_X_H, ANSWER_Y_H, currentH.ToString("D2") + ":");
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontIPAexGothicNumberSymbol16, 90, ANSWER_X_M, ANSWER_Y_M, currentM.ToString("D2") + ":");
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontIPAexGothicNumberSymbol16, 90, ANSWER_X_S, ANSWER_Y_S, currentS.ToString("D2"));

                ++showAnswerCounter;

                if (showAnswerCounter > SHOW_ANSWER_SECOND)
                {
                    showAnswer = false;
                    showAnswerCounter = 0;
                    oldH = -1;
                    oldM = -1;
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
                    if (showAnswer == false)
                    {
                        --displayMode;
                        if (displayMode < 0)
                        {
                            displayMode = MAX_DISPLAY_MODE;
                        }
                    }
                    else
                    {
                        showAnswer = false;
                        oldH = -1;
                        oldM = -1;
                    }
                }
                else if (button == Buttons.MiddleRight)
                {
                    if (showAnswer != true)
                    {
                        showAnswer = true;
                        showAnswerCounter = 0;
                    }
                    else
                    {
                        showAnswer = false;
                        oldH = -1;
                        oldM = -1;
                    }
                }
                else if (button == Buttons.BottomRight)
                {
                    if (showAnswer == false)
                    {
                        ++displayMode;
                        if (displayMode > MAX_DISPLAY_MODE)
                        {
                            displayMode = 0;
                        }
                    }
                    else
                    {
                        showAnswer = false;
                        oldH = -1;
                        oldM = -1;
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


    }
}
