using MudBlazor;
using Newtonsoft.Json;

namespace SAPDocumentMangement.Shared
{
    public class Methodes : IMethodes
    {
        private readonly ISnackbar _snackbar;
        private readonly IDialogService _dialogService;

        public Methodes(ISnackbar snackbar, IDialogService dialogService)
        {
            _snackbar = snackbar;
            _dialogService = dialogService;
        }

        public Task SnackSuccess(string message)
        {
            _snackbar.Add(message, Severity.Success);
            return Task.CompletedTask;
        }

        public Task SnackError(string message)
        {
            _snackbar.Add(message, Severity.Error);
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteDialog(string itemName)
        {
            var parameters = new DialogParameters<DeleteDialog>
        {
            { x => x.ContentText, $"Do you really want to delete this {itemName}." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };


            var dialog = _dialogService.Show<DeleteDialog>("Delete", parameters, options);
            var result = await dialog.Result;
            if (result?.Canceled == false)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> WarningDialog(string itemName,string action)
        {
            var parameters = new DialogParameters<DeleteDialog>
        {
            { x => x.ContentText, $"Do you really want to {action.ToLower()} this {itemName}." },
            { x => x.ButtonText, "Yes I'm sure" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };


            var dialog = await _dialogService.ShowAsync<DeleteDialog>("Delete", parameters, options);
            var result = await dialog.Result;
            if (result?.Canceled == false)
            {
                return true;
            }
            return false;
        }



        public Task<string> BuildFilterExpressions<T>(GridState<T> state)
        {
            var filters = new List<FilterExpression>();

            foreach (var def in state.FilterDefinitions)
            {
                var propName = def.Column?.PropertyName;
                var value = def.Value?.ToString();
                var op = def.Operator.ToString();

                if (!string.IsNullOrWhiteSpace(propName) && !string.IsNullOrWhiteSpace(value))
                {
                    filters.Add(new FilterExpression
                    {
                        Property = propName,
                        Operator = op,
                        Value = value
                    });
                }
            }
            var json = JsonConvert.SerializeObject(filters);
            return Task.FromResult(json);
        }
    }
}
