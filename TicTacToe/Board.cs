using System.Collections.Generic;
using SplashKitSDK;
using System;

namespace TicTacToe
{
    public class Board
    {
        private char[,] _grid = new char[3, 3];
        public int CellSize { get; } = 200;
        public (int, int, int, int)? WinningLine { get; private set; }
        private double _time = 0;
        private double[,] _cellAnimations = new double[3, 3];
        private bool[,] _cellHover = new bool[3, 3];

        public Board()
        {
            Reset();
        }

        public void Reset()
        {
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    _grid[r, c] = ' ';
                    _cellAnimations[r, c] = 0;
                    _cellHover[r, c] = false;
                }
            WinningLine = null;
        }

        public bool PlaceSymbol(int row, int col, char symbol)
        {
            if (row < 0 || row > 2 || col < 0 || col > 2) return false;
            if (_grid[row, col] != ' ') return false;
            _grid[row, col] = symbol;
            _cellAnimations[row, col] = 1.0; // Start animation
            return true;
        }

        public void UndoMove(int row, int col)
        {
            _grid[row, col] = ' ';
            _cellAnimations[row, col] = 0;
        }

        public char GetCell(int row, int col)
        {
            return _grid[row, col];
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
            WinningLine = null;

            for (int r = 0; r < 3; r++)
                if (_grid[r, 0] == symbol && _grid[r, 1] == symbol && _grid[r, 2] == symbol)
                {
                    WinningLine = (0, r * CellSize + CellSize / 2, 600, r * CellSize + CellSize / 2);
                    return true;
                }

            for (int c = 0; c < 3; c++)
                if (_grid[0, c] == symbol && _grid[1, c] == symbol && _grid[2, c] == symbol)
                {
                    WinningLine = (c * CellSize + CellSize / 2, 0, c * CellSize + CellSize / 2, 600);
                    return true;
                }

            if (_grid[0, 0] == symbol && _grid[1, 1] == symbol && _grid[2, 2] == symbol)
            {
                WinningLine = (0, 0, 600, 600);
                return true;
            }

            if (_grid[0, 2] == symbol && _grid[1, 1] == symbol && _grid[2, 0] == symbol)
            {
                WinningLine = (600, 0, 0, 600);
                return true;
            }

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
            _time += 0.016;
            UpdateHoverStates();
            UpdateAnimations();

            // Enhanced grid drawing
            DrawEnhancedGrid(w);
            DrawCells(w);
            DrawWinningLine(w);
        }

        private void UpdateHoverStates()
        {
            int mx = (int)SplashKit.MouseX();
            int my = (int)SplashKit.MouseY();

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    int x = c * CellSize;
                    int y = r * CellSize;
                    
                    _cellHover[r, c] = (my < 600 && mx >= x && mx < x + CellSize && 
                                       my >= y && my < y + CellSize && _grid[r, c] == ' ');
                }
            }
        }

        private void UpdateAnimations()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (_cellAnimations[r, c] > 0)
                    {
                        _cellAnimations[r, c] -= 0.02;
                        if (_cellAnimations[r, c] < 0) _cellAnimations[r, c] = 0;
                    }
                }
            }
        }

        private void DrawEnhancedGrid(Window w)
        {
            // Draw background with subtle pattern
            Color bgColor = SplashKit.RGBAColor(30, 40, 60, 200);
            w.FillRectangle(bgColor, 0, 0, 600, 600);

            // Draw grid lines with glow effect
            Color gridColor = SplashKit.RGBColor(100, 149, 237);
            Color glowColor = SplashKit.RGBAColor(100, 149, 237, 100);

            // Vertical lines
            for (int i = 1; i < 3; i++)
            {
                int x = i * CellSize;
                // Glow effect
                w.FillRectangle(glowColor, x - 3, 0, 6, 600);
                // Main line
                w.FillRectangle(gridColor, x - 1, 0, 2, 600);
            }

            // Horizontal lines
            for (int i = 1; i < 3; i++)
            {
                int y = i * CellSize;
                // Glow effect
                w.FillRectangle(glowColor, 0, y - 3, 600, 6);
                // Main line
                w.FillRectangle(gridColor, 0, y - 1, 600, 2);
            }
        }

        private void DrawCells(Window w)
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    int x = c * CellSize;
                    int y = r * CellSize;
                    int cx = x + CellSize / 2;
                    int cy = y + CellSize / 2;

                    // Draw hover effect for empty cells
                    if (_cellHover[r, c])
                    {
                        double pulse = 0.3 + 0.2 * Math.Sin(_time * 8);
                        Color hoverColor = SplashKit.RGBAColor(255, 255, 255, (byte)(pulse * 100));
                        w.FillRectangle(hoverColor, x + 10, y + 10, CellSize - 20, CellSize - 20);
                    }

                    // Draw symbols with enhanced effects
                    if (_grid[r, c] == 'X')
                    {
                        DrawEnhancedX(w, cx, cy, r, c);
                    }
                    else if (_grid[r, c] == 'O')
                    {
                        DrawEnhancedO(w, cx, cy, r, c);
                    }
                }
            }
        }

        private void DrawEnhancedX(Window w, int cx, int cy, int row, int col)
        {
            // Animation scale effect
            double scale = 1.0;
            if (_cellAnimations[row, col] > 0)
            {
                scale = 1.0 + _cellAnimations[row, col] * 0.5;
            }

            // Pulsing effect
            double pulse = 0.8 + 0.2 * Math.Sin(_time * 3 + row + col);
            
            Color xColor = SplashKit.RGBColor((int)(34 * pulse), (int)(139 * pulse), (int)(34 * pulse));
            Color shadowColor = SplashKit.RGBAColor(0, 0, 0, 150);
            
            int size = (int)(80 * scale);
            
            // Draw shadow
            w.DrawLine(shadowColor, cx - size + 2, cy - size + 2, cx + size + 2, cy + size + 2, SplashKit.OptionLineWidth(8));
            w.DrawLine(shadowColor, cx + size + 2, cy - size + 2, cx - size + 2, cy + size + 2, SplashKit.OptionLineWidth(8));
            
            // Draw main X with glow
            Color glowColor = SplashKit.RGBAColor(xColor.R, xColor.G, xColor.B, 100);
            w.DrawLine(glowColor, cx - size - 2, cy - size - 2, cx + size + 2, cy + size + 2, SplashKit.OptionLineWidth(12));
            w.DrawLine(glowColor, cx + size + 2, cy - size - 2, cx - size - 2, cy + size + 2, SplashKit.OptionLineWidth(12));
            
            w.DrawLine(xColor, cx - size, cy - size, cx + size, cy + size, SplashKit.OptionLineWidth(8));
            w.DrawLine(xColor, cx + size, cy - size, cx - size, cy + size, SplashKit.OptionLineWidth(8));
        }

        private void DrawEnhancedO(Window w, int cx, int cy, int row, int col)
        {
            // Animation scale effect
            double scale = 1.0;
            if (_cellAnimations[row, col] > 0)
            {
                scale = 1.0 + _cellAnimations[row, col] * 0.5;
            }

            // Pulsing effect
            double pulse = 0.8 + 0.2 * Math.Sin(_time * 3 + row + col);
            
            Color oColor = SplashKit.RGBColor((int)(220 * pulse), (int)(20 * pulse), (int)(60 * pulse));
            Color shadowColor = SplashKit.RGBAColor(0, 0, 0, 150);
            
            int radius = (int)(80 * scale);
            
            // Draw shadow
            w.DrawCircle(shadowColor, cx + 2, cy + 2, radius, SplashKit.OptionLineWidth(8));
            
            // Draw main O with glow
            Color glowColor = SplashKit.RGBAColor(oColor.R, oColor.G, oColor.B, 100);
            w.DrawCircle(glowColor, cx, cy, radius + 2, SplashKit.OptionLineWidth(12));
            w.DrawCircle(oColor, cx, cy, radius, SplashKit.OptionLineWidth(8));
        }

        private void DrawWinningLine(Window w)
        {
            if (WinningLine != null)
            {
                var (x1, y1, x2, y2) = WinningLine.Value;
                
                // Animated winning line with rainbow effect
                double colorShift = _time * 5;
                Color lineColor = SplashKit.RGBColor(
                    (int)(128 + 127 * Math.Sin(colorShift)),
                    (int)(128 + 127 * Math.Sin(colorShift + 2)),
                    (int)(128 + 127 * Math.Sin(colorShift + 4))
                );
                
                // Glow effect
                Color glowColor = SplashKit.RGBAColor(lineColor.R, lineColor.G, lineColor.B, 100);
                w.DrawLine(glowColor, x1, y1, x2, y2, SplashKit.OptionLineWidth(15));
                
                // Main line
                w.DrawLine(lineColor, x1, y1, x2, y2, SplashKit.OptionLineWidth(8));
                
                // Sparkling effect along the line
                DrawSparkles(w, x1, y1, x2, y2);
            }
        }

        private void DrawSparkles(Window w, int x1, int y1, int x2, int y2)
        {
            int sparkles = 8;
            for (int i = 0; i <= sparkles; i++)
            {
                double t = (double)i / sparkles;
                int x = (int)(x1 + t * (x2 - x1));
                int y = (int)(y1 + t * (y2 - y1));
                
                double sparkleTime = _time * 4 + i * 0.5;
                double brightness = (Math.Sin(sparkleTime) + 1) * 0.5;
                
                if (brightness > 0.7)
                {
                    Color sparkleColor = SplashKit.RGBAColor(255, 255, 255, (byte)(brightness * 255));
                    w.FillCircle(sparkleColor, x, y, 3);
                }
            }
        }
    }
}