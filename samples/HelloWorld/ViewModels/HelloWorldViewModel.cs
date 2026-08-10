using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloWorld.ViewModels;

public sealed partial class HelloWorldViewModel : ObservableObject {

	public ObservableCollection<string> Messages { get; } = new();
	public HelloWorldViewModel() {
		Messages.Add("Hello World!");
		Messages.Add($"Current Time: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
	}
}
