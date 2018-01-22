using System;
using System.Collections.Generic;

namespace Plugfire17
{
    public class Plugfire
    {
        public const int WIDTH = 320;
        public const int HEIGHT = 200;
        public byte[] _bBack = new byte[WIDTH * HEIGHT];
        public byte[] _bFront = new byte[WIDTH * HEIGHT];
        public byte[] _bOverlay = new byte[WIDTH * HEIGHT];
        public byte[] _bBumpmap = new byte[WIDTH * HEIGHT];
        public uint[] _rgbBuffer = new uint[WIDTH * HEIGHT];
        private List<Dot> _dots = new List<Dot>();
        private int _numDots = 1;
        private Random _ran = new Random();
        private double _elapsed = 0;
        private double _t = 0;
        private double _lastT = 0;
        public int Mode = -1;
        private int _jumpToMode = -1;
        private TimeSpan[] _modes = new TimeSpan[] {
            TimeSpan.FromSeconds(0), // flame
            TimeSpan.FromSeconds(7.5), // single dot
            TimeSpan.FromSeconds(15.5), // many dots
            TimeSpan.FromSeconds(23), // sphere
            TimeSpan.FromSeconds(30), // sphere pulsate
            TimeSpan.FromSeconds(37.5) // scroller
        };
        public uint[] Palette = new uint[256];
        private bool _enableBump = false;

        #region FONT 8x7
        private byte[][] _font = new[]
        {
            // A
            new byte[]{
                0,0,0,1,1,0,0,0,
                0,0,1,0,0,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,1,1,1,1,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
            },
            // B
            new byte[]{
                0,1,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,1,1,1,1,0,0,
            },
            // C
            new byte[]{
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
            },
            // D
            new byte[]{
                0,1,1,1,1,0,0,0,
                0,1,0,0,0,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,1,0,0,
                0,1,1,1,1,0,0,0,
            },
            // E
            new byte[]{
                0,1,1,1,1,1,1,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,1,1,1,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
            },
            // F
            new byte[]{
                0,1,1,1,1,1,1,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
            },
            // G
            new byte[]{
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,0,0,
                0,1,0,1,1,1,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
            },
            // H
            new byte[]{
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,1,1,1,1,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
            },
            // I
            new byte[]{
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
            },
            // J
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
            // K
            new byte[]{
                0,1,0,0,0,1,0,0,
                0,1,0,0,1,0,0,0,
                0,1,0,1,0,0,0,0,
                0,1,1,0,0,0,0,0,
                0,1,0,1,0,0,0,0,
                0,1,0,0,1,0,0,0,
                0,1,0,0,0,1,0,0,
            },
            // L
            new byte[]{
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
            },
            // M
            new byte[]{
                0,1,0,0,0,0,1,0,
                0,1,1,0,0,1,1,0,
                0,1,0,1,1,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
            },
            // N
            new byte[]{
                0,1,0,0,0,0,1,0,
                0,1,1,0,0,0,1,0,
                0,1,0,1,0,0,1,0,
                0,1,0,0,1,0,1,0,
                0,1,0,0,0,1,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
            },
            // O
            new byte[]{
                0,0,0,1,1,0,0,0,
                0,0,1,0,0,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,0,0,1,0,0,
                0,0,0,1,1,0,0,0,
            },
            // P
            new byte[]{
                0,1,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,1,1,1,1,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,
            },
            // Q
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
            // R
            new byte[]{
                0,1,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,1,1,1,1,0,0,
                0,1,0,0,1,0,0,0,
                0,1,0,0,0,1,0,0,
                0,1,0,0,0,0,1,0,
            },
            // S
            new byte[]{
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,0,0,
                0,0,1,1,1,1,0,0,
                0,0,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
            },
            // T
            new byte[]{
                1,1,1,1,1,1,1,0,
                0,0,0,1,0,0,0,0,
                0,0,0,1,0,0,0,0,
                0,0,0,1,0,0,0,0,
                0,0,0,1,0,0,0,0,
                0,0,0,1,0,0,0,0,
                0,0,0,1,0,0,0,0,
            },
            // U
            new byte[]{
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
            },
            // V
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
            // W
            new byte[]{
                1,0,0,0,0,0,1,0,
                1,0,0,0,0,0,1,0,
                1,0,0,0,0,0,1,0,
                1,0,0,1,0,0,1,0,
                1,0,1,0,1,0,1,0,
                1,1,0,0,0,1,1,0,
                1,0,0,0,0,0,1,0,
            },
            // X
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
            // Y
            new byte[]{
                0,1,0,0,0,0,0,1,
                0,0,1,0,0,0,1,0,
                0,0,0,1,0,1,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
            },
            // Z
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
            // 0
            new byte[]{
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
            },
            // 1
            new byte[]{
                0,0,0,0,1,0,0,0,
                0,0,0,1,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,1,1,1,0,0,
            },
            // 2
            new byte[]{
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,0,0,0,0,0,1,0,
                0,0,0,0,0,1,0,0,
                0,0,0,1,1,0,0,0,
                0,0,1,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
            },
            // 3
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
            // 4
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
            // 5
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
            // 6
            new byte[]{
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,0,0,
                0,1,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
            },
            // 7
            new byte[]{
                0,1,1,1,1,1,1,0,
                0,0,0,0,0,0,1,0,
                0,0,0,0,0,1,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
                0,0,0,0,1,0,0,0,
            },
            // 8
            new byte[]{
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
            },
            // 9
            new byte[]{
                0,0,1,1,1,1,0,0,
                0,1,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,1,0,
                0,0,0,0,0,0,1,0,
                0,1,0,0,0,0,1,0,
                0,0,1,1,1,1,0,0,
            },

            // space
            new byte[]{
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
            },
        };

        private double _startScroller;

        #endregion

        public Plugfire()
        {
            uint n1 = 96;
            for (uint i = 0; i < n1; i++)
                Palette[255-i] = (uint)((255) | (255 << 8) | ((255 - 255 * i / n1) << 16));

            uint n2 = 96+32;
            for (uint i = 0; i < n2; i++)
                Palette[255 - (i + n1)] = (255) | ((255 - 255 * i / n2) << 8);

            uint n3 = 32;
            for (uint i = 0; i < n3; i++)
                Palette[255 - (i + n1 + n2)] = ((255 - 255 * i / n3));

            // ensure alpha is set
            for (uint i = 0; i < 256; i++)
                Palette[i] = Palette[i] | 0xFF000000;
        }

        public void Frame(double elapsed, double audioElapsed)
        {
            _elapsed = elapsed;
            _t = _elapsed - _lastT;
            _lastT = _elapsed;

            // Swap buffers
            var swap = _bFront;
            _bFront = _bBack;
            _bBack = swap;

            if ((Mode < _modes.Length - 1 && audioElapsed > _modes[Mode + 1].TotalMilliseconds) || Mode < _jumpToMode)
            {
                Mode++;
                Init(Mode);
            }

            Draw();
        }

        class Vector
        {
            public double X;
            public double Y;
            public double Z;

            public Vector() { }
            public Vector(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public void Rotate(double aX, double aY, double aZ)
            {
                double x, y, z;

                // aX
                x = X;
                y = Y * Math.Cos(aX) - Z * Math.Sin(aX);
                z = Z * Math.Cos(aX) + Y * Math.Sin(aX);
                X = x; Y = y; Z = z;

                // aY
                x = X * Math.Cos(aY) - Z * Math.Sin(aY);
                y = Y;
                z = Z * Math.Cos(aY) + X * Math.Sin(aY);
                X = x; Y = y; Z = z;

                // aZ
                x = X * Math.Cos(aZ) - Y * Math.Sin(aZ);
                y = Y * Math.Cos(aZ) + X * Math.Sin(aZ);
                z = Z;
                X = x; Y = y; Z = z;
            }
        }

        class Dot
        {
            public double A;
            public double S;
            public double R;
            public double F;
            public double X;
            public double Y;
            public double Fade = 1;

            public Vector Pos = new Vector();

            public void Project(double scale = 1)
            {
                var fov = 100;
                X = (Pos.X*scale) / ((Pos.Z*scale + fov) / fov) + WIDTH * 0.5;
                Y = (Pos.Y*scale) / ((Pos.Z*scale + fov) / fov) + HEIGHT * 0.5;
            }
        }

        void Init(int mode)
        {
            if (mode == 0)
            {
                DrawString("PLUGFIRE", 160 - "PLUGFIRE".Length * 8, 50, _bOverlay);
                DrawString("REGURGITATED", 160 - "REGURGITATED".Length * 8, 80, _bOverlay);
            }
            if (mode == 1)
            {
                Array.Clear(_bOverlay, 0, _bFront.Length);
                _dots.Clear();
                _dots.Add(new Dot
                {
                    R = 0.8,
                    A = 0,
                    S = 0.01,
                    F = 2.4,
                    Fade = 0
                });
            }
            if (mode == 2)
            {
                var numDots = 20;
                for (int i = 0; i < numDots; i++)
                {
                    _dots.Add(new Dot
                    {
                        R = 0.9 * (1 - _ran.NextDouble() * 0.4),
                        A = _ran.NextDouble() * Math.PI * 2,
                        S = 0.02 + _ran.NextDouble() * 0.1 / numDots,
                        F = 1 + i * 0.4 / numDots,
                        Fade = 0
                    });
                }
            }
            if (mode == 3)
            {
                _dots.Clear();
                var nRows = 10;
                var nCols = 10;
                var radius = HEIGHT * 0.3;
                for (int row = 0; row < nRows; row++)
                {
                    var fRow = (double)row / nRows + _ran.NextDouble() * 0.02;
                    for (int col = 0; col < nCols; col++)
                    {
                        var fCol = (double)col / nCols + _ran.NextDouble()*0.02;
                        var rr = radius * (1 + _ran.NextDouble()*0.4);
                        _dots.Add(new Dot
                        {
                            Pos = new Vector(
                                rr * Math.Sin(fCol * Math.PI * 2) * Math.Sin(fRow * Math.PI),
                                rr * Math.Cos(fRow * Math.PI),
                                rr * Math.Cos(fCol * Math.PI * 2) * Math.Sin(fRow * Math.PI)
                            ),
                            Fade = 0
                        });
                    }
                }
            }
            if (mode == 5)
            {
                _dots.Clear();
                _enableBump = true;
                _startScroller = _elapsed;
                Array.Clear(_bFront, 0, _bFront.Length);
                Array.Clear(_bBack, 0, _bFront.Length);
            }
        }

        private void MoveDots(int mode)
        {
            if (mode == 1 || mode == 2)
            {
                foreach (var dot in _dots)
                {
                    dot.A += dot.S;
                    dot.X = -Math.Cos(dot.A) * dot.R * WIDTH / 2.0 + WIDTH / 2.0;
                    dot.Y = -Math.Cos(dot.A * dot.F) * dot.R * HEIGHT / 2.0 + HEIGHT/ 2.0;
                    dot.Fade = Math.Min(1, dot.Fade + 0.01);
                }
            }
            if (mode == 3 || mode == 4)
            {
                var m = _t * 0.02;
                var n = _elapsed * 0.001;
                foreach (var dot in _dots)
                {
                    dot.Pos.Rotate(m * 0.07 * Math.Cos(n * 1.8), m * 0.2 * Math.Sin(n), m * 0.12 * Math.Cos(n * 0.6));
                    dot.Fade = Math.Min(1, dot.Fade + 0.01);
                    double scale = 1;
                    if (mode == 4)
                        scale = 0.8 + Math.Sin(_elapsed*0.01)*0.1;

                    dot.Project(scale);
                }
            }
        }

        private void Draw()
        {
            MoveDots(Mode);
            if (Mode == 0)
                DrawFlamingBottom();

            if (!_enableBump)
                Flame();

            DrawDots();

            if (_enableBump)
                DrawBump();
        }

        private void DrawBump()
        {
            DrawScroller();

            var lightX = Math.Floor(Math.Sin(_elapsed * 0.001) * 80 + 160);
            var lightY = Math.Floor(Math.Cos(_elapsed * 0.001) * 40 + 100);

            var lightZ = 5.0;
            var lightZ2 = lightZ*lightZ;

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    var p1 = (double)_bBumpmap[Ofs(x, y)];
                    var p2 = (double)_bBumpmap[Ofs(x - 1, y)];
                    var p3 = (double)_bBumpmap[Ofs(x, y - 1)];

                    var nX = p1 - p2;
                    var nY = p1 - p3;
                    var nLen = Math.Sqrt(nX*nX + nY*nY + lightZ2);

                    var vlX = x - lightX;
                    var vlY = y - lightY;
                    var vlLen = Math.Sqrt(vlX*vlX + vlY*vlY + lightZ2);

                    var angle = Math.Acos((nX*vlX + nY*vlY + lightZ2) / (nLen * vlLen));
                    if (angle > Math.PI/2) angle = Math.PI/2;
                    var c = (byte)(255 - (angle/(Math.PI/2))*255);
                    _bOverlay[Ofs(x, y)] = c;
                }
            }
            //_bOverlay[Ofs(lightX, lightY)] = 255;
        }

        private string[] _scrollerText =
        {
            "FOAM",
            "presents",
            "",
            "plugfire",
            "regurgitated",
            "",
            "",
            "a blast",
            "from the past",
            "",
            "",
            "concept by",
            "foam",
            "at TG96",
            "",
            "",
            "music by",
            "foam",
            "at TG98",
            "",
            "",
            "now regurgitated",
            "at TG17",
            "",
            "20 years",
            "later",
            "",
            "",
            "",
            "coded in a rush",
            "at TG17",
            "",
            "while being",
            "blasted with loud",
            "music",
            "",
            "and shouts",
            "for arne",
            "",
            "",
            "keep the",
            "secrets",
            "secret",
        };

        private void DrawScroller()
        {
            Array.Clear(_bBumpmap,0,_bBumpmap.Length);
            var pos = 200 - (int)((_elapsed - _startScroller)*30/1000);
            foreach (var str in _scrollerText)
            {
                DrawString(str, 160 - str.Length*8, pos, _bBumpmap);
                pos += 32;
            }
        }

        private void DrawString(string s, int posX, int posY, byte[] buffer = null)
        {
            foreach (var letter in s.ToUpper())
            {
                DrawLetter(letter, posX, posY, buffer);
                posX += 16;
            }
        }

        private void DrawLetter(char letter, int posX, int posY, byte[] buffer = null)
        {
            buffer = buffer ?? _bOverlay;
            int index = letter - 'A';
            if (letter >= '0' && letter <= '9')
                index = letter - '0' + ('Z' - 'A') + 1;
            if (letter == ' ')
                index = _font.Length - 1;

            var map = _font[index];
            var width = 8;
            for (int y = 0; y < 2*map.Length/width; y++)
            {
                if (posY + y < 0 || posY + y > 200) continue;
                for (int x = 0; x < 2*width; x++)
                {
                    if (posX + x < 0 || posX + x > 320) continue;
                    var c = (byte)(map[(y/2)*width + (x/2)]*0xFF);
                    if (c != 0)
                    {
                        buffer[Ofs(posX + x, posY + y)] = c;
                    }
                }
            }
        }

        private void DrawFlamingBottom()
        {
            var ran = _ran.NextDouble();
            for (int x = 0; x < WIDTH; x++)
            {
                if (x%3 == 0)
                    ran = _ran.NextDouble();

                _bBack[Ofs(x, HEIGHT - 1)] = (byte) (255 - Math.Pow(ran, 1)*255);
            }
        }

        private int CapX(double v)
        {
            return (int)Math.Min(Math.Max(v, 0), WIDTH - 1);
        }
        private int CapY(double v)
        {
            return (int)Math.Min(Math.Max(v, 0), HEIGHT - 1);
        }
        private int Ofs(double x, double y)
        {
            return (CapY(y) * WIDTH + CapX(x));
        }

        private void DrawDots()
        {
            byte color = 0xFF;
            foreach (var dot in _dots)
            {
                var col = (byte)(color*dot.Fade);
                _bFront[Ofs(dot.X, dot.Y)] = col;
                _bFront[Ofs(dot.X + 1, dot.Y)] = col;
                _bFront[Ofs(dot.X, dot.Y + 1)] = col;
                _bFront[Ofs(dot.X + 1, dot.Y + 1)] = col;
            }
        }

        private void Flame()
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    _bFront[Ofs(x, y)] = (byte)((
                        _bBack[Ofs(x, y)] +
                        _bBack[Ofs(x + 2, y)] +
                        _bBack[Ofs(x - 2, y)] +
                        _bBack[Ofs(x, y + 1)] +
                        _bBack[Ofs(x, y + 2)]) * 0.199);
                }
            }
        }

        public uint[] GetRgbImage()
        {
            for (int i = 0; i < WIDTH*HEIGHT; i++)
                _rgbBuffer[i] = Palette[Math.Min((byte)255, _bFront[i] + _bOverlay[i])];

            return _rgbBuffer;
        }
    }
}