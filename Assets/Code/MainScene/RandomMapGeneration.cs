using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class RandomMapGeneration : MonoBehaviour
{
    public static RandomMapGeneration Instance;

    public Tile WallTile;
    public Tile TreasureChestTile;

    Tilemap tilemap;
    public Dictionary<Vector3Int, Tile> TileMaps;
    HashSet<Vector3Int> safeZoneBoundary; // �߾� �� ���� ��� ����

    const int MAP_WIDTH = 200;
    const int MAP_HEIGHT = 200;
    const int CELLULAR_ITERATIONS = 20;
    readonly int safeZoneRadius = 20;
    readonly int safeZoneRadiusSquared; // �߾� �� ���� �������� ����
    readonly int outerSafeZoneRadius;  // �߾� �� ���� ������
    readonly int outerSafeZoneRadiusSquared; // �߾� �� ���� �������� ����
    readonly int mapCenterX = MAP_WIDTH / 2;
    readonly int mapCenterY = MAP_HEIGHT / 2;

    // ����
    static readonly Vector3Int[] adjacentDirections = 
    {
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, 1, 0),
        new(0, -1, 0)
    };

    readonly System.Random random = new();

    public RandomMapGeneration()
    {
        // ������ ���� ������ �ʱ�ȭ
        safeZoneRadiusSquared = safeZoneRadius * safeZoneRadius;
        outerSafeZoneRadius = safeZoneRadius + 5;
        outerSafeZoneRadiusSquared = outerSafeZoneRadius * outerSafeZoneRadius;

        TileMaps = new Dictionary<Vector3Int, Tile>(MAP_WIDTH * MAP_HEIGHT);
        safeZoneBoundary = new HashSet<Vector3Int>();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        tilemap = GetComponent<Tilemap>();
    }

    void Start()
    {
        // ���� �ʱ�ȭ�ϰ�, �߾� �� ������ �����ϰ�, ���귯 ���丶Ÿ�� �����ϰ�, ���ڸ� ��ġ��
        InitializeRandomMap();
        CreateSafeZone();
        ApplyCellularAutomata();
        PrintMap();
        PlaceTreasureChests();
    }

    void InitializeRandomMap()
    {
        Vector3Int tilePosition = new();
        // �� ��ü�� ���� Ÿ�� ����
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                tilePosition.x = x;
                tilePosition.y = y;

                // �����ϰ� �� Ÿ�� �Ǵ� null�� �Ҵ�
                TileMaps[tilePosition] = random.NextDouble() < 0.5 ? WallTile : null;
            }
        }
    }

    void ApplyCellularAutomata()
    {
        var nextIterationTiles = new Dictionary<Vector3Int, Tile>(MAP_WIDTH * MAP_HEIGHT);
        Vector3Int tilePosition = new();

        // ������ Ƚ����ŭ ���귯 ���丶Ÿ ����
        for (int iteration = 0; iteration < CELLULAR_ITERATIONS; iteration++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                for (int y = 0; y < MAP_HEIGHT; y++)
                {
                    tilePosition.x = x;
                    tilePosition.y = y;

                    // �߾� �� ���� ��踦 �����ϰ� ���� ��ֹ� ���� ���� Ÿ�� ����
                    if (!safeZoneBoundary.Contains(tilePosition))
                    {
                        int adjacentObstacles = CountAdjacentObstacles(x, y);
                        nextIterationTiles[tilePosition] = adjacentObstacles > 5 ? WallTile :
                            adjacentObstacles < 3 ? null :
                            TileMaps[tilePosition];
                    }
                    else
                    {
                        nextIterationTiles[tilePosition] = TileMaps[tilePosition];
                    }
                }
            }

            // �� �ݺ� �� Ÿ�ϸ� ��ü
            Dictionary<Vector3Int, Tile> tempTiles = TileMaps;
            TileMaps = nextIterationTiles;
            nextIterationTiles = tempTiles;
        }
    }

    int CountAdjacentObstacles(int centerX, int centerY)
    {
        int obstacleCount = 0;
        Vector3Int checkPosition = new();

        // ������ ��ġ �ֺ� 3x3 ������ ��ֹ� ������ ��
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if (x < 0 || x >= MAP_WIDTH || y < 0 || y >= MAP_HEIGHT)
                {
                    obstacleCount++;
                }
                else if (x != centerX || y != centerY)
                {
                    checkPosition.x = x;
                    checkPosition.y = y;
                    obstacleCount += TileMaps[checkPosition] == WallTile ? 1 : 0;
                }
            }
        }
        return obstacleCount;
    }

    void CreateSafeZone()
    {
        Vector3Int tilePosition = new();

        // �� �߾ӿ� �� ������ ����
        for (int y = -outerSafeZoneRadius; y <= outerSafeZoneRadius; y++)
        {
            for (int x = -outerSafeZoneRadius; x <= outerSafeZoneRadius; x++)
            {
                int mapX = mapCenterX + x;
                int mapY = mapCenterY + y;

                if (mapX >= 0 && mapX < MAP_WIDTH && mapY >= 0 && mapY < MAP_HEIGHT)
                {
                    tilePosition.x = mapX;
                    tilePosition.y = mapY;

                    int distanceSquared = x * x + y * y;

                    // �߾� �� ���� ������ ���� Ÿ���� null�� ����
                    if (distanceSquared < safeZoneRadiusSquared)
                    {
                        TileMaps[tilePosition] = null;
                    }
                    // �߾� �� ������ �Ϻ� Ÿ���� �����ϰ� ���
                    else if (distanceSquared < outerSafeZoneRadiusSquared && random.NextDouble() < 0.4)
                    {
                        TileMaps[tilePosition] = null;
                    }

                    // �߾� �� ���� ��迡 �ش��ϴ� Ÿ���� ǥ��
                    if (distanceSquared >= (safeZoneRadius - 1) * (safeZoneRadius - 1)
                        && distanceSquared < safeZoneRadiusSquared)
                    {
                        TileMaps[tilePosition] = WallTile;
                        safeZoneBoundary.Add(tilePosition);
                    }
                }
            }
        }
    }

    public bool IsValidPosition(Vector3Int position)
    {
        // ��ġ�� �� ���� ���� �ְ� Ÿ���� �Ҵ�Ǿ����� Ȯ��
        return position.x >= 0 && position.x < MAP_WIDTH &&
               position.y >= 0 && position.y < MAP_HEIGHT &&
               TileMaps.ContainsKey(position);
    }

    public Tile GetTileAt(Vector3Int position)
    {
        // Ư�� ��ġ�� Ÿ���� ��ȯ (�����ϴ� ���)
        return TileMaps.TryGetValue(position, out Tile tile) ? tile : null;
    }

    public void SetTileAt(Vector3Int position, Tile tile)
    {
        // ��ġ�� ��ȿ�� ��� Ư�� ��ġ�� Ÿ���� ����
        if (IsValidPosition(position))
        {
            TileMaps[position] = tile;
            tilemap.SetTile(position, tile);
        }
    }

    void PrintMap()
    {
        tilemap.ClearAllTiles();
        foreach (var tileData in TileMaps)
        {
            if (tileData.Value != null)
            {
                tilemap.SetTile(tileData.Key, tileData.Value);
            }
        }
    }

    void PlaceTreasureChests()
    {
        HashSet<Vector3Int> placedChests = new();
        const int TREASURE_CHEST_TOTAL = 300;
        const int MIN_CHEST_SPACING = 3;

        float currentRadius = safeZoneRadius + 2;
        float angleStep = 20f;
        float currentAngle = 0f;
        int chestsPlaced = 0;
        int maxAttempts = TREASURE_CHEST_TOTAL * 4;
        int attempts = 0;
        int maxRadius = Mathf.Min(MAP_WIDTH, MAP_HEIGHT) / 2;

        while (chestsPlaced < TREASURE_CHEST_TOTAL && currentRadius <= maxRadius && attempts < maxAttempts)
        {
            attempts++;
            float radians = currentAngle * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(mapCenterX + currentRadius * Mathf.Cos(radians));
            int y = Mathf.RoundToInt(mapCenterY + currentRadius * Mathf.Sin(radians));
            Vector3Int position = new(x, y, 0);

            // Check if the position is valid for placing a chest
            if (IsValidChestPosition(position, placedChests, MIN_CHEST_SPACING))
            {
                SetTileAt(position, TreasureChestTile);
                placedChests.Add(position);
                chestsPlaced++;
                if (chestsPlaced % 50 == 0)
                {
                    Debug.Log($"Placed {chestsPlaced} chests");
                }
            }

            // Update angle and radius to place chests in a spiral pattern
            currentAngle += angleStep;
            if (currentAngle >= 360f)
            {
                currentAngle = 0f;
                currentRadius += MIN_CHEST_SPACING;
                angleStep = Mathf.Max(10f, 360f / (2f * Mathf.PI * currentRadius / MIN_CHEST_SPACING));
            }
        }

        Debug.Log($"Chest placement complete: {chestsPlaced} chests placed after {attempts} attempts");
    }

    bool IsValidChestPosition(Vector3Int position, HashSet<Vector3Int> placedChests, int minSpacing)
    {
        // Check map bounds and if position is empty
        if (!IsValidPosition(position) || GetTileAt(position) != null)
        {
            return false;
        }

        // Check spacing from other chests
        for (int x = -minSpacing; x <= minSpacing; x++)
        {
            for (int y = -minSpacing; y <= minSpacing; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector3Int checkPosition = new(position.x + x, position.y + y, 0);
                if (placedChests.Contains(checkPosition))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
