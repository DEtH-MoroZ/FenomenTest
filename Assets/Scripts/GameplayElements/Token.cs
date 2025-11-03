using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Token : MonoBehaviour
{
    public int id;
    public int EndPosition;
    public int CurrentPosition { get; private set; }

    public GameObject SelectionGraphics;
    
    [SerializeField] private SpriteRenderer spRenderer;
    [SerializeField] private Text nameLabel;
    private void Start()
    {
        SelectionGraphics.SetActive(false);        
    }

    public Color GetColor ()
    {
        return spRenderer.color;
    }

    public void SetRandomColor()
    {
        spRenderer.color = Random.ColorHSV();
    }

    public void MoveByTraectory(Vector3[] path, int targetNodeId)
    {
        CurrentPosition = targetNodeId;

        StartCoroutine(SmoothMovementOfTokenByPath(path));
    }

    IEnumerator SmoothMovementOfTokenByPath(Vector3[] path)
    {

        float timer = 0.0f;
        for (int a = 0; a < path.Length; a++)
        {
            
            
            timer = 0.0f;
            while (Vector3.Distance(transform.position, path[a]) > 0.05f)
            {
                transform.position = Vector3.Lerp(transform.position, path[a], timer);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        yield return new WaitForEndOfFrame();
    }


    public void Move (Vector3 targetPosition, int targetNodeId) //animation purpose
    {
        CurrentPosition = targetNodeId;
       
        StartCoroutine(SmoothMovementOfToken(targetPosition));
    }

    IEnumerator SmoothMovementOfToken (Vector3 targetPosition)
    {
        float timer = 0.0f;
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, timer);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForEndOfFrame();
    }

    public void SpRendererSwitch(bool switcher)
    {
        spRenderer.enabled = switcher;
        nameLabel.gameObject.SetActive(switcher);
    }

    public void Select ()
    {
        SelectionGraphics.SetActive(true);
        GameplayManager.instance.selectedToken = id;
    }

    public void DeSelect ()
    {
        SelectionGraphics.SetActive(false);
    }

    public void SetVisualSign (string name)
    {
        nameLabel.text = name;
    }

}
