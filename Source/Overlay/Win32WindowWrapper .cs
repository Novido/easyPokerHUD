using System;
using System.Windows.Forms;

namespace easyPokerHUD
{
    /// <summary>
    /// Extends my handle, so I can use additional functions
    /// </summary>
    public class Win32WindowWrapper : IWin32Window
    {
        private readonly IntPtr _handle;

        public Win32WindowWrapper(IntPtr handle)
        {
            _handle = handle;
        }

        public IntPtr Handle => _handle;
    }
}
