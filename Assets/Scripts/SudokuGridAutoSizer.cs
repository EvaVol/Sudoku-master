using UnityEngine;
using UnityEngine.UI;

public class SudokuGridAutoSizer : MonoBehaviour
{
    [Header("Cílová velikost celé mřížky (px) na PC")]
    public float gridSizePx = 620f;

    [Header("Mezery")]
    public float blockGap = 6f;   // mezera mezi 5×5 bloky
    public float cellGap = 2f;   // mezera mezi buňkami uvnitř bloku

    RectTransform rt;
    GridLayoutGroup outer;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        outer = GetComponent<GridLayoutGroup>();
    }

    void Start() => Apply();
#if UNITY_EDITOR
    void OnValidate() => Apply();
#endif

    public void Apply()
    {
        if (rt == null) rt = GetComponent<RectTransform>();
        if (outer == null) outer = GetComponent<GridLayoutGroup>();
        if (outer == null) return;

        // 1) nastav velikost kontejneru na čtverec
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridSizePx);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gridSizePx);

        // 2) spočítej velikost jednoho bloku (z 5 bloků + 4 mezery)
        float blockSize = (gridSizePx - blockGap * 4f) / 5f;

        outer.cellSize = new Vector2(blockSize, blockSize);
        outer.spacing = new Vector2(blockGap, blockGap);

        // 3) každému bloku nastav vnitřní GridLayoutGroup (5×5)
        for (int i = 0; i < transform.childCount; i++)
        {
            var block = transform.GetChild(i);
            var inner = block.GetComponent<GridLayoutGroup>();
            if (inner == null) continue;

            float cellSize = (blockSize - cellGap * 4f) / 5f;
            inner.cellSize = new Vector2(cellSize, cellSize);
            inner.spacing = new Vector2(cellGap, cellGap);
        }
    }
}
