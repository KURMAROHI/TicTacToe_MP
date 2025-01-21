using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager InStance { get; private set; }
    public event EventHandler<ClickedOnGridPositionEventArgs> OnClickedOnGrisPosition;
    public class ClickedOnGridPositionEventArgs : EventArgs
    {
        public int X;
        public int Y;
        public PlayerType playerType;
    }

    public event EventHandler OnGameStarted;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }
    public event EventHandler OnCurrentPlayblePlayerChange;
    public event EventHandler OnRematch;

    private void Awake()
    {
        if (InStance == null)
            InStance = this;

        _playerTypeArray = new PlayerType[3, 3];

        _lineList = new List<Line>
        {

            //Horizantal lines
                new Line
                {
                    GridVectorIntList=new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(2,0),},
                    CenterGridPosition=new Vector2Int(1,0),
                    orienation=Orienation.Horizantal
                },
                new Line
                {
                    GridVectorIntList=new List<Vector2Int>{new Vector2Int(0,1),new Vector2Int(1,1),new Vector2Int(2,1),},
                    CenterGridPosition=new Vector2Int(1,1),
                    orienation=Orienation.Horizantal
                },
                new Line
                {
                    GridVectorIntList=new List<Vector2Int>{new Vector2Int(0,2),new Vector2Int(1,2),new Vector2Int(2,2),},
                    CenterGridPosition=new Vector2Int(1,2),
                    orienation=Orienation.Horizantal
                },

                //vertical lines
                new Line
                {
                    GridVectorIntList=new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(0,1),new Vector2Int(0,2),},
                    CenterGridPosition=new Vector2Int(0,1),
                    orienation=Orienation.vertical
                },
                new Line
                {
                    GridVectorIntList=new List<Vector2Int>{new Vector2Int(1,0),new Vector2Int(1,1),new Vector2Int(1,2),},
                    CenterGridPosition=new Vector2Int(1,1),
                    orienation=Orienation.vertical
                },
                new Line
                {
                    GridVectorIntList=new List<Vector2Int>{new Vector2Int(2,0),new Vector2Int(2,1),new Vector2Int(2,2),},
                    CenterGridPosition=new Vector2Int(2,1),
                    orienation=Orienation.vertical
                },


                //Daigonal lines
                new Line
                {
                    GridVectorIntList=new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,1),new Vector2Int(2,2),},
                    CenterGridPosition=new Vector2Int(1,1),
                    orienation=Orienation.DaigonalA
                },
                new Line
                {
                    GridVectorIntList=new List<Vector2Int>{new Vector2Int(0,2),new Vector2Int(1,1),new Vector2Int(2,0),},
                    CenterGridPosition=new Vector2Int(1,1),
                    orienation=Orienation.DaigonalB
                },

        };

    }


    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }


    [Rpc(SendTo.Server)]
    public void ClickOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("Click on grid position");
        if (playerType != _currentPlayblePlayerType.Value)
        {
            return;
        }

        if (_playerTypeArray[x, y] != PlayerType.None)
        {
            return;
        }

        _playerTypeArray[x, y] = playerType;

        OnClickedOnGrisPosition?.Invoke(this, new ClickedOnGridPositionEventArgs
        {
            X = x,
            Y = y,
            playerType = playerType,
        });

        switch (_currentPlayblePlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                _currentPlayblePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                _currentPlayblePlayerType.Value = PlayerType.Cross;
                break;
        }

        TestWinner();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("localClienId" + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            _localPlayerType = PlayerType.Cross;
        }
        else
        {
            _localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetWorkManager_OnClienConnectCallBack;
        }


        //need to Check in build event on networvariable
        _currentPlayblePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayblePlayerChange?.Invoke(this, EventArgs.Empty);
        };

    }

    private bool TestWinnerLine(Line line)
    {
        // Debug.Log("==>0 index:[" + line.GridVectorIntList[0] + "]::" + _playerTypeArray[line.GridVectorIntList[0].x, line.GridVectorIntList[0].y]
        //  + "::1 index[" + line.GridVectorIntList[1] + "]::" + _playerTypeArray[line.GridVectorIntList[1].x, line.GridVectorIntList[1].y]
        //  + "::2 index[" + line.GridVectorIntList[2] + "]::" + _playerTypeArray[line.GridVectorIntList[2].x, line.GridVectorIntList[2].y]);
        return TestWinnerLine(
            _playerTypeArray[line.GridVectorIntList[0].x, line.GridVectorIntList[0].y],
            _playerTypeArray[line.GridVectorIntList[1].x, line.GridVectorIntList[1].y],
            _playerTypeArray[line.GridVectorIntList[2].x, line.GridVectorIntList[2].y]
        );
    }
    private bool TestWinnerLine(PlayerType aplayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        // Debug.Log("==>" + aplayerType.ToString());
        return aplayerType != PlayerType.None &&
            aplayerType == bPlayerType &&
            bPlayerType == cPlayerType;
    }

    private void TestWinner()
    {
        for (int i = 0; i < _lineList.Count; i++)
        {
            Line line = _lineList[i];
            if (TestWinnerLine(line))
            {
                Debug.Log("Winner");
                _currentPlayblePlayerType.Value = PlayerType.None;
                TriggerOnGameWindRpc(i, _playerTypeArray[line.CenterGridPosition.x, line.CenterGridPosition.y]);
                break;

            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWindRpc(int lineIndex, PlayerType winPlayerType)
    {
        Line line = _lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line = line,
            winPlayerType = winPlayerType
        });
    }

    private void NetWorkManager_OnClienConnectCallBack(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            _currentPlayblePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }
    public PlayerType GetCurrentPlayblePlayerType()
    {
        return _currentPlayblePlayerType.Value;
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for (int x = 0; x < _playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < _playerTypeArray.GetLength(1); y++)
            {
                _playerTypeArray[x, y] = PlayerType.None;
            }
        }

        _currentPlayblePlayerType.Value = PlayerType.Cross;
        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);

    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
    public enum Orienation
    {
        Horizantal,
        vertical,
        DaigonalA,
        DaigonalB
    }

    public struct Line
    {
        public List<Vector2Int> GridVectorIntList;
        public Vector2Int CenterGridPosition;

        public Orienation orienation;
    }

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayblePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] _playerTypeArray;
    private List<Line> _lineList;
}
