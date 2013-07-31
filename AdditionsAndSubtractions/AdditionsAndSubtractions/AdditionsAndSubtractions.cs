using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace AdditionsAndSubtractions
{
    public class AdditionsAndSubtractions
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontIPAGothicFixedNumberSymbol16 = Resources.GetFont(Resources.FontResources.IPAGothicFixedNumberSymbol16);

        static AZMDrawing _azmdrawing;

        static Color colorForeground;
        static Color colorBackground;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int questionX = 0;
        static int questionYH = 0;
        static int questionYM = 0;
        static int questionYS = 0;

        static int answerX = 0;
        static int answerY = 0;

        static int fontWidth = 0;
        static int fontHeight = 0;
        
        static int currentH = 0;
        static int currentM = 0;
        static int currentS = 0;

        static int oldH = 0;
        static int oldM = 0;

        static int lH = 0;
        static int rH = 0;
        static string operatorH = "";
        static int lM = 0;
        static int rM = 0;
        static string operatorM = "";
        static int lS = 0;
        static int rS = 0;
        static string operatorS = "";

        static Random rnd;
        static int rndMax = 0;
        static int rndVal = 0;

        const int MARGIN_Y = 11;

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

            fontWidth = fontIPAGothicFixedNumberSymbol16.CharWidth('0');
            fontHeight = fontIPAGothicFixedNumberSymbol16.Height;


            questionX = ((screenWidth - ((fontWidth * 6) + fontHeight)) / 2) + 1;
            questionYH = MARGIN_Y;
            questionYM = MARGIN_Y + fontHeight + ((screenHeight - ((MARGIN_Y * 2) + (fontHeight * 3))) / 2);
            questionYS = MARGIN_Y + (fontHeight * 2) + (screenHeight - ((MARGIN_Y * 2) + (fontHeight * 3)));

            answerX = (questionX * 2) + (fontWidth * 6);
            answerY = MARGIN_Y + (fontWidth * 4);

            displayMode = DISPLAY_MODE_BLACK;
            colorForeground = Color.White;
            colorBackground = Color.Black;

            oldH = -1;
            oldM = -1;

            showAnswer = false;
            //showAnswer = true;

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

                if (currentH == 0 || (currentH * 2) + 1 > 99)
                {
                    rndMax = 99;
                }
                else
                {
                    rndMax = (currentH * 2) + 1;
                }

                rndVal = 0;

                while (rndVal == 0 || rndVal == currentH)
                {
                    rndVal = rnd.Next(rndMax);
                }

                if (rndVal < currentH)
                {
                    lH = currentH - rndVal;
                    rH = currentH - lH;
                    operatorH = "+";
                }
                else
                {
                    lH = rndVal;
                    rH = rndVal - currentH;
                    operatorH = "-";
                }

                oldH = currentH;

            }

            if (oldM != currentM)
            {

                if (currentM == 0 || (currentM * 2) + 1 > 99)
                {
                    rndMax = 99;
                }
                else
                {
                    rndMax = (currentM * 2) + 1;
                }

                rndVal = 0;

                while (rndVal == 0 || rndVal == currentM)
                {
                    rndVal = rnd.Next(rndMax);
                }

                if (rndVal < currentM)
                {
                    lM = currentM - rndVal;
                    rM = currentM - lM;
                    operatorM = "+";
                }
                else
                {
                    lM = rndVal;
                    rM = rndVal - currentM;
                    operatorM = "-";
                }

                oldM = currentM;

            }


            if (currentS == 0 || (currentS * 2) + 1 > 99)
            {
                rndMax = 99;
            }
            else
            {
                rndMax = (currentS * 2) + 1;
            }

            rndVal = 0;

            while (rndVal == 0 || rndVal == currentS)
            {
                rndVal = rnd.Next(rndMax);
            }

            if (rndVal < currentS)
            {
                lS = currentS - rndVal;
                rS = currentS - lS;
                operatorS = "+";
            }
            else
            {
                lS = rndVal;
                rS = rndVal - currentS;
                operatorS = "-";
            }


            _display.DrawText(lH.ToString("D2") + operatorH + rH.ToString("D2") + "=", fontIPAGothicFixedNumberSymbol16, colorForeground, questionX, questionYH);
            _display.DrawText(lM.ToString("D2") + operatorM + rM.ToString("D2") + "=", fontIPAGothicFixedNumberSymbol16, colorForeground, questionX, questionYM);
            _display.DrawText(lS.ToString("D2") + operatorS + rS.ToString("D2") + "=", fontIPAGothicFixedNumberSymbol16, colorForeground, questionX, questionYS);

            if (showAnswer == true)
            {
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontIPAGothicFixedNumberSymbol16, 90, answerX, answerY, currentH.ToString("D2") + ":" + currentM.ToString("D2") + ":" + currentS.ToString("D2"));

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

