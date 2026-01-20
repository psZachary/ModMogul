using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMogul
{
	public class SettingsSetter : MonoBehaviour
	{
		protected class ModSettings
		{
			public string modName;
			public Transform modLabel;
			public List<BaseSettingOption> settings;
		}

		GameObject generalSettingsView;
		GameObject settingsTab;
		GameObject settingsView;
		Transform settingsContent = null;
		List<ModSettings> modSettings;

		void Awake ()
		{
			StartCoroutine(WaitForMenu());
		}

		IEnumerator WaitForMenu ()
		{
			// Eventually add a new tab instead of putting it all under General
			while (settingsContent == null)
			{
				foreach (ScrollRect s in GameObject.FindObjectsByType<ScrollRect>(FindObjectsInactive.Include, FindObjectsSortMode.None))
				{
					if (s.gameObject.name == "GeneralScrollView")
					{
						generalSettingsView = s.gameObject;
						settingsView = Instantiate(s.gameObject, s.transform.parent);
						settingsView.name = "ModSettingsView";
						foreach (Transform t in settingsView.GetComponentsInChildren<Transform>())
						{
							if (t.name == "Content")
							{
								settingsContent = t.transform;
								foreach (Transform tr in t.GetComponentsInChildren<Transform>())
								{
									if (tr == t) continue;
									Destroy(tr.gameObject);
								}
								break;
							}
						}
					}
					if (settingsContent != null)
					{
						foreach (UIButtonSounds b in FindObjectsByType<UIButtonSounds>(FindObjectsInactive.Include, FindObjectsSortMode.None))
						{
							if (b.name == "AccessibilityTabButton")
							{
								foreach (Transform t in b.transform.parent)
								{
									t.GetComponent<Button>().onClick.AddListener(() =>
									{
										settingsView.SetActive(false);
									});
								}
								settingsTab = Instantiate(b.gameObject, b.transform.parent);
								settingsTab.GetComponentInChildren<TMP_Text>().text = "Mods";
								settingsTab.GetComponent<Button>().onClick.RemoveAllListeners();
								foreach (Transform t in settingsView.transform.parent)
								{
									if (t.name.ToLower().Contains("view"))
									{
										settingsTab.GetComponent<Button>().onClick.AddListener(() =>
										{
											t.gameObject.SetActive(false);
										});
									}
								}
								settingsTab.GetComponent<Button>().onClick.AddListener(() =>
								{
									settingsView.SetActive(true);
								});
							}
						}
						break;
					}
				}
				yield return null;
			}
			settingsView.SetActive(false);
		}

		public void UpdateUI ()
		{
			int i = 0;
			foreach (ModSettings s in modSettings)
			{
				s.modLabel.SetSiblingIndex(i++);
				foreach (BaseSettingOption b in s.settings)
				{
					b.transform.SetSiblingIndex(i++);
				}
			}
		}

		ModSettings SetupModSettings (string modName)
		{
			modSettings ??= new();
			ModSettings settings = null;
			foreach (ModSettings s in modSettings)
			{
				if (s.modName == modName)
				{
					settings = s;
					break;
				}
			}
			if (settings == null)
			{
				GameObject template = null;
				foreach (RectTransform r in generalSettingsView.GetComponentsInChildren<RectTransform>(includeInactive: true))
				{
					if (r.GetComponent<SettingToggle>())
					{
						template = r.gameObject;
						break;
					}
				}

				GameObject modLabel = new GameObject(modName, typeof(RectTransform));
				modLabel.transform.SetParent(settingsContent, false);
				RectTransform rectTransform = modLabel.GetComponent<RectTransform>();
				rectTransform.anchorMin = Vector3.zero;
				rectTransform.anchorMax = Vector3.zero;
				rectTransform.offsetMin = Vector3.zero;
				rectTransform.offsetMax = Vector3.zero;
				rectTransform.sizeDelta = new Vector2(0, 50);
				Instantiate(template.GetComponentInChildren<TMP_Text>().gameObject, modLabel.transform);
				TMP_Text modTMP = modLabel.GetComponentInChildren<TMP_Text>();
				rectTransform = modTMP.GetComponent<RectTransform>();
				rectTransform.anchorMin = Vector3.zero;
				rectTransform.anchorMax = Vector3.one;
				rectTransform.offsetMin = Vector3.zero;
				rectTransform.offsetMax = Vector3.zero;
				modTMP.transform.localScale = Vector3.one;
				modTMP.text = modName;
				modTMP.alignment = TextAlignmentOptions.Center;
				settings = new ModSettings()
				{
					modName = modName,
					modLabel = modLabel.transform,
					settings = new()
				};
				modSettings.Add(settings);
			}
			return settings;
		}

		public void RegisterToggleSetting (string settingName, string settingKey, bool defaultValue = false)
		{
			string modName = Assembly.GetCallingAssembly().GetName().Name;
			StartCoroutine(WaitForSettings(modName, settingName, settingKey, defaultValue));
		}

		IEnumerator WaitForSettings (string modName, string settingName, string settingKey, bool defaultValue)
		{
			while (settingsContent == null)
			{
				yield return null;
			}
			ModSettings settings = SetupModSettings(modName);

			GameObject template = null;
			foreach (RectTransform r in generalSettingsView.GetComponentsInChildren<RectTransform>(includeInactive: true ))
			{
				if (r.GetComponent<SettingToggle>())
				{
					template = r.gameObject;
					break;
				}
			}

			GameObject toggleSettingPrefab = GameObject.Instantiate(template, settingsContent);
			toggleSettingPrefab.name = settingName;
			ModSettingsToggle toggle = toggleSettingPrefab.AddComponent<ModSettingsToggle>();
			foreach (Transform t in toggleSettingPrefab.GetComponentsInChildren<Transform>())
			{
				if (t.GetComponent<Toggle>())
				{
					toggle.toggle = t.GetComponent<Toggle>();
				}
				if (t.name == "OnOff")
				{
					toggle._onOffLabel = t.GetComponent<TMP_Text>();
				}
				if (t.name == "Title")
				{
					t.GetComponent<TMP_Text>().text = settingName;
				}
			}
			toggle.defaultValue = defaultValue;
			toggle.settingKey = settingKey;
			GameObject.Destroy(toggleSettingPrefab.GetComponent<SettingToggle>());

			settings.settings.Add(toggle);
		}
	}
}
