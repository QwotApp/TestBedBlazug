using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Text;

namespace BlazugLib;

public class Controls : IDisposable
{
    private readonly IJSRuntime JS;

    private readonly ILogger Log;

    private bool Initialized;

    private int ErrorReported;

    private readonly DotNetObjectReference<Controls> DotNetHelper;

    private readonly Dictionary<string, Action> Buttons;

    private readonly Dictionary<string, Action<bool>> Switches;

    private readonly Dictionary<string, Action<int>> Radios;

    private readonly Dictionary<string, Func<Task>> ButtonsAsync;

    private readonly Dictionary<string, Func<bool, Task>> SwitchesAsync;

    private readonly Dictionary<string, Func<int, Task>> RadiosAsync;

    private readonly Dictionary<string, Func<ValueTask>> ButtonsValueAsync;

    private readonly Dictionary<string, Func<bool, ValueTask>> SwitchesValueAsync;

    private readonly Dictionary<string, Func<int, ValueTask>> RadiosValueAsync;


    public Controls(IJSRuntime js, ILogger<Controls> logger)
    {
        JS = js;

        Log = logger;

        Initialized = false;

        ErrorReported = 0;

        DotNetHelper = DotNetObjectReference.Create(this);

        Buttons = new();

        Switches = new();

        Radios = new();

        ButtonsAsync = new();

        SwitchesAsync = new();

        RadiosAsync = new();

        ButtonsValueAsync = new();

        SwitchesValueAsync = new();

        RadiosValueAsync = new();
    }

    internal async ValueTask InitAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("blazug.initDotnet", DotNetHelper);

            Initialized = true;
        }
        catch(JSException)
        {
            if(ErrorReported++ == 0)
            {
                Log.LogWarning("Blazug needs blazug.js & blazug.css to be added to index.html first to catch console logs. See github...");
            }
        }
    }

    internal void Init()
    {
        Task.Run(async () => await InitAsync());
    }


    // Show/Hide

    private async ValueTask ShowAsync(bool visible)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.show", visible);
    }

    public void Show(bool visible)
    {
        Task.Run(async () => await ShowAsync(visible));
    }

    // MaxLogs

    private async ValueTask SetMaxLogsAsync(int maxLogs)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.setMaxLogs", maxLogs);
    }

    public void SetMaxLogs(int maxLogs)
    {
        Task.Run(async () => await SetMaxLogsAsync(maxLogs));
    }


    // GetLogs

    public async Task<string> GetLogsAsync()
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        var strlogs = await JS.InvokeAsync<string>("blazug.getLogs");

        return strlogs;
    }

    public async Task DownloadString(string filename, string logs) =>
        await DownloadString(filename, logs, Encoding.UTF8);

    public async Task DownloadString(string filename,string logs, Encoding encoding)
    {

        var memStream = new MemoryStream(encoding.GetBytes(logs));

        using var streamRef = new DotNetStreamReference(stream: memStream);

        await JS.InvokeVoidAsync("blazug.downloadFileFromStream", filename, streamRef);

    }

    // Display Text

    private async ValueTask DisplayTextAsync(string id, string value, ControlSize minSize = ControlSize.Medium)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.displayText", id, value, minSize.ToString().ToLower());
    }

    public void DisplayText(string id, string value, ControlSize minSize = ControlSize.Medium)
    {
        Task.Run(async () => await DisplayTextAsync(id, value, minSize));
    }

    // Button

    private async ValueTask CreateButtonAsync(string id, string buttonText, Action onClick, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createButton", id, buttonText, minSize.ToString().ToLower());

        Buttons.Add(id, onClick);
    }

    private async ValueTask CreateButtonAsync(string id, string buttonText, Func<Task> onClick, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createButton", id, buttonText, minSize.ToString().ToLower());

        ButtonsAsync.Add(id, onClick);
    }

    private async ValueTask CreateButtonAsync(string id, string buttonText, Func<ValueTask> onClick, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createButton", id, buttonText, minSize.ToString().ToLower());

        ButtonsValueAsync.Add(id, onClick);
    }

    public void CreateButton(string id, string buttonText, Action onClick, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateButtonAsync(id, buttonText, onClick, minSize));
    }

    public void CreateButton(string id, string buttonText, Func<Task> onClick, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateButtonAsync(id, buttonText, onClick, minSize));
    }
    public void CreateButton(string id, string buttonText, Func<ValueTask> onClick, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateButtonAsync(id, buttonText, onClick, minSize));
    }


    [JSInvokable]
    public async ValueTask ButtonClicked(string id)
    {
        if (Buttons.ContainsKey(id))
        {
            Buttons[id].Invoke();
        }

        if (ButtonsAsync.ContainsKey(id))
        {
            await ButtonsAsync[id].Invoke();
        }

        if (ButtonsValueAsync.ContainsKey(id))
        {
            await ButtonsValueAsync[id].Invoke();
        }
    }

    // Switch

    private async ValueTask CreateSwitchAsync(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Action<bool> onSwitch, ControlSize minSize = ControlSize.Small)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createSwitch", id, initialState, switchTextWhenOn, switchTextWhenOff, minSize.ToString().ToLower());

        Switches.Add(id, onSwitch);
    }

    private async ValueTask CreateSwitchAsync(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Func<bool, Task> onSwitch, ControlSize minSize = ControlSize.Small)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createSwitch", id, initialState, switchTextWhenOn, switchTextWhenOff, minSize.ToString().ToLower());

        SwitchesAsync.Add(id, onSwitch);
    }

    private async ValueTask CreateSwitchAsync(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Func<bool, ValueTask> onSwitch, ControlSize minSize = ControlSize.Small)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createSwitch", id, initialState, switchTextWhenOn, switchTextWhenOff, minSize.ToString().ToLower());

        SwitchesValueAsync.Add(id, onSwitch);
    }

    public void CreateSwitch(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Action<bool> onSwitch, ControlSize minSize = ControlSize.Small)
    {
        Task.Run(async () => await CreateSwitchAsync(id, initialState, switchTextWhenOn, switchTextWhenOff, onSwitch, minSize));
    }

    public void CreateSwitch(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Func<bool, Task> onSwitch, ControlSize minSize = ControlSize.Small)
    {
        Task.Run(async () => await CreateSwitchAsync(id, initialState, switchTextWhenOn, switchTextWhenOff, onSwitch, minSize));
    }
    public void CreateSwitch(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Func<bool, ValueTask> onSwitch, ControlSize minSize = ControlSize.Small)
    {
        Task.Run(async () => await CreateSwitchAsync(id, initialState, switchTextWhenOn, switchTextWhenOff, onSwitch, minSize));
    }

    [JSInvokable]
    public async ValueTask SwitchClicked(string id, bool state)
    {
        if (Switches.ContainsKey(id))
        {
            Switches[id].Invoke(state);
        }

        if (SwitchesAsync.ContainsKey(id))
        {
            await SwitchesAsync[id].Invoke(state);
        }

        if (SwitchesValueAsync.ContainsKey(id))
        {
            await SwitchesValueAsync[id].Invoke(state);
        }
    }

    // Radio Buttons

    private async ValueTask CreateRadioButtonsAsync(string id, int initialState, List<string> buttonsText, Action<int> onRadio, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createRadio", id, initialState, buttonsText, minSize.ToString().ToLower());

        Radios.Add(id, onRadio);
    }

    private async ValueTask CreateRadioButtonsAsync(string id, int initialState, List<string> buttonsText, Func<int, Task> onRadio, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createRadio", id, initialState, buttonsText, minSize.ToString().ToLower());

        RadiosAsync.Add(id, onRadio);
    }

    private async ValueTask CreateRadioButtonsAsync(string id, int initialState, List<string> buttonsText, Func<int, ValueTask> onRadio, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createRadio", id, initialState, buttonsText, minSize.ToString().ToLower());

        RadiosValueAsync.Add(id, onRadio);
    }

    public void CreateRadioButtons(string id, int initialState, List<string> buttonsText, Action<int> onRadio, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateRadioButtonsAsync(id, initialState, buttonsText, onRadio, minSize));
    }

    public void CreateRadioButtons(string id, int initialState, List<string> buttonsText, Func<int, Task> onRadio, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateRadioButtonsAsync(id, initialState, buttonsText, onRadio, minSize));
    }
    public void CreateRadioButtons(string id, int initialState, List<string> buttonsText, Func<int, ValueTask> onRadio, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateRadioButtonsAsync(id, initialState, buttonsText, onRadio, minSize));
    }


    [JSInvokable]
    public async ValueTask RadioClicked(string id, int index)
    {
        if (Radios.ContainsKey(id))
        {
            Radios[id].Invoke(index);
        }

        if (RadiosAsync.ContainsKey(id))
        {
            await RadiosAsync[id].Invoke(index);
        }

        if (RadiosValueAsync.ContainsKey(id))
        {
            await RadiosValueAsync[id].Invoke(index);
        }
    }

    public void Dispose() 
    {
        Initialized = true;

        DotNetHelper.Dispose();
    }
}
