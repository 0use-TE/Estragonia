using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Avalonia.Threading;
using Godot;
using GdDispatcher = Godot.Dispatcher;
using SysTimer = System.Threading.Timer;

namespace JLeb.Estragonia;

/// <summary>An implementation of <see cref="IDispatcherImpl"/> that uses the underlying Godot dispatcher.</summary>
[SuppressMessage(
	"Design",
	"CA1001:Types that own disposable fields should be disposable",
	Justification = "Disposed by GodotPlatform.Reset when the editor unloads assemblies"
)]
internal sealed class GodotDispatcherImpl : IDispatcherImpl {

	private readonly Thread _mainThread;
	private readonly SendOrPostCallback _invokeSignaled;
	private readonly SendOrPostCallback _invokeTimer;
	private SysTimer? _timer;

	public long Now
		=> (long) Time.GetTicksMsec();

	public bool CurrentThreadIsLoopThread
		=> _mainThread == Thread.CurrentThread;

	public event Action? Signaled;

	public event Action? Timer;

	public GodotDispatcherImpl(Thread mainThread) {
		_mainThread = mainThread;
		_invokeSignaled = InvokeSignaled;
		_invokeTimer = InvokeTimer;
		_timer = new(OnTimerTick, this, Timeout.Infinite, Timeout.Infinite);
	}

	public void UpdateTimer(long? dueTimeInMs) {
		if (_timer is null)
			return;

		var interval = dueTimeInMs is { } value
			? Math.Clamp(value - Now, 0L, 0xFFFFFFFEL)
			: Timeout.Infinite;

		_timer.Change(interval, Timeout.Infinite);
	}

	/// <summary>
	/// Drops the thread-pool timer. That timer is a GC root outside the collectible ALC
	/// and will block Godot assembly unload if left alive.
	/// </summary>
	public void Pause() {
		_timer?.Change(Timeout.Infinite, Timeout.Infinite);
		_timer?.Dispose();
		_timer = null;
		Signaled = null;
		Timer = null;
	}

	/// <summary>Recreates the thread-pool timer after <see cref="Pause"/>.</summary>
	public void Resume() {
		_timer ??= new(OnTimerTick, this, Timeout.Infinite, Timeout.Infinite);
	}

	private void OnTimerTick(object? state)
		=> GdDispatcher.SynchronizationContext.Post(_invokeTimer, null);

	public void Signal()
		=> GdDispatcher.SynchronizationContext.Post(_invokeSignaled, this);

	private void InvokeSignaled(object? state)
		=> Signaled?.Invoke();

	private void InvokeTimer(object? state)
		=> Timer?.Invoke();

}
