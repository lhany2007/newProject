using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class RandomMapGeneration : MonoBehaviour
{
    [SerializeField] Tile wallTile; // �� Ÿ��
    Tilemap tilemap;

    Dictionary<Vector2Int, Tile> tileData = new Dictionary<Vector2Int, Tile>();

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        GenerateCircleTilemap(0, 0, 100f, 30f);
        ApplyTileDataToTilemap();
    }

    /// <summary>
    /// Ÿ�ϸ��� ���� ������ �� Ÿ�Ϸ� ä��
    /// </summary>
    /// <param name="centerX">���� �߽� X ��ǥ</param>
    /// <param name="centerY">���� �߽� Y ��ǥ</param>
    /// <param name="outerRadius">���� ������</param>
    /// <param name="innerRadius">���� ���� ������</param>
    public void GenerateCircleTilemap(float centerX, float centerY, float outerRadius, float innerRadius)
    {
        // �ٱ� ���� ��� ����(bounding box)�� ���
        int minX = Mathf.FloorToInt(centerX - outerRadius);
        int maxX = Mathf.CeilToInt(centerX + outerRadius);
        int minY = Mathf.FloorToInt(centerY - outerRadius);
        int maxY = Mathf.CeilToInt(centerY + outerRadius);

        // ������ Ÿ�� ��ġ�� �ݺ������� Ȯ��
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                // ���� ������ ���� �߽ɱ����� �Ÿ��� ���
                float distanceFromCenter = Mathf.Sqrt(
                    Mathf.Pow(x - centerX, 2) +
                    Mathf.Pow(y - centerY, 2)
                );

                // ���� �ٱ� ������ �ȿ� �ְ� ���� ������ �ٱ��� �ִ��� Ȯ��
                if (distanceFromCenter <= outerRadius && distanceFromCenter >= innerRadius)
                {
                    Vector2Int tilePosition = new Vector2Int(x, y);

                    // tileData�� �߰�
                    tileData[tilePosition] = wallTile;
                }
            }
        }
    }

    /// <summary>
    /// tileData�� ����� ������ Tilemap�� �ݿ�
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
