using System;
using System.Linq;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreWeb.Controllers
{
	public class ChatController : AreaBaseController.RootAreaBaseController
	{
		#region Public Chat Actions

		public ActionResult Index(string name)
		{
			if (!CheckChatEnabled())
			{
				return BounceToHome();
			}
			if (!CheckAccess())
			{
				return BounceToLogin();
			}

			if (ChatName() != null)
			{
				return RedirectToAction("Messages");
			}

			if (!string.IsNullOrWhiteSpace(name))
			{
				name = name.Trim();
				if (NameIsTaken(name))
				{
					this.ModelState.AddModelError("name", "Sorry, the name '" + name + "' is already taken, please enter a new name.");
				}
				else
				{
					SetChatNameCookie(name.Trim());
					return RedirectToAction("Index");
				}
			}

			return View("Index");
		}

		public ActionResult Signout()
		{
			ClearChatNameCookie();
			return RedirectToAction("Index");
		}

		public ActionResult Messages()
		{
			if (!CheckChatEnabled())
			{
				return BounceToHome();
			}
			if (!CheckAccess())
			{
				return BounceToLogin();
			}

			string chatName = ChatName();
			if (chatName == null)
			{
				return RedirectToAction("Index");
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Chat, UserActionActionEnum.Chat_Start, "Chat Name: " + chatName, true);
			return View("Messages", model: chatName);
		}

		#endregion

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.CartTheme.FolderName;
			}
		}

		protected string ChatName()
		{
			if (User.IsRegistered())
			{
				return CurrentUserProfileOrThrow.FullName;
			}
			else if (Request.Cookies.AllKeys.Contains("ChatName"))
			{
				return "[" + Request.Cookies.Get("ChatName").Value + "]";
			}

			return null;
		}

		protected void SetChatNameCookie(string name)
		{
			if (Request.Cookies["ChatName"] == null)
			{
				System.Web.HttpCookie newCookie = new System.Web.HttpCookie("ChatName", name);
				Response.AppendCookie(newCookie);
			}
			else
			{
				System.Web.HttpCookie oldCookie = Request.Cookies["ChatName"];
				oldCookie.Value = name;
				Response.SetCookie(oldCookie);
			}
		}

		protected void ClearChatNameCookie()
		{
			if (Request.Cookies["ChatName"] != null)
			{
				System.Web.HttpCookie myCookie = new System.Web.HttpCookie("ChatName");
				myCookie.Expires = DateTime.Now.AddDays(-1);
				Response.Cookies.Add(myCookie);
			}
		}


		protected bool CheckChatEnabled()
		{
			if (!Settings.AppEnableChat)
			{
				AddUserMessage("Chat Unavailable", "Sorry, chat is not available for this site.", UserMessageType.Danger);
				return false;
			}

			if (!CurrentStoreFrontConfigOrThrow.ChatEnabled)
			{
				AddUserMessage("Chat Unavailable", "Sorry, chat is not available for this store.", UserMessageType.Danger);
				return false;
			}

			return true;
		}

		protected bool CheckAccess()
		{
			if ((CurrentStoreFrontConfigOrThrow.ChatRequireLogin) && (CurrentUserProfileOrNull == null))
			{
				return false;
			}
			return true;
		}

		protected bool NameIsTaken(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return true;
			}

			return Hubs.ChatHub.UserExists("[" + name + "]");
		}

		protected ActionResult BounceToLogin()
		{
			AddUserMessage("Login required.", "You must log in to use Chat.", UserMessageType.Info);
			return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
		}

		protected ActionResult BounceToHome()
		{
			return Redirect(Url.GStoreLocalUrl("/"));
		}
	}
}
