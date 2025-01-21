using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] private Transform _crossPrefab;
    [SerializeField] private Transform _circlePrefab;
    [SerializeField] private Transform _lineCompletePrefab;
    private const float _gridSize = 3.1f;
    private List<GameObject> _visualGameObjects;

    private void Awake()
    {
        _visualGameObjects = new List<GameObject>();
    }
    private void Start()
    {
        GameManager.InStance.OnClickedOnGrisPosition += GameManager_OnClickedOnGridPosition;
        GameManager.InStance.OnGameWin += GameManager_OnGameWin;
        GameManager.InStance.OnRematch += GameManager_OnRematch;
    }

    private void GameManager_OnRematch(object sender, EventArgs e)
    {

        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        
        foreach (GameObject visualGameObject in _visualGameObjects)
        {
            Destroy(visualGameObject);
        }
        _visualGameObjects.Clear();
    }

    private void OnDisable()
    {

        GameManager.InStance.OnClickedOnGrisPosition -= GameManager_OnClickedOnGridPosition;
        GameManager.InStance.OnGameWin -= GameManager_OnGameWin;
        GameManager.InStance.OnRematch -= GameManager_OnRematch;
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {

        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        float eulerZ = 0;
        switch (e.line.orienation)
        {
            default:
            case GameManager.Orienation.Horizantal: eulerZ = 0; break;
            case GameManager.Orienation.vertical: eulerZ = 90; break;
            case GameManager.Orienation.DaigonalA: eulerZ = 45; break;
            case GameManager.Orienation.DaigonalB: eulerZ = -45; break;
        }
        Transform linecomplete = Instantiate(_lineCompletePrefab, GetGridWorldPosition(e.line.CenterGridPosition.x, e.line.CenterGridPosition.y), Quaternion.Euler(0, 0, eulerZ));
        linecomplete.GetComponent<NetworkObject>().Spawn(true);

        _visualGameObjects.Add(linecomplete.gameObject);
    }

    private void GameManager_OnClickedOnGridPosition(object sender, GameManager.ClickedOnGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.X, e.Y, e.playerType);
        Debug.Log("GameManager_OnClickedOnGridPosition:");
    }

    private Vector2 GetGridWorldPosition(int x, int y)
    {
        return new Vector2(-_gridSize + x * _gridSize, -_gridSize + y * _gridSize);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {
        Debug.Log("SpawnObject:");
        Transform prefab = null;
        switch (playerType)
        {
            case GameManager.PlayerType.Cross:
                prefab = _crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = _circlePrefab;
                break;
        }
        Transform spawnedTransform = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
        spawnedTransform.GetComponent<NetworkObject>().Spawn(true);
        // spawnedTransform.position=GetGridWorldPosition(x, y);
        _visualGameObjects.Add(spawnedTransform.gameObject);
    }
}
