using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Windexer.Components.Shared
{
    public partial class EventConsole: ComponentBase
    {
        [Inject] IJSRuntime JSRuntime { get; set; }

        private bool _paused;
        private class Message
        {
            public DateTime Date { get; set; }
            public string Text { get; set; }
            public AlertStyle Style { get; set; }
            public bool CanBeDeleted { get; set; }
        }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attributes { get; set; }
        IList<Message> _messages = new List<Message>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender || _paused)
                return;
            
            await JSRuntime.InvokeVoidAsync("eval", $"document.getElementById('event-console').scrollTop = document.getElementById('event-console').scrollHeight");            
        }

        private void OnPauseClick()
        {
            _paused = !_paused;
        }

        void OnClearClick() => Clear();

        public void Clear()
        {
            _messages.Clear();
            if (!_paused)
                InvokeAsync(StateHasChanged);
        }

        public void Log(string message, AlertStyle alertStyle = AlertStyle.Base, bool? canBeDeleted = null)
        {
            if (!canBeDeleted.HasValue)
            {
                canBeDeleted = alertStyle != AlertStyle.Danger
                            && alertStyle != AlertStyle.Warning
                            && alertStyle != AlertStyle.Success;
            }

            _messages.Add(new Message { Date = DateTime.Now, Text = message, Style = alertStyle, CanBeDeleted = canBeDeleted.Value });

            if (_messages.Count > 200)
            {
                var nbToDelete = _messages.Count - 200;
                var toDelete = _messages.Where(m_ => m_.CanBeDeleted).Take(nbToDelete);
                _messages = _messages.Except(toDelete).ToList();
            }
            if (!_paused)
                InvokeAsync(StateHasChanged);
        }
    }
}
