using FlyBit.Controllers;
using UnityEditor;
using UnityEngine;

namespace FlyBit.Editor
{

    [CustomEditor(typeof(GameController))]
    class GameControllerEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                GUILayout.Space(20f);

                if (GUILayout.Button("Invert Power Up"))
                {
                    EffectsController.Singleton.ToggleInvertEffect();
                }

                if (GUILayout.Button("Hyperdrive Power Up"))
                {
                    EffectsController.Singleton.HyperdriveTravel();
                }
            }
        }

    }

}
