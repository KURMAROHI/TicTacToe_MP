using System;
using UnityEngine;
using UnityEngine.UI;

public class GamoverUI : MonoBehaviour
{
    [SerializeField] private Text _resultText;
    [SerializeField] private Color _winColor;
    [SerializeField] private Color _loseColor;
    [SerializeField] private Color _tieColor;
    [SerializeField] private Button _reMatchButton;

    private void Awake()
    {
        _reMatchButton.onClick.AddListener(() =>
        {
            GameManager.InStance.RematchRpc();
        });
    }

    private void Start()
    {
        Hide();
        GameManager.InStance.OnGameWin += GameManager_OnGameWin;
        GameManager.InStance.OnRematch += GameManager_OnRemacth;
        GameManager.InStance.OnGameTied += GameManager_OnGameTied;


    }

    private void GameManager_OnGameTied(object sender, EventArgs e)
    {
        _resultText.text = "TIE!";
        _resultText.color = _tieColor;
        Show();
    }

    private void GameManager_OnRemacth(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.winPlayerType == GameManager.InStance.GetLocalPlayerType())
        {
            _resultText.text = "You Win!";
            _resultText.color = _winColor;
        }
        else
        {
            _resultText.text = "You Lose!";
            _resultText.color = _loseColor;
        }

        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
