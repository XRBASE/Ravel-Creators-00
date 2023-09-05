using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ObjectLayoutgroup : MonoBehaviour
{
    [SerializeField] private float _spacing;

    [SerializeField, HideInInspector] private Vector2 _dir;

    private List<int> _childGuids = new List<int>();
    private int activeChildren = 0;

    private void Start() {
        ForceReorder();
    }

    private void OnEnable()
    {
        ForceReorder();
    }

    private void ForceReorder() {
        if (transform.childCount == 0 && 
            _childGuids.Count > 0)
            return;
        
        CountActiveChildren();
        ReorderChildren();
    }

    private void Update()
    {
        CountActiveChildren();
        
        if (activeChildren == 0) {
            if (_childGuids.Count > 0) {
                _childGuids.Clear();
            }

            return;
        }

        //check if any new child objects have been spawned
        bool dirty = activeChildren != _childGuids.Count;

        if (!dirty) {
            for (int i = 0; i < _childGuids.Count; i++) {
                if (_childGuids[i] != transform.GetChild(i).gameObject.GetInstanceID()) {
                    //if guid does not match
                    dirty = true;
                    break;
                }
            }
        }
        
        if (dirty) {
            ReorderChildren();
        }
    }

    private void CountActiveChildren()
    {
        activeChildren = 0;
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).gameObject.activeSelf) {
                activeChildren++;
            }
        }
    }

    private void ReorderChildren()
    {
        //half span in direction of dir (i.e. positive or negative)
        Vector2 start = ((activeChildren - 1) * _spacing * -_dir) / 2f;

        for (int i = 0; i < activeChildren; i++) {
            transform.GetChild(i).transform.localPosition = start + _dir * (_spacing * i);
            
            //it is possible to move the objects, but if you don't they should keep the correct positions all the time.
            if (i < _childGuids.Count) {
                _childGuids[i] = transform.GetChild(i).gameObject.GetInstanceID();
            }
            else {
                _childGuids.Add(transform.GetChild(i).gameObject.GetInstanceID());
            }
        }
        
        //keep removing last guid untill list lengths match.
        for (int i = activeChildren; i < _childGuids.Count;) {
            _childGuids.RemoveAt(i);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ObjectLayoutgroup))]
    private class ObjectLayoutgroupEditor : Editor
    {
        private enum ObjLayoutDir
        {
            Custom = 0,
            HorizontalPositive,
            HorizontalNegative,
            VerticalPositive,
            VerticalNegative,
        }

        private ObjLayoutDir _editorDir;

        public override void OnInspectorGUI()
        {
            bool dirty = false;
            bool redraw = false;
            
            ObjectLayoutgroup instance = (ObjectLayoutgroup) target;
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            redraw = EditorGUI.EndChangeCheck();

            if (instance._dir.magnitude < 0.0001f) {
                instance._dir = new Vector2(1, 0);
                _editorDir = ObjLayoutDir.HorizontalPositive;

                dirty = true;
            }

            EditorGUI.BeginChangeCheck();
            _editorDir = (ObjLayoutDir) EditorGUILayout.EnumPopup("direction", _editorDir);
            if (EditorGUI.EndChangeCheck()) {
                instance._dir = GetDirVector(_editorDir, instance._dir);

                dirty = true;
            }

            if (_editorDir == ObjLayoutDir.Custom) {
                EditorGUI.BeginChangeCheck();
                instance._dir = EditorGUILayout.Vector2Field("direction (normalized)", instance._dir);

                dirty = EditorGUI.EndChangeCheck();
            }

            if (GUILayout.Button("Reorder")) {
                redraw = true;
            }

            if (dirty) {
                redraw = true;
                instance._dir = instance._dir.normalized;
                EditorUtility.SetDirty(instance);
            }

            if (redraw) {
                instance.ReorderChildren();
            }
        }

        private Vector2 GetDirVector(ObjLayoutDir dirEn, Vector2 current)
        {
            switch (dirEn) {
                case ObjLayoutDir.HorizontalPositive:
                    return new Vector2(1, 0);
                case ObjLayoutDir.HorizontalNegative:
                    return new Vector2(-1, 0);
                case ObjLayoutDir.VerticalPositive:
                    return new Vector2(0, 1);
                case ObjLayoutDir.VerticalNegative:
                    return new Vector2(0, -1);
                case ObjLayoutDir.Custom:
                default:
                    return current;
            }
        }
    }
#endif
}
