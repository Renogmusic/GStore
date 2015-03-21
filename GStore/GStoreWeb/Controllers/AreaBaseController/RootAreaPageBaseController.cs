using GStoreData.ControllerBase;

namespace GStoreWeb.Controllers.AreaBaseController
{
	public abstract class RootAreaPageBaseController: PageBaseController
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

