using Microsoft.JSInterop;

namespace Blazug;

public class Controls : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> ModuleTask;

    private readonly DotNetObjectReference<Controls> DotNetHelper;


    private readonly Dictionary<string, Action> Buttons;

    private readonly Dictionary<string, Action<bool>> Switches;

    private readonly Dictionary<string, Action<int>> Radios;

    private readonly Dictionary<string, Func<Task>> ButtonsAsync;

    private readonly Dictionary<string, Func<bool, Task>> SwitchesAsync;

    private readonly Dictionary<string, Func<int, Task>> RadiosAsync;

    public Controls(IJSRuntime jsRuntime)
    {
        ModuleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Blazug/blazug.js").AsTask());

        DotNetHelper = DotNetObjectReference.Create(this);

        Buttons = new();

        Switches = new();

        Radios = new();

        ButtonsAsync = new();

        SwitchesAsync = new();

        RadiosAsync = new();
    }

    internal async ValueTask InitAsync(int maxLogs)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("init", maxLogs, DotNetHelper);
    }

    internal void Init(int maxLogs) =>
        Task.Run(async () => await InitAsync(maxLogs));

    public async ValueTask DisplayTextAsync(string id, string value, ControlSize minSize = ControlSize.Half)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("displayText", id, value, minSize.ToString().ToLower());
    }

    public void DisplayText(string id, string value, ControlSize minSize = ControlSize.Half) =>
        Task.Run(async () => await DisplayTextAsync(id, value, minSize));


    public async ValueTask CreateButtonAsync(string id, string buttonText, Action onClick, ControlSize minSize = ControlSize.Content)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createButton", id, buttonText, minSize.ToString().ToLower());

        Buttons.Add(id, onClick);
    }

    public async ValueTask CreateButtonAsync(string id, string buttonText, Func<Task> onClick, ControlSize minSize = ControlSize.Content)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createButton", id, buttonText, minSize.ToString().ToLower());

        ButtonsAsync.Add(id, onClick);
    }

    public void CreateButton(string id, string buttonText, Action onClick, ControlSize minSize = ControlSize.Content) =>
        Task.Run(async () => await CreateButtonAsync(id, buttonText, onClick, minSize));
    public void CreateButton(string id, string buttonText, Func<Task> onClick, ControlSize minSize = ControlSize.Content) =>
        Task.Run(async () => await CreateButtonAsync(id, buttonText, onClick, minSize));


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
    }

    public async ValueTask CreateSwitchAsync(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Action<bool> onSwitch, ControlSize minSize = ControlSize.OneThird)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createSwitch", id, initialState, switchTextWhenOn, switchTextWhenOff, minSize.ToString().ToLower());

        Switches.Add(id, onSwitch);
    }

    public async ValueTask CreateSwitchAsync(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Func<bool, Task> onSwitch, ControlSize minSize = ControlSize.OneThird)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createSwitch", id, initialState, switchTextWhenOn, switchTextWhenOff, minSize.ToString().ToLower());

        SwitchesAsync.Add(id, onSwitch);
    }

    public void CreateSwitch(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Action<bool> onSwitch, ControlSize minSize = ControlSize.OneThird) =>
        Task.Run(async () => await CreateSwitchAsync(id, initialState, switchTextWhenOn, switchTextWhenOff, onSwitch, minSize));
    public void CreateSwitch(string id, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Func<bool, Task> onSwitch, ControlSize minSize = ControlSize.OneThird) =>
        Task.Run(async () => await CreateSwitchAsync(id, initialState, switchTextWhenOn, switchTextWhenOff, onSwitch, minSize));



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
    }

    public async ValueTask CreateRadioButtonsAsync(string id, int initialState, List<string> buttonsText, Action<int> onRadio, ControlSize minSize = ControlSize.Content)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createRadio", id, initialState, buttonsText, minSize.ToString().ToLower());

        Radios.Add(id, onRadio);
    }


    public async ValueTask CreateRadioButtonsAsync(string id, int initialState, List<string> buttonsText, Func<int, Task> onRadio, ControlSize minSize = ControlSize.Content)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createRadio", id, initialState, buttonsText, minSize.ToString().ToLower());

        RadiosAsync.Add(id, onRadio);
    }

    public void CreateRadioButtons(string id, int initialState, List<string> buttonsText, Action<int> onRadio, ControlSize minSize = ControlSize.Content) =>
        Task.Run(async () => await CreateRadioButtonsAsync(id, initialState, buttonsText, onRadio, minSize));

    public void CreateRadioButtons(string id, int initialState, List<string> buttonsText, Func<int, Task> onRadio, ControlSize minSize = ControlSize.Content) =>
        Task.Run(async () => await CreateRadioButtonsAsync(id, initialState, buttonsText, onRadio, minSize));


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
    }



    public async ValueTask DisposeAsync()
    {
        if (ModuleTask.IsValueCreated)
        {
            var module = await ModuleTask.Value;

            await module.DisposeAsync();
        }

        DotNetHelper.Dispose();
    }



}
