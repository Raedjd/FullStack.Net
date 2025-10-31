using MudBlazor;

namespace SAPDocumentMangement.Shared
{
    public static class MudDataGridExtensions
    {
        public static async Task NavigateToLastPageWithCollection<T>(
        this MudDataGrid<T> grid,
        ICollection<T> items,
        Action stateHasChanged)
        {
            // Reload the grid data
            await grid.ReloadServerData();

            // Then navigate to last page
            var itemCount = items?.Count ?? 0;
            var pageSize = grid.RowsPerPage;
            var lastPage = (int)Math.Ceiling((double)itemCount / pageSize);

            if (lastPage > 1)
            {
                grid.CurrentPage = lastPage - 1;
                stateHasChanged();
            }
        }
    }
}

