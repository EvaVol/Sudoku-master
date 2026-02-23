using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SudokuCell : MonoBehaviour
{
    Board board;

    int row;
    int col;
    int value;
    public int Row => row;
    public int Col => col;

    // true = buňka byla předvyplněná (nejde editovat)
    bool initialGiven;
    public bool IsEditable => initialGiven == false;


    string id;

    public TMP_Text t;


    [SerializeField] private UnityEngine.UI.Image bg;
    public int Value => value;
    public Board Board => board;


    /*public void SetValues(int _row, int _col, int value, string _id, Board _board)
    {
        row = _row;
        col = _col; 
        id = _id;
        board = _board;

        Debug.Log(t.text);

        if (value != 0)
        {
            t.text = value.ToString();
        }
        else
        {
            t.text = " ";
        }

        if (value != 0)
        {
            GetComponentInParent<Button>().enabled = false;
        }
        else
        {
            t.color = new Color32(0, 102,187,255);
        }
    }*/

    public void SetValues(int _row, int _col, int value, string _id, Board _board)
    {
        row = _row;
        col = _col;
        id = _id;
        board = _board;

        // ULOŽ i do fieldu, ať je stav konzistentní
        this.value = value;

        // Ošetři null (když není Text přiřazen v prefabu)
        if (t == null)
        {
            Debug.LogError($"[SudokuCell] Text 't' není přiřazen v prefabu pro {id}.");
            return;
        }

        if (bg == null)
        {
            bg = GetComponent<UnityEngine.UI.Image>();
        }


        // zapamatuj si, jestli šlo o „danou“ hodnotu z puzzle
        initialGiven = (value != 0);

        if (initialGiven)
        {
            t.text = SymbolForValue(value);
            t.color = Color.black;
            //var btn = GetComponentInParent<Button>();
            var btn = GetComponent<Button>();
            if (btn != null) btn.enabled = false;   // dané buňky nelze přepisovat klikem
        }
        else
        {
            t.text = "";
            t.color = new Color32(0, 102, 187, 255); // tvoje modrá pro editovatelné
        }

        /*if (value != 0)
        {
            t.text = SymbolForValue(value);
            t.color = Color.black;
            GetComponentInParent<Button>().enabled = false;
        }
        else
        {
            t.text = "";
            t.color = new Color32(0, 102, 187, 255);
        }*/

        ApplyHighlight(false, false, false, false);


    }

    // Povolene symboly – sama si urcis ktera pismena chces
    // Tady je 25 znaku bez Q
    char[] symbols = new char[]
    {
    'W','E','R','T','Y',
    'U','I','O','P','A',
    'S','D','F','G','H',
    'J','K','L','Z','X', // vynechali jsme Q
    'C','V','B','N','M'
    };


    [Header("Cell Colors")]
    [SerializeField] private Color normalBg = Color.white;
    [SerializeField] private Color givenBg = new Color(0.95f, 0.95f, 0.96f);
    [SerializeField] private Color selectedBg = new Color(0.78f, 0.88f, 1f);
    [SerializeField] private Color relatedBg = new Color(0.90f, 0.95f, 1f);
    [SerializeField] private Color sameBg = new Color(0.86f, 0.94f, 1f);
    [SerializeField] private Color conflictBg = new Color(1f, 0.85f, 0.85f);


    string SymbolForValue(int v)
    {
        // 1 -> A, 2 -> B, ... 25 -> Y
        if (v <= 0 || v > symbols.Length) return "";
        return symbols[v - 1].ToString();
    }

    public void ApplyHighlight(bool selected, bool related, bool same, bool conflict)
    {
        if (bg == null) return;

        Color baseCol = initialGiven ? givenBg : normalBg;

        if (conflict) bg.color = conflictBg;
        else if (selected) bg.color = selectedBg;
        else if (same) bg.color = sameBg;
        else if (related) bg.color = relatedBg;
        else bg.color = baseCol;
    }


    public void ButtonClicked()
    {
        board.SelectCell(this);
        InputButton.instance.ActivateInputButton(this);
    }

    /*public void UpdateValue(int newValue)
    {
        value = newValue;

        if (value != 0)
        {
            t.text = SymbolForValue(value);
        }
        else
        {
            t.text = "";
        }
        board.UpdatePuzzle(row, col, value);


        /*if (value != 0)
        {
            t.text = value.ToString();
        }
        else
        {
            t.text = "";
        }
        board.UpdatePuzzle(row, col, value);//
    }*/

    public void UpdateValue(int newValue)
    {
        if (!IsEditable) return;
        value = newValue;
        t.text = (value != 0) ? SymbolForValue(value) : "";
        board.UpdatePuzzle(row, col, value);
    }

}
