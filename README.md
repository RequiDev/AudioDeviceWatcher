# AudioDeviceWatcher
Windows 10 loves to change your default audio device when updating graphics drivers or plugging in a device that has audio capabilities (controller). Because there is no way to disable this behaviour despite me setting my default device for a reason, I made this tool which will automatically set your default device back to what it was once it gets changed to a device it shouldn't be.

# Usage
Once you compiled it using Visual Studio and .NET 7, you just have to run it and it'll keep running in the background.
It automatically sets itself to be started when Windows boots.
Once your device gets changed, it'll beep twice and change it back to what it's meant to be.
