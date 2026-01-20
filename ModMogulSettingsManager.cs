using UnityEngine;

namespace ModMogul
{
	internal class ModMogulSettingsManager : MonoBehaviour
	{
		void OnEnable ()
		{
			ModMogulPlugin.SettingSetter.UpdateUI();
		}
	}
}
