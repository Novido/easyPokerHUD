using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Win32Interop.WinHandles;

namespace easyPokerHUD
{
    internal class OverlayFrame : Form
    {
        protected RECT rect;
        protected RECT oldrect = new RECT();
        protected IntPtr handle;
        protected string tableName;
        protected string tableWindowName;
        protected System.Windows.Forms.Timer overlayFrameUpdateTimer = new System.Windows.Forms.Timer();

        /// <summary>
        /// Structure needed for defining a window size
        /// </summary>
        protected struct RECT
        {
            public int left, top, right, bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        protected static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        protected static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        protected static extern int SetWindowLong(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        protected static extern IntPtr FindWindow(string lp1, string lp2);

        [DllImport("user32.dll", SetLastError = true)]
        protected static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        /// <summary>
        /// Starts the update timer for the overlay
        /// </summary>
        protected void StartOverlayFrameUpdateTimer()
        {
            overlayFrameUpdateTimer.Interval = 250;
            overlayFrameUpdateTimer.Tick += UpdateOverlayFrame;
            overlayFrameUpdateTimer.Enabled = true;
        }

        /// <summary>
        /// Updates the size, transparency and position of the form
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eve"></param>
        protected void UpdateOverlayFrame(object obj, EventArgs eve)
        {
            GetWindowRect(handle, out rect);
            if (!rect.Equals(oldrect))
            {
                oldrect = rect;
                SetTransparency();
                SetSize();
                SetPosition();
            }
        }

        /// <summary>
        /// Makes the form window transparent
        /// </summary>
        protected void SetTransparency()
        {
            BackColor = Color.White;
            TransparencyKey = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            int initialStyle = GetWindowLong(Handle, -20);
            SetWindowLong(Handle, -20, initialStyle | 0x80000 | 0x20);
        }

        /// <summary>
        /// Resizes the form to the size of the table-window
        /// </summary>
        protected void SetSize()
        {
            Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
            Top = rect.top;
            Left = rect.left;
        }

        /// <summary>
        /// Positions the form above the table-window
        /// </summary>
        protected void SetPosition()
        {
            if (rect.left == -32000)
            {
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
                Location = new Point(rect.left, rect.top);
            }
        }

        /// <summary>
        /// Sets the PokerStars table window as the owner of this window
        /// </summary>
        protected void SetOwner()
        {
            if (GetTableWindowName() == null)
            {
                Close();
                Thread.CurrentThread.Abort();
            }
            handle = FindWindow(null, GetTableWindowName());
            Win32WindowWrapper wrapper = new Win32WindowWrapper(handle);
            SetWindowLong(new HandleRef(this, Handle), -8, new HandleRef(wrapper, handle));
        }

        /// <summary>
        /// Gets the window name of specified table
        /// </summary>
        /// <returns></returns>
        protected string GetTableWindowName()
        {
            try
            {
                var window = TopLevelWindowUtils.FindWindow(wh => wh.GetWindowText().Contains(tableName) && !wh.GetWindowText().Contains("Lobby"));
                return window.GetWindowText();
            }
            catch
            {
                return null;
            }
        }
    }
}
