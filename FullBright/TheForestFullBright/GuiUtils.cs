using BepInEx.Configuration;
using UnityEngine;

namespace TheForestFullBright;

public static class GuiUtils
{
	private static Texture2D WhiteTex;

	public static Texture2D WhiteTexture
	{
		get
		{
			if (WhiteTex == null)
			{
				WhiteTex = new Texture2D(1, 1);
				WhiteTex.SetPixel(0, 0, Color.white);
				WhiteTex.Apply();
				WhiteTex.hideFlags = HideFlags.HideAndDontSave;
			}
			return WhiteTex;
		}
	}

	public static int Scale(float value, float uiScale) => Mathf.RoundToInt(value * uiScale);

	public static string StateLabel(string text, bool on) => $"{(on ? "★ " : "✖ ")}{text}: {(on ? "BẬT" : "TẮT")}";

	public static void DrawBarRow(string label, ConfigEntry<float> entry, float min, float max, int id, GUIStyle labelStyle, GUIStyle smallBtnStyle, GUIStyle barTextStyle, ref int activeBarId, ref bool dirty, float uiScale)
	{
		GUILayout.Label($"{label} {entry.Value:0.00}", labelStyle);
		GUILayout.BeginHorizontal();

		if (GUILayout.Button("−", smallBtnStyle, GUILayout.Width(Scale(38f, uiScale)), GUILayout.Height(Scale(26f, uiScale))))
		{
			entry.Value = Mathf.Clamp(Mathf.Round((entry.Value - 0.05f) * 100f) / 100f, min, max);
			dirty = true;
		}

		Rect rect = GUILayoutUtility.GetRect(Scale(100f, uiScale), Scale(22f, uiScale), GUILayout.ExpandWidth(true));
		Event current = Event.current;

		if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition) && Input.GetMouseButton(0))
		{
			activeBarId = id;
		}

		if (activeBarId == id && Input.GetMouseButton(0) && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag))
		{
			float ratio = Mathf.Clamp01((current.mousePosition.x - rect.x) / Mathf.Max(1f, rect.width));
			float newValue = Mathf.Round(Mathf.Lerp(min, max, ratio) * 100f) / 100f;
			if (Mathf.Abs(newValue - entry.Value) > 0.0005f)
			{
				entry.Value = newValue;
				dirty = true;
			}
			if (current.type == EventType.MouseDown) current.Use();
		}

		if (current.type == EventType.MouseUp && activeBarId == id) activeBarId = 0;

		if (current.type == EventType.Repaint)
		{
			float fillRatio = Mathf.InverseLerp(min, max, entry.Value);
			GUI.color = new Color(0f, 0f, 0f, 0.55f);
			GUI.DrawTexture(rect, WhiteTexture);
			GUI.color = new Color(0.3f, 0.68f, 0.98f, 0.85f);
			GUI.DrawTexture(new Rect(rect.x + 1f, rect.y + 1f, Mathf.Max(0f, (rect.width - 2f) * fillRatio), rect.height - 2f), WhiteTexture);
			GUI.color = Color.white;
		}

		GUI.Label(rect, $"  {entry.Value:0.00}", barTextStyle);

		if (GUILayout.Button("+", smallBtnStyle, GUILayout.Width(Scale(38f, uiScale)), GUILayout.Height(Scale(26f, uiScale))))
		{
			entry.Value = Mathf.Clamp(Mathf.Round((entry.Value + 0.05f) * 100f) / 100f, min, max);
			dirty = true;
		}

		GUILayout.EndHorizontal();
	}
}
