using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using PropertyKey = NAudio.CoreAudioApi.PropertyKey;

namespace AudioDeviceWatcher
{
	public class PolicyConfigClient
	{
		private readonly IPolicyConfig? _policyConfig;
		private readonly IPolicyConfigVista? _policyConfigVista;
		private readonly IPolicyConfig10? _policyConfig10;

		public PolicyConfigClient()
		{
			_policyConfig = new _PolicyConfigClient() as IPolicyConfig;
			if (_policyConfig != null)
				return;

			_policyConfigVista = new _PolicyConfigClient() as IPolicyConfigVista;
			if (_policyConfigVista != null)
				return;

			_policyConfig10 = new _PolicyConfigClient() as IPolicyConfig10;
		}

		public void SetDefaultEndpoint(string deviceId, Role role)
		{
			if (_policyConfig != null)
			{
				Marshal.ThrowExceptionForHR(_policyConfig.SetDefaultEndpoint(deviceId, role));
				return;
			}
			if (_policyConfigVista != null)
			{
				Marshal.ThrowExceptionForHR(_policyConfigVista.SetDefaultEndpoint(deviceId, role));
				return;
			}
			if (_policyConfig10 != null)
			{
				Marshal.ThrowExceptionForHR(_policyConfig10.SetDefaultEndpoint(deviceId, role));
			}
		}
	}

	internal class AudioDeviceWatcher : IMMNotificationClient
	{
		private const string SettingsFilePath = "settings.txt";
		private string _presetInputDevice = null!;
		private string _presetOutputDevice = null!;

		private readonly MMDeviceEnumerator _deviceEnumerator;
		private readonly PolicyConfigClient _policyConfigClient;

		public AudioDeviceWatcher()
		{
			_deviceEnumerator = new MMDeviceEnumerator();
			LoadSettings();

			_policyConfigClient = new PolicyConfigClient();

			_deviceEnumerator.RegisterEndpointNotificationCallback(this);
		}

		private void LoadSettings()
		{
			if (File.Exists(SettingsFilePath))
			{
				var settings = File.ReadAllLines(SettingsFilePath);
				if (settings.Length >= 2)
				{
					_presetInputDevice = settings[0];
					_presetOutputDevice = settings[1];
				}
			}
			else
			{
				var defaultInputDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
				var defaultOutputDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);
				_presetInputDevice = defaultInputDevice.ID;
				_presetOutputDevice = defaultOutputDevice.ID;

				using var writer = new StreamWriter(SettingsFilePath);
				writer.WriteLine(_presetInputDevice);
				writer.WriteLine(_presetOutputDevice);
			}
		}

		public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
		{
			if (role != Role.Communications)
				return;

			switch (flow)
			{
				case DataFlow.Capture when !string.IsNullOrEmpty(_presetInputDevice) && defaultDeviceId != _presetInputDevice && _deviceEnumerator.GetDevice(_presetInputDevice) != null:
					SetDefaultAudioDevice(_presetInputDevice);
					break;
				case DataFlow.Render when !string.IsNullOrEmpty(_presetOutputDevice) && defaultDeviceId != _presetOutputDevice && _deviceEnumerator.GetDevice(_presetOutputDevice) != null:
					SetDefaultAudioDevice(_presetOutputDevice);
					break;
			}

			Console.Beep();
		}

		private void SetDefaultAudioDevice(string deviceId)
		{
			for (var i = 0; i <= (int)Role.Communications; i++)
			{
				_policyConfigClient.SetDefaultEndpoint(deviceId, (Role)i);
			}
		}

        #region not important
		public void OnDeviceStateChanged(string deviceId, DeviceState newState)
		{
            // not important
		}

		public void OnDeviceAdded(string pwstrDeviceId)
		{
			// not important
		}

		public void OnDeviceRemoved(string deviceId)
		{
			// not important
		}

		public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
		{
			// not important
		}
        #endregion
	}

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
}
