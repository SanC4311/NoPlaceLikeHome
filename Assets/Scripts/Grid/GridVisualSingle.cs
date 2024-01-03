using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualSingle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outerVisual;
    [SerializeField] private SpriteRenderer innerVisual;

    // private void Start()
    // {
    //     outerVisual.enabled = false;
    //     outerVisual.enabled = false;
    // }

    public void Show()
    {
        outerVisual.enabled = true;
        innerVisual.enabled = true;
    }

    public void Hide()
    {
        outerVisual.enabled = false;
        innerVisual.enabled = false;
    }
}
