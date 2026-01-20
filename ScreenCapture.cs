using System;
using System.Drawing;
using System.Windows.Forms;

public class ScreenCapture
{
    public Bitmap CaptureScreenWithCursor()
    {
        // Capture the screen
        Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            // Draw the cursor
            var cursorInfo = new NativeMethods.CURSORINFO();
            cursorInfo.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(cursorInfo);
            if (NativeMethods.GetCursorInfo(out cursorInfo))
            {
                if (cursorInfo.flags == NativeMethods.CURSOR_SHOWING)
                {
                    // Draw the cursor at its position
                    var cursor = new Cursor(cursorInfo.hCursor);
                    cursor.Draw(g, new Rectangle(
                        cursorInfo.ptScreenPos.x,
                        cursorInfo.ptScreenPos.y,
                        cursor.Size.Width,
                        cursor.Size.Height));
                }
            }
        }
        return bmp;
    }

    internal class NativeMethods
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public POINT ptScreenPos;
        }

        public const int CURSOR_SHOWING = 0x00000001;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CURSORINFO pci);
    }
}