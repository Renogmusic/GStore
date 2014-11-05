namespace GStore.AppHtmlHelpers
{
	public class GaEvent
	{
		public GaEvent(string category, string action, string label)
		{
			this.Category = category;
			this.Action = action;
			this.Label = label;
		}

		public GaEvent() { }

		public string Category { get; set; }
		public string Action { get; set; }
		public string Label { get; set; }
	}
}