using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Layout : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}*/


[ExecuteAlways]
public class Layout : MonoBehaviour
{
    [Header("Mřížka (pevná velikost buněk)")]
    [Min(1)] public int columns = 5;
    public Vector2 cellSize = new Vector2(1f, 1f);
    public Vector2 spacing = new Vector2(0.2f, 0.2f);

    [Header("Zarovnání")]
    [Tooltip("Když nenastavíš Reference Renderer, centruje se na (0,0) parentu.")]
    public Renderer referenceRenderer;      // sem dej např. SpriteRenderer pozadí
    [Tooltip("Vycentrovat i neplný poslední řádek (případně každý řádek)?")]
    public bool centerEachRow = true;

    [Header("Další")]
    public bool includeInactive = false;
    public bool zeroChildZ = true;

    void OnValidate() => Arrange();
    void OnTransformChildrenChanged() => Arrange();
    [ContextMenu("Arrange Now")] public void Arrange() => ArrangeInternal();

    void ArrangeInternal()
    {
        var kids = new List<Transform>();
        foreach (Transform t in transform)
            if (includeInactive || t.gameObject.activeSelf) kids.Add(t);
        if (kids.Count == 0) return;

        int cols = Mathf.Clamp(columns, 1, Mathf.Max(1, kids.Count));
        int rows = Mathf.CeilToInt(kids.Count / (float)cols);
        float totalW = cols * cellSize.x + (cols - 1) * spacing.x;
        float totalH = rows * cellSize.y + (rows - 1) * spacing.y;

        // střed mřížky v lokálních souřadnicích parentu (buď (0,0), nebo střed referenčního rendereru)
        Vector2 gridCenterLocal = Vector2.zero;
        if (referenceRenderer != null)
        {
            var b = referenceRenderer.bounds;
            Vector3 cLocal3 = transform.InverseTransformPoint(b.center);
            gridCenterLocal = new Vector2(cLocal3.x, cLocal3.y);
        }

        // top-left center první buňky, aby byl obdélník mřížky centrovaný na gridCenterLocal
        Vector2 origin = new Vector2(
            gridCenterLocal.x - totalW * 0.5f + cellSize.x * 0.5f,
            gridCenterLocal.y + totalH * 0.5f - cellSize.y * 0.5f
        );

        for (int i = 0; i < kids.Count; i++)
        {
            int row = i / cols;
            int col = i % cols;

            int rowCount = (row == rows - 1) ? (kids.Count - row * cols) : cols;
            if (rowCount <= 0) rowCount = cols;

            float rowOriginX = origin.x;
            if (centerEachRow && rowCount < cols)
            {
                float rowW = rowCount * cellSize.x + (rowCount - 1) * spacing.x;
                rowOriginX = gridCenterLocal.x - rowW * 0.5f + cellSize.x * 0.5f;
            }

            float x = rowOriginX + col * (cellSize.x + spacing.x);
            float y = origin.y - row * (cellSize.y + spacing.y);

            var p = kids[i].localPosition;
            kids[i].localPosition = new Vector3(x, y, zeroChildZ ? 0f : p.z);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        int count = 0; foreach (Transform t in transform) if (includeInactive || t.gameObject.activeSelf) count++;
        if (count == 0) return;

        int cols = Mathf.Clamp(columns, 1, Mathf.Max(1, count));
        int rows = Mathf.CeilToInt(count / (float)cols);
        float totalW = cols * cellSize.x + (cols - 1) * spacing.x;
        float totalH = rows * cellSize.y + (rows - 1) * spacing.y;

        Vector2 center = Vector2.zero;
        if (referenceRenderer != null)
        {
            var b = referenceRenderer.bounds;
            Vector3 cL = transform.InverseTransformPoint(b.center);
            center = new Vector2(cL.x, cL.y);
        }

        Vector3 bl = new Vector3(center.x - totalW * 0.5f, center.y - totalH * 0.5f, 0);
        Vector3 br = new Vector3(center.x + totalW * 0.5f, center.y - totalH * 0.5f, 0);
        Vector3 tl = new Vector3(center.x - totalW * 0.5f, center.y + totalH * 0.5f, 0);
        Vector3 tr = new Vector3(center.x + totalW * 0.5f, center.y + totalH * 0.5f, 0);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.TransformPoint(bl), transform.TransformPoint(br));
        Gizmos.DrawLine(transform.TransformPoint(br), transform.TransformPoint(tr));
        Gizmos.DrawLine(transform.TransformPoint(tr), transform.TransformPoint(tl));
        Gizmos.DrawLine(transform.TransformPoint(tl), transform.TransformPoint(bl));

        // křížek v cílovém středu
        Gizmos.DrawLine(transform.TransformPoint(new Vector3(center.x - 0.1f, center.y, 0)),
                        transform.TransformPoint(new Vector3(center.x + 0.1f, center.y, 0)));
        Gizmos.DrawLine(transform.TransformPoint(new Vector3(center.x, center.y - 0.1f, 0)),
                        transform.TransformPoint(new Vector3(center.x, center.y + 0.1f, 0)));
    }
#endif
}
