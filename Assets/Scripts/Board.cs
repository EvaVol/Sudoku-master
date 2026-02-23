using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Create the intital Sudoku Grid
    int[,] grid = new int[25,25];
    int[,] puzzle = new int[25, 25];

    // default numbers removed
    int difficulty = 120;

    /*public Transform square00, square01, square02, square03, square04,
                     square10, square11, square12, square13, square14,
                     square20, square21, square22, square23, square24,
                     square30, square31, square32, square33, square34,
                     square40, square41, square42, square43, square44;*/

    //[SerializeField] private Transform gridRoot; // SudokuGrid
    private Transform[] blocks;                 // 25 bloků


    public GameObject SudokuCell_Prefab;
    public GameObject winMenu;
    [SerializeField] GameObject loseText;

    // Start is called before the first frame update

    // === Výběr a přístup k buňkám ===
    public SudokuCell CurrentCell { get; private set; }
    private SudokuCell[,] cells = new SudokuCell[25, 25];

    public void SelectCell(SudokuCell cell)
    {
        CurrentCell = cell;
        RefreshHighlights();

        //// TODO: zvýraznění (outline/pozadí) – můžeš sem dát vlastní highlight
    }




    public void SelectCell(int row, int col)
    {
        row = Mathf.Clamp(row, 0, 24);
        col = Mathf.Clamp(col, 0, 24);
        CurrentCell = cells[row, col];
        RefreshHighlights();

        //// TODO: highlight
    }

    public void MoveSelection(int dx, int dy)
    {
        if (CurrentCell == null) return;
        int r = CurrentCell.Row;
        int c = CurrentCell.Col;
        int nr = Mathf.Clamp(r + dy, 0, 24);
        int nc = Mathf.Clamp(c + dx, 0, 24);
        SelectCell(nr, nc);
    }

    void RefreshHighlights()
    {
        if (CurrentCell == null) return;

        int sr = CurrentCell.Row;
        int sc = CurrentCell.Col;
        int sv = CurrentCell.Value;

        for (int r = 0; r < 25; r++)
            for (int c = 0; c < 25; c++)
            {
                var cell = cells[r, c];
                if (cell == null) continue;

                bool selected = (r == sr && c == sc);
                bool related = (r == sr) || (c == sc) || ((r / 5 == sr / 5) && (c / 5 == sc / 5));
                bool same = (sv != 0 && cell.Value == sv);

                bool conflict = cell.IsEditable && cell.Value != 0 && cell.Value != grid[r, c];

                cell.ApplyHighlight(selected, related, same, conflict);
            }
    }



    void Start()
    {
        blocks = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            blocks[i] = transform.GetChild(i);

        if (blocks.Length != 25)
        {
            Debug.LogError($"SudokuGrid musí mít 25 bloků jako children, ale má {blocks.Length}.");
        }


        if (winMenu != null) winMenu.SetActive(false);
        if (loseText != null) loseText.SetActive(false);

        difficulty = PlayerSettings.difficulty;
        //Debug.Log("Difficulty is: " + difficulty);
        CreateGrid();
        CreatePuzzle();

        CreateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConsoleOutputGrid(int [,] g)
    {
        string output = "";
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                output += g[i, j];
            }
            output += "\n";
        }
        //Debug.Log(output);
    }

    bool ColumnContainsValue(int col, int value)
    {
        for (int i = 0; i < 25; i++)
        {
            if (grid[i, col] == value)
            {
                return true;
            }
        }

        return false;
    }

    bool RowContainsValue(int row, int value)
    {
        for (int i = 0; i < 25; i++)
        {
            if (grid[row, i] == value)
            {
                return true;
            }
        }

        return false;
    }

    bool SquareContainsValue(int row, int col, int value)
    {
        //blocks are 0-2, 3-5, 6-8
        //row / 3 is the first grid coord * 3 
        //ints 

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (grid[ row / 5 * 5 + i , col / 5 * 5 + j ] == value)
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool CheckAll(int row, int col, int value)
    {
        if (ColumnContainsValue(col,value)) {
            //Debug.Log(row + " " + col);
            return false;
        }
        if (RowContainsValue(row, value))
        {
            //Debug.Log(row + " " + col);
            return false;
        }
        if (SquareContainsValue(row, col, value))
        {
            //Debug.Log(row + " " + col);
            return false;
        }

        return true;
    }

    bool IsValid()
    {
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                if (grid[i,j] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /*void CreateGrid()
    {
        List<int> rowList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
        List<int> colList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };

        int value = rowList[Random.Range(0, rowList.Count)];
        grid[0, 0] = value;
        rowList.Remove(value);
        colList.Remove(value);

        for (int i = 1; i < 25; i++)
        {
            value = rowList[Random.Range(0, rowList.Count)];
            grid[i, 0] = value;
            rowList.Remove(value);
        }

        for (int i = 1; i < 25; i++)
        {
            value = colList[Random.Range(0, colList.Count)];
            if (i < 5)
            {
                while(SquareContainsValue(0, 0, value))
                {
                    value = colList[Random.Range(0, colList.Count)]; // reroll
                }
            }
            grid[0, i] = value;
            colList.Remove(value);
        }

        for (int i = 20; i < 25; i++)
        {
            value = Random.Range(1, 26);
            while (SquareContainsValue(0, 24, value) || SquareContainsValue(24, 0, value) || SquareContainsValue(24, 24, value))
            {
                value = Random.Range(1, 26);
            }
            grid[i, i] = value;
        }

        ConsoleOutputGrid(grid);

        SolveSudoku();

    }*/


    void CreateGrid()
    {
        const int n = 5;              // velikost boxu
        const int side = n * n;       // 25

        // 1) Základní vzor (platné sudoku)
        for (int r = 0; r < side; r++)
            for (int c = 0; c < side; c++)
                grid[r, c] = (n * (r % n) + r / n + c) % side + 1;

        // 2) Zamíchej řádky, sloupce v rámci pásů i pořadí pásů
        List<int> rowOrder = BuildBandedOrder(n);
        List<int> colOrder = BuildBandedOrder(n);

        int[,] mixed = new int[side, side];
        for (int r = 0; r < side; r++)
            for (int c = 0; c < side; c++)
                mixed[r, c] = grid[rowOrder[r], colOrder[c]];

        // 3) Permutace symbolů 1..25 (aby řešení vypadalo náhodně)
        List<int> symbols = new List<int>();
        for (int v = 1; v <= side; v++) symbols.Add(v);
        RandomizeList(symbols);

        for (int r = 0; r < side; r++)
            for (int c = 0; c < side; c++)
                grid[r, c] = symbols[mixed[r, c] - 1];

        // Volitelné: Debug.Log hotového řešení
        // ConsoleOutputGrid(grid);
    }

    // Pomocník: vygeneruje pořadí 0..24 tak, že nejdřív zamíchá pořadí 5 pásů
    // a v každém pásu zamíchá 5 řádků/sloupců.
    List<int> BuildBandedOrder(int n)
    {
        List<int> bands = new List<int>();
        for (int b = 0; b < n; b++) bands.Add(b);
        RandomizeList(bands);

        List<int> result = new List<int>();
        foreach (int b in bands)
        {
            List<int> within = new List<int>();
            for (int i = 0; i < n; i++) within.Add(b * n + i);
            RandomizeList(within);
            result.AddRange(within);
        }
        return result;
    }


/*    bool SolveSudoku()
    {
        int row = 0;
        int col = 0;

        if (IsValid())
        {
            return true;
        }

        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                if (grid[i, j] == 0)
                {
                    row = i;
                    col = j;
                }
            }
        }

        for (int i = 1; i <=25; i++)
        {
            if (CheckAll(row, col, i)) {
                grid[row, col] = i;
                //ConsoleOutputGrid(grid);
                
                if (SolveSudoku())
                {
                    return true;
                }
                else
                {
                    grid[row, col] = 0;
                }
            }
        }
        return false;
    }*/

    void CreatePuzzle()
    {
        
            // zkopíruj řešení do puzzle
            System.Array.Copy(grid, puzzle, grid.Length);

            // velikost mřížky (25×25), ale ať je to obecné
            int side = grid.GetLength(0);
            int total = side * side;

            // difficulty = 0..100 (% děr)
            int pct = Mathf.Clamp(PlayerSettings.difficulty, 0, 100);
            int holesToMake = Mathf.RoundToInt(pct / 100f * total);

            // náhodné pořadí všech buněk
            List<int> cells = new List<int>();
            for (int k = 0; k < total; k++) cells.Add(k);
            RandomizeList(cells);

            // odstraň „holesToMake“ buněk (nastav na 0)
            foreach (int k in cells)
            {
                if (holesToMake <= 0) break;
                int r = k / side, c = k % side;
                if (puzzle[r, c] == 0) continue;
                puzzle[r, c] = 0;
                holesToMake--;
            }

            ConsoleOutputGrid(puzzle);
        



        /*(System.Array.Copy(grid, puzzle, grid.Length);

        int holesToMake = Mathf.Clamp(difficulty, 0, 25 * 25);
        List<int> cells = new List<int>();
        for (int k = 0; k < 25 * 25; k++) cells.Add(k);
        RandomizeList(cells);

        foreach (int k in cells)
        {
            if (holesToMake <= 0) break;
            int r = k / 25, c = k % 25;
            if (puzzle[r, c] == 0) continue;
            puzzle[r, c] = 0;
            holesToMake--;
        }
        
        ConsoleOutputGrid(puzzle);*/

    }

    void RandomizeList(List<int> l)
    {
        //var count = l.Count;
        //var last = count - 1;
        for (var i = 0; i < l.Count - 1; i++)
        {
            int rand = Random.Range(i, l.Count);
            int temp = l[i];
            l[i] = l[rand];
            l[rand] = temp;
        }
    }

        void CreateButtons()
    {
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                GameObject newButton = Instantiate(SudokuCell_Prefab);
                SudokuCell sudokuCell = newButton.GetComponent<SudokuCell>();
                sudokuCell.SetValues(i, j, puzzle[i, j], i + "," + j, this);
                cells[i, j] = sudokuCell;
                //newButton.name = i.ToString() + j.ToString();
                newButton.name = $"{i}_{j}";


                /*if (i < 5)
                {
                    if (j < 5)
                    {
                        newButton.transform.SetParent(square00, false);
                    }
                    if (j >= 5 && j < 10)
                    {
                        newButton.transform.SetParent(square01, false);
                    }
                    if (j >= 10 && j < 15)
                    {
                        newButton.transform.SetParent(square02, false);
                    }
                    if (j >= 15 && j < 20)
                    {
                        newButton.transform.SetParent(square03, false);
                    }
                    if (j >= 20)
                    {
                        newButton.transform.SetParent(square04, false);
                    }
                }

                if (i >= 5 && i < 10)
                {
                    if (j < 5)
                    {
                        newButton.transform.SetParent(square10, false);
                    }
                    if (j >= 5 && j < 10)
                    {
                        newButton.transform.SetParent(square11, false);
                    }
                    if (j >= 10 && j < 15)
                    {
                        newButton.transform.SetParent(square12, false);
                    }
                    if (j >= 15 && j < 20)
                    {
                        newButton.transform.SetParent(square13, false);
                    }
                    if (j >= 20)
                    {
                        newButton.transform.SetParent(square14, false);
                    }
                }

                if (i >= 10 && i < 15)
                {
                    if (j < 5)
                    {
                        newButton.transform.SetParent(square20, false);
                    }
                    if (j >= 5 && j < 10)
                    {
                        newButton.transform.SetParent(square21, false);
                    }
                    if (j >= 10 && j < 15)
                    {
                        newButton.transform.SetParent(square22, false);
                    }
                    if (j >= 15 && j < 20)
                    {
                        newButton.transform.SetParent(square23, false);
                    }
                    if (j >= 20)
                    {
                        newButton.transform.SetParent(square24, false);
                    }
                }

                if (i >= 15 && i < 20)
                {
                    if (j < 5)
                    {
                        newButton.transform.SetParent(square30, false);
                    }
                    if (j >= 5 && j < 10)
                    {
                        newButton.transform.SetParent(square31, false);
                    }
                    if (j >= 10 && j < 15)
                    {
                        newButton.transform.SetParent(square32, false);
                    }
                    if (j >= 15 && j < 20)
                    {
                        newButton.transform.SetParent(square33, false);
                    }
                    if (j >= 20)
                    {
                        newButton.transform.SetParent(square34, false);
                    }
                }

                if (i >= 20 && i < 25)
                {
                    if (j < 5)
                    {
                        newButton.transform.SetParent(square40, false);
                    }
                    if (j >= 5 && j < 10)
                    {
                        newButton.transform.SetParent(square41, false);
                    }
                    if (j >= 10 && j < 15)
                    {
                        newButton.transform.SetParent(square42, false);
                    }
                    if (j >= 15 && j < 20)
                    {
                        newButton.transform.SetParent(square43, false);
                    }
                    if (j >= 20)
                    {
                        newButton.transform.SetParent(square44, false);
                    }
                }*/

                int blockIndex = (i / 5) * 5 + (j / 5);
                newButton.transform.SetParent(blocks[blockIndex], false);

            }
        }
    }

    public void UpdatePuzzle(int row, int col, int value)
    {
        puzzle[row, col] = value;
    }

    

    bool CheckGrid()
    {
        for (int i = 0; i < 25; i++)
        {
            for (int j =  0; j < 25; j++)
            {
                if (puzzle[i,j] != grid[i,j])
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public void CheckComplete()
    {
        if (CheckGrid())
        {
            // ZASTAV TIMER
            var timer = FindObjectOfType<TimerCountUp>();
            if (timer != null) timer.StopTimer();

            winMenu.SetActive(true);
        }
        else
        {
            loseText.SetActive(true);
            //Debug.Log("Loser");
        }
    }
}
