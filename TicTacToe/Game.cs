using SplashKitSDK;
using System;

namespace TicTacToe
{
    public class Game
    {
        private Board _board;
        private IPlayer _playerX;
        private IPlayer _playerO;
        private IPlayer _current;
        private bool _gameOver;
        private string _result;
        private bool _menuActive;

        // Name popup fields
        private bool _namePopupActive = false;
        private bool _enteringFirst = true;
        private string _playerXName = "";
        private string _playerOName = "";
        private bool _vsAI = false;

        // Animation variables
        private double _time = 0;
        private int _buttonHover = -1; // -1: none, 0: vs AI, 1: vs Friend, 2: restart

        public Game()
        {
            _board = new Board();
            _menuActive = true;   // show menu first
        }

        public void Update()
        {
            // Handle name popup logic
            if (_namePopupActive)
            {
                if (SplashKit.KeyTyped(KeyCode.ReturnKey))
                {
                    if (_vsAI)
                    {
                        _playerX = new HumanPlayer('X');
                        _playerO = new AIPlayer('O');
                        _current = _playerX;
                        _gameOver = false;
                        _namePopupActive = false;
                        _menuActive = false;
                    }
                    else
                    {
                        if (_enteringFirst)
                        {
                            _enteringFirst = false; // now ask for Player O
                        }
                        else
                        {
                            _playerX = new HumanPlayer('X');
                            _playerO = new HumanPlayer('O');
                            _current = _playerX;
                            _gameOver = false;
                            _namePopupActive = false;
                            _menuActive = false;
                        }
                    }
                }
                else if (SplashKit.KeyTyped(KeyCode.BackspaceKey))
                {
                    if (_enteringFirst && _playerXName.Length > 0)
                        _playerXName = _playerXName.Substring(0, _playerXName.Length - 1);
                    else if (!_enteringFirst && _playerOName.Length > 0)
                        _playerOName = _playerOName.Substring(0, _playerOName.Length - 1);
                }
                else
                {
                    foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (SplashKit.KeyTyped(key))
                        {
                            string keyName = SplashKit.KeyName(key);
                            if (!string.IsNullOrEmpty(keyName) && keyName.Length == 1)
                            {
                                if (_enteringFirst)
                                    _playerXName += keyName[0];
                                else
                                    _playerOName += keyName[0];
                            }
                        }
                    }
                }
                return; // stop game update while entering names
            }

            _time += 0.016; // Approximate frame time for 60fps

            if (_menuActive)
            {
                UpdateMenuHover();
                return;
            }

            if (_gameOver)
            {
                UpdateGameOverHover();
                if (SplashKit.KeyTyped(KeyCode.RKey) || (_buttonHover == 2 && SplashKit.MouseClicked(MouseButton.LeftButton)))
                {
                    _board.Reset();
                    _menuActive = true;
                    _buttonHover = -1;
                }
                return;
            }

            int before = _board.EmptyCells().Count;
            _current.MakeMove(_board);
            int after = _board.EmptyCells().Count;
if (after < before)
{
    if (_board.HasWinner(_current.Symbol))
    {
        if (_vsAI && _current.Symbol == 'O')
            _result = "AI won!";
        else
        {
            string winnerName = _current.Symbol == 'X' ? _playerXName : _playerOName;
            _result = $"{winnerName} won!";
        }
        _gameOver = true;
    }
    else if (_board.IsDraw())
    {
        _result = "It's a draw!";
        _gameOver = true;
    }
    else
    {
        _current = (_current == _playerX) ? _playerO : _playerX;
    }
}
        }

        private void UpdateMenuHover()
        {
            int mx = (int)SplashKit.MouseX();
            int my = (int)SplashKit.MouseY();

            int popupX = 100, popupY = 150;
            int btnW = 180, btnH = 70;
            int btn1X = popupX + 35, btn1Y = popupY + 180;
            int btn2X = popupX + 225, popupY2 = popupY + 180;

            _buttonHover = -1;
            if (mx >= btn1X && mx <= btn1X + btnW && my >= btn1Y && my <= btn1Y + btnH)
                _buttonHover = 0;
            else if (mx >= btn2X && mx <= btn2X + btnW && my >= popupY2 && my <= popupY2 + btnH)
                _buttonHover = 1;

            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                if (_buttonHover == 0) // Play with AI
                {
                    _vsAI = true;
                    _namePopupActive = true;
                    _enteringFirst = true;
                    _playerXName = "";
                    _playerOName = "";
                }
                else if (_buttonHover == 1) // Play with Friend
                {
                    _vsAI = false;
                    _namePopupActive = true;
                    _enteringFirst = true;
                    _playerXName = "";
                    _playerOName = "";
                }
            }
        }

        private void UpdateGameOverHover()
        {
            int mx = (int)SplashKit.MouseX();
            int my = (int)SplashKit.MouseY();

            int btnX = 200, btnY = 540, btnW = 200, btnH = 50;

            _buttonHover = -1;
            if (mx >= btnX && mx <= btnX + btnW && my >= btnY && my <= btnY + btnH)
                _buttonHover = 2;
        }

        public void Draw(Window w)
        {
            DrawAnimatedBackground(w);

            if (_namePopupActive)
            {
                DrawNamePopup(w);
                return;
            }

            if (_menuActive)
            {
                DrawEnhancedPopup(w);
                return;
            }

            _board.Draw(w);
            DrawEnhancedGameUI(w);
        }

        private void DrawNamePopup(Window w)
{
    int popupX = 80, popupY = 180, popupW = 440, popupH = 300;

    // Shadow
    w.FillRectangle(SplashKit.RGBAColor(0, 0, 0, 100), popupX + 5, popupY + 5, popupW, popupH);

    // Gradient background
    DrawGradientRect(w, popupX, popupY, popupW, popupH,
        SplashKit.RGBAColor(240, 248, 255, 250),
        SplashKit.RGBAColor(220, 230, 255, 250));

    // Border
    w.DrawRectangle(SplashKit.RGBColor(100, 149, 237), popupX, popupY, popupW, popupH);
    w.DrawRectangle(SplashKit.RGBAColor(100, 149, 237, 100), popupX - 1, popupY - 1, popupW + 2, popupH + 2);

    // Title
    double titleGlow = 0.5 + 0.5 * Math.Sin(_time * 3);
    Color titleColor = SplashKit.RGBColor((int)(50 + titleGlow * 50), (int)(100 + titleGlow * 50), 200);
    w.DrawText("PLAYER SETUP", titleColor, "Arial", 36, popupX + 90, popupY + 20);

    // Instructions
    string prompt;
    if (_vsAI)
        prompt = "Enter your name:";
    else if (_enteringFirst)
        prompt = "Enter Player X name:";
    else
        prompt = "Enter Player O name:";

    w.DrawText(prompt, SplashKit.RGBColor(70, 70, 70), "Arial", 24, popupX + 40, popupY + 100);

    // Name input box
    string currentInput = _enteringFirst ? _playerXName : _playerOName;
    w.FillRectangle(SplashKit.RGBAColor(255, 255, 255, 200), popupX + 40, popupY + 140, 360, 50);
    w.DrawRectangle(Color.Black, popupX + 40, popupY + 140, 360, 50);
    w.DrawText(currentInput + "|", Color.Black, "Arial", 28, popupX + 50, popupY + 150);

    // Hint text
    w.DrawText("Press ENTER when done", SplashKit.RGBColor(120, 120, 120), "Arial", 18, popupX + 100, popupY + 220);
}

        private void DrawAnimatedBackground(Window w)
        {
            Color topColor = SplashKit.RGBColor(
                (int)(45 + 15 * Math.Sin(_time * 0.5)),
                (int)(25 + 10 * Math.Cos(_time * 0.3)),
                (int)(85 + 20 * Math.Sin(_time * 0.7))
            );
            Color bottomColor = SplashKit.RGBColor(
                (int)(25 + 10 * Math.Sin(_time * 0.4)),
                (int)(45 + 15 * Math.Cos(_time * 0.6)),
                (int)(95 + 25 * Math.Sin(_time * 0.8))
            );

            for (int y = 0; y < 700; y += 2)
            {
                float ratio = (float)y / 700f;
                Color blendColor = SplashKit.RGBColor(
                    (int)(topColor.R * (1 - ratio) + bottomColor.R * ratio),
                    (int)(topColor.G * (1 - ratio) + bottomColor.G * ratio),
                    (int)(topColor.B * (1 - ratio) + bottomColor.B * ratio)
                );
                w.FillRectangle(blendColor, 0, y, 600, 2);
            }

            DrawAnimatedStars(w);
        }

        private void DrawAnimatedStars(Window w)
        {
            Random rand = new Random(42);

            for (int i = 0; i < 20; i++)
            {
                double x = (rand.NextDouble() * 600 + _time * 10 * (i % 3 + 1)) % 600;
                double y = rand.NextDouble() * 700;
                double brightness = 0.5 + 0.5 * Math.Sin(_time * 2 + i);

                Color starColor = SplashKit.RGBAColor(255, 255, 255, (byte)(brightness * 100));
                w.FillCircle(starColor, x, y, 1 + brightness);
            }
        }

        private void DrawEnhancedPopup(Window w)
        {
            int popupX = 50, popupY = 100, popupW = 500, popupH = 400;

            w.FillRectangle(SplashKit.RGBAColor(0, 0, 0, 100), popupX + 5, popupY + 5, popupW, popupH);

            DrawGradientRect(w, popupX, popupY, popupW, popupH,
                SplashKit.RGBAColor(240, 248, 255, 250),
                SplashKit.RGBAColor(220, 230, 255, 250));

            w.DrawRectangle(SplashKit.RGBColor(100, 149, 237), popupX, popupY, popupW, popupH);
            w.DrawRectangle(SplashKit.RGBAColor(100, 149, 237, 100), popupX - 1, popupY - 1, popupW + 2, popupH + 2);

            double titleGlow = 0.5 + 0.5 * Math.Sin(_time * 3);
            Color titleColor = SplashKit.RGBColor((int)(50 + titleGlow * 50), (int)(100 + titleGlow * 50), 200);

            w.DrawText("TIC TAC TOE", titleColor, "Arial", 42, popupX + 120, popupY + 30);
            w.DrawText("Choose Your Battle!", SplashKit.RGBColor(70, 70, 70), "Arial", 24, popupX + 140, popupY + 90);

            DrawEnhancedButton(w, popupX + 35, popupY + 180, 180, 70, "Play with AI", _buttonHover == 0, 0);
            DrawEnhancedButton(w, popupX + 225, popupY + 180, 180, 70, "Play with a FRIEND", _buttonHover == 1, 1);
        }

        private void DrawEnhancedButton(Window w, int x, int y, int width, int height, string text, bool hovered, int buttonIndex)
        {
            Color baseColor1 = buttonIndex == 0 ? SplashKit.RGBColor(100, 149, 237) : SplashKit.RGBColor(60, 179, 113);
            Color baseColor2 = buttonIndex == 0 ? SplashKit.RGBColor(70, 130, 180) : SplashKit.RGBColor(46, 139, 87);

            if (hovered)
            {
                double pulse = 0.8 + 0.2 * Math.Sin(_time * 8);
                baseColor1 = SplashKit.RGBColor((int)(baseColor1.R * pulse), (int)(baseColor1.G * pulse), (int)(baseColor1.B * pulse));
                baseColor2 = SplashKit.RGBColor((int)(baseColor2.R * pulse), (int)(baseColor2.G * pulse), (int)(baseColor2.B * pulse));

                w.FillRectangle(SplashKit.RGBAColor(255, 255, 255, 50), x - 3, y - 3, width + 6, height + 6);
            }

            w.FillRectangle(SplashKit.RGBAColor(0, 0, 0, 100), x + 3, y + 3, width, height);

            DrawGradientRect(w, x, y, width, height, baseColor1, baseColor2);

            Color borderColor = hovered ? SplashKit.RGBColor(255, 255, 255) : SplashKit.RGBColor(200, 200, 200);
            w.DrawRectangle(borderColor, x, y, width, height);

            int fontSize = 20;
            int textW = SplashKit.TextWidth(text, "Arial", fontSize);
            int textH = SplashKit.TextHeight(text, "Arial", fontSize);

            int textX = x + (width - textW) / 2;
            int textY = y + (height - textH) / 2;

            w.DrawText(text, SplashKit.RGBAColor(0, 0, 0, 150), "Arial", fontSize, textX + 1, textY + 1);
            w.DrawText(text, Color.White, "Arial", fontSize, textX, textY);
        }

        private void DrawGradientRect(Window w, int x, int y, int width, int height, Color topColor, Color bottomColor)
        {
            for (int i = 0; i < height; i += 2)
            {
                float ratio = (float)i / height;
                Color blendColor = SplashKit.RGBColor(
                    (int)(topColor.R * (1 - ratio) + bottomColor.R * ratio),
                    (int)(topColor.G * (1 - ratio) + bottomColor.G * ratio),
                    (int)(topColor.B * (1 - ratio) + bottomColor.B * ratio)
                );
                w.FillRectangle(blendColor, x, y + i, width, 2);
            }
        }

        private void DrawEnhancedGameUI(Window w)
        {
            DrawGradientRect(w, 0, 600, 600, 100,
                SplashKit.RGBAColor(240, 248, 255, 200),
                SplashKit.RGBAColor(220, 230, 255, 200));

            w.DrawRectangle(SplashKit.RGBColor(100, 149, 237), 0, 600, 600, 100);

            if (_gameOver)
            {
                double glow = 0.7 + 0.3 * Math.Sin(_time * 4);
                Color glowColor = SplashKit.RGBColor((int)(255 * glow), (int)(100 * glow), (int)(100 * glow));

                w.DrawText("GAME OVER", SplashKit.RGBAColor(0, 0, 0, 100), "Arial", 40, 181, 621);
                w.DrawText("GAME OVER", glowColor, "Arial", 40, 180, 620);
                int resultW = SplashKit.TextWidth(_result, "Arial", 28);
                int resultX = (600 - resultW) / 2;  // 600 = window width
                w.DrawText(_result, SplashKit.RGBColor(220, 20, 60), "Arial", 28, resultX, 660);


                DrawEnhancedButton(w, 200, 540, 200, 50, "NEW GAME", _buttonHover == 2, 0);
            }
            else
            {
                double pulse = 0.8 + 0.2 * Math.Sin(_time * 5);
                Color playerColor = _current.Symbol == 'X' ?
                    SplashKit.RGBColor((int)(34 * pulse), (int)(139 * pulse), (int)(34 * pulse)) :
                    SplashKit.RGBColor((int)(220 * pulse), (int)(20 * pulse), (int)(60 * pulse));

                string currentName = _current.Symbol == 'X' ? _playerXName : _playerOName;
                string turnText = $"{currentName}'s Turn";

                int textWidth = SplashKit.TextWidth(turnText, "Arial", 32);
                int textX = (600 - textWidth) / 2;

                w.DrawText(turnText, SplashKit.RGBAColor(0, 0, 0, 100), "Arial", 32, textX + 2, 642);
                w.DrawText(turnText, playerColor, "Arial", 32, textX, 640);

                int symbolX = textX + textWidth + 20;
                if (_current.Symbol == 'X')
                {
                    w.DrawLine(playerColor, symbolX - 10, 650, symbolX + 10, 670, SplashKit.OptionLineWidth(4));
                    w.DrawLine(playerColor, symbolX + 10, 650, symbolX - 10, 670, SplashKit.OptionLineWidth(4));
                }
                else
                {
                    w.DrawCircle(playerColor, symbolX, 660, 12, SplashKit.OptionLineWidth(4));
                }
            }
        }
    }
}
