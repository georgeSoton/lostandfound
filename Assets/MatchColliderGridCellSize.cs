using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchColliderGridCellSize : MonoBehaviour
{
    GridLayoutGroup group;
    BoxCollider2D boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        group = transform.parent.GetComponent<GridLayoutGroup>();
        boxCollider = GetComponent<BoxCollider2D>();
        if(group != null && boxCollider != null) 
        {
            boxCollider.size = group.cellSize;
        }
    }
}
