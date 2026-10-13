using UnityEngine;
using UnityEngine.Rendering;

namespace TheForestFullBright;

public class FullBrightService
{
	private Light Sun;
	private bool AmbientBoosting;
	private Color BaselineAmbient = Color.black;
	private Color LastWrittenAmbient;
	private AmbientMode SavedMode;
	private Color SavedSky;
	private Color SavedEquator;
	private Color SavedGround;
	private float SavedIntensity;

	private bool FogKilled;
	private bool SavedFog;
	private FogMode SavedFogMode;

	public void OnEnable() => Camera.onPreCull += HandlePreCull;

	public void OnDisable()
	{
		Camera.onPreCull -= HandlePreCull;
		if (Sun != null) Sun.enabled = false;
	}

	public void Toggle()
	{
		bool nextState = !FullBrightPlugin.Enabled.Value;
		FullBrightPlugin.Enabled.Value = nextState;
		if (!nextState)
		{
			RestoreAll();
			if (Sun != null) Sun.enabled = false;
		}
		FullBrightPlugin.Log.LogInfo("FullBright: " + (nextState ? "ON" : "OFF"));
		FullBrightPlugin.Cfg.Save();
	}

	public void HandlePreCull(Camera cam)
	{
		if (cam == null) return;

		if (!FullBrightPlugin.Enabled.Value)
		{
			RestoreAll();
			if (Sun != null) Sun.enabled = false;
			return;
		}

		ApplyAmbient();
		ApplyFog();
		EnsureSun();
	}

	private void ApplyAmbient()
	{
		float targetIntensity = FullBrightPlugin.AmbientIntensity.Value;
		Color currentAmbient = RenderSettings.ambientLight;

		if (!AmbientBoosting || currentAmbient != LastWrittenAmbient) BaselineAmbient = currentAmbient;

		if (targetIntensity <= 0.001f)
		{
			if (AmbientBoosting)
			{
				RestoreAmbient();
				FullBrightPlugin.Log.LogInfo("ambient boost OFF (k=0)");
			}
			return;
		}

		if (!AmbientBoosting)
		{
			SavedMode = RenderSettings.ambientMode;
			SavedIntensity = RenderSettings.ambientIntensity;
			SavedSky = RenderSettings.ambientSkyColor;
			SavedEquator = RenderSettings.ambientEquatorColor;
			SavedGround = RenderSettings.ambientGroundColor;
			AmbientBoosting = true;
			FullBrightPlugin.Log.LogInfo($"ambient boost ON  k={targetIntensity:0.00} gameAmbient=({BaselineAmbient.r:0.00},{BaselineAmbient.g:0.00},{BaselineAmbient.b:0.00}) mode={RenderSettings.ambientMode}");
		}

		RenderSettings.ambientMode = AmbientMode.Flat;
		RenderSettings.ambientIntensity = 1f;
		RenderSettings.ambientSkyColor = Color.white;
		RenderSettings.ambientEquatorColor = Color.white;
		RenderSettings.ambientGroundColor = Color.white;

		Color boostedColor = new Color(
			Mathf.Max(BaselineAmbient.r, targetIntensity),
			Mathf.Max(BaselineAmbient.g, targetIntensity),
			Mathf.Max(BaselineAmbient.b, targetIntensity));

		RenderSettings.ambientLight = boostedColor;
		LastWrittenAmbient = boostedColor;
	}

	private void ApplyFog()
	{
		if (FullBrightPlugin.DisableFog.Value)
		{
			if (!FogKilled)
			{
				SavedFog = RenderSettings.fog;
				SavedFogMode = RenderSettings.fogMode;
				FogKilled = true;
			}
			RenderSettings.fog = false;
			return;
		}

		if (FogKilled)
		{
			RenderSettings.fog = SavedFog;
			RenderSettings.fogMode = SavedFogMode;
			FogKilled = false;
		}
	}

	private void EnsureSun()
	{
		if (Sun == null)
		{
			GameObject sunObj = new GameObject("FullBrightSun");
			Object.DontDestroyOnLoad(sunObj);
			if (FullBrightPlugin.HideSunObject.Value) sunObj.hideFlags = HideFlags.HideAndDontSave;
			Sun = sunObj.AddComponent<Light>();
			Sun.type = LightType.Directional;
			Sun.shadows = LightShadows.None;
			Sun.color = Color.white;
			sunObj.transform.rotation = Quaternion.Euler(55f, -35f, 0f);
		}

		if (!Sun.enabled) Sun.enabled = true;
		Sun.intensity = FullBrightPlugin.SunIntensity.Value;
	}

	public void ApplyPreset(float ambient, float sunIntensity)
	{
		FullBrightPlugin.Enabled.Value = true;
		FullBrightPlugin.AmbientIntensity.Value = ambient;
		FullBrightPlugin.SunIntensity.Value = sunIntensity;
	}

	public void RestoreAmbient()
	{
		if (!AmbientBoosting) return;

		RenderSettings.ambientMode = SavedMode;
		RenderSettings.ambientIntensity = SavedIntensity;
		RenderSettings.ambientSkyColor = SavedSky;
		RenderSettings.ambientEquatorColor = SavedEquator;
		RenderSettings.ambientGroundColor = SavedGround;
		RenderSettings.ambientLight = BaselineAmbient;
		LastWrittenAmbient = BaselineAmbient;
		AmbientBoosting = false;
	}

	public void RestoreAll()
	{
		RestoreAmbient();
		if (!FogKilled) return;

		RenderSettings.fog = SavedFog;
		RenderSettings.fogMode = SavedFogMode;
		FogKilled = false;
	}

	public void Destroy()
	{
		if (Sun != null) Object.Destroy(Sun.gameObject);
	}
}
