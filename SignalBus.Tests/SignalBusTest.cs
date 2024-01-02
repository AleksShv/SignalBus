using SignalBus.Contracts;
using System.Diagnostics;

namespace SignalBus.Tests;

public class SignalBusTest
{
    private readonly TestSignal _testSignal = new();

    [Fact]
    public void Trigger_RunAsyncHandlersSynchronously_CompleteSynchronously()
    {
        using var bus = SignalBus.Core.SignalBus.Instance;
        RegisterSignals(bus);

        var sw = Stopwatch.StartNew();
        sw.Start();
        bus.Trigger(_testSignal);
        sw.Stop();

        Assert.Equal(8, sw.Elapsed.Seconds);
    }

    [Fact]
    public async Task TriggerAsync_RunAsyncHandlersAsynchronously_CompleteAsynchronously()
    {
        using var bus = SignalBus.Core.SignalBus.Instance;
        RegisterSignals(bus);

        var sw = Stopwatch.StartNew();
        sw.Start();
        await bus.TriggerAsync(_testSignal);
        sw.Stop();

        Assert.Equal(5, sw.Elapsed.Seconds);
    }


    private void RegisterSignals(ISignalBus bus)
    {
        bus.Register<TestSignal>(OnTest1);
        bus.Register<TestSignal>(OnTest2);
    }

    private async Task OnTest1(TestSignal signal)
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
    }

    private async Task OnTest2(TestSignal signal)
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
    }


    public record TestSignal : ISignal;
}