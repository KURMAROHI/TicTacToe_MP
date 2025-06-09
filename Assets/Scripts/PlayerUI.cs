using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject _crossArrowGameObject;
    [SerializeField] private GameObject _circleArrowGameObject;
    [SerializeField] private GameObject _circleYouTextGameObject;
    [SerializeField] private GameObject _crossYouTextGameObject;

    [SerializeField] private Text _playerCrossScore;
    [SerializeField] private Text _playerCircleScore;

    private void Awake()
    {
        Debug.Log("===>PlayerUI");
        _crossArrowGameObject.SetActive(false);
        _circleArrowGameObject.SetActive(false);
        _crossYouTextGameObject.SetActive(false);
        _circleYouTextGameObject.SetActive(false);
        _playerCrossScore.text = "";
        _playerCircleScore.text = "";
    }

    private void Start()
    {
        GameManager.InStance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.InStance.OnCurrentPlayblePlayerChange += GameManager_OnCurrentPlayblePlayerChanges;
        GameManager.InStance.OnScoreChange += GameManager_OnScoreChange;
    }

    private void GameManager_OnScoreChange(object sender, EventArgs e)
    {
        GameManager.InStance.GetScores(out int playerCrossScore, out int playerCirCleScore);
        _playerCrossScore.text = playerCrossScore.ToString();
        _playerCircleScore.text = playerCirCleScore.ToString();
    }



    private void GameManager_OnCurrentPlayblePlayerChanges(object sender, EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        if (GameManager.InStance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            _crossYouTextGameObject.SetActive(true);
        }
        else
        {
            _circleYouTextGameObject.SetActive(true);
        }

        UpdateCurrentArrow();

        _playerCrossScore.text = "0";
        _playerCircleScore.text = "0";
    }

    private void UpdateCurrentArrow()
    {
        if (GameManager.InStance.GetCurrentPlayblePlayerType() == GameManager.PlayerType.Cross)
        {
            _crossArrowGameObject.SetActive(true);
            _circleArrowGameObject.SetActive(false);
        }
        else
        {
            _crossArrowGameObject.SetActive(false);
            _circleArrowGameObject.SetActive(true);
        }
    }
}
