# AudioDeviceWatcher
Windows 10 loves to change your default audio device when updating graphics drivers or plugging in a device that has audio capabilities (controller). Because there is no way to disable this behaviour despite me setting my default device for a reason, I made this tool which will automatically set your default device back to what it was once it gets changed to a device it shouldn't be.

# Requirements
* Windows 10 or higher
* .NET 7

# Usage
Once you downloaded the [latest release](https://github.com/RequiDev/AudioDeviceWatcher/releases/latest) or compiled it yourself using Visual Studio and .NET 7, you just have to run it and it'll keep running in the background.
After grabbing your current default devices and saving it to a settings file in the same directory, it automatically sets itself to be started when Windows boots.
Once your device gets changed, it'll beep twice and change it back to what it's meant to be.
