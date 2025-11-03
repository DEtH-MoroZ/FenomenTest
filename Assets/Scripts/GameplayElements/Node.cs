using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int id;
    public List<int> connectedId;

    [SerializeField] private Collider2D col;

    private SpriteRenderer spRenderer;
    private Color baseColor = new Color(1f, 1f, 1f, 1f);
    private Color hightlightedColor = new Color(0f, 1f, 0f, 1f);

    private bool hightlightEnabled = true;

    public bool isOccupied { get; private set; } = false;

    public bool isChecked { get; private set; } = false;

    private void Start()
    {
        transform.SetAsFirstSibling();
        
        DisableHightlight();

        
    }

    public void RegisterConnection (int con)
    {
        for (int a = 0; a < connectedId.Count; a++)
        {
            if (connectedId[a] == con)
            {
                return;
            }
        }
        connectedId.Add(con);
    }


    public void SetOccupied ()
    {
        isOccupied = true;
        col.enabled = false;
    }

    public void SetNotOccupied ()
    {
        isOccupied = false;
        col.enabled = true;
    }

    public void EnableHightlight()
    {
        if (spRenderer == null) spRenderer = GetComponent<SpriteRenderer>();
        if (hightlightEnabled == true) return;
        spRenderer.color = hightlightedColor;
        hightlightEnabled = true;
    }

    public void DisableHightlight()
    {
        if (spRenderer == null) spRenderer = GetComponent<SpriteRenderer>();
        if (hightlightEnabled == false) return;
        spRenderer.color = baseColor;
        hightlightEnabled = false;
    }

    public void SetChecked()
    {
        isChecked = true;
    }

    public void SetUnchecked()
    {
        isChecked = false;
    }

}
