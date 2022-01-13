using UnityEngine;
using UnityEditor;

namespace GameCreator.Weather
{
    [CustomEditor(typeof(LightingController))]
    public class LightingControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LightingController light = (LightingController)target;

            if (GUILayout.Button("Morning"))
                light.SetTimeOfDay(TimeOfDay.Morning);

            if (GUILayout.Button("Day"))
                light.SetTimeOfDay(TimeOfDay.Day);

            if (GUILayout.Button("Evening"))
                light.SetTimeOfDay(TimeOfDay.Evening);

            if (GUILayout.Button("Night"))
                light.SetTimeOfDay(TimeOfDay.Night);

            DrawDefaultInspector();
        }
    }
}