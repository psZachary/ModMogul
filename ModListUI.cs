using System;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Bootstrap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMogul
{
	public static class ModListUI
	{
		/// <summary>
		/// Writes a list of loaded plugins and dependency load errors into a TMP_Text.
		/// Call this after Chainloader has initialized (e.g., in a menu scene Start()).
		/// </summary>
		public static void Populate(TMP_Text text)
		{
			if (text == null) return;

			var sb = new StringBuilder(4096);

			// ---- Loaded plugins ----
			var loaded = Chainloader.PluginInfos
				.Select(kvp => kvp.Value)
				.OrderBy(pi => pi.Metadata.Name, StringComparer.OrdinalIgnoreCase)
				.ThenBy(pi => pi.Metadata.GUID, StringComparer.OrdinalIgnoreCase)
				.ToList();

			sb.AppendLine($"<b>Mods loaded:</b> {loaded.Count}");
			foreach (var pi in loaded)
			{
				// Metadata has GUID / Name / Version
				// Instance is the BaseUnityPlugin
				var meta = pi.Metadata;
				sb.Append("• ");
				sb.Append(meta.Name);
				//sb.Append(" <color=#888888>(");
				//sb.Append(meta.GUID);
				//sb.Append(")</color> ");
				sb.Append("<color=#AAAAAA>v</color>");
				sb.Append(meta.Version);

				/*
				// Optional: show the plugin type
				if (pi.Instance != null)
				{
					sb.Append("  <color=#888888>[");
					sb.Append(pi.Instance.GetType().Name);
					sb.Append("]</color>");
				}
				*/

				sb.AppendLine();
			}

			// ---- Failed (dependency / load errors BepInEx knows about) ----
			// Not every failure ends up here (some things fail later), but this catches a lot of "couldn't load" cases.
			var depErrors = Chainloader.DependencyErrors;

			if (depErrors != null && depErrors.Count > 0)
			{
				sb.AppendLine();
				sb.AppendLine($"<b><color=#FF6666>Failed to load:</color></b> {depErrors.Count}");

				// This is typically: plugin GUID -> list of error strings
				foreach (var kvp in depErrors.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
				{
					sb.Append("• <b>");
					sb.Append(kvp);
					sb.AppendLine("</b>");

					/*
					foreach (var err in kvp)
					{
						sb.Append("   <color=#FF8888>- ");
						sb.Append(err);
						sb.AppendLine("</color>");
					}
					*/
				}
			}

			text.text = sb.ToString();
		}

		public static ScrollRect SpawnScrollRect(Transform parent, out Transform contentTransform)
		{
			var root = new GameObject("ScrollView", typeof(RectTransform), typeof(CanvasRenderer), typeof(ScrollRect), typeof(Image));
			root.transform.SetParent(parent, false);
			root.GetComponent<ScrollRect>().elasticity = 0;
			root.GetComponent<ScrollRect>().horizontal = false;
			root.GetComponent<ScrollRect>().inertia = false;
			root.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
			root.GetComponent<Image>().color = new Color(.2f, .2f, .2f, .9f);

			var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
			viewport.transform.SetParent(root.transform, false);
			viewport.GetComponent<RectTransform>().anchorMin = Vector2.one * 0.1f;
			viewport.GetComponent<RectTransform>().anchorMax = Vector2.one * 0.9f;
			viewport.GetComponent<RectTransform>().offsetMin = Vector2.zero;
			viewport.GetComponent<RectTransform>().offsetMax = Vector2.zero;

			var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
			content.transform.SetParent(viewport.transform, false);
			contentTransform = content.transform;
			content.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
			content.GetComponent<RectTransform>().anchorMax = Vector2.one;
			content.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
			content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			content.GetComponent<RectTransform>().offsetMin = Vector2.zero;
			content.GetComponent<RectTransform>().offsetMax= Vector2.zero;

			var scrollRect = root.GetComponent<ScrollRect>();
			scrollRect.viewport = viewport.GetComponent<RectTransform>();
			scrollRect.content = content.GetComponent<RectTransform>();
			scrollRect.horizontal = false;
			scrollRect.vertical = true;

			var fitter = content.GetComponent<ContentSizeFitter>();
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			return scrollRect;
		}
	}
}