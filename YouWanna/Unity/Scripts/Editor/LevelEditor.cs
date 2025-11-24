#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using IWanna.Level;

namespace IWanna.Editor
{
    public class LevelEditor : EditorWindow
    {
        private LevelData currentLevel;
        private Tilemap tilemap;
        private TileBase selectedTile;
        private LevelData.TileType selectedTileType = LevelData.TileType.Ground;
        private LevelData.HazardType selectedHazardType = LevelData.HazardType.SpikeUp;
        
        private enum EditMode
        {
            Tiles,
            Hazards,
            SavePoints,
            PlayerSpawn
        }
        
        private EditMode currentEditMode = EditMode.Tiles;
        private Vector2 scrollPosition;
        
        [MenuItem("I Wanna/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditor>("I Wanna Level Editor");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("I Wanna Level Editor", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            // Level Data Selection
            DrawLevelDataSection();
            
            if (currentLevel == null)
                return;
            
            EditorGUILayout.Space();
            
            // Edit Mode Selection
            DrawEditModeSection();
            
            EditorGUILayout.Space();
            
            // Tool-specific UI
            switch (currentEditMode)
            {
                case EditMode.Tiles:
                    DrawTileEditingSection();
                    break;
                case EditMode.Hazards:
                    DrawHazardEditingSection();
                    break;
                case EditMode.SavePoints:
                    DrawSavePointSection();
                    break;
                case EditMode.PlayerSpawn:
                    DrawPlayerSpawnSection();
                    break;
            }
            
            EditorGUILayout.Space();
            
            // Level Actions
            DrawLevelActionsSection();
        }
        
        private void DrawLevelDataSection()
        {
            GUILayout.Label("Level Data", EditorStyles.boldLabel);
            
            LevelData newLevel = (LevelData)EditorGUILayout.ObjectField(
                "Current Level", currentLevel, typeof(LevelData), false);
            
            if (newLevel != currentLevel)
            {
                currentLevel = newLevel;
                FindTilemap();
            }
            
            if (GUILayout.Button("Create New Level"))
            {
                CreateNewLevel();
            }
        }
        
        private void DrawEditModeSection()
        {
            GUILayout.Label("Edit Mode", EditorStyles.boldLabel);
            currentEditMode = (EditMode)GUILayout.Toolbar((int)currentEditMode, 
                System.Enum.GetNames(typeof(EditMode)));
        }
        
        private void DrawTileEditingSection()
        {
            GUILayout.Label("Tile Editing", EditorStyles.boldLabel);
            
            selectedTile = (TileBase)EditorGUILayout.ObjectField(
                "Selected Tile", selectedTile, typeof(TileBase), false);
            
            selectedTileType = (LevelData.TileType)EditorGUILayout.EnumPopup(
                "Tile Type", selectedTileType);
            
            if (selectedTile != null && tilemap != null)
            {
                EditorGUILayout.HelpBox(
                    "Click on the tilemap in the scene to place tiles.\n" +
                    "Hold Shift and click to remove tiles.", MessageType.Info);
            }
        }
        
        private void DrawHazardEditingSection()
        {
            GUILayout.Label("Hazard Editing", EditorStyles.boldLabel);
            
            selectedHazardType = (LevelData.HazardType)EditorGUILayout.EnumPopup(
                "Hazard Type", selectedHazardType);
            
            EditorGUILayout.HelpBox(
                "Click in the scene view to place hazards.\n" +
                "Hold Shift and click to remove hazards.", MessageType.Info);
        }
        
        private void DrawSavePointSection()
        {
            GUILayout.Label("Save Point Editing", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(
                "Click in the scene view to place save points.\n" +
                "Hold Shift and click to remove save points.", MessageType.Info);
            
            if (currentLevel.savePoints.Length > 0)
            {
                GUILayout.Label($"Current Save Points: {currentLevel.savePoints.Length}");
            }
        }
        
        private void DrawPlayerSpawnSection()
        {
            GUILayout.Label("Player Spawn Point", EditorStyles.boldLabel);
            
            currentLevel.playerSpawnPoint = EditorGUILayout.Vector3Field(
                "Spawn Position", currentLevel.playerSpawnPoint);
            
            if (GUILayout.Button("Set Spawn to Scene View Center"))
            {
                if (SceneView.lastActiveSceneView != null)
                {
                    currentLevel.playerSpawnPoint = SceneView.lastActiveSceneView.camera.transform.position;
                    currentLevel.playerSpawnPoint.z = 0;
                }
            }
        }
        
        private void DrawLevelActionsSection()
        {
            GUILayout.Label("Level Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Load Level to Scene"))
            {
                LoadLevelToScene();
            }
            
            if (GUILayout.Button("Save Scene to Level"))
            {
                SaveSceneToLevel();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear Level"))
            {
                if (EditorUtility.DisplayDialog("Clear Level", 
                    "Are you sure you want to clear all level data?", "Yes", "No"))
                {
                    currentLevel.ClearLevel();
                    EditorUtility.SetDirty(currentLevel);
                }
            }
            
            if (GUILayout.Button("Validate Level"))
            {
                ValidateLevel();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void CreateNewLevel()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Level", "NewLevel", "asset", "Create a new level data asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                LevelData newLevel = CreateInstance<LevelData>();
                newLevel.InitializeEmpty();
                
                AssetDatabase.CreateAsset(newLevel, path);
                AssetDatabase.SaveAssets();
                
                currentLevel = newLevel;
                Selection.activeObject = newLevel;
            }
        }
        
        private void FindTilemap()
        {
            tilemap = FindObjectOfType<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogWarning("No Tilemap found in the scene. Please create a Tilemap for level editing.");
            }
        }
        
        private void LoadLevelToScene()
        {
            if (currentLevel == null || tilemap == null)
                return;
            
            // Clear existing tiles
            tilemap.SetTilesBlock(tilemap.cellBounds, new TileBase[tilemap.cellBounds.size.x * tilemap.cellBounds.size.y * tilemap.cellBounds.size.z]);
            
            // Load tiles from level data
            foreach (var tileData in currentLevel.tileData)
            {
                tilemap.SetTile(tileData.position, tileData.tile);
            }
            
            // TODO: Load hazards and save points
            
            Debug.Log($"Loaded level '{currentLevel.levelName}' to scene");
        }
        
        private void SaveSceneToLevel()
        {
            if (currentLevel == null || tilemap == null)
                return;
            
            // Save tiles from tilemap to level data
            BoundsInt bounds = tilemap.cellBounds;
            var tileDataList = new System.Collections.Generic.List<LevelData.LevelTileData>();
            
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    TileBase tile = tilemap.GetTile(position);
                    
                    if (tile != null)
                    {
                        tileDataList.Add(new LevelData.LevelTileData
                        {
                            position = position,
                            tile = tile,
                            tileType = selectedTileType
                        });
                    }
                }
            }
            
            currentLevel.tileData = tileDataList.ToArray();
            EditorUtility.SetDirty(currentLevel);
            
            Debug.Log($"Saved scene to level '{currentLevel.levelName}'");
        }
        
        private void ValidateLevel()
        {
            if (currentLevel == null)
                return;
            
            int issues = 0;
            
            if (string.IsNullOrEmpty(currentLevel.levelName))
            {
                Debug.LogWarning("Level name is empty");
                issues++;
            }
            
            if (currentLevel.tileData.Length == 0)
            {
                Debug.LogWarning("Level has no tiles");
                issues++;
            }
            
            if (currentLevel.savePoints.Length == 0)
            {
                Debug.LogWarning("Level has no save points");
                issues++;
            }
            
            if (issues == 0)
            {
                Debug.Log($"Level '{currentLevel.levelName}' validation passed!");
            }
            else
            {
                Debug.LogWarning($"Level validation found {issues} issues");
            }
        }
        
        private void OnSceneGUI(SceneView sceneView)
        {
            if (currentLevel == null || currentEditMode == EditMode.Tiles)
                return;
            
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector3 mousePosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                mousePosition.z = 0;
                
                bool isRemoving = Event.current.shift;
                
                switch (currentEditMode)
                {
                    case EditMode.Hazards:
                        if (!isRemoving)
                        {
                            currentLevel.AddHazard(mousePosition, selectedHazardType);
                            EditorUtility.SetDirty(currentLevel);
                        }
                        break;
                        
                    case EditMode.SavePoints:
                        if (!isRemoving)
                        {
                            currentLevel.AddSavePoint(mousePosition);
                            EditorUtility.SetDirty(currentLevel);
                        }
                        break;
                        
                    case EditMode.PlayerSpawn:
                        currentLevel.playerSpawnPoint = mousePosition;
                        EditorUtility.SetDirty(currentLevel);
                        break;
                }
                
                Event.current.Use();
            }
        }
        
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}
#endif
