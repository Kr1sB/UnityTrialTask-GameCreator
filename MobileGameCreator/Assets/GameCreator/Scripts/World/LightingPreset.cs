using UnityEngine;

[CreateAssetMenu(
    fileName = "New Lighting Preset",
    menuName = "GameCreator/Lighting Preset"
)]
public class LightingPreset : ScriptableObject
{
    [System.Serializable]
    public class SunSettings
    {
        public Vector3 rotation;
        public float intensity;

        [ColorUsage(false)]
        public Color color;
    }

    [System.Serializable]
    public class FogSettings
    {
        public bool enabled;
        public float density;

        [ColorUsage(false)]
        public Color color;
    }

    [System.Serializable]
    public class SkyboxSettings
    {
        public Material material;
    }

    public SunSettings sun;
    public FogSettings fog;
    public SkyboxSettings skybox;
}
