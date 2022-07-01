using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BayatGames.Utilities.Editor
{

    /// <summary>
    /// Scene manager window, an editor window for managing scenes.
    /// </summary>
    public class SceneManagerWindow : EditorWindow
    {

        public enum ScenesSource
        {
            BuildSettings,
            Project,
            Manual
        }

        protected Vector2 scrollPosition;
        protected Vector2 scenesTabScrollPosition;
        protected ScenesSource scenesSource = ScenesSource.BuildSettings;
        protected NewSceneSetup newSceneSetup = NewSceneSetup.DefaultGameObjects;
        protected NewSceneMode newSceneMode = NewSceneMode.Single;
        protected OpenSceneMode openSceneMode = OpenSceneMode.Single;
        protected bool showPath = false;
        protected bool showAddToBuild = true;
        protected bool askBeforeDelete = true;
        protected bool[] selectedScenes;
        protected string[] guids;
        protected int selectedTab = 0;
        protected string[] tabs = new string[] {
            "Scenes",
            "Settings"
        };
        protected string lastScene;
        protected string searchFolder = "Assets";

        [MenuItem("Tools/Scene Manager")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<SceneManagerWindow>("Scene Manager");
            window.minSize = new Vector2(400f, 200f);
            window.Show();
        }

        protected virtual void PlayModeStateChanged(PlayModeStateChange state)
        {
            Debug.Log(state);
            Debug.Log(this.lastScene);
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    if (!string.IsNullOrEmpty(this.lastScene))
                    {
                        Open(this.lastScene);
                        this.lastScene = null;
                    }
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
            this.scenesSource = (ScenesSource)EditorPrefs.GetInt(
                "SceneManager.scenesSource",
                (int)ScenesSource.BuildSettings);
            this.searchFolder = EditorPrefs.GetString("SceneManager.searchFolder", "Assets");
            this.newSceneSetup = (NewSceneSetup)EditorPrefs.GetInt(
                "SceneManager.newSceneSetup",
                (int)NewSceneSetup.DefaultGameObjects);
            this.newSceneMode = (NewSceneMode)EditorPrefs.GetInt("SceneManager.newSceneMode", (int)NewSceneMode.Single);
            this.openSceneMode = (OpenSceneMode)EditorPrefs.GetInt(
                "SceneManager.openSceneMode",
                (int)OpenSceneMode.Single);
            this.showPath = EditorPrefs.GetBool("SceneManager.showPath", false);
            this.showAddToBuild = EditorPrefs.GetBool("SceneManager.showAddToBuild", true);
            this.askBeforeDelete = EditorPrefs.GetBool("SceneManager.askBeforeDelete", true);
        }

        protected virtual void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            EditorPrefs.SetInt("SceneManager.scenesSource", (int)this.scenesSource);
            EditorPrefs.SetString("SceneManager.searchFolder", this.searchFolder);
            EditorPrefs.SetInt("SceneManager.newSceneSetup", (int)this.newSceneSetup);
            EditorPrefs.SetInt("SceneManager.newSceneMode", (int)this.newSceneMode);
            EditorPrefs.SetInt("SceneManager.openSceneMode", (int)this.openSceneMode);
            EditorPrefs.SetBool("SceneManager.showPath", this.showPath);
            EditorPrefs.SetBool("SceneManager.showAddToBuild", this.showAddToBuild);
            EditorPrefs.SetBool("SceneManager.askBeforeDelete", this.askBeforeDelete);
        }

        protected virtual void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            this.selectedTab = GUILayout.Toolbar(this.selectedTab, this.tabs, EditorStyles.toolbarButton);
            EditorGUILayout.EndHorizontal();
            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            EditorGUILayout.BeginVertical();
            switch (this.selectedTab)
            {
                case 0:
                    ScenesTabGUI();
                    break;
                case 1:
                    SettingsTabGUI();
                    break;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.Label("Made with ❤️ by Bayat Games", EditorStyles.centeredGreyMiniLabel);
        }

        protected virtual void SettingsTabGUI()
        {
            this.scenesSource = (ScenesSource)EditorGUILayout.EnumPopup("Scenes Source", this.scenesSource);
            if (this.scenesSource == ScenesSource.Manual)
            {
                this.searchFolder = EditorGUILayout.TextField("Search Folder", this.searchFolder);
            }
            this.newSceneSetup = (NewSceneSetup)EditorGUILayout.EnumPopup("New Scene Setup", this.newSceneSetup);
            this.newSceneMode = (NewSceneMode)EditorGUILayout.EnumPopup("New Scene Mode", this.newSceneMode);
            this.openSceneMode = (OpenSceneMode)EditorGUILayout.EnumPopup("Open Scene Mode", this.openSceneMode);
            this.showPath = EditorGUILayout.Toggle("Show Path", this.showPath);
            this.showAddToBuild = EditorGUILayout.Toggle("Show Add To Build", this.showAddToBuild);
            this.askBeforeDelete = EditorGUILayout.Toggle("Ask Before Delete", this.askBeforeDelete);
        }

        protected virtual void ScenesTabGUI()
        {
            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            this.guids = AssetDatabase.FindAssets("t:Scene");
            if (this.selectedScenes == null || this.selectedScenes.Length != guids.Length)
            {
                this.selectedScenes = new bool[guids.Length];
            }
            this.scenesTabScrollPosition = EditorGUILayout.BeginScrollView(this.scenesTabScrollPosition);
            EditorGUILayout.BeginVertical();
            if (guids.Length == 0)
            {
                GUILayout.Label("No Scenes Found", EditorStyles.centeredGreyMiniLabel);
                GUILayout.Label("Create New Scenes", EditorStyles.centeredGreyMiniLabel);
                GUILayout.Label("And Manage them here", EditorStyles.centeredGreyMiniLabel);
            }
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                EditorBuildSettingsScene buildScene = buildScenes.Find((editorBuildScene) =>
                {
                    return editorBuildScene.path == path;
                });
                Scene scene = SceneManager.GetSceneByPath(path);
                bool isOpen = scene.IsValid() && scene.isLoaded;
                switch (this.scenesSource)
                {
                    case ScenesSource.BuildSettings:
                        if (buildScene == null)
                        {
                            continue;
                        }
                        break;
                    case ScenesSource.Manual:
                        if (!path.Contains(this.searchFolder))
                        {
                            continue;
                        }
                        break;
                }
                EditorGUILayout.BeginHorizontal();
                this.selectedScenes[i] = EditorGUILayout.Toggle(this.selectedScenes[i], GUILayout.Width(15));
                if (isOpen)
                {
                    GUILayout.Label(sceneAsset.name, EditorStyles.whiteLabel);
                }
                else
                {
                    GUILayout.Label(sceneAsset.name, EditorStyles.wordWrappedLabel);
                }
                if (this.showPath)
                {
                    GUILayout.Label(path, EditorStyles.wordWrappedLabel);
                }
                if (this.showAddToBuild)
                {
                    if (GUILayout.Button(buildScene == null ? "+ Build" : "- Build", GUILayout.Width(60)))
                    {
                        if (buildScene == null)
                        {
                            AddToBuild(path);
                        }
                        else
                        {
                            RemoveFromBuild(path);
                        }
                    }
                }
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                if (GUILayout.Button("Play", GUILayout.Width(50)))
                {
                    this.lastScene = EditorSceneManager.GetActiveScene().path;
                    Open(path);
                    EditorApplication.isPlaying = true;
                }
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button(isOpen ? "Close" : "Open", GUILayout.Width(50)))
                {
                    if (isOpen)
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                    else
                    {
                        Open(path);
                    }
                }
                if (GUILayout.Button("Delete", GUILayout.Width(50)))
                {
                    Delete(path);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Create New Scene"))
            {
                Scene newScene = EditorSceneManager.NewScene(this.newSceneSetup, this.newSceneMode);
                EditorSceneManager.SaveScene(newScene);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Bulk Actions", EditorStyles.boldLabel);
            bool anySelected = false;
            for (int i = 0; i < this.selectedScenes.Length; i++)
            {
                anySelected |= this.selectedScenes[i];
            }
            EditorGUI.BeginDisabledGroup(!anySelected);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete"))
            {
                for (int i = 0; i < this.selectedScenes.Length; i++)
                {
                    if (this.selectedScenes[i])
                    {
                        Delete(AssetDatabase.GUIDToAssetPath(this.guids[i]));
                    }
                }
            }
            if (GUILayout.Button("Open Additive"))
            {
                OpenSceneMode openMode = this.openSceneMode;
                this.openSceneMode = OpenSceneMode.Additive;
                for (int i = 0; i < this.selectedScenes.Length; i++)
                {
                    if (this.selectedScenes[i])
                    {
                        Open(AssetDatabase.GUIDToAssetPath(this.guids[i]));
                    }
                }
                this.openSceneMode = openMode;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            GUILayout.Label("General Actions", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Modified Scenes"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }
            if (GUILayout.Button("Save Open Scenes"))
            {
                EditorSceneManager.SaveOpenScenes();
            }
            EditorGUILayout.EndHorizontal();
        }

        public virtual void Open(string path)
        {
            if (EditorSceneManager.EnsureUntitledSceneHasBeenSaved("You don't have saved the Untitled Scene, Do you want to leave?"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(path, this.openSceneMode);
            }
        }

        public virtual void Delete(string path)
        {
            if (!askBeforeDelete || EditorUtility.DisplayDialog(
                     "Delete Scene",
                     string.Format(
                         "Are you sure you want to delete the below scene: {0}",
                         path),
                     "Delete",
                     "Cancel"))
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();
            }
        }

        public virtual void AddToBuild(string path)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        public virtual void RemoveFromBuild(string path)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.RemoveAll(scene =>
            {
                return scene.path == path;
            });
            EditorBuildSettings.scenes = scenes.ToArray();
        }

    }

}