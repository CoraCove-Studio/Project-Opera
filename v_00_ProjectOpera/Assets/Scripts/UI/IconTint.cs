using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class IconTint : MonoBehaviour
{
    [SerializeField] private Image[] sprites;
    [SerializeField] private Color tint;

    // Start is called before the first frame update
    void Start()
    {
        ColorChange();
    }

    private void ColorChange()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = tint;
        }
    }
}
