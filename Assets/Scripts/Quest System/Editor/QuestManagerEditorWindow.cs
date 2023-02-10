using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Slax.QuestSystem
{
    public class QuestManagerEditorWindow : EditorWindow
    {
        [Header("Editor Window Related")]
        private Vector2 _scroll;
        private int _selected;

        List<string> _SOTypes;
        string[] _objectsGUIDs;
        string[] _objectsPaths;
        ScriptableObject[] _objects;

        string[] _displayObjectsGUIDs;
        List<string> _displayObjectsPaths;
        List<ScriptableObject> _displayObjects;

        readonly string[] KNOWN_TYPES = { "Slax.QuestSystem.QuestLineSO", "Slax.QuestSystem.QuestSO", "Slax.QuestSystem.QuestStepSO" };

        private void OnEnable()
        {
            FindAllSOs();
            FindDisplaySOs();
        }

        void OnFocus()
        {
            FindAllSOs();
            FindDisplaySOs();
        }

        [MenuItem("Slax/QuestSystem/QuestManager")]
        private static void ShowWindow()
        {
            GetWindow<QuestManagerEditorWindow>(false, "QuestManager", true);
        }

        void OnGUI()
        {
            if (GUILayout.Button(new GUIContent("Create Quest Line", "Creates a new Quest Line at defined Quest Line Asset Path"), GUILayout.Height(20)))
            {
                QuestAssetCreator.CreateQuestLine("");
            }
            if (GUILayout.Button(new GUIContent("Create Quest", "Creates a new Quest at defined Quest Asset Path"), GUILayout.Height(20)))
            {
                QuestAssetCreator.CreateQuest("");
            }
            if (GUILayout.Button(new GUIContent("Create Quest Step", "Creates a new Quest Step at Quest Step asset path"), GUILayout.Height(20)))
            {
                QuestAssetCreator.CreateQuestStep("");
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);

            GUILayout.BeginHorizontal();

            DrawSOsPicker();
            if (GUILayout.Button("Refresh All"))
            {
                FindAllSOs();
                FindDisplaySOs();
            }

            GUILayout.EndHorizontal();

            DrawSOsList();
        }

        void DrawSOsPicker()
        {
            EditorGUI.BeginChangeCheck();
            _selected = EditorGUILayout.Popup(GUIContent.none, _selected, _SOTypes.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                FindDisplaySOs();
            }
        }

        void DrawSOsList()
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _displayObjectsGUIDs.Length; i++)
            {
                GUILayout.Label(i + 1 + ". " + _displayObjects[i].name);

                if (GUILayout.Button("Locate Quickly"))
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(_displayObjects[i]);
                }

                GUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            GUILayout.EndScrollView();
        }

        void FindAllSOs()
        {
            _objectsGUIDs = AssetDatabase.FindAssets("t:ScriptableObject") as string[];

            _objectsPaths = new string[_objectsGUIDs.Length];
            _objects = new ScriptableObject[_objectsGUIDs.Length];

            _SOTypes = new List<string>();

            for (int i = 0; i < _objectsGUIDs.Length; i++)
            {
                _objectsPaths[i] = AssetDatabase.GUIDToAssetPath(_objectsGUIDs[i]);
                _objects[i] = (ScriptableObject)AssetDatabase.LoadAssetAtPath(_objectsPaths[i], typeof(ScriptableObject));
            }

            for (int i = 0; i < _objects.Length; i++)
            {
                if (_SOTypes.IndexOf(_objects[i].GetType().ToString()) == -1)
                {
                    if (!KNOWN_TYPES.Contains(_objects[i].GetType().ToString())) continue;
                    _SOTypes.Add(_objects[i].GetType().ToString());
                }
            }
        }

        void FindDisplaySOs()
        {
            if (_displayObjects != null)
            {
                _displayObjects.Clear();
            }
            if (_displayObjectsPaths != null)
            {
                _displayObjectsPaths.Clear();
            }

            string type = _SOTypes[_selected];
            string queryString = "t:" + type;

            _displayObjectsGUIDs = AssetDatabase.FindAssets(queryString);

            _displayObjectsPaths = new List<string>(_displayObjectsGUIDs.Length);
            _displayObjects = new List<ScriptableObject>(_displayObjectsGUIDs.Length);

            for (int i = 0; i < _displayObjectsGUIDs.Length; i++)
            {
                _displayObjectsPaths.Add(AssetDatabase.GUIDToAssetPath(_displayObjectsGUIDs[i]));
                _displayObjects.Add(AssetDatabase.LoadAssetAtPath(_displayObjectsPaths[i], typeof(ScriptableObject)) as ScriptableObject);
            }
        }
    }
}
