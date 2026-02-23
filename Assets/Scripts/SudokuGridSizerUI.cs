using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SudokuGridSizerUI : MonoBehaviour
{
    [Header("Celková velikost mřížky (px)")]
    public float gridSize = 720f;

    [Header("Mezery")]
    public float blockGap = 8f; // mezery mezi 5×5 bloky
    public float cellGap = 2f;  // mezery mezi buňkami uvnitř bloku

    RectTransform rt;
    GridLayoutGroup outer;

    void OnEnable() => Apply();
    void OnValidate() => Apply();

    public void Apply()
    {
        rt = GetComponent<RectTransform>();
        outer = GetComponent<GridLayoutGroup>();
        if (rt == null || outer == null) return;

        // 1) Parent je čtverec gridSize x gridSize
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridSize);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gridSize);

        // 2) Spočítej velikost jednoho bloku (5 bloků + 4 mezery)
        float blockSize = (gridSize - blockGap * 4f) / 5f;

        outer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        outer.constraintCount = 5;
        outer.cellSize = new Vector2(blockSize, blockSize);
        outer.spacing = new Vector2(blockGap, blockGap);

        // 3) V každém bloku spočítej velikost buňky (5 buněk + 4 mezery)
        float cellSize = (blockSize - cellGap * 4f) / 5f;

        for (int i = 0; i < transform.childCount; i++)
        {
            var block = transform.GetChild(i);
            var inner = block.GetComponent<GridLayoutGroup>();
            if (inner == null) continue;

            inner.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            inner.constraintCount = 5;
            inner.cellSize = new Vector2(cellSize, cellSize);
            inner.spacing = new Vector2(cellGap, cellGap);
        }
    }
}
