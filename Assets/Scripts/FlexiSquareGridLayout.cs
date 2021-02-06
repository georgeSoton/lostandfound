using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexiSquareGridLayout : LayoutGroup

{
    public float spacing;
    public List<Vector2Int> layouts;

    public int rows;
    public int cols;
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (rectChildren.Count == 0) { return; }

        Vector2Int chosenLayout = new Vector2Int(1, rectChildren.Count);
        float runningSquareSize = 0;

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        foreach (var layout in layouts)
        {
            if (layout.x == 0 || layout.y == 0) { continue; }
            var currentsquaresize = Mathf.Min((parentWidth / layout.x - spacing), (parentHeight / layout.y - spacing));
            if (currentsquaresize > runningSquareSize)
            {
                runningSquareSize = currentsquaresize;
                chosenLayout = layout;
            }
        }

        rows = chosenLayout.y;
        cols = chosenLayout.x;
        var squaresize = runningSquareSize;
        var spacedsquaresize = squaresize + spacing;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            int rowcount = i / cols;
            int colcount = i % cols;

            float rowoffset = (rowcount) + -0.5f * (rows - 1);
            float coloffset = (colcount) + -0.5f * (cols - 1);

            var item = rectChildren[i];

            var xcentre = parentWidth * 0.5f;
            var ycentre = parentHeight * 0.5f;

            var xpos = xcentre + (spacedsquaresize * coloffset);
            var ypos = ycentre + (spacedsquaresize * rowoffset);

            SetChildAlongAxis(item, 0, xpos - squaresize * 0.5f, squaresize);
            SetChildAlongAxis(item, 1, ypos - squaresize * 0.5f, squaresize);
        }

    }
    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
