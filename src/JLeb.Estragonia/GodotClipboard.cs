using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Godot;

namespace JLeb.Estragonia;

/// <summary>An implementation of <see cref="IClipboard"/> that uses Godot clipboard methods.</summary>
internal sealed class GodotClipboard : IClipboard {

	public Task ClearAsync() {
		DisplayServer.ClipboardSet(String.Empty);
		return Task.CompletedTask;
	}

	public Task FlushAsync()
		=> Task.CompletedTask;

	public async Task SetDataAsync(IAsyncDataTransfer? dataTransfer) {
		if (dataTransfer is null) {
			DisplayServer.ClipboardSet(String.Empty);
			return;
		}

		var text = await dataTransfer.TryGetTextAsync();
		DisplayServer.ClipboardSet(text ?? String.Empty);
	}

	public Task<IAsyncDataTransfer?> TryGetDataAsync() {
		var text = DisplayServer.ClipboardGet();
		if (String.IsNullOrEmpty(text))
			return Task.FromResult<IAsyncDataTransfer?>(null);

		var transfer = new DataTransfer();
		transfer.Add(DataTransferItem.CreateText(text));
		return Task.FromResult<IAsyncDataTransfer?>(transfer);
	}

	public Task<IAsyncDataTransfer?> TryGetInProcessDataAsync()
		=> TryGetDataAsync();

}
