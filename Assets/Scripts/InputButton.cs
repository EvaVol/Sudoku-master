using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputButton : MonoBehaviour
{
    public static InputButton instance;
    SudokuCell lastCell;
    [SerializeField] GameObject wrongText;

    private void Awake()
    {
        //instance = this;
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ActivateInputButton(SudokuCell cell)
    {
        this.gameObject.SetActive(true);
        lastCell= cell;
    }

    /*public void ClickedButton(int num)
    {
        lastCell.UpdateValue(num);
        wrongText.SetActive(false);
        this.gameObject.SetActive(false);
    }*/
    public void ClickedButton(int num)
    {
        if (lastCell == null) return;
        lastCell.UpdateValue(num);
        lastCell.Board.SelectCell(lastCell);
        if (wrongText != null) wrongText.SetActive(false);
        gameObject.SetActive(false);
    }




}
