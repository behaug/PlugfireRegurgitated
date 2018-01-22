using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using SharpDX.IO;
using SharpDX.MediaFoundation;
using SharpDX.XAudio2;

namespace Plugfire17
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            try
            {
                D2d();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private static XAudio2 xaudio2;
        private static MasteringVoice masteringVoice;

        private static void InitializeXAudio2()
        {
            // This is mandatory when using any of SharpDX.MediaFoundation classes
            MediaManager.Startup();

            // Starts The XAudio2 engine
            xaudio2 = new XAudio2();
            xaudio2.StartEngine();
            masteringVoice = new MasteringVoice(xaudio2);
        }

        static void D2d()
        {
            var plugfire = new Plugfire();

            InitializeXAudio2();
            //var fileStream = new NativeFileStream(@"D:\plug17.mp3", NativeFileMode.Open, NativeFileAccess.Read);
            var embeddedStream = Assembly.GetEntryAssembly().GetManifestResourceStream(typeof(Program), "music.mp3");
            var audioPlayer = new AudioPlayer(xaudio2, embeddedStream);
            audioPlayer.Play();

            var form = new RenderForm("SharpDX - MiniCube Direct3D11 Sample");
            form.Width = Plugfire.WIDTH*4;
            form.Height = Plugfire.HEIGHT*4;
            Device device;
            SwapChain swapChain;
            var d2dFactory = new SharpDX.Direct2D1.Factory();

            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = false,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, new SharpDX.Direct3D.FeatureLevel[] { SharpDX.Direct3D.FeatureLevel.Level_10_0 }, desc, out device, out swapChain);

            // Ignore all windows events
            Factory factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.F5)
                    swapChain.SetFullscreenState(true, null);
                else if (args.KeyCode == Keys.F4)
                    swapChain.SetFullscreenState(false, null);
                else if (args.KeyCode == Keys.Escape)
                    form.Close();
            };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Texture2D backBuffer = null;
            Bitmap bitmap = null;
            RenderTarget render = null;
            RenderTargetView renderView = null;

            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            //var textBuffer = new Texture2D(device, new Texture2DDescription {Height = 100, Width = 100, Format = Format.R8G8B8A8_UNorm});
            renderView = new RenderTargetView(device, backBuffer);
            //var textView = new RenderTargetView(device, textBuffer);
            Surface surface = backBuffer.QueryInterface<Surface>();
            //Surface textSurface = textBuffer.QueryInterface<Surface>();
            render = new RenderTarget(d2dFactory, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
            //var textRender = new RenderTarget(d2dFactory, textSurface, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
            bitmap = new Bitmap(render, new Size2(Plugfire.WIDTH, Plugfire.HEIGHT), new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Ignore)));

            SharpDX.DirectWrite.Factory FactoryDWrite = new SharpDX.DirectWrite.Factory();

            var whiteTextBrush = new SolidColorBrush(render, new RawColor4(1, 1, 1, 1));
            var debugTextFormat = new TextFormat(FactoryDWrite, "Arial", 10);
            var titleTextBrush = new SolidColorBrush(render, new RawColor4(1, 1, 0.3f, 1));
            var foamTextFormat = new TextFormat(FactoryDWrite, "Arial", FontWeight.Bold, FontStyle.Normal, 100);
            var presentsTextFormat = new TextFormat(FactoryDWrite, "Arial", FontWeight.Bold, FontStyle.Normal, 60);
            var titleTextFormat = new TextFormat(FactoryDWrite, "Arial", FontWeight.Bold, FontStyle.Normal, 80);

            // Main loop
            RenderLoop.Run(form, () =>
            {
                plugfire.Frame(stopwatch.ElapsedMilliseconds, audioPlayer.Position.TotalMilliseconds);
                bitmap.CopyFromMemory(plugfire.GetRgbImage(), Plugfire.WIDTH*4);

                render.BeginDraw();
                //render.Clear(Color.Wheat);
                //solidColorBrush.Color = new Color4(1, 1, 1, (float)Math.Abs(Math.Cos(stopwatch.ElapsedMilliseconds * .001)));
                //d2dRenderTarget.FillGeometry(rectangleGeometry, solidColorBrush, null);
                render.DrawBitmap(bitmap, new RawRectangleF(0,0,render.Size.Width, render.Size.Height), 1.0f, BitmapInterpolationMode.Linear);
                //render.DrawText(audioPlayer.Position.ToString(), debugTextFormat, new RawRectangleF(10,10,100,100), whiteTextBrush);

                //if (plugfire.Mode == 0)
                //{
                //    var fade = (float)Math.Min((stopwatch.Elapsed.TotalMilliseconds - 500) / 2000, 1);
                //    titleTextBrush.Color = new RawColor4(fade, fade, fade * 0.3f, 1);
                //    render.DrawText("FOAM", foamTextFormat, new RawRectangleF(500, 100, 2000, 2000), titleTextBrush);
                //    render.DrawText("presents", presentsTextFormat, new RawRectangleF(520, 200, 2000, 2000), titleTextBrush);
                //    render.DrawText("Plugfire Regurgitated", titleTextFormat, new RawRectangleF(250, 400, 2000, 2000), titleTextBrush);
                //}

                render.EndDraw();

                swapChain.Present(0, PresentFlags.None);
            });

            // Release all resources
            renderView.Dispose();
            backBuffer.Dispose();
            device.ImmediateContext.ClearState();
            device.ImmediateContext.Flush();
            device.Dispose();
            device.Dispose();
            swapChain.Dispose();
            factory.Dispose();
        }
    }
}
