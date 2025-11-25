using Core.Models.EstimationEnteties;

namespace SharedModule.Components.View;
public class SheetItemContainer
{
    public SheetItemContainer(SheetItem sheetItem, bool isExpanded = true, SheetItemContainer? parent = null)
    {
        SheetItem = sheetItem;
        IsExpanded = isExpanded;
        Parent = parent;
        Children.AddRange(SheetItem.Children.Select(x => new SheetItemContainer(x, isExpanded, this)));
    }

    public SheetItem SheetItem { get; }
    public SheetItemContainer? Parent { get; set; }
    public List<SheetItemContainer> Children { get; set; } = new();
    public bool IsExpanded { get; set; }
    public int Level
    {
        get
        {
            if (Parent is null)
            {
                return 0;
            }
            return Parent.Level + 1;
        }
    }
    public IEnumerable<SheetItemContainer> AllExpandedInOrder
    {
        get
        {
            //If this is expanded return this and all children
            if (IsExpanded)
            {
                return new[] { this }.Concat(Children.SelectMany(x => x.AllExpandedInOrder));
            }
            //If this is not expanded return this
            return new[] { this };

        }
    }
    public override string ToString()
    {
        return SheetItem.Description;
    }
}
