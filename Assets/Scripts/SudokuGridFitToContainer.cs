using UnityEngine;

[ExecuteAlways]
public class SudokuGridFitToContainer : MonoBehaviour
{
    public RectTransform container;     // sem dáš GridContainer
    public float margin = 10f;          // vnitřní okraj
    public float maxGridSize = 720f;    // strop na PC
    public float minGridSize = 300f;    // aby to nebylo moc malé

    SudokuGridSizerUI sizer;
    RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        sizer = GetComponent<SudokuGridSizerUI>();
    }

    void OnEnable() => Apply();
    void Update()
    {
        // v editoru to občas potřebuje průběžně
#if UNITY_EDITOR
        if (!Application.isPlaying) Apply();
#endif
    }

    public void Apply()
    {
        if (container == null) return;

        // Pokud SudokuGridSizerUI není přidaný jako komponent na stejném objektu,
        // tak se tato část škálování nepoužije (je to volitelné / z dřívější verze).
        if (sizer == null)
        {
#if UNITY_EDITOR
            // Volitelné: můžeš odkomentovat, pokud chceš vidět varování v editoru
            // Debug.LogWarning("SudokuGridSizerUI component not found - grid sizing via sizer is disabled.", this);
#endif
            return;
        }

        float available = Mathf.Min(container.rect.width, container.rect.height) - margin * 2f;
        sizer.gridSize = Mathf.Clamp(available, minGridSize, maxGridSize);
        sizer.Apply();

        // Volitelně: udrž SudokuGrid vycentrovaný v kontejneru
        if (rt != null)
            rt.anchoredPosition = Vector2.zero;
    }
}
