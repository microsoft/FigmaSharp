using System;
using System.Runtime.InteropServices;
using AppKit;
using CoreGraphics;

namespace FigmaSharp.Views.Cocoa.Views
{
    public class ImageView : NSImageView
    {
        NSImage orig;
        public override NSImage Image
        {
            get => base.Image;
            set
            {
                orig = value;
                //base.Image = value;
                Refresh();
            }
        }

        public override void ViewDidChangeEffectiveAppearance()
        {
            base.ViewDidChangeEffectiveAppearance();
            Refresh();
        }

        void Refresh()
        {
            if (orig == null)
                return;

            var cgimg = ImageHelpers.ReplacePixelColor(orig.CGImage, NSColor.Red, NSColor.ControlAccentColor, 100);
            base.Image = new NSImage(cgimg, orig.Size);
        }
    }

    public static class ImageHelpers
    {
        static bool IsRange(byte color, byte expected, int range)
        {
            return Math.Abs(color - expected) < range;
        }

        public static CGImage ReplacePixelColor(CGImage cgImage, NSColor chroma, NSColor change, int range = 10)
        {
            var chromaColor = chroma.ColorWithAlphaComponent(1);
            byte chromaR = (byte)(chromaColor.RedComponent * 255f);
            byte chromaG = (byte)(chromaColor.GreenComponent * 255f);
            byte chromaB = (byte)(chromaColor.BlueComponent * 255f);

            var resultColor = change.ColorWithAlphaComponent(1);
            byte resultR = (byte)(resultColor.RedComponent * 255f);
            byte resultG = (byte)(resultColor.GreenComponent * 255f);
            byte resultB = (byte)(resultColor.BlueComponent * 255f);
            byte resultA = (byte)(resultColor.AlphaComponent * 255f);

            var bytesPerPixel = 4;
            var bitsPerComponent = 8;
            var bytesPerUInt32 = sizeof(UInt32) / sizeof(byte);

            var width = cgImage.Width;
            var height = cgImage.Height;

            var bytesPerRow = bytesPerPixel * cgImage.Width;
            var numOfBytes = cgImage.Height * cgImage.Width * bytesPerUInt32;

            CGImage newCGImage = null;
            IntPtr pixelPtr = IntPtr.Zero;
            try
            {
                pixelPtr = Marshal.AllocHGlobal((int)numOfBytes);

                using (var colorSpace = CGColorSpace.CreateDeviceRGB())
                {

                    using (var context = new CGBitmapContext(pixelPtr, width, height, bitsPerComponent, bytesPerRow, colorSpace, CGImageAlphaInfo.PremultipliedLast))
                    {
                        context.DrawImage(new CGRect(0, 0, width, height), cgImage);
                        unsafe
                        {
                            var currentPixel = (byte*)pixelPtr.ToPointer();
                            for (int i = 0; i < height; i++)
                            {
                                for (int j = 0; j < width; j++)
                                {

                                    // RGBA8888 pixel format
                                    if (IsRange(*currentPixel, chromaR, range) &&
                                        IsRange(*(currentPixel + 1), chromaG, range) &&
                                        IsRange(*(currentPixel + 2), chromaB, range))
                                    {
                                        //Console.WriteLine("R:" + *currentPixel + "G:" + *(currentPixel + 1) + "B:" + *(currentPixel + 2));
                                        *currentPixel = resultR;
                                        *(currentPixel + 1) = resultG;
                                        *(currentPixel + 2) = resultB; //B
                                        *(currentPixel + 3) = resultA;

                                        //*currentPixel = 0;
                                        //*(currentPixel + 1) = 0;
                                        //*(currentPixel + 2) = 0; //B
                                        //*(currentPixel + 3) = 0;
                                    }
                                    currentPixel += 4;
                                }
                            }
                        }
                        newCGImage = context.ToImage();
                    }

                }
            }
            finally
            {
                if (pixelPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(pixelPtr);
            }
            return newCGImage;
        }
    }
}
