namespace PCBManufacturing.PresentationLogic;

internal static class UiThread
{
    public static SynchronizationContext? Capture() => SynchronizationContext.Current;

    public static void Run(SynchronizationContext? context, Action action)
    {
        if (context is null || SynchronizationContext.Current == context)
        {
            action();
            return;
        }

        context.Send(_ => action(), null);
    }
}
