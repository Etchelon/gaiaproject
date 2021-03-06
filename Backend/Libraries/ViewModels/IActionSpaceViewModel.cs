namespace GaiaProject.ViewModels
{
	public interface IActionSpaceViewModel<T>
	{
		string Kind { get; }
		T Type { get; set; }
		bool IsAvailable { get; set; }
	}
}