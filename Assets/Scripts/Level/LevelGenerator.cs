using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    [DisallowMultipleComponent]
    public sealed class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject manualLevel;
        [SerializeField] private GameObject[] tilePrefabs = new GameObject[9];

        public GameObject GeneratedLevel { get; private set; }
        public LevelMapLayout GeneratedLayout { get; private set; }
        public event Action<LevelMapLayout> LevelGenerated;

        // Replace this quadrant to test another layout using the same tile legend.
        public int[,] levelMap =
        {
            {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
            {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
            {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
            {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
            {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
            {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
            {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
            {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
            {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
            {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
            {0,0,0,0,0,0,5,0,0,0,4,0,0,0}
        };

        public LevelMapLayout CreateLayout() => new LevelMapLayout(levelMap);

        private void Start()
        {
            try { GenerateLevel(); }
            catch (Exception exception)
            {
                Debug.LogError("Level generation failed: " + exception.Message, this);
                enabled = false;
            }
        }

        public void GenerateLevel()
        {
            if (!Application.isPlaying)
                throw new InvalidOperationException("Level generation is available only during Play.");
            var layout = CreateLayout();
            int[,] angles = WallOrientationSolver.Solve(layout);
            for (int row = 0; row < layout.SourceRows; row++)
            for (int column = 0; column < layout.SourceColumns; column++)
            {
                int category = layout.SourceCategory(row, column);
                if (category != 0 && (tilePrefabs == null || category >= tilePrefabs.Length || !tilePrefabs[category]))
                    throw new InvalidOperationException($"Missing prefab for tile category {category}.");
            }

            var candidate = new GameObject("Level01_Generated");
            candidate.SetActive(false);
            try
            {
                BuildQuadrant(candidate.transform, layout, angles, false, false);
                BuildQuadrant(candidate.transform, layout, angles, true, false);
                BuildQuadrant(candidate.transform, layout, angles, false, true);
                BuildQuadrant(candidate.transform, layout, angles, true, true);
            }
            catch
            {
                Destroy(candidate);
                throw;
            }

            // Keep the saved manual scene intact; these changes exist only during Play.
            if (manualLevel)
            {
                manualLevel.SetActive(false);
                Destroy(manualLevel);
            }
            if (GeneratedLevel)
            {
                GeneratedLevel.SetActive(false);
                Destroy(GeneratedLevel);
            }
            GeneratedLayout = layout;
            GeneratedLevel = candidate;
            candidate.SetActive(true);
            LevelGenerated?.Invoke(layout);
        }

        private void BuildQuadrant(Transform parent, LevelMapLayout layout, int[,] angles, bool reflectX, bool reflectY)
        {
            string name = (reflectY ? "Bottom" : "Top") + (reflectX ? "Right" : "Left");
            var quadrant = new GameObject(name).transform;
            quadrant.SetParent(parent, false);
            quadrant.localPosition = layout.QuadrantOrigin(reflectX, reflectY);
            quadrant.localScale = LevelMapLayout.Reflection(reflectX, reflectY);
            for (int row = 0; row < layout.QuadrantRowCount(reflectY); row++)
            {
                var rowGroup = new GameObject($"Row{row:00}").transform;
                rowGroup.SetParent(quadrant, false);
                for (int column = 0; column < layout.SourceColumns; column++)
                {
                    int category = layout.SourceCategory(row, column);
                    if (category == 0) continue;
                    GameObject prefab = tilePrefabs[category];
                    var tile = Instantiate(prefab, rowGroup, false);
                    tile.name = $"R{row:00}_C{column:00}_{prefab.name}";
                    tile.transform.localPosition = LevelMapLayout.CellPosition(row, column);
                    tile.transform.localRotation = Quaternion.Euler(0, 0, angles[row, column]);
                }
            }
        }
    }
}
