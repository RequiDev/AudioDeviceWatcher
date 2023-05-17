using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace AudioDeviceWatcher;

internal class Program
{
	private static void Main()
	{
		AddToAutoStart();
		RunInBackground();

		var audioDeviceWatcher = new AudioDeviceWatcher();

		Console.ReadLine();
	}

	private static void AddToAutoStart()
	{
		var asm = Assembly.GetExecutingAssembly();
		string executablePath;
		if (string.IsNullOrEmpty(asm.Location))
		{
			executablePath = Path.Combine(AppContext.BaseDirectory,
				$"{Process.GetCurrentProcess().ProcessName}.exe");
		}
		else
		{
			executablePath = $"dotnet {asm.Location}";
		}

		var startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
		startupKey?.SetValue("AudioDeviceWatcher", executablePath);
	}

	private static void RunInBackground()
	{
		var handle = GetConsoleWindow();
		ShowWindow(handle, 0); // Hide the console window
	}

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}