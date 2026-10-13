using UnityEngine;

namespace TheForestFullBright;

public class FullBrightUi
{
	private const int WindowId = 23063;
	private Rect WindowRect = new Rect(30f, 140f, 430f, 350f);
	private int ActiveBarId;

	public bool IsOpen { get; set; }

	public void Toggle()
	{
		IsOpen = !IsOpen;
		FullBrightPlugin.Log.LogInfo("FullBright panel: " + (IsOpen ? "OPEN" : "CLOSED"));
	}

	public void Draw(FullBrightService service, GUIStyle winStyle, GUIStyle labelStyle, GUIStyle btnStyle, GUIStyle smallBtnStyle, GUIStyle barTextStyle, float uiScale, ref bool dirty)
	{
		if (!IsOpen) return;

		Rect rect = WindowRect;
		rect.width = GuiUtils.Scale(430f, uiScale);
		bool localDirty = dirty;
		WindowRect = GUILayout.Window(WindowId, rect, (id) => DrawContent(service, winStyle, labelStyle, btnStyle, smallBtnStyle, barTextStyle, uiScale, ref localDirty), "The Forest — FullBright   [F8]", winStyle);
		dirty = localDirty;
	}

	private void DrawContent(FullBrightService service, GUIStyle winStyle, GUIStyle labelStyle, GUIStyle btnStyle, GUIStyle smallBtnStyle, GUIStyle barTextStyle, float uiScale, ref bool dirty)
	{
		GUILayout.BeginVertical(winStyle);

		bool isEnabled = FullBrightPlugin.Enabled.Value;
		if (GUILayout.Button(isEnabled ? "★ FullBright: BẬT" : "✖ FullBright: TẮT", btnStyle, GUILayout.Height(GuiUtils.Scale(34f, uiScale))))
		{
			service.Toggle();
			dirty = true;
		}

		GUILayout.Space(GuiUtils.Scale(8f, uiScale));
		GuiUtils.DrawBarRow("Độ sáng nền (Ambient):", FullBrightPlugin.AmbientIntensity, 0f, 2f, 1, labelStyle, smallBtnStyle, barTextStyle, ref ActiveBarId, ref dirty, uiScale);
		GUILayout.Space(GuiUtils.Scale(4f, uiScale));
		GuiUtils.DrawBarRow("Đèn hướng (Sun):", FullBrightPlugin.SunIntensity, 0f, 2f, 2, labelStyle, smallBtnStyle, barTextStyle, ref ActiveBarId, ref dirty, uiScale);
		GUILayout.Space(GuiUtils.Scale(6f, uiScale));

		if (GUILayout.Button(GuiUtils.StateLabel("Tắt sương mù (fog)", FullBrightPlugin.DisableFog.Value), btnStyle, GUILayout.Height(GuiUtils.Scale(28f, uiScale))))
		{
			FullBrightPlugin.DisableFog.Value = !FullBrightPlugin.DisableFog.Value;
			dirty = true;
		}

		GUILayout.Space(GuiUtils.Scale(6f, uiScale));
		GUILayout.Label("Preset:", labelStyle);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Dịu", btnStyle, GUILayout.Height(GuiUtils.Scale(30f, uiScale)))) { service.ApplyPreset(0.02f, 0f); dirty = true; }
		if (GUILayout.Button("Vừa", btnStyle, GUILayout.Height(GuiUtils.Scale(30f, uiScale)))) { service.ApplyPreset(0.05f, 0f); dirty = true; }
		if (GUILayout.Button("Sáng", btnStyle, GUILayout.Height(GuiUtils.Scale(30f, uiScale)))) { service.ApplyPreset(0.1f, 0f); dirty = true; }
		if (GUILayout.Button("Max", btnStyle, GUILayout.Height(GuiUtils.Scale(30f, uiScale)))) { service.ApplyPreset(0.25f, 0.1f); dirty = true; }
		GUILayout.EndHorizontal();

		GUILayout.Space(GuiUtils.Scale(4f, uiScale));
		if (GUILayout.Button("Tắt fullbright", btnStyle, GUILayout.Height(GuiUtils.Scale(30f, uiScale))))
		{
			FullBrightPlugin.Enabled.Value = false;
			service.RestoreAll();
			dirty = true;
		}

		GUILayout.Space(GuiUtils.Scale(6f, uiScale));
		if (GUILayout.Button("Lưu vào config", btnStyle, GUILayout.Height(GuiUtils.Scale(30f, uiScale))))
		{
			FullBrightPlugin.Cfg.Save();
			dirty = false;
		}

		GUILayout.Space(GuiUtils.Scale(2f, uiScale));
		GUILayout.Label("F7: bật/tắt nhanh · F8: ẩn/hiện bảng · kéo thanh tiêu đề để di chuyển", labelStyle);
		GUILayout.EndVertical();

		GUI.DragWindow(new Rect(0f, 0f, 100000f, GuiUtils.Scale(26f, uiScale)));
	}
}
