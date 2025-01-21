using UnityEngine;
using UnityEngine.EventSystems;

public class GridPosition : MonoBehaviour
{

    [SerializeField] private int _xPos;
    [SerializeField] private int _yPos;



    private void OnMouseDown()
    {
        Debug.Log("Click");
        GameManager.InStance.ClickOnGridPositionRpc(_xPos, _yPos,GameManager.InStance.GetLocalPlayerType());
    }
}
