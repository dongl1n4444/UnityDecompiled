namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;

    public interface IEdgeGUI
    {
        void BeginSlotDragging(Slot slot, bool allowStartDrag, bool allowEndDrag);
        void DoDraggedEdge();
        void DoEdges();
        void EndDragging();
        void EndSlotDragging(Slot slot, bool allowMultiple);
        Edge FindClosestEdge();
        void SlotDragging(Slot slot, bool allowEndDrag, bool allowMultiple);

        List<int> edgeSelection { get; set; }

        GraphGUI host { get; set; }
    }
}

