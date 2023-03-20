using System;
using Base.Ravel.Networking;
using UnityEditor;
using UnityEngine;

public class EnvironmentState : CreatorWindowState
{
    private const int ENVIRONMENT_BTN_COUNT = 3;
    
    public override CreatorWindow.State State {
        get { return CreatorWindow.State.Environment; }
    }

    private bool _envFoldOpen = false;
    private string[] _names;
    private Environment[] _environments;
    private int _curEnv = -1;
    private bool _retrievingImg = false;
    private Sprite _curEnvImg;
    
    public EnvironmentState(CreatorWindow wnd) : base(wnd) { }
    
    public override void OnGUI(Rect position) {
        int indent = EditorGUI.indentLevel;
        _envFoldOpen = EditorGUILayout.Foldout(_envFoldOpen, "Existing");

        if (_envFoldOpen) {
            if (GUILayout.Button("Fetch environments")) {
                FetchEnvironments();
            }

            if (GUILayout.Button("Clear environments")) {
                Clear();
            }
            GUILayout.Space(20f);
            
            if (_environments != null && _environments.Length > 0) {
                if (_curEnv < 0) {
                    _curEnv = 0;
                }
                
                EditorGUI.BeginChangeCheck();
                _curEnv = GUILayout.SelectionGrid(_curEnv, _names, ENVIRONMENT_BTN_COUNT);
                if (EditorGUI.EndChangeCheck()) {
                    _curEnvImg = null;
                }

                if (_curEnvImg == null && !_retrievingImg) {
                    EditorImageService.GetSprite(_environments[_curEnv].imageUrl, ImageSize.I1920, OnImageRetrieved);
                    _retrievingImg = true;
                }
                
                GUILayout.Space(20f);
                GUILayout.Label($"Name: \t\t{_environments[_curEnv].name}");
                GUILayout.Label($"Guid: \t\t{_environments[_curEnv].environmentUuid}");
                GUILayout.Label($"description: \t{_environments[_curEnv].description}");
                
                
                //indent by space
                GUILayout.BeginHorizontal();
                GUILayout.Space(100f);
                GUILayout.BeginVertical();
                
                GUI.enabled = false;
                GUILayout.Toggle(_environments[_curEnv].active, "active:");
                GUILayout.Toggle(_environments[_curEnv].publicEnvironment, "public:");
                GUI.enabled = true;
                
                //undo indent
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(5f);
                if (_curEnvImg != null) {
                    RavelEditor.DrawTextureScaledGUI(new Vector2(0, GUILayoutUtility.GetLastRect().yMax),
                        position.width, _curEnvImg.texture);
                }
            }
        }
    }

    private void FetchEnvironments() {
        RavelWebRequest res = CreatorRequest.GetCreatorEnvironments(RavelEditor.User.userUUID);
        EditorWebRequests.SendWebRequest(res, OnEnvironmentsRetrieved, this);
    }

    private void OnEnvironmentsRetrieved(RavelWebResponse res) {
        if (res.Success && res.TryGetCollection(out ProxyCollection<Environment> environments)) {
            _environments = environments.Array;
            _names = _environments.GetNames();
        }
        else {
            Debug.LogError($"error fetching environments {res.Error.FullMessage}!");
        }
    }

    private void OnImageRetrieved(Sprite sprite, ImageSize size) {
        _curEnvImg = sprite;
        _retrievingImg = false;
    }

    private void Clear() {
        _environments = Array.Empty<Environment>();
        _names = Array.Empty<string>();
        _curEnv = -1;
    }
}
