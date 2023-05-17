using AudioDeviceWatcher.COM;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace AudioDeviceWatcher;

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
			var defaultInputDevice =
				_deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
			var defaultOutputDevice =
				_deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);
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
			case DataFlow.Capture when !string.IsNullOrEmpty(_presetInputDevice) &&
			                           defaultDeviceId != _presetInputDevice &&
			                           _deviceEnumerator.GetDevice(_presetInputDevice) != null:
				SetDefaultAudioDevice(_presetInputDevice);
				break;
			case DataFlow.Render when !string.IsNullOrEmpty(_presetOutputDevice) &&
			                          defaultDeviceId != _presetOutputDevice &&
			                          _deviceEnumerator.GetDevice(_presetOutputDevice) != null:
				SetDefaultAudioDevice(_presetOutputDevice);
				break;
		}

		Console.Beep();
	}

	private void SetDefaultAudioDevice(string deviceId)
	{
		for (var i = 0; i <= (int) Role.Communications; i++)
		{
			_policyConfigClient.SetDefaultEndpoint(deviceId, (Role) i);
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