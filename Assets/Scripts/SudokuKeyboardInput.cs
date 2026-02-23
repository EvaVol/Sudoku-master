using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuKeyboardInput : MonoBehaviour
{
    public Board board;

    // stejné pořadí symbolů jako v SudokuCell
    private readonly char[] symbols = new char[]
    {
        'W','E','R','T','Y',
        'U','I','O','P','A',
        'S','D','F','G','H',
        'J','K','L','Z','X',
        'C','V','B','N','M'
    };

    void Update()
    {
        if (board == null) return;

        // ===== VSTUP HODNOT =====
        // 1) písmena A–Z (bez Q) – mapujeme na index v 'symbols'
        for (int i = 0; i < symbols.Length; i++)
        {
            char ch = symbols[i];
            KeyCode kc = LetterToKeyCode(ch);
            if (Input.GetKeyDown(kc))
            {
                WriteValue(i + 1); // 1..25
                return;
            }
        }

        // 2) horní řada číslic 1..9 a numpad 1..9 – rychlé zadání první devítky
        for (int n = 1; n <= 9; n++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + n) || Input.GetKeyDown(KeyCode.Keypad0 + n))
            {
                WriteValue(n);
                return;
            }
        }

        // 3) smazání
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete) ||
            Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            WriteValue(0);
            return;
        }

        // ===== POHYB ŠIPKAMI =====
        int dx = 0, dy = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) dx = -1;
        if (Input.GetKeyDown(KeyCode.RightArrow)) dx = +1;
        if (Input.GetKeyDown(KeyCode.UpArrow)) dy = -1;
        if (Input.GetKeyDown(KeyCode.DownArrow)) dy = +1;

        if (dx != 0 || dy != 0)
        {
            board.MoveSelection(dx, dy);
        }
    }

    void WriteValue(int v)
    {
        var cell = board.CurrentCell;
        if (cell == null) return;
        if (!cell.IsEditable) return;      // nepřepisuj givens
        cell.UpdateValue(v);
        board.SelectCell(cell);

        // Volitelně: board.CheckComplete();
    }

    KeyCode LetterToKeyCode(char c)
    {
        // funguje pro A–Z
        c = char.ToUpperInvariant(c);
        return KeyCode.A + (c - 'A');
    }
}
