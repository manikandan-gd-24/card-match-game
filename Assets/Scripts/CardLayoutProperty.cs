using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardLayoutProperty", menuName = "Card Layout Property")]
public class CardLayoutProperty : ScriptableObject
{
    [System.Serializable]
    public class Layout
    {
        public int Rows;
        public int Columns;
    }

    public Layout[] LayoutList;

    public void ValidateLayouts()
    {
        List<Layout> validLayouts = new List<Layout>();

        foreach (var layout in LayoutList)
        {
            var count = layout.Rows * layout.Columns;

            if (count % 2 == 0 && count > 0)
            {
                validLayouts.Add(layout);
            }
        }

        LayoutList = validLayouts.ToArray();
    }
}
