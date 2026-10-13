using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace TheForestFullBright;

[BepInPlugin("com.lunar.theforest.fullbright", "The Forest FullBright", "1.10.1")]
public class FullBrightPlugin : BaseUnityPlugin
{
	public const string Guid = "com.lunar.theforest.fullbright";

	internal static ManualLogSource Log;
	internal static ConfigFile Cfg;

	internal static ConfigEntry<bool> Enabled;
	internal static ConfigEntry<KeyCode> ToggleKey;
	internal static ConfigEntry<KeyCode> UiKey;

	internal static ConfigEntry<float> AmbientIntensity;
	internal static ConfigEntry<float> SunIntensity;
	internal static ConfigEntry<bool> DisableFog;
	internal static ConfigEntry<bool> HideSunObject;

	private void Awake()
	{
		Log = Logger;
		Cfg = Config;
		Config.SaveOnConfigSet = false;

		Enabled = Config.Bind("General", "Enabled", true, "Current state of fullbright. Toggle in game with the hotkey or the panel.");
		ToggleKey = Config.Bind("General", "ToggleKey", KeyCode.F7, "Key that toggles fullbright on and off.");
		UiKey = Config.Bind("General", "UiKey", KeyCode.F8, "Key that shows and hides the brightness panel.");

		AmbientIntensity = Config.Bind("Light", "AmbientIntensity", 0.05f, "Flat ambient lift. The game's own ambient is pure black, so this value is ADDED on top of every scene. The vanilla game is very dark, so even small values change a lot. 0.02 = subtle, 0.05 = default, 0.10+ = noticeably bright. Recommended 0.02..0.08.");
		SunIntensity = Config.Bind("Light", "SunIntensity", 0f, "Extra shadowless directional light. 0 = off (recommended). Values above ~0.2 blow out daylight.");
		DisableFog = Config.Bind("Light", "DisableFog", true, "Disable distance fog so the world stays clear, day or night.");
		HideSunObject = Config.Bind("Light", "HideSunObject", true, "Keep the added sun out of the hierarchy to avoid touching the scene.");

		GameObject host = new GameObject("FullBrightHost");
		DontDestroyOnLoad(host);
		host.hideFlags = HideFlags.HideAndDontSave;
		host.AddComponent<FullBrightController>();

		Config.Save();
		Logger.LogInfo("FullBright ready. F7 fullbright, F8 fullbright UI");
	}
}
