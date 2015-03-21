
namespace GStoreData.ViewModels
{
	public class DigitalDownloadFileInfo
	{
		public DigitalDownloadFileInfo(string digitalDownloadFileName, int productId)
		{
			this.DigitalDownloadFileName = digitalDownloadFileName;
			this.ProductId = productId;
		}
		public string DigitalDownloadFileName { get; set;}
		public int ProductId { get; set; }
	}
}
