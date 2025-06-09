using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform _placeSfxPrefab;
    [SerializeField] private Transform _winSfxPrefab;
    [SerializeField] private Transform _loseSfxPrefab;

    private void Start()
    {
        GameManager.InStance.OnPlaceObject += GameManager_OnPlaceObject;
        GameManager.InStance.OnGameWin += GameManager_OnGameWin;

    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)  //For Spawning Sounds Depends on win or lose
    {
        if (GameManager.InStance.GetLocalPlayerType() == e.winPlayerType)
        {
            Transform sfxTransform = Instantiate(_winSfxPrefab);
            Destroy(sfxTransform.gameObject, 5f);
        }
        else
        {
            Transform sfxTransform = Instantiate(_loseSfxPrefab);
            Destroy(sfxTransform.gameObject, 5f);
        }
    }

    private void GameManager_OnPlaceObject(object sender, EventArgs e) //For playing Sounds while placing objects
    {
        Transform sfxTransform = Instantiate(_placeSfxPrefab);
        Destroy(sfxTransform.gameObject, 5f);
    }
}

