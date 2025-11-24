using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace IWanna.Level
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "I Wanna/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Information")]
        public string levelName = "Untitled Level";
        public string description = "";
        public Sprite levelThumbnail;
        
        [Header("Level Settings")]
        public Vector2Int levelSize = new Vector2Int(50, 30);
        public Vector3 playerSpawnPoint = Vector3.zero;
        public Color backgroundColor = Color.cyan;
        
        [Header("Tile Data")]
        public TileBase[] availableTiles;
        public LevelTileData[] tileData;
        
        [Header("Hazards and Objects")]
        public HazardData[] hazards;
        public SavePointData[] savePoints;
        
        [System.Serializable]
        public class LevelTileData
        {
            public Vector3Int position;
            public TileBase tile;
            public TileType tileType;
        }
        
        [System.Serializable]
        public class HazardData
        {
            public Vector3 position;
            public HazardType hazardType;
            public Vector3 rotation;
        }
        
        [System.Serializable]
        public class SavePointData
        {
            public Vector3 position;
            public bool isActive;
        }
        
        public enum TileType
        {
            Ground,
            Wall,
            Platform,
            Decoration
        }
        
        public enum HazardType
        {
            SpikeUp,
            SpikeDown,
            SpikeLeft,
            SpikeRight,
            MovingPlatform,
            Crusher
        }
        
        public void InitializeEmpty()
        {
            tileData = new LevelTileData[0];
            hazards = new HazardData[0];
            savePoints = new SavePointData[0];
        }
        
        public void AddTile(Vector3Int position, TileBase tile, TileType type)
        {
            var newTileData = new LevelTileData
            {
                position = position,
                tile = tile,
                tileType = type
            };
            
            // Add to array (in a real implementation, you'd use a List)
            var tileList = new List<LevelTileData>(tileData);
            tileList.Add(newTileData);
            tileData = tileList.ToArray();
        }
        
        public void RemoveTile(Vector3Int position)
        {
            var tileList = new List<LevelTileData>(tileData);
            tileList.RemoveAll(t => t.position == position);
            tileData = tileList.ToArray();
        }
        
        public LevelTileData GetTileAt(Vector3Int position)
        {
            foreach (var tile in tileData)
            {
                if (tile.position == position)
                    return tile;
            }
            return null;
        }
        
        public void AddHazard(Vector3 position, HazardType type, Vector3 rotation = default)
        {
            var newHazard = new HazardData
            {
                position = position,
                hazardType = type,
                rotation = rotation
            };
            
            var hazardList = new List<HazardData>(hazards);
            hazardList.Add(newHazard);
            hazards = hazardList.ToArray();
        }
        
        public void AddSavePoint(Vector3 position)
        {
            var newSavePoint = new SavePointData
            {
                position = position,
                isActive = true
            };
            
            var savePointList = new List<SavePointData>(savePoints);
            savePointList.Add(newSavePoint);
            savePoints = savePointList.ToArray();
        }
        
        public bool IsValidPosition(Vector3Int position)
        {
            return position.x >= 0 && position.x < levelSize.x &&
                   position.y >= 0 && position.y < levelSize.y;
        }
        
        public void ClearLevel()
        {
            tileData = new LevelTileData[0];
            hazards = new HazardData[0];
            savePoints = new SavePointData[0];
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Validate Level Data")]
        private void ValidateLevelData()
        {
            Debug.Log($"Level: {levelName}");
            Debug.Log($"Size: {levelSize}");
            Debug.Log($"Tiles: {tileData.Length}");
            Debug.Log($"Hazards: {hazards.Length}");
            Debug.Log($"Save Points: {savePoints.Length}");
        }
        #endif
    }
}
