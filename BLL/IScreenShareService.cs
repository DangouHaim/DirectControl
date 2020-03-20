using System;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public interface IScreenShareService
    {
        event EventHandler<ScreenShareEventArgs> OnFrame;
        void StartShare(IScreenShotService screenShotService);
        void StartReceive();
        void Stop();
    }
}
