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

    private async ValueTask InitAsync()
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

    private void Init()
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

        await JS.InvokeVoidAsync("blazug.show", visible ? "normal" : "hidden");
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
    private async Task<string> DownloadLogsAsync()
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        var strlogs = await JS.InvokeAsync<string>("blazug.downloadLogs");

        return strlogs;
    }

    public void DownloadLogs()
    {
        Task.Run(async () => await DownloadLogsAsync());
    }

    // Display Text

    private async ValueTask DisplayTextAsync(string id, string text, ControlSize minSize = ControlSize.Small)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.displayText", id, text, minSize.ToString().ToLower());
    }

    public void DisplayText(string id, string text, ControlSize minSize = ControlSize.Small)
    {
        Task.Run(async () => await DisplayTextAsync(id, text, minSize));
    }

    // Button

    private async ValueTask CreateButtonAsync(string id, string text, Action onClick, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createButton", id, text, minSize.ToString().ToLower());

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

    private async ValueTask CreateSwitchAsync(string id,  Action<bool> onSwitch, bool initialState, string labelTextWhenOn, string labelTextWhenOff, ControlSize minSize = ControlSize.Small)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createSwitch", id,  labelTextWhenOn, labelTextWhenOff, initialState, minSize.ToString().ToLower());

        Switches.Add(id, onSwitch);
    }

    private async ValueTask CreateSwitchAsync(string id,  Func<bool, Task> onSwitch, bool initialState, string labelTextWhenOn, string labelTextWhenOff, ControlSize minSize = ControlSize.Small)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createSwitch", id,  labelTextWhenOn, labelTextWhenOff, initialState, minSize.ToString().ToLower());

        SwitchesAsync.Add(id, onSwitch);
    }

    private async ValueTask CreateSwitchAsync(string id,  Func<bool, ValueTask> onSwitch, bool initialState, string labelTextWhenOn, string labelTextWhenOff, ControlSize minSize = ControlSize.Small)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createSwitch", id, labelTextWhenOn, labelTextWhenOff, initialState, minSize.ToString().ToLower());

        SwitchesValueAsync.Add(id, onSwitch);
    }

    public void CreateSwitch(string id,  Action<bool> onSwitch, bool initialState = false, string labelTextWhenOn = "On", string labelTextWhenOff = "Off", ControlSize minSize = ControlSize.Small)
    {
        Task.Run(async () => await CreateSwitchAsync(id,  onSwitch, initialState, labelTextWhenOn, labelTextWhenOff, minSize));
    }

    public void CreateSwitch(string id,  Func<bool, Task> onSwitch, bool initialState = false, string labelTextWhenOn = "On", string labelTextWhenOff = "Off", ControlSize minSize = ControlSize.Small)
    {
        Task.Run(async () => await CreateSwitchAsync(id,onSwitch, initialState, labelTextWhenOn, labelTextWhenOff, minSize));
    }
    public void CreateSwitch(string id,  Func<bool, ValueTask> onSwitch, bool initialState = false, string labelTextWhenOn = "On", string labelTextWhenOff = "Off", ControlSize minSize = ControlSize.Small)
    {
        Task.Run(async () => await CreateSwitchAsync(id,  onSwitch,initialState, labelTextWhenOn, labelTextWhenOff, minSize));
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

    private async ValueTask CreateRadioButtonsAsync(string id,  List<string> buttonsText, Action<int> onRadio, int initialState, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createRadio", id,  buttonsText, initialState, minSize.ToString().ToLower());

        Radios.Add(id, onRadio);
    }

    private async ValueTask CreateRadioButtonsAsync(string id,  List<string> buttonsText, Func<int, Task> onRadio, int initialState, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createRadio", id,  buttonsText, initialState, minSize.ToString().ToLower());

        RadiosAsync.Add(id, onRadio);
    }

    private async ValueTask CreateRadioButtonsAsync(string id, List<string> buttonsText, Func<int, ValueTask> onRadio, int initialState, ControlSize minSize = ControlSize.Content)
    {
        if (Initialized == false)
        {
            await InitAsync();
        }

        await JS.InvokeVoidAsync("blazug.createRadio", id,  buttonsText, initialState, minSize.ToString().ToLower());

        RadiosValueAsync.Add(id, onRadio);
    }

    public void CreateRadioButtons(string id, List<string> buttonsText, Action<int> onRadio, int initialState = 0, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateRadioButtonsAsync(id,  buttonsText, onRadio, initialState, minSize));
    }

    public void CreateRadioButtons(string id,  List<string> buttonsText, Func<int, Task> onRadio, int initialState = 0, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateRadioButtonsAsync(id,  buttonsText, onRadio, initialState, minSize));
    }
    public void CreateRadioButtons(string id, List<string> buttonsText, Func<int, ValueTask> onRadio, int initialState = 0, ControlSize minSize = ControlSize.Content)
    {
        Task.Run(async () => await CreateRadioButtonsAsync(id,  buttonsText, onRadio, initialState, minSize));
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
