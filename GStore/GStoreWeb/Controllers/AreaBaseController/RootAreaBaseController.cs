
namespace GStoreWeb.Controllers.AreaBaseController
{
	public abstract class RootAreaBaseController: GStoreData.ControllerBase.BaseController
	{
		public override sealed GStoreData.ControllerBase.BaseController GStoreErrorControllerForArea
		{
			get
			{
				return new GStoreWeb.Controllers.GStoreController();
			}
		}

		protected override sealed string Area
		{
			get
			{
				return "";
			}
		}
	}
}

