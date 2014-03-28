using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace TinCanTelephone
{
    public class TinCanTelephone
    {

        static Bitmap _display;

        static Bitmap _bmpAsk;
        static Bitmap _bmpAnswer;
        static Bitmap _bmpBalloon;
        
        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontSetoTCT10eng = Resources.GetFont(Resources.FontResources.SetoTCT10eng);
        static Font fontSetoTCT16jpn = Resources.GetFont(Resources.FontResources.SetoTCT16jpn);

        static Color colorForeground;
        static Color colorBackground;

        static Random _random;

        static AZMDrawing _azmdrawing;

        static int screenWidth = 0;
        static int screenHeight = 0;

        //static int screenCenterX = 0;
        //static int screenCenterY = 0;

        static int balloonWidth = 0;
        static int balloonHeight = 0;

        static int tctWidth = 0;
        static int tctHeight = 0;

        static bool asking = false;
        static bool tired = false;
        static int lang = LANG_ENG;
        static int speakCounter = 0;
        static int waitCounter = 0;

        static string ampm = "";
        static string h1 = "";
        static string h2 = "";
        static string m1 = "";
        static string m2 = "";

        static string line01 = "";
        static string line02 = "";


        const int INTERVAL_MILLISECOND = 100;
        const int WAIT_MILLISECOND = 3000;

        const int PROBABILITY_TIRED = 9;

        static int displayMode = DISPLAY_MODE_WHITE_ENG;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SECOND_SHOW_DIGITAL = 10;

        const int LANG_ENG = 0;
        const int LANG_JPN = 1;

        const int LINE_X_01_01_ENG = 10;
        const int LINE_Y_01_01_ENG = 29;

        const int LINE_X_01_02_ENG = 10;
        const int LINE_Y_01_02_ENG = 19;
        const int LINE_X_02_02_ENG = 45;
        const int LINE_Y_02_02_ENG = 38;

        const int LINE_X_01_01_JPN = 13;
        const int LINE_Y_01_01_JPN = 25;


        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK_ENG = 0;
        const int DISPLAY_MODE_WHITE_ENG = 1;
        const int DISPLAY_MODE_BLACK_JPN = 2;
        const int DISPLAY_MODE_WHITE_JPN = 3;

        const string MESSAGE_ASK_ENG_STRING_01 = "Do you have";
        const string MESSAGE_ASK_ENG_STRING_02 = "the time?";

        const string MESSAGE_ANSWER_ENG_STRING_01 = "It's ";
        const string MESSAGE_ANSWER_ENG_STRING_02 = ":";
        const string MESSAGE_ANSWER_ENG_STRING_03 = " ";
        const string MESSAGE_ANSWER_ENG_STRING_04 = ".";

        const string MESSAGE_RND_ASK_ENG_STRING_01 = "I'm tired";
        const string MESSAGE_RND_ASK_ENG_STRING_02 = " of this.";

        const string MESSAGE_RND_ANSWER_ENG_STRING_01 = "Indeed...";

        const string MESSAGE_ASK_JPN_STRING_01 = "今何時？";

        const string MESSAGE_ANSWER_JPN_STRING_01 = "時";
        const string MESSAGE_ANSWER_JPN_STRING_02 = "分";
        const string MESSAGE_ANSWER_JPN_STRING_03 = "丁度";

        const string MESSAGE_RND_ASK_JPN_STRING_01 = "飽きた･･･";

        const string MESSAGE_RND_ANSWER_JPN_STRING_01 = "俺も･･･";



        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            colorForeground = new Color();
            colorBackground = new Color();

            _bmpAsk = new Bitmap(Resources.GetBytes(Resources.BinaryResources.TinCanTelephone01), Bitmap.BitmapImageType.Gif);
            _bmpAnswer = new Bitmap(Resources.GetBytes(Resources.BinaryResources.TinCanTelephone02), Bitmap.BitmapImageType.Gif);
            _bmpBalloon = new Bitmap(Resources.GetBytes(Resources.BinaryResources.TinCanTelephone03), Bitmap.BitmapImageType.Gif);

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            //screenCenterX = screenWidth / 2;
            //screenCenterY = screenHeight / 2;

            balloonWidth = _bmpBalloon.Width;
            balloonHeight = _bmpBalloon.Height;

            tctWidth = _bmpAsk.Width;
            tctHeight = _bmpAsk.Height;

            displayMode = DISPLAY_MODE_BLACK_ENG;
            SetDisplayMode();


            asking = true;

            showDigital = false;

            currentTime = new DateTime();

            _random = new Random(currentTime.Millisecond);

            asking = true;
            lang = LANG_ENG;

            //UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 0, INTERVAL_MILLISECOND);

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

            currentTime = DateTime.Now;

            if (showDigital == false)
            {

                DrawWatchFace();
                FlushDisplay();

            }
            else
            {

                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                ++showDigitalCounter;

                if (showDigitalCounter > SECOND_SHOW_DIGITAL)
                {
                    showDigital = false;
                    showDigitalCounter = 0;
                    DrawWatchFace();
                }

                _display.Flush();

            }

        }

        private static void DrawWatchFace()
        {

            if (asking == true && speakCounter == 0)
            {

                if (tired != true)
                {

                    if (_random.Next(PROBABILITY_TIRED) == 0)
                    {
                        tired = true;
                    }

                }
                else
                {

                    tired = false;
                
                }

            }

            if (speakCounter == 0)
            {

                DrawWatchFaceBack();
                InitiateLines();

            }
            
            if (speakCounter <= line01.Length + line02.Length)
            {

                _display.DrawRectangle(colorBackground, 0, 0, balloonHeight, screenWidth, screenHeight - (balloonHeight + tctHeight), 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                for (int i = 0; i < 3; i++)
                {
                    _display.DrawLine(colorForeground, 1, 2 + i, balloonHeight, 2 + i, screenHeight - tctHeight - 1);
                    _display.DrawLine(colorForeground, 1, screenWidth - 5 + i, balloonHeight, screenWidth - 5 + i, screenHeight - tctHeight - 1);
                }

                if (asking == true && lang == LANG_ENG)
                {

                    if (speakCounter <= line01.Length)
                    {
                        _display.DrawText(line01.Substring(0, speakCounter), fontSetoTCT10eng, colorForeground, LINE_X_01_02_ENG, LINE_Y_01_02_ENG);
                    }
                    else
                    {
                        _display.DrawText(line01, fontSetoTCT10eng, colorForeground, LINE_X_01_02_ENG, LINE_Y_01_02_ENG);
                        _display.DrawText(line02.Substring(0, speakCounter - line01.Length), fontSetoTCT10eng, colorForeground, LINE_X_02_02_ENG, LINE_Y_02_02_ENG);
                    }

                }
                else if (lang == LANG_ENG)
                {

                    _display.DrawText(line01.Substring(0, speakCounter), fontSetoTCT10eng, colorForeground, LINE_X_01_01_ENG, LINE_Y_01_01_ENG);

                }
                else if (lang == LANG_JPN)
                {

                    _display.DrawText(line01.Substring(0, speakCounter), fontSetoTCT16jpn, colorForeground, LINE_X_01_01_JPN, LINE_Y_01_01_JPN);

                }
                
                ++speakCounter;

            }
            else if (waitCounter <= WAIT_MILLISECOND / INTERVAL_MILLISECOND)
            {

                ++waitCounter;

            }
            else
            {

                if (asking == true)
                {
                    asking = false;
                }
                else
                {
                    asking = true;
                }
                    
                speakCounter = 0;
                waitCounter = 0;

            }

            FlushDisplay();

        }

        private static void DrawWatchFaceBack()
        {

            //_display.Clear();

            //_display.DrawRectangle(colorBackground, 0, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _display.DrawImage(0, 0, _bmpBalloon, 0, 0, balloonWidth, balloonHeight);

            //_display.DrawRectangle(colorBackground, 0, 0, balloonHeight, screenWidth, screenHeight - (balloonHeight + tctHeight), 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            //for (int i = 0; i < 3; i++)
            //{
            //    _display.DrawLine(colorForeground, 1, 2 + i, balloonHeight, 2 + i, screenHeight - tctHeight - 1);
            //    _display.DrawLine(colorForeground, 1, screenWidth - 5 + i, balloonHeight, screenWidth - 5 + i, screenHeight - tctHeight - 1);
            //}

            if (asking == true)
            {
                _display.DrawImage(0, screenHeight - tctHeight, _bmpAsk, 0, 0, tctWidth, tctHeight);
            }
            else
            {
                _display.DrawImage(0, screenHeight - tctHeight, _bmpAnswer, 0, 0, tctWidth, tctHeight);

            }

        }

        private static void InitiateLines()
        {

            if (currentTime.Hour < 12)
            {
                ampm = "AM";
            }
            else
            {
                ampm = "PM";
            }

            if (currentTime.Hour % 12 == 0)
            {
                h1 = "1";
                h2 = "2";
            }
            else if ((currentTime.Hour % 12) < 10)
            {
                h1 = "";
                h2 = (currentTime.Hour % 12).ToString();
            }
            else
            {
                h1 = "";
                h2 = ((currentTime.Hour % 12) % 10).ToString();
            }

            m1 = (currentTime.Minute / 10).ToString();
            m2 = (currentTime.Minute % 10).ToString();

            if (tired == false && asking == true && lang == LANG_ENG)
            {

                line01 = MESSAGE_ASK_ENG_STRING_01;
                line02 = MESSAGE_ASK_ENG_STRING_02;

            }
            else if (tired == false && asking == false && lang == LANG_ENG)
            {

                line01 = MESSAGE_ANSWER_ENG_STRING_01 + h1 + h2 + MESSAGE_ANSWER_ENG_STRING_02 + m1 + m2 + MESSAGE_ANSWER_ENG_STRING_03 + ampm + MESSAGE_ANSWER_ENG_STRING_04;
                line02 = "";

            }
            else if (tired == true && asking == true && lang == LANG_ENG)
            {

                line01 = MESSAGE_RND_ASK_ENG_STRING_01;
                line02 = MESSAGE_RND_ASK_ENG_STRING_02;

            }
            else if (tired == true && asking == false && lang == LANG_ENG)
            {

                line01 = MESSAGE_RND_ANSWER_ENG_STRING_01;
                line02 = "";

            }
            else if (tired == false && asking == true && lang == LANG_JPN)
            {

                line01 = MESSAGE_ASK_JPN_STRING_01;
                line02 = "";

            }
            else if (tired == false && asking == false && lang == LANG_JPN)
            {

                if (m1 == "0" && m2 == "0")
                {

                    line01 = h1 + h2 + MESSAGE_ANSWER_JPN_STRING_01 + MESSAGE_ANSWER_JPN_STRING_03;

                }
                else if (m1 == "0")
                {

                    line01 = h1 + h2 + MESSAGE_ANSWER_JPN_STRING_01 + m2 + MESSAGE_ANSWER_JPN_STRING_02;

                }
                else
                {

                    line01 = h1 + h2 + MESSAGE_ANSWER_JPN_STRING_01 + m1 + m2 + MESSAGE_ANSWER_JPN_STRING_02;

                }


                line02 = "";

            }
            else if (tired == true && asking == true && lang == LANG_JPN)
            {

                line01 = MESSAGE_RND_ASK_JPN_STRING_01;
                line02 = "";

            }
            else if (tired == true && asking == false && lang == LANG_JPN)
            {

                line01 = MESSAGE_RND_ANSWER_JPN_STRING_01;
                line02 = "";

            }

        }


        private static void FlushDisplay()
        {

            if (showDigital != true)
            {
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
                        SetDisplayMode();
                        UpdateTime(null);
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
                        tired = true;
                        asking = true;
                        speakCounter = 0;
                        waitCounter = 0;
                        UpdateTimeDigital(null);

                        try
                        {
                            _updateClockTimerDigital.Dispose();
                        }
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            _updateClockTimerDigital = new Timer(UpdateTimeDigital, null, dueTimeDigital, periodDigital);
                        }
                    
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
                        SetDisplayMode();
                        UpdateTime(null);
                    }
                    else
                    {
                        showDigital = false;
                    }
                }


            }

        }

        private static void SetDisplayMode()
        {

            switch (displayMode)
            {


                case DISPLAY_MODE_WHITE_ENG:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    lang = LANG_ENG;

                    break;

                case DISPLAY_MODE_BLACK_ENG:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    lang = LANG_ENG;

                    break;

                case DISPLAY_MODE_WHITE_JPN:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    lang = LANG_JPN;

                    break;

                case DISPLAY_MODE_BLACK_JPN:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    lang = LANG_JPN;

                    break;

            }

            _azmdrawing.DrawImageReverseBW(_bmpBalloon, 0, 0, _bmpBalloon, 0, 0, balloonWidth, balloonHeight);
            _azmdrawing.DrawImageReverseBW(_bmpAsk, 0, 0, _bmpAsk, 0, 0, tctWidth, tctHeight);
            _azmdrawing.DrawImageReverseBW(_bmpAnswer, 0, 0, _bmpAnswer, 0, 0, tctWidth, tctHeight);

            tired = true;
            asking = true;
            speakCounter = 0;
            waitCounter = 0;
            //UpdateTime(null);

        }

        private static void UpdateTimeDigital(object state)
        {

            currentTime = DateTime.Now;


            if (showDigital == false || showDigitalCounter > SECOND_SHOW_DIGITAL)
            {
                showDigital = false;
                tired = true;
                asking = true;
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

