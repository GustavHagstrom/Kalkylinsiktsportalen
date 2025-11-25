using Core.Models.EstimationView;

namespace SharedModule.Services.Views;
public interface IViewStyleService
{
    string CreateFormatStyles(IFormat format);
    //string CreateSheetColumnStyles(SheetColumn column, int allColumnsWidthSum);
}