using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Slax.QuestSystem
{
    public class QuestManagerEditorWindow : EditorWindow
    {
        private Vector2 _scroll;
        private int _selected;

        List<string> _SOTypes;
        string[] _objectsGUIDs;
        string[] _objectsPaths;
        ScriptableObject[] _objects;

        string[] _displayObjectsGUIDs;
        List<string> _displayObjectsPaths;
        List<ScriptableObject> _displayObjects;

        [SerializeField] private QuestManagerEditorWindowSO _editorData;

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

        [MenuItem("Tools/Slax/Quest Manager")]
        private static void ShowWindow()
        {
            GetWindow<QuestManagerEditorWindow>(false, "Quest Manager", true);
        }

        void OnGUI()
        {
            SerializedObject editorData = new SerializedObject(_editorData);
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
            GUILayout.Box("The Quest Manager helps create and manage all asset scriptable objects related to Quests in the project.");
            EditorGUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(130));

            string[] tabs = { "Asset Creator", "Asset Finder" };

            for (int i = 0; i < tabs.Length; i++)
            {
                bool tabSelected = GUILayout.Button(tabs[i]);
                if (tabSelected) editorData.FindProperty("LastSelectedTab").intValue = i;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            if (_editorData.LastSelectedTab == 0)
            {
                EditorGUILayout.LabelField("Quest asset creator", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("When creating Quests it is recommended to follow the following pattern in order to keep things easy to maintain : QuestLineX_QuestX_StepX (shorter : QLX_QX_SX) where X is a number.", MessageType.Info);
                DrawAssetCreator("QuestLine", editorData);
                DrawAssetCreator("Quest", editorData);
                DrawAssetCreator("QuestStep", editorData);

                DrawQuestConfigs();
            }
            else
            {
                EditorGUILayout.LabelField("Quest asset finder", EditorStyles.boldLabel);
                EditorGUILayout.Space(15);

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

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            editorData.ApplyModifiedProperties();
            EditorUtility.SetDirty(_editorData);
        }

        void DrawAssetCreator(string assetType, SerializedObject editorData)
        {
            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField($"Path for {assetType} asset to be created.");

            switch (assetType)
            {
                case "QuestLine":
                    EditorGUILayout.PropertyField(editorData.FindProperty("QuestLineAssetPath"));
                    break;
                case "Quest":
                    EditorGUILayout.PropertyField(editorData.FindProperty("QuestAssetPath"));
                    break;
                case "QuestStep":
                    EditorGUILayout.PropertyField(editorData.FindProperty("QuestStepAssetPath"));
                    break;
                default:
                    break;
            }

            ScriptableObject createdAsset = null;

            switch (assetType)
            {
                case "QuestLine":
                    if (GUILayout.Button(new GUIContent("Create Quest Line", "Creates a new Quest Line asset Scriptable Object"), GUILayout.Height(20)))
                    {
                        createdAsset = QuestAssetCreator.CreateQuestLine(_editorData.QuestLineAssetPath);
                    }
                    break;
                case "Quest":
                    if (GUILayout.Button(new GUIContent("Create Quest", "Creates a new Quest asset Scriptable Object"), GUILayout.Height(20)))
                    {
                        createdAsset = QuestAssetCreator.CreateQuest(_editorData.QuestAssetPath);
                    }
                    break;
                case "QuestStep":
                    if (GUILayout.Button(new GUIContent("Create Quest Step", "Creates a new Quest Step asset Scriptable Object"), GUILayout.Height(20)))
                    {
                        createdAsset = QuestAssetCreator.CreateQuestStep(_editorData.QuestStepAssetPath);
                    }
                    break;
            }

            if (createdAsset)
            {
                // editorData.FindProperty("LastCreatedAsset").objectReferenceValue = createdAsset;
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(createdAsset);
            }

            // EditorGUILayout.BeginVertical();

            // if (_editorData.LastCreatedAsset != null)
            // {
            //     if (GUILayout.Button("Locate last asset"))
            //     {
            //         EditorUtility.FocusProjectWindow();
            //         EditorGUIUtility.PingObject(_editorData.LastCreatedAsset);
            //     }
            // }

            // EditorGUILayout.EndVertical();
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

        void DrawQuestConfigs()
        {
            string[] questLinesGuids = AssetDatabase.FindAssets("t:Slax.QuestSystem.QuestLineSO") as string[];
            if (questLinesGuids.Length == 0)
            {
                EditorGUILayout.LabelField("No Quest Lines created");
                return;
            }

            for (int i = 0; i < questLinesGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(_objectsGUIDs[i]);
                QuestLineSO questLine = (QuestLineSO)AssetDatabase.LoadAssetAtPath(_objectsPaths[i], typeof(QuestLineSO));
                DrawQuestLineInfo(questLine);
            }
        }

        void DrawQuestLineInfo(QuestLineSO questLine)
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField($"{questLine.Name} - {questLine.name}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Quests: {questLine.Quests.Count}");
            if (questLine.Quests.Count > 0)
            {
                EditorGUI.indentLevel = 1;
                questLine.Quests.ForEach((quest) =>
                {
                    string questPrefix = quest.Completed ? "O" : "X";
                    EditorGUILayout.LabelField($"{questPrefix} {quest.Name}");

                    EditorGUI.indentLevel = 2;
                    quest.Steps.ForEach((step) =>
                    {
                        string stepPrefix = step.Completed ? "O" : "X";
                        EditorGUILayout.LabelField($"{stepPrefix} {step.Name}");
                    });
                    EditorGUI.indentLevel = 1;
                });
                EditorGUI.indentLevel = 0;
            }
        }

        void DrawSOsList()
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _displayObjectsGUIDs.Length; i++)
            {
                GUILayout.Label(i + 1 + ". " + _displayObjects[i].name);

                if (GUILayout.Button("Locate"))
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
                    _SOTypes.Add(_objects[i].GetType().ToString().Split('.')[2]);
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
