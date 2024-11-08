using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class RandomMapGeneration : MonoBehaviour
{
    public static RandomMapGeneration Instance;

    public Tile WallTile;
    public Tile TreasureChestTile;

    Tilemap tilemap;

    public Dictionary<Vector3Int, Tile> TileMaps; // ��� Ÿ���� ���� ���¸� ����

    HashSet<Vector3Int> safeZoneBoundary; // �߾� �� ������ ��ǥ

    int TreasureChestTotal = 100; // �� ��ü�� ������ ���������� ��

    const int MAP_WIDTH = 200;
    const int MAP_HEIGHT = 200;
    const int CELLULAR_ITERATIONS = 20; // ���귯 ���丶Ÿ ������ �ݺ� Ƚ��
    const int MIN_ROOM_SIZE = 10; // ���� ��ȿ�ϴٰ� �ǴܵǴ� �ּ� ũ��(TreasureChestTile ������ �� ���)

    readonly System.Random random = new System.Random();

    // �� ������ ũ�⸦ ����� ���� ����
    readonly int safeZoneRadius = 20; // �߾� �� ���� ������ �ݰ�
    readonly int safeZoneRadiusSquared; // �� ���� �Ÿ� Ȯ���� ���� ������ �ݰ�
    readonly int outerSafeZoneRadius; // �� �κ��� ��� �ݰ�
    readonly int outerSafeZoneRadiusSquared;
    readonly int mapCenterX = MAP_WIDTH / 2;
    readonly int mapCenterY = MAP_HEIGHT / 2;

    // ����
    static readonly Vector3Int[] adjacentDirections =
    {
        new(1, 0, 0), // ������
        new(-1, 0, 0), // ����
        new(0, 1, 0), // ��
        new(0, -1, 0) // �Ʒ�
    };

    /// <summary>
    /// �̸� ���� ���� �ڷᱸ���� �� �����⸦ �ʱ�ȭ
    /// </summary>
    public RandomMapGeneration()
    {
        // ���� �Ÿ��� �̸� ���
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
        InitializeRandomMap();           // �ʱ� ���� ������ ����
        CreateSafeZone();               // �߾� �� ���� ����
        ApplyCellularAutomata();        // ���귯 ���丶Ÿ�� ����Ͽ� �� ������
        DistributeCollectibles();       // ��ȿ�� ��ġ�� �������� ��ġ
        PrintMap();                    // ���� �� ǥ��
    }

    /// <summary>
    /// ���� ����� �е��� 50%�� �ʱ� ������ ������ ���� ����
    /// </summary>
    void InitializeRandomMap()
    {
        Vector3Int tilePosition = new();
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                tilePosition.x = x;
                tilePosition.y = y;

                TileMaps[tilePosition] = random.NextDouble() < 0.5 ? WallTile : null;
            }
        }
    }

    /// <summary>
    /// ���귯 ���丶Ÿ ��Ģ�� �����Ͽ� ���� �ε巴�� �ϰ� �ڿ������� ������ ����
    /// </summary>
    void ApplyCellularAutomata()
    {
        var nextIterationTiles = new Dictionary<Vector3Int, Tile>(MAP_WIDTH * MAP_HEIGHT);
        Vector3Int tilePosition = new();

        for (int iteration = 0; iteration < CELLULAR_ITERATIONS; iteration++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                for (int y = 0; y < MAP_HEIGHT; y++)
                {
                    tilePosition.x = x;
                    tilePosition.y = y;

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

            Dictionary<Vector3Int, Tile> tempTiles = TileMaps;
            TileMaps = nextIterationTiles;
            nextIterationTiles = tempTiles;
        }
    }

    /// <summary>
    /// �־��� ��ġ�� ������ �� Ÿ���� ���� ���
    /// </summary>
    /// <param name="centerX">�߽� Ÿ���� X ��ǥ</param>
    /// <param name="centerY">�߽� Ÿ���� Y ��ǥ</param>
    /// <returns>������ �� Ÿ���� ��</returns>
    int CountAdjacentObstacles(int centerX, int centerY)
    {
        int obstacleCount = 0;
        Vector3Int checkPosition = new();

        // �ֺ� Ÿ�� 8���� ��� Ȯ��
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if (x < 0 || x >= MAP_WIDTH || y < 0 || y >= MAP_HEIGHT)
                {
                    // ��踦 ��� ��ġ�� ������ ���
                    obstacleCount++;
                }
                else if (x != centerX || y != centerY)
                {
                    // ��ȿ�� ��ġ�� �ִ� ���� ���� ���� ���
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

        for (int y = -outerSafeZoneRadius; y <= outerSafeZoneRadius; y++)
        {
            for (int x = -outerSafeZoneRadius; x <= outerSafeZoneRadius; x++)
            {
                int mapX = mapCenterX + x;
                int mapY = mapCenterY + y;

                // ��ġ�� ���� ��� ���� �ִ��� Ȯ��
                if (mapX >= 0 && mapX < MAP_WIDTH && mapY >= 0 && mapY < MAP_HEIGHT)
                {
                    tilePosition.x = mapX;
                    tilePosition.y = mapY;

                    int distanceSquared = x * x + y * y;

                    if (distanceSquared < safeZoneRadiusSquared)
                    {
                        TileMaps[tilePosition] = null;
                    }
                    else if (distanceSquared < outerSafeZoneRadiusSquared && random.NextDouble() < 0.4)
                    {
                        TileMaps[tilePosition] = null;
                    }

                    // �� ���� ��� ����
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

    /// <summary>
    /// ������ ��ȿ�� ��鿡 ���������� �й�
    /// </summary>
    void DistributeCollectibles()
    {
        // ��ȿ�� ���� ��� ã�� (�߾� �� ���� �� ���� �� ����)
        var availableRooms = FindAvailableRooms()
            .Where(room => room.Count >= MIN_ROOM_SIZE && !IsSafeZoneOverlap(room))
            .ToList();

        if (availableRooms.Count == 0)
        {
            Debug.LogWarning("�������ڸ� ���� �� �ִ� ��ȿ�� ���� ã�� �� ����");
            return;
        }
        int remainingCollectibles = TreasureChestTotal;
        int attempts = 0;
        const int maxAttempts = 1000; // ���� ���� ����

        int collectiblesPerRoom = Mathf.Max(1, TreasureChestTotal / availableRooms.Count);
        foreach (var room in availableRooms)
        {
            var validPositions = GetValidPositionsInRoom(room);
            if (validPositions.Count == 0) continue;

            for (int i = 0; i < collectiblesPerRoom && remainingCollectibles > 0; i++)
            {
                if (PlaceCollectibleInRoom(validPositions))
                {
                    remainingCollectibles--;
                }
            }
        }

        while (remainingCollectibles > 0 && attempts < maxAttempts)
        {
            foreach (var room in availableRooms)
            {
                if (remainingCollectibles <= 0) break;

                var validPositions = GetValidPositionsInRoom(room);
                if (validPositions.Count > 0 && PlaceCollectibleInRoom(validPositions))
                {
                    remainingCollectibles--;
                }
            }
            attempts++;
        }

        if (remainingCollectibles > 0)
        {
            Debug.LogWarning($"Could only place {TreasureChestTotal - remainingCollectibles} out of {TreasureChestTotal} treasure chests. Consider adjusting parameters.");
        }
    }

    List<Vector3Int> GetValidPositionsInRoom(HashSet<Vector3Int> room)
    {
        return room.Where(pos => HasRequiredSpacing(pos) && TileMaps[pos] != TreasureChestTile).ToList();
    }

    bool PlaceCollectibleInRoom(List<Vector3Int> validPositions)
    {
        if (validPositions.Count == 0) return false;

        int randomIndex = random.Next(validPositions.Count);
        Vector3Int position = validPositions[randomIndex];
        TileMaps[position] = TreasureChestTile;
        return true;
    }

    /// <summary>
    /// �������� ������ ���� ����� �� ������ ��ġ �ֺ��� �ִ��� Ȯ��
    /// </summary>
    bool HasRequiredSpacing(Vector3Int position)
    {
        Vector3Int checkPosition = new();
        // ��ġ �ֺ��� 3x3 ������ Ȯ��
        foreach (var direction in adjacentDirections)
        {
            checkPosition.x = position.x + direction.x;
            checkPosition.y = position.y + direction.y;

            if (!IsValidPosition(checkPosition) || TileMaps[checkPosition] == TreasureChestTile)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ���� ���� ������ ��ġ���� Ȯ��
    /// </summary>
    bool IsSafeZoneOverlap(HashSet<Vector3Int> room)
    {
        foreach (var position in room)
        {
            int dx = position.x - mapCenterX;
            int dy = position.y - mapCenterY;
            if (dx * dx + dy * dy <= safeZoneRadiusSquared)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// �ʿ��� ��� ���ӵ� �� ����(���)�� ã��
    /// </summary>
    /// <returns>�� Ÿ�� ��Ʈ�� ����Ʈ</returns>
    List<HashSet<Vector3Int>> FindAvailableRooms()
    {
        var rooms = new List<HashSet<Vector3Int>>();
        var visitedTiles = new HashSet<Vector3Int>();
        Vector3Int tilePosition = new();

        // �湮���� ���� �� ������ �ִ��� ��ü ������ ��ĵ
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                tilePosition.x = x;
                tilePosition.y = y;
                if (!visitedTiles.Contains(tilePosition) && TileMaps[tilePosition] == null)
                {
                    rooms.Add(MapConnectedRoom(tilePosition, visitedTiles));
                }
            }
        }

        return rooms;
    }

    /// <summary>
    /// �־��� ��ġ���� �����Ͽ� ����� ��� �� Ÿ���� ����
    /// </summary>
    /// <param name="startPosition">���� Ÿ�� ��ġ</param>
    /// <param name="visitedTiles">�̹� �湮�� Ÿ�ϵ��� ��Ʈ</param>
    /// <returns>����� �� Ÿ�ϵ��� ��Ʈ</returns>
    HashSet<Vector3Int> MapConnectedRoom(Vector3Int startPosition, HashSet<Vector3Int> visitedTiles)
    {
        var roomTiles = new HashSet<Vector3Int>();
        var tilesToCheck = new Queue<Vector3Int>();
        tilesToCheck.Enqueue(startPosition);
        Vector3Int nextPosition = new();

        // "�÷��� ��" �˰������� ����� Ÿ���� ã��
        while (tilesToCheck.Count > 0)
        {
            var currentPosition = tilesToCheck.Dequeue();
            if (!visitedTiles.Contains(currentPosition) &&
                IsValidPosition(currentPosition) &&
                TileMaps[currentPosition] == null)
            {
                roomTiles.Add(currentPosition);
                visitedTiles.Add(currentPosition);

                // ������ Ÿ�� Ȯ��
                foreach (var direction in adjacentDirections)
                {
                    nextPosition.x = currentPosition.x + direction.x;
                    nextPosition.y = currentPosition.y + direction.y;
                    if (!visitedTiles.Contains(nextPosition) && IsValidPosition(nextPosition))
                    {
                        tilesToCheck.Enqueue(nextPosition);
                    }
                }
            }
        }

        return roomTiles;
    }

    /// <summary>
    /// ��ġ�� �� ��� ���� �ְ� TileMaps�� �����ϴ��� Ȯ��
    /// </summary>
    bool IsValidPosition(Vector3Int position)
    {
        return position.x >= 0 && position.x < MAP_WIDTH &&
               position.y >= 0 && position.y < MAP_HEIGHT &&
               TileMaps.ContainsKey(position);
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
}