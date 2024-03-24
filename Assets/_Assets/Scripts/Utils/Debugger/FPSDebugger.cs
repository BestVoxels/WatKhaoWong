using System;
using UnityEngine;

namespace WatKhaoWong.Utils.Debugger
{
    /// <summary>
    /// Attach this script on any GameObject in the Scene, GameObject must not be disabled. Any Level is fine, parent or child level, fine.
    /// </summary>
    public class FPSDebugger : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Values Settings")]
        [SerializeField] private int[] _vsyncValues = { 0, 1, 2, 3, 4 };
        [SerializeField] private int[] _targetFPSValues = { 30, 45, 60, 90, 120, 144, -1 };
        [SerializeField]
        [Range(0.01f, 1f)] private float _fpsRefreshTime = 0.4f;
        [Space]
        [Header("Visual Decoration Settings")]
        [SerializeField] private Color _backgroundColor = new Color(1f, 1f, 1f, 0.35f);
        [SerializeField] private Texture2D _backgroundImage;
        [SerializeField] private Texture2D _buttonImage;
        #endregion



        #region --Fields-- (In Class)
        private float _timer = 1f;
        private int _fps = 0;
        private byte _vsyncIndexer = 0;
        private byte _targetFPSIndexer = 0;

        private GUIStyle _headerStyle = new GUIStyle();
        private GUIStyle _buttonStyle = new GUIStyle();
        private GUIStyle _textStyle = new GUIStyle();
        #endregion



        #region --Fields-- (Constant)
        private const int _panelWidth = 400;
        private const int _panelHeight = 500;

        private const int _buttonWidth = 300;
        private const int _buttonHeight = 80;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _headerStyle.alignment = TextAnchor.UpperCenter;
            _headerStyle.fontSize = 45;
            _headerStyle.normal.textColor = Color.black;
            _headerStyle.normal.background = _backgroundImage;

            _buttonStyle.alignment = TextAnchor.MiddleCenter;
            _buttonStyle.fontSize = 30;
            _buttonStyle.normal.textColor = Color.black;
            _buttonStyle.normal.background = _buttonImage;

            _textStyle.alignment = TextAnchor.MiddleLeft;
            _textStyle.fontSize = 30;
            _textStyle.normal.textColor = Color.black;
        }

        private void OnGUI()
        {
            // All UI Background's Color
            GUI.backgroundColor = _backgroundColor;

            // Background Box
            GUI.Box(new Rect((Screen.width - _panelWidth) / 2, (Screen.height - _panelHeight), _panelWidth, _panelHeight), "FPS Debugger", _headerStyle);

            // Buttons
            if (GUI.Button(new Rect((Screen.width - _buttonWidth) / 2, (Screen.height - _buttonHeight) - 250, _buttonWidth, _buttonHeight), "Set\nTargetFrameRate", _buttonStyle))
            {
                UpdateTargetFrameRate();
            }
            if (GUI.Button(new Rect((Screen.width - _buttonWidth) / 2, (Screen.height - _buttonHeight) - 350, _buttonWidth, _buttonHeight), "Set\nV Sync", _buttonStyle))
            {
                UpdateVsync();
            }

            // Texts
            if (_timer >= _fpsRefreshTime)
            {
                _fps = (int)Math.Round(1f / Time.deltaTime, MidpointRounding.AwayFromZero);
                _timer = 0f;
            }
            _timer += Time.deltaTime;

            GUI.Label(new Rect((Screen.width - _buttonWidth) / 2, (Screen.height - _buttonHeight) - 150, _buttonWidth, _buttonHeight), $"FPS : {_fps}", _textStyle);

            GUI.Label(new Rect((Screen.width - _buttonWidth) / 2, (Screen.height - _buttonHeight) - 100, _buttonWidth, _buttonHeight), $"TargetFrameRate : {Application.targetFrameRate}", _textStyle);

            GUI.Label(new Rect((Screen.width - _buttonWidth) / 2, (Screen.height - _buttonHeight) - 50, _buttonWidth, _buttonHeight), $"VSync : {QualitySettings.vSyncCount}", _textStyle);

            GUI.Label(new Rect((Screen.width - _buttonWidth) / 2, (Screen.height - _buttonHeight), _buttonWidth, _buttonHeight), $"Specs : {Screen.currentResolution}", _textStyle);


            //// EXAMPLES - 4 Corners on screen
            //GUI.Box(new Rect(0, 0, 100, 50), "Top-left");
            //GUI.Box(new Rect(Screen.width - 100, 0, 100, 50), "Top-right");
            //GUI.Box(new Rect(0, Screen.height - 50, 100, 50), "Bottom-left");
            //GUI.Box(new Rect(Screen.width - 100, Screen.height - 50, 100, 50), "Bottom-right");
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateTargetFrameRate()
        {
            Application.targetFrameRate = _targetFPSValues[_targetFPSIndexer];

            _targetFPSIndexer = (byte)(_targetFPSIndexer == _targetFPSValues.Length - 1 ? 0 : ++_targetFPSIndexer);

            print($"Application.targetFrameRate : {Application.targetFrameRate}");
            print($"_targetFPSIndexer : {_targetFPSIndexer}");
        }

        private void UpdateVsync()
        {
            QualitySettings.vSyncCount = _vsyncValues[_vsyncIndexer];

            _vsyncIndexer = (byte)(_vsyncIndexer == _vsyncValues.Length - 1 ? 0 : ++_vsyncIndexer);

            print($"QualitySettings.vSyncCount : {QualitySettings.vSyncCount}");
            print($"_vsyncIndexer : {_vsyncIndexer}");
        }
        #endregion
    }
}