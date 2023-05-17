using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

namespace AudioDeviceWatcher.COM;

[ComImport, Guid("870af99c-171d-4f9e-af0d-e63df40c2bc9")]
internal class _PolicyConfigClient
{
}
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