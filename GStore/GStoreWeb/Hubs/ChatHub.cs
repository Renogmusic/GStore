using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStoreData;
using GStoreData.Models;
using Microsoft.AspNet.SignalR;

namespace GStoreWeb.Hubs
{
	public interface iChatHub
	{
		//client-side functions
		void UpdatedUserList(IList<string> listOfNames);
		void MessageIncoming(string message, string sender, bool isPrivate, bool isMine);
		void UserJoined(string userName);
		void UserLeft(string userName);
	}

	public class ChatHub : Hub<iChatHub>
	{
		#region static members and consts

		public const string storeFrontGroupPrefix = "GStore_StoreFront";

		public struct ChatUserInfo
		{
			public int StoreFrontConfigurationId;
			public string Name;
			public string ConnectionId;
			public bool IsSet;
		}
		protected static List<ChatUserInfo> _userList = new List<ChatUserInfo>();

		public static long ActiveUsers(int storeFrontConfigurationId)
		{
			return _userList.Count(u => u.StoreFrontConfigurationId == storeFrontConfigurationId);
		}

		public static bool UserExists(StoreFrontConfiguration config, string name)
		{
			if (_userList.Any(u => u.StoreFrontConfigurationId == config.StoreFrontConfigurationId && u.Name.ToLower() == name.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static string StoreFrontGroupName(StoreFrontConfiguration config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			return "[" + storeFrontGroupPrefix + config.StoreFrontConfigurationId + "]";
		}

		protected static string StoreFrontGroupName(int storeFrontConfigId)
		{
			if (storeFrontConfigId == 0)
			{
				throw new ArgumentNullException("storeFrontConfigId");
			}
			return "[" + storeFrontGroupPrefix + storeFrontConfigId + "]";
		}

		#endregion


		public virtual void SendToAll(string message)
		{
			UpdateSfIdFromQuerystring();
			string user = UserName();
			Clients.OthersInGroup(StoreFrontGroupName(_currentStoreFrontConfigurationId)).MessageIncoming(message, user, false, false);
			Clients.Caller.MessageIncoming(message, user, false, true);
		}

		public virtual void SendToUser(string message, string toUser)
		{
			UpdateSfIdFromQuerystring();
			string sender = UserName();
			IList<string> userList = CurrentStoreFrontUserList();

			Clients.Group(toUser).MessageIncoming(message, "From " + sender, true, false);
			Clients.Group(toUser).UpdatedUserList(userList);

			Clients.Group(sender).MessageIncoming(message, "To " + toUser, true, true);
			Clients.Group(sender).UpdatedUserList(userList);
		}

		[Authorize(Roles = "SystemAdmin")]
		public virtual void GlobalSendToAllAsAdmin(string message)
		{
			string user = "System Admin";
			Clients.All.MessageIncoming(message, user, false, false);
		}

		[Authorize(Roles = "SystemAdmin")]
		public virtual void SendToStoreAllAsAdmin(int sfId, string message)
		{
			string user = "System Admin";
			CurrentStoreFrontGroup.MessageIncoming(message, user, false, false);
		}

		public override System.Threading.Tasks.Task OnConnected()
		{
			UpdateSfIdFromQuerystring();
			Join();
			return base.OnConnected();
		}

		public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
		{
			UpdateSfIdFromQuerystring();
			string userName = UserName();
			ChatUserInfo user = _userList.SingleOrDefault(u => u.StoreFrontConfigurationId == _currentStoreFrontConfigurationId && u.Name.ToLower() == userName.ToLower());
			if (user.IsSet)
			{
				_userList.Remove(user);
				CurrentStoreFrontGroup.UserLeft(userName);
				CurrentStoreFrontGroup.UpdatedUserList(CurrentStoreFrontUserList());
			}
			return base.OnDisconnected(stopCalled);
		}

		public override System.Threading.Tasks.Task OnReconnected()
		{
			UpdateSfIdFromQuerystring();
			Join();
			return base.OnReconnected();
		}

		protected void UpdateSfIdFromQuerystring()
		{
			if (string.IsNullOrEmpty(Context.QueryString["sfId"]))
			{
				throw new ArgumentNullException("sfId");
			}

			int result;
			if (!int.TryParse(Context.QueryString["sfId"], out result))
			{
				throw new ArgumentException("sfId must be numeric", "sfId");
			}
			if (result == 0)
			{
				throw new ArgumentException("sfId cannot be zero", "sfId");
			}
			_currentStoreFrontConfigurationId = result;
		}

		protected virtual IList<string> CurrentStoreFrontUserList()
		{
			return _userList.Where(u => u.StoreFrontConfigurationId == _currentStoreFrontConfigurationId).OrderBy(u => u.Name).Select(u => u.Name).ToList();
		}

		protected void Join()
		{
			string userName = UserName();
			Groups.Add(Context.ConnectionId, userName);
			Groups.Add(Context.ConnectionId, StoreFrontGroupName(_currentStoreFrontConfigurationId));
			ChatUserInfo user = _userList.SingleOrDefault(u => u.StoreFrontConfigurationId == _currentStoreFrontConfigurationId && u.Name.ToLower() == userName.ToLower());
			if (!user.IsSet)
			{
				user = new ChatUserInfo() { ConnectionId = this.Context.ConnectionId, Name = userName, StoreFrontConfigurationId = _currentStoreFrontConfigurationId, IsSet = true };
				_userList.Add(user);
				CurrentStoreFrontGroup.UpdatedUserList(CurrentStoreFrontUserList());
			}

			CurrentStoreFrontGroup.UserJoined(userName);
			Clients.Caller.UpdatedUserList(CurrentStoreFrontUserList());
		}


		protected string UserName()
		{
			string userName = string.Empty;
			
			if (Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
			{
				IGstoreDb ctx = RepositoryFactory.StoreFrontRepository(this.Context.Request.GetHttpContext());
				UserProfile profile = UserProfileExtensions.GetCurrentUserProfile(ctx, true, true);
				userName = profile.FullName;
			}
			else
			{
				if (Context.RequestCookies.ContainsKey("ChatName"))
				{
					Cookie cookie;
					Context.RequestCookies.TryGetValue("ChatName", out cookie);
					userName = "[" + cookie.Value + "]";
				}
				else
				{
					userName = "Guest ID#" + base.Context.ConnectionId;
				}
			}

			return userName;
		}

		protected int _currentStoreFrontConfigurationId = 0;

		protected iChatHub CurrentStoreFrontGroup
		{
			get
			{
				return this.Clients.Group(StoreFrontGroupName(_currentStoreFrontConfigurationId));
			}
		}


	}
}