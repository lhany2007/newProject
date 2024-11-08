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

    public Dictionary<Vector3Int, Tile> TileMaps; // 모든 타일의 현재 상태를 저장

    HashSet<Vector3Int> safeZoneBoundary; // 중앙 빈 구역의 좌표

    int TreasureChestTotal = 100; // 맵 전체에 분포될 보물상자의 수

    const int MAP_WIDTH = 200;
    const int MAP_HEIGHT = 200;
    const int CELLULAR_ITERATIONS = 20; // 셀룰러 오토마타 스무딩 반복 횟수
    const int MIN_ROOM_SIZE = 10; // 방이 유효하다고 판단되는 최소 크기(TreasureChestTile 생성할 때 사용)

    readonly System.Random random = new System.Random();

    // 빈 구역의 크기를 계산을 위한 값들
    readonly int safeZoneRadius = 20; // 중앙 빈 구역 지대의 반경
    readonly int safeZoneRadiusSquared; // 더 빠른 거리 확인을 위한 제곱된 반경
    readonly int outerSafeZoneRadius; // 빈 부분의 경계 반경
    readonly int outerSafeZoneRadiusSquared;
    readonly int mapCenterX = MAP_WIDTH / 2;
    readonly int mapCenterY = MAP_HEIGHT / 2;

    // 방향
    static readonly Vector3Int[] adjacentDirections =
    {
        new(1, 0, 0), // 오른쪽
        new(-1, 0, 0), // 왼쪽
        new(0, 1, 0), // 위
        new(0, -1, 0) // 아래
    };

    /// <summary>
    /// 미리 계산된 값과 자료구조로 맵 생성기를 초기화
    /// </summary>
    public RandomMapGeneration()
    {
        // 제곱 거리를 미리 계산
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
        InitializeRandomMap();           // 초기 랜덤 노이즈 생성
        CreateSafeZone();               // 중앙 빈 구역 생성
        ApplyCellularAutomata();        // 셀룰러 오토마타를 사용하여 맵 스무딩
        DistributeCollectibles();       // 유효한 위치에 보물상자 배치
        PrintMap();                    // 최종 맵 표시
    }

    /// <summary>
    /// 벽과 빈공간 밀도가 50%인 초기 무작위 노이즈 맵을 생성
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
    /// 셀룰러 오토마타 규칙을 적용하여 맵을 부드럽게 하고 자연스러운 동굴을 생성
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
    /// 주어진 위치에 인접한 벽 타일의 수를 계산
    /// </summary>
    /// <param name="centerX">중심 타일의 X 좌표</param>
    /// <param name="centerY">중심 타일의 Y 좌표</param>
    /// <returns>인접한 벽 타일의 수</returns>
    int CountAdjacentObstacles(int centerX, int centerY)
    {
        int obstacleCount = 0;
        Vector3Int checkPosition = new();

        // 주변 타일 8개를 모두 확인
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if (x < 0 || x >= MAP_WIDTH || y < 0 || y >= MAP_HEIGHT)
                {
                    // 경계를 벗어난 위치를 벽으로 계산
                    obstacleCount++;
                }
                else if (x != centerX || y != centerY)
                {
                    // 유효한 위치에 있는 실제 벽의 수를 계산
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

                // 위치가 지도 경계 내에 있는지 확인
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

                    // 원 구역 경계 생성
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
    /// 동굴의 유효한 방들에 보물상자을 분배
    /// </summary>
    void DistributeCollectibles()
    {
        // 유효한 방을 모두 찾음 (중앙 빈 구역 및 작은 방 제외)
        var availableRooms = FindAvailableRooms()
            .Where(room => room.Count >= MIN_ROOM_SIZE && !IsSafeZoneOverlap(room))
            .ToList();

        if (availableRooms.Count == 0)
        {
            Debug.LogWarning("보물상자를 놓을 수 있는 유효한 방을 찾을 수 없음");
            return;
        }
        int remainingCollectibles = TreasureChestTotal;
        int attempts = 0;
        const int maxAttempts = 1000; // 무한 루프 방지

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
    /// 보물상자 생성을 위한 충분한 빈 공간이 위치 주변에 있는지 확인
    /// </summary>
    bool HasRequiredSpacing(Vector3Int position)
    {
        Vector3Int checkPosition = new();
        // 위치 주변의 3x3 영역을 확인
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
    /// 방이 안전 구역과 겹치는지 확인
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
    /// 맵에서 모든 연속된 빈 공간(방들)을 찾음
    /// </summary>
    /// <returns>방 타일 세트의 리스트</returns>
    List<HashSet<Vector3Int>> FindAvailableRooms()
    {
        var rooms = new List<HashSet<Vector3Int>>();
        var visitedTiles = new HashSet<Vector3Int>();
        Vector3Int tilePosition = new();

        // 방문하지 않은 빈 공간이 있는지 전체 지도를 스캔
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
    /// 주어진 위치에서 시작하여 연결된 모든 빈 타일을 매핑
    /// </summary>
    /// <param name="startPosition">시작 타일 위치</param>
    /// <param name="visitedTiles">이미 방문한 타일들의 세트</param>
    /// <returns>연결된 방 타일들의 세트</returns>
    HashSet<Vector3Int> MapConnectedRoom(Vector3Int startPosition, HashSet<Vector3Int> visitedTiles)
    {
        var roomTiles = new HashSet<Vector3Int>();
        var tilesToCheck = new Queue<Vector3Int>();
        tilesToCheck.Enqueue(startPosition);
        Vector3Int nextPosition = new();

        // "플러드 필" 알고리즘으로 연결된 타일을 찾기
        while (tilesToCheck.Count > 0)
        {
            var currentPosition = tilesToCheck.Dequeue();
            if (!visitedTiles.Contains(currentPosition) &&
                IsValidPosition(currentPosition) &&
                TileMaps[currentPosition] == null)
            {
                roomTiles.Add(currentPosition);
                visitedTiles.Add(currentPosition);

                // 인접한 타일 확인
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
    /// 위치가 맵 경계 내에 있고 TileMaps에 존재하는지 확인
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