using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(SpriteRenderer))]
public class TokenFinalPosition : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spRendenerer;
    [SerializeField] private Text nameLabel;
    private void Start()
    {
        spRendenerer = GetComponent<SpriteRenderer>();
        spRendenerer.enabled = false;
    }

    public void SetColor(Color finalColor)
    {
        spRendenerer.color = finalColor;
    }
    
    public void SpRendererSwitch (bool switcher)
    {
        spRendenerer.enabled = switcher;
        nameLabel.gameObject.SetActive(switcher);
    }
    
    public void SetVisualSign(string name)
    {
        nameLabel.text = name;
    }
}
