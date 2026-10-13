using UnityEngine;

namespace TheForestFullBright;

public class FullBrightController : MonoBehaviour
{
	private readonly FullBrightService Service = new FullBrightService();
	private readonly FullBrightUi Ui = new FullBrightUi();

	private bool Dirty;
	private float NextSaveAt;
	private float UiScale = 1f;

	private CursorLockMode PrevLock;
	private bool PrevCursorVisible;
	private bool WasUiOpen;

	private GUIStyle WinStyle;
	private GUIStyle LabelStyle;
	private GUIStyle BtnStyle;
	private GUIStyle SmallBtnStyle;
	private GUIStyle BarTextStyle;

	private void OnEnable() => Service.OnEnable();
	private void OnDisable() => Service.OnDisable();
	private void Update() => HandleHotkeys();
	private void OnDestroy() => Service.Destroy();

	private void HandleHotkeys()
	{
		if (Input.GetKeyDown(FullBrightPlugin.ToggleKey.Value)) Service.Toggle();
		if (Input.GetKeyDown(FullBrightPlugin.UiKey.Value)) Ui.Toggle();
	}

	private void OnGUI()
	{
		UiScale = Mathf.Clamp(Screen.height / 1080f, 1f, 2.5f);
		EnsureStyles();

		UpdateCursorState(Ui.IsOpen);
		Ui.Draw(Service, WinStyle, LabelStyle, BtnStyle, SmallBtnStyle, BarTextStyle, UiScale, ref Dirty);

		if (Dirty && Time.unscaledTime >= NextSaveAt)
		{
			NextSaveAt = Time.unscaledTime + 0.5f;
			FullBrightPlugin.Cfg.Save();
			Dirty = false;
		}
	}

	private void UpdateCursorState(bool isUiOpen)
	{
		if (isUiOpen)
		{
			if (!WasUiOpen)
			{
				PrevLock = Cursor.lockState;
				PrevCursorVisible = Cursor.visible;
				WasUiOpen = true;
			}
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			return;
		}

		if (WasUiOpen)
		{
			Cursor.lockState = PrevLock;
			Cursor.visible = PrevCursorVisible;
			WasUiOpen = false;
			if (Dirty)
			{
				FullBrightPlugin.Cfg.Save();
				Dirty = false;
			}
		}
	}

	private void EnsureStyles()
	{
		if (WinStyle == null)
		{
			WinStyle = new GUIStyle(GUI.skin.window);
			LabelStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
			BtnStyle = new GUIStyle(GUI.skin.button);
			SmallBtnStyle = new GUIStyle(GUI.skin.button);
			BarTextStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };
		}

		int fontSize = Mathf.RoundToInt(14f * UiScale);
		WinStyle.fontSize = fontSize;
		LabelStyle.fontSize = fontSize;
		BtnStyle.fontSize = fontSize;
		SmallBtnStyle.fontSize = fontSize;
		BarTextStyle.fontSize = fontSize;
	}
}
