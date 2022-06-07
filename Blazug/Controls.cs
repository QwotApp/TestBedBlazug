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

    internal async ValueTask Init(int maxLogs)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("init", maxLogs, DotNetHelper);
    }

    public async ValueTask DisplayText(string title, string value, BlazugItemSize minSize = BlazugItemSize.Half)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("displayText", title, value, minSize.ToString().ToLower());
    }

    public async ValueTask CreateButton(string title, string buttonText, Action onClick, BlazugItemSize minSize = BlazugItemSize.Content)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createButton", title, buttonText, minSize.ToString().ToLower());

        Buttons.Add(title, onClick);
    }

    public async ValueTask CreateButton(string title, string buttonText, Func<Task> onClick, BlazugItemSize minSize = BlazugItemSize.Content)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createButton", title, buttonText, minSize.ToString().ToLower());

        ButtonsAsync.Add(title, onClick);
    }

    [JSInvokable]
    public async ValueTask ButtonClicked(string title)
    {
        if (Buttons.ContainsKey(title))
        {
            Buttons[title].Invoke();
        }

        if (ButtonsAsync.ContainsKey(title))
        {
            await ButtonsAsync[title].Invoke();
        }
    }

    public async ValueTask CreateSwitch(string title, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Action<bool> onSwitch, BlazugItemSize minSize = BlazugItemSize.OneThird)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createSwitch", title, initialState, switchTextWhenOn, switchTextWhenOff, minSize.ToString().ToLower());

        Switches.Add(title, onSwitch);
    }

    public async ValueTask CreateSwitch(string title, bool initialState, string switchTextWhenOn, string switchTextWhenOff, Func<bool, Task> onSwitch, BlazugItemSize minSize = BlazugItemSize.OneThird)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createSwitch", title, initialState, switchTextWhenOn, switchTextWhenOff, minSize.ToString().ToLower());

        SwitchesAsync.Add(title, onSwitch);
    }

    [JSInvokable]
    public async ValueTask SwitchClicked(string title, bool state)
    {
        if (Switches.ContainsKey(title))
        {
            Switches[title].Invoke(state);
        }

        if (SwitchesAsync.ContainsKey(title))
        {
            await SwitchesAsync[title].Invoke(state);
        }
    }

    public async ValueTask CreateRadio(string title, int initialState, List<string> buttonsText, Action<int> onRadio, BlazugItemSize minSize = BlazugItemSize.Content)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createRadio", title, initialState, buttonsText, minSize.ToString().ToLower());

        Radios.Add(title, onRadio);
    }


    public async ValueTask CreateRadio(string title, int initialState, List<string> buttonsText, Func<int, Task> onRadio, BlazugItemSize minSize = BlazugItemSize.Content)
    {
        var module = await ModuleTask.Value;

        await module.InvokeVoidAsync("createRadio", title, initialState, buttonsText, minSize.ToString().ToLower());

        RadiosAsync.Add(title, onRadio);
    }

    [JSInvokable]
    public async ValueTask RadioClicked(string title, int index)
    {
        if (Radios.ContainsKey(title))
        {
            Radios[title].Invoke(index);
        }

        if (RadiosAsync.ContainsKey(title))
        {
            await RadiosAsync[title].Invoke(index);
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
