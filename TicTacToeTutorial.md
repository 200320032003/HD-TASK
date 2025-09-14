# SIT771 Task 7.4HD – Something Awesome
## Building an Enhanced Tic Tac Toe with AI and Visual Effects using SplashKit

---

### Introduction

In this tutorial we are going to build an **enhanced Tic Tac Toe game** in C# using the **SplashKitSDK**. 
The purpose of this project is to take a simple, well-known game and turn it into a polished, interactive 
application that demonstrates object-oriented programming (OOP) principles and modern coding practices.

Before we begin, here are some important terms you will see throughout this guide:

- **SplashKitSDK**: a software development toolkit that provides libraries for graphics, input handling, 
  sound, and more. It allows us to move from simple text-based programs to fully interactive applications 
  with windows, animations, and real-time user input.
- **Game Loop**: the continuous cycle that keeps a game running. It usually includes processing input, 
  updating the game state, and drawing the results to the screen.
- **Object-Oriented Programming (OOP)**: a programming style that structures code into classes and objects. 
  In this project we will use the four core principles:
  - *Abstraction*: using interfaces to define what players can do.
  - *Encapsulation*: keeping board state private and exposing only controlled methods.
  - *Inheritance*: organising shared behaviour in a structured way.
  - *Polymorphism*: allowing different types of players (human and AI) to act through the same interface.
- **Minimax Algorithm**: a decision-making algorithm used in games. It explores possible moves and ensures 
  the AI always plays optimally.
- **Encapsulation of State**: a concept where the game board manages its own state and rules, preventing 
  other parts of the program from changing it incorrectly.
- **Graphical Effects**: using SplashKit’s drawing functions to add polish such as glowing lines, pulsing 
  animations, and interactive menus.

In this walkthrough, we will:

1. Set up a new SplashKit project in MSYS2.
2. Write the game step by step, starting with the main entry file (`Program.cs`) and adding supporting 
   classes (`Board`, `HumanPlayer`, `AIPlayer`, `Game`,`IPlayer`).
3. Explain how each section of code works, what the terms mean, and how the parts connect together.
4. Add interactive menus, an unbeatable AI, and graphical enhancements.
5. Test and run the final game.

By the end of this tutorial, you will have created a fully working graphical game and developed a deeper 
understanding of coding conventions, debugging, OOP principles, and interactive software design.

### Check out my video on explanation regarding the concepts
[![Watch the demo](https://img.youtube.com/vi/YL8jDFC28GY/0.jpg)](https://youtu.be/YL8jDFC28GY)


### UML Class Diagram

The UML diagram of the Tic Tac Toe project is below.  
![Class Diagram](https://raw.githubusercontent.com/200320032003/HD-TASK/9b96a496c3f912d7754d3dd9f2c58c68b55e8e37/Untitled%20Diagram.drawio.png)


---

## Setting up the Project

### Step 1: Open MSYS2

Open the **MSYS2 UCRT64 terminal**. This terminal allows us to run commands to create and build SplashKit projects.

### Step 2: Create a New Folder

```bash
mkdir TicTacToe
cd TicTacToe
```

The command `mkdir TicTacToe` creates a directory named `TicTacToe`. The command `cd TicTacToe` moves into that directory so we can work inside it.

### Step 3: Initialise a SplashKit Project

```bash
skm dotnet new console
skm dotnet restore
```

This command uses the SplashKit manager (`skm`) to create a new C# console project preconfigured with SplashKit. It generates:

- `Program.cs` – the entry point of the program.
- `.csproj` – the project configuration file that contains references to SplashKit.

### Step 4: Open in Visual Studio Code

```bash
code .
```

This opens the current folder in Visual Studio Code, where we will edit the files.

---
### Step 5: Add the Font File

SplashKit requires a **TrueType Font (.ttf)** file when drawing text.  
If you use `"Arial"`, you need to make sure the `Arial.ttf` file is available in your project folder.

- **Windows:** Fonts are usually stored in `C:\Windows\Fonts\`.  
  Look for `Arial.ttf` and copy it into your project folder.  
- **Mac:** Fonts can be found in `/System/Library/Fonts/Supplemental/`.  
- **Linux:** Fonts are often stored in `/usr/share/fonts/truetype/`.

Once copied, place the `.ttf` file inside your `TicTacToe` folder (where your `.cs` files are).

Update your code to load it like this:

```csharp
SplashKit.LoadFont("Arial", "ARIAL.TTF");
```
### Step-by-Step Development


We will build the game step by step. The final structure will include:

- `Program.cs` – main entry point and game loop.
- `Board.cs` – manages the grid and rules of Tic Tac Toe.
- `IPlayer.cs` – interface for players.
- `HumanPlayer.cs` – allows mouse-controlled moves.
- `AIPlayer.cs` – implements the Minimax algorithm for the AI.
- `Game.cs` – handles menus, turns, and overall flow.

---

### Program.cs – Entry Point and Game Loop

```csharp
using SplashKitSDK;

namespace TicTacToe
{
    public static class Program
    {
        public static void Main()
        {
            SplashKit.LoadFont("Arial", @"C:\msys64\home\TicTacToe\ARIAL.TTF");
            Window w = new("Tic Tac Toe", 600, 700);

            Game game = new Game();

            while (!w.CloseRequested)
            {
                SplashKit.ProcessEvents();
                game.Update();
                w.Clear(Color.DarkOliveGreen);
                game.Draw(w);
                w.Refresh(60);
            }
        }
    }
}
```

This file sets up the **game loop**, which is the continuous cycle of updating and drawing used in almost all games.  

This program is the **starting point of the game**. It does three main jobs:  

1. Opens the game window where everything will be drawn.  
2. Creates the game object that knows all the rules of Tic Tac Toe.  
3. Starts the game loop, which keeps running until the window is closed.  

Inside the loop:  
- `ProcessEvents()` listens for mouse clicks and key presses.  
- `Update()` checks the game logic (whose turn, win or draw).  
- `Clear()` wipes the screen with a background color.  
- `Draw()` redraws the board, players, and effects.  
- `Refresh(60)` shows the new frame 60 times per second.  

---
### IPlayer.cs – Player Interface

```csharp
namespace TicTacToe
{
    public interface IPlayer
    {
        char Symbol { get; }
        void MakeMove(Board board);
    }
}
```
This interface defines the **contract for all players**.  

- `char Symbol { get; }` means every player must have a symbol (`X` or `O`).  
- `void MakeMove(Board board);` means every player must know how to make a move on the board.  

By using an **interface**, the game does not need to know whether the player is a **human** or an **AI**.  
Both must follow the same rules, which makes the design flexible and easy to extend.  

---

### HumanPlayer.cs – Human Player

```csharp
using SplashKitSDK;

namespace TicTacToe
{
    public class HumanPlayer : IPlayer
    {
        public char Symbol { get; }
        private bool _clicked;

        public HumanPlayer(char symbol)
        {
            Symbol = symbol;
        }

        public void MakeMove(Board board)
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton) && !_clicked)
            {
                if (SplashKit.MouseY() < 600)
                {
                    int row = (int)(SplashKit.MouseY() / board.CellSize);
                    int col = (int)(SplashKit.MouseX() / board.CellSize);
                    if (board.PlaceSymbol(row, col, Symbol))
                    {
                        _clicked = true;
                    }
                }
            }

            if (!SplashKit.MouseDown(MouseButton.LeftButton))
            {
                _clicked = false;
            }
        }
    }
}
```

This class represents a **human player** in the game.  

Think of it like this: when you’re playing Tic Tac Toe on paper, you take a pen and draw an `X` or `O` in a square.  
Here, the **mouse click** is your pen.  

- `Symbol` is your chosen mark (either `X` or `O`).  
- `_clicked` is like a safety lock — it makes sure one mouse press doesn’t accidentally place two marks.  
- `MouseClicked(MouseButton.LeftButton)` listens for you to press the left mouse button, just like saying “I want to mark this spot.”  
- `MouseX()` and `MouseY()` check *where* you clicked on the grid. For example, if you click near the top-left, it figures out you mean row 0, column 0.  
- `board.PlaceSymbol(row, col, Symbol)` is like writing your mark in that square.  

**Example:**  
If you click in the middle of the board, the program calculates the row and column (row 1, column 1).  
Then it places your `X` or `O` right in the center square.  

In short: this code lets you **play naturally by clicking on the board**, just like drawing on a real Tic Tac Toe grid.  


---

### AIPlayer.cs – AI Player with Minimax

```csharp
using System;

namespace TicTacToe
{
    public class AIPlayer : IPlayer
    {
        public char Symbol { get; }

        public AIPlayer(char symbol)
        {
            Symbol = symbol;
        }

        public void MakeMove(Board board)
        {
            int bestScore = int.MinValue;
            (int row, int col) bestMove = (-1, -1);

            foreach (var (r, c) in board.EmptyCells())
            {
                board.PlaceSymbol(r, c, Symbol);
                int score = Minimax(board, 0, false);
                board.UndoMove(r, c);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = (r, c);
                }
            }

            if (bestMove != (-1, -1))
            {
                board.PlaceSymbol(bestMove.row, bestMove.col, Symbol);
            }
        }

        private int Minimax(Board board, int depth, bool isMaximizing)
        {
            char opponent = (Symbol == 'O') ? 'X' : 'O';

            if (board.HasWinner(Symbol)) return 10 - depth;
            if (board.HasWinner(opponent)) return depth - 10;
            if (board.IsDraw()) return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                foreach (var (r, c) in board.EmptyCells())
                {
                    board.PlaceSymbol(r, c, Symbol);
                    int score = Minimax(board, depth + 1, false);
                    board.UndoMove(r, c);
                    bestScore = Math.Max(bestScore, score);
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                foreach (var (r, c) in board.EmptyCells())
                {
                    board.PlaceSymbol(r, c, opponent);
                    int score = Minimax(board, depth + 1, true);
                    board.UndoMove(r, c);
                    bestScore = Math.Min(bestScore, score);
                }
                return bestScore;
            }
        }
    }
}
```
This class represents the **AI player**, which always plays perfectly using the **Minimax algorithm**.  

---

### What is the Minimax Algorithm?  
The **Minimax algorithm** is a decision-making process used in two-player games like Tic Tac Toe, Chess, or Connect Four.  
It works by **simulating every possible move** and assuming both players will always play their best.  

- The **AI tries to maximize its chances of winning** (the "max" part).  
- The **opponent tries to minimize the AI’s chances** (the "min" part).  

That’s why it’s called *Minimax*: one side tries to maximize the score, while the other tries to minimize it.  
---

### How It Works ?  
Before the AI places its symbol, it:  

1. Tries out every possible move.  
2. For each move, it asks:  
   - *“If I go here, what will the opponent do next?”*  
   - *“If the opponent goes there, what should I do after that?”*  
3. It keeps repeating this until the game ends (win, lose, or draw).  
4. Each outcome gets a score:  
   - Win - **+10** (good).  
   - Loss - **-10** (bad).  
   - Draw - **0** (neutral).  
5. Finally, the AI chooses the move with the **highest score**, guaranteeing the best possible result.  

---

### Board.cs – Game Board

```csharp
using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace TicTacToe
{
    public class Board
    {
        private char[,] _grid = new char[3, 3];
        public int CellSize { get; } = 200;

        public Board()
        {
            Reset();
        }

        public void Reset()
        {
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    _grid[r, c] = ' ';
        }

        public bool PlaceSymbol(int row, int col, char symbol)
        {
            if (row < 0 || row > 2 || col < 0 || col > 2) return false;
            if (_grid[row, col] != ' ') return false;
            _grid[row, col] = symbol;
            return true;
        }

        public void UndoMove(int row, int col)
        {
            _grid[row, col] = ' ';
        }

        public List<(int, int)> EmptyCells()
        {
            var list = new List<(int, int)>();
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (_grid[r, c] == ' ') list.Add((r, c));
            return list;
        }

        public bool HasWinner(char symbol)
        {
            for (int r = 0; r < 3; r++)
                if (_grid[r, 0] == symbol && _grid[r, 1] == symbol && _grid[r, 2] == symbol)
                    return true;

            for (int c = 0; c < 3; c++)
                if (_grid[0, c] == symbol && _grid[1, c] == symbol && _grid[2, c] == symbol)
                    return true;

            if (_grid[0, 0] == symbol && _grid[1, 1] == symbol && _grid[2, 2] == symbol)
                return true;

            if (_grid[0, 2] == symbol && _grid[1, 1] == symbol && _grid[2, 0] == symbol)
                return true;

            return false;
        }

        public bool IsDraw()
        {
            foreach (var cell in _grid)
                if (cell == ' ') return false;
            return true;
        }

        public void Draw(Window w)
        {
            for (int i = 1; i < 3; i++)
            {
                w.DrawLine(Color.White, i * CellSize, 0, i * CellSize, 600);
                w.DrawLine(Color.White, 0, i * CellSize, 600, i * CellSize);
            }

            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    int x = c * CellSize;
                    int y = r * CellSize;
                    if (_grid[r, c] == 'X')
                        w.DrawText("X", Color.Green, "Arial", 40, x + 70, y + 70);
                    else if (_grid[r, c] == 'O')
                        w.DrawText("O", Color.Red, "Arial", 40, x + 70, y + 70);
                }
        }
    }
}
```
This class represents the **game board**.  
Think of it like the paper grid you’d normally draw for Tic Tac Toe, but here it’s digital.  

- `_grid` is a 3×3 box that stores the moves (`X`, `O`, or empty).  
- `CellSize` is how big each square looks on the screen (200 pixels).  

**Key methods:**  

- `Reset()` - clears the board (all cells go back to empty).  
- `PlaceSymbol(row, col, symbol)` - tries to put `X` or `O` into a cell.  
   - It returns `false` if the spot is already taken.  
- `UndoMove(row, col)` - erases a move (used by the AI when it “tests” moves).  
- `EmptyCells()` - gives a list of all the blank spots left.  
- `HasWinner(symbol)` - checks if a player (`X` or `O`) has won by filling a row, column, or diagonal.  
- `IsDraw()` - checks if the board is full and nobody won.  
- `Draw(Window w)` - actually draws the lines, `X`s, and `O`s onto the game window.  

---
### Game.cs – Main Game Controller

```csharp
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
```

### Game.cs – Main Game Controller

The `Game` class is like the **director of a play**.  
It doesn’t act on stage itself (that’s the Board and Players), but it decides when the lights go on,  when the actors move, and how the story flows from beginning to end.

---

#### What Does `Game.cs` Do?

1. **The Menu Screen**
   - When you first start the game, you don’t jump straight into Tic Tac Toe.  
   - The menu appears with two glowing buttons:
     - *Play with AI* - the computer becomes your opponent.
     - *Play with a Friend* - you and another human take turns.  
   - The buttons light up when your mouse hovers over them, thanks to `_buttonHover`.

2. **The Name Popup**
   - Once you choose who to play with, the game asks for names.  
   - If you picked AI, it just asks for your name.  
   - If you picked Friend, it first asks for Player X’s name, then Player O’s.  
   - You can type letters, use Backspace to delete, and press ENTER to confirm.

3. **The Gameplay Loop**
   - After setup, the board appears.  
   - The game keeps track of whose turn it is with `_current`.  
   - Every time someone makes a move:
     - The board checks if that move caused a **win**.
       - If yes, the game stops and shows the winner.
     - If not, it checks if the board is full (a **draw**).
       - If it’s a draw, the game stops and says so.
     - Otherwise, it switches turns to the other player.

4. **Game Over Screen**
   - When the game ends (win or draw), a big glowing **“GAME OVER”** message appears.  
   - It also shows who won (with their name) or if it’s a draw.  
   - A *NEW GAME* button glows at the bottom. Clicking it resets the board and takes you back to the menu.

5. **Animations and Effects**
   - `_time` is a timer that keeps increasing.  
   - It’s used in creative ways:
     - To make the menu title and buttons glow and pulse.
     - To draw an animated background with moving stars.
     - To give text (like “GAME OVER” or “Player X’s Turn”) a glowing, lively effect.
   - These touches make the game feel polished, not just functional.

---

#### Why This Class is Important

Without the `Game` class, the other parts (Board, HumanPlayer, AIPlayer) wouldn’t know when to act.  
It’s the **coordinator**:
- The **Board** knows the rules.
- The **Players** know how to make moves.
- But the **Game** decides *when* the board should check for winners, *when* to switch turns, and *when* to stop everything.

In other words, `Game.cs` turns your code from a **pile of pieces** into a **real playable game**.

## Conclusion

By following this tutorial, we have taken the classic Tic Tac Toe game and transformed it into a 
modern, interactive application with polished graphics and an unbeatable AI opponent.  

Along the way, we learned how to:

- Set up and structure a SplashKit project in C#.
- Apply **Object-Oriented Programming principles**:
  - Abstraction with interfaces (`IPlayer`).
  - Encapsulation with the `Board` managing its own state.
  - Polymorphism by letting Human and AI players share the same interface.
  - Inheritance where shared behavior is organized logically.
- Implement the **Minimax algorithm**, ensuring the AI always plays optimally.
- Manage a **game loop** that constantly listens, updates, and redraws.
- Add **animations and effects** that make the game engaging and professional.
- Use debugging and testing to refine the flow and polish the experience.

This project demonstrates more than just writing code.  
It shows how to combine algorithms, user interaction, and visual design into a complete piece of software.  

---

### What’s Next?

Now that the core game is finished, here are a few ideas to extend it further:

- Add sound effects when placing X or O.
- Highlight the winning line with animated colors.
- Keep track of player scores across multiple rounds.
- Create different difficulty levels for the AI.
- Package it so others can download and play.

---

### Final Thought

The goal of this task was to show **creativity, technical skill, and polish**.  
From menus and animations to AI decision-making, this enhanced Tic Tac Toe game is a 
small but powerful demonstration of how coding conventions, algorithms, and design 
come together in a real-world project.

---

## Demo Video of the Game

[![Watch the demo](https://img.youtube.com/vi/rdsFkJnbGGQ/0.jpg)](https://youtu.be/rdsFkJnbGGQ)
