namespace Blazug;

internal record DisplayTextDescriptor(string Id,string Value);
internal record ButtonDescriptor(string Id, string ButtonText, Action OnClick, ControlSize MinSize);
internal record ButtonAsyncDescriptor(string Id, string ButtonText, Func<Task> OnClick, ControlSize MinSize);
internal record SwitchDescriptor(string Id, bool InitialState, string SwitchTextWhenOn, string SwitchTextWhenOff, Action<bool> OnSwitch, ControlSize MinSize);
internal record SwitchAsyncDescriptor(string Id, bool InitialState, string switchTextWhenOn, string SwitchTextWhenOff, Func<bool, Task> OnSwitch, ControlSize MinSize);
internal record RadioButtonDescriptor(string Id, int InitialState, List<string> ButtonsText, Action<int> OnRadio, ControlSize MinSize);
internal record RadioButtonAsyncDescriptor(string Id, int InitialState, List<string> ButtonsText, Func<int, Task> OnRadio, ControlSize MinSize);