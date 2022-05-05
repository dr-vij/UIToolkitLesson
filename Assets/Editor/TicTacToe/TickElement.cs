using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TickElement : VisualElement
{
    public TickElement()
    {
        generateVisualContent += OnMeshGenerationContext; //<------- I am soooo important, cause OnDraw is not like Update/FixedUpdate. Nobody will call it
        style.flexGrow = 1; //<---- flexgrow is 0 by default
    }

    public new class UxmlFactory : UxmlFactory<TickElement, UxmlTraits> { }

    private void OnMeshGenerationContext(MeshGenerationContext context)
    {
        //Prepare painter
        var painter = context.painter2D;
        painter.strokeColor = Color.blue;
        painter.lineCap = LineCap.Round;
        painter.lineWidth = 5f;

        //Lets make some line art (it draws in local coords of localbound)
        painter.BeginPath();
        painter.MoveTo(new Vector2(0, 0) + Vector2.one * 5);
        painter.LineTo(localBound.size - Vector2.one * 5);
        painter.MoveTo(new Vector2(0, localBound.size.y) + new Vector2(5, -5));
        painter.LineTo(new Vector2(localBound.size.x, 0) + new Vector2(-5, 5));
        painter.Stroke();
    }
}