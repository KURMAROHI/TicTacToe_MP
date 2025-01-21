using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject _crossArrowGameObject;
    [SerializeField] private GameObject _circleArrowGameObject;
    [SerializeField] private GameObject _circleYouTextGameObject;
    [SerializeField] private GameObject _crossYouTextGameObject;

    private void Awake()
    {
        _crossArrowGameObject.SetActive(false);
        _circleArrowGameObject.SetActive(false);
        _crossYouTextGameObject.SetActive(false);
        _circleYouTextGameObject.SetActive(false);
    }

    private void Start()
    {
        GameManager.InStance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.InStance.OnCurrentPlayblePlayerChange += GameManager_OnCurrentPlayblePlayerChanges;
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
