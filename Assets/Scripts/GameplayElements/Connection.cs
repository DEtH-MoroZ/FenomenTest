using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public int ConnectionStart;
    public int ConnectionEnd;

    public int ConnectionId;

    public float width = 15f;

    private Vector2 ConnectionStartCoords;
    private Vector2 ConnectionEndCoords;

    private Color baseColor = new Color ( 0.5f,0.5f,0.5f, 1f);
    private Color hightlightedColor = new Color(0f, 1f, 0f, 1f);

    private SpriteRenderer spRenderer;

    public void SetConnection(int start, int end)
    {
        ConnectionStart = start;
        ConnectionEnd = end;

        spRenderer = GetComponent<SpriteRenderer>();

        SetStartEndCoords();
        SetLength();        
        SetCenteredPosition();
        SetRotation();
        DisableHightlight();
    }

    private void SetStartEndCoords()
    {
        ConnectionStartCoords = PuzzleDataManager.instance.currentPuzzleData.NodeCoords[ConnectionStart];
        ConnectionEndCoords = PuzzleDataManager.instance.currentPuzzleData.NodeCoords[ConnectionEnd];
    }
    
    private void SetLength ()
    {
        float length = Vector2.Distance(ConnectionStartCoords, ConnectionEndCoords);
        transform.localScale = new Vector2(length, width);
    }

    private void SetCenteredPosition()
    {
        transform.position = Vector2.Lerp( ConnectionEndCoords , ConnectionStartCoords, 0.5f);
        //or as an option startPosition + dir * 0.5f
    }

    private void SetRotation ()
    {
        Vector2 dir = (ConnectionEndCoords - ConnectionStartCoords).normalized;        
        transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI);
        //just in case : transform.forward = dir; doesnt work as intended in 2d, cos you got to change projection plane
    }

    public void EnableHightlight ()
    {
        spRenderer.color = hightlightedColor;
    }

    public void DisableHightlight () {
        spRenderer.color = baseColor;
    }

}
