using System;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public class ScreenShareEventArgs : EventArgs
    {
        public byte[] Capture { get; set; }
    }
}
