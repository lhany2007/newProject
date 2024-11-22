using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class RandomMapGeneration : MonoBehaviour
{
    [SerializeField] Tile wallTile; // 벽 타일
    Tilemap tilemap;

    Dictionary<Vector2Int, Tile> tileData = new Dictionary<Vector2Int, Tile>();

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        GenerateCircleTilemap(0, 0, 100f, 30f);
        ApplyTileDataToTilemap();
    }

    /// <summary>
    /// 타일맵을 원형 패턴의 벽 타일로 채움
    /// </summary>
    /// <param name="centerX">원의 중심 X 좌표</param>
    /// <param name="centerY">원의 중심 Y 좌표</param>
    /// <param name="outerRadius">원의 반지름</param>
    /// <param name="innerRadius">내부 원의 반지름</param>
    public void GenerateCircleTilemap(float centerX, float centerY, float outerRadius, float innerRadius)
    {
        // 바깥 원의 경계 상자(bounding box)를 계산
        int minX = Mathf.FloorToInt(centerX - outerRadius);
        int maxX = Mathf.CeilToInt(centerX + outerRadius);
        int minY = Mathf.FloorToInt(centerY - outerRadius);
        int maxY = Mathf.CeilToInt(centerY + outerRadius);

        // 가능한 타일 위치를 반복적으로 확인
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                // 현재 점에서 원의 중심까지의 거리를 계산
                float distanceFromCenter = Mathf.Sqrt(
                    Mathf.Pow(x - centerX, 2) +
                    Mathf.Pow(y - centerY, 2)
                );

                // 점이 바깥 반지름 안에 있고 내부 반지름 바깥에 있는지 확인
                if (distanceFromCenter <= outerRadius && distanceFromCenter >= innerRadius)
                {
                    Vector2Int tilePosition = new Vector2Int(x, y);

                    // tileData에 추가
                    tileData[tilePosition] = wallTile;
                }
            }
        }
    }

    /// <summary>
    /// tileData에 저장된 정보를 Tilemap에 반영
    /// </summary>
    public void ApplyTileDataToTilemap()
    {
        foreach (var kvp in tileData)
        {
            Vector3Int tilePosition = new Vector3Int(kvp.Key.x, kvp.Key.y, 0);
            tilemap.SetTile(tilePosition, kvp.Value);
        }
    }
}
