using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace WinDexer.Components.Shared
{
    public partial class EventConsole: ComponentBase
    {
        [Inject] IJSRuntime JsRuntime { get; set; } = null!;

        private bool _paused;
        private bool _truncateMessageAdded;
        private class Message
        {
            public DateTime Date { get; set; }
            public string Text { get; set; } = null!;
            public AlertStyle Style { get; set; }
            public bool CanBeDeleted { get; set; }
        }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
        private List<Message> _messages = new ();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender || _paused)
                return;
            
            await JsRuntime.InvokeVoidAsync("eval", "document.getElementById('event-console').scrollTop = document.getElementById('event-console').scrollHeight");            
        }

        private void OnPauseClick()
        {
            _paused = !_paused;
        }

        void OnClearClick() => Clear();

        public void Clear()
        {
            _messages.Clear();
            _truncateMessageAdded = false;
            if (!_paused)
                InvokeAsync(StateHasChanged);
        }

        public void Log(string message, AlertStyle alertStyle = AlertStyle.Base, bool? canBeDeleted = null)
        {
            canBeDeleted ??= alertStyle != AlertStyle.Danger
                          && alertStyle != AlertStyle.Warning
                          && alertStyle != AlertStyle.Success;

            _messages.Add(new Message { Date = DateTime.Now, Text = message, Style = alertStyle, CanBeDeleted = canBeDeleted.Value });

            if (_messages.Count > 200)
            {
                AddTruncateMessage();

                var nbToDelete = _messages.Count - 200;
                var toDelete = _messages.Where(m_ => m_.CanBeDeleted).Take(nbToDelete);
                _messages = _messages.Except(toDelete).ToList();
                
            }
            if (!_paused)
                InvokeAsync(StateHasChanged);
        }

        public void AddTruncateMessage()
        {
            if (_truncateMessageAdded)
                return;
            
            _truncateMessageAdded = true;
            var truncPos = _messages.FindIndex(i_ => i_.CanBeDeleted);
            if (truncPos < 0)
                truncPos = _messages.Count;
            _messages.Insert(truncPos, new Message { Date = DateTime.Now, Text = "...", CanBeDeleted = false, Style = AlertStyle.Base });
        }
    }
}
