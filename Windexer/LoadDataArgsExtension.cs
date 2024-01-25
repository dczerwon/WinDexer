using WinDexer.Core.ViewModels;
using Radzen; 
namespace WinDexer;

public static class LoadDataArgsExtension
{
    public static FilteredListRequest ToRequest(this LoadDataArgs args)
        => new FilteredListRequest
        {
            Filter = args.Filter,
            OrderBy = args.OrderBy,
            Skip = args.Skip ?? 0,
            Top = args.Top ?? 0
        };
}
