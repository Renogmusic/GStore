using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStoreData;
using GStoreData.Models;
using Microsoft.AspNet.SignalR;

namespace GStoreWeb.Hubs
{
	public class ChatHub : Hub
	{
		private static List<String> _userList = new List<string>();

		public static Int64 ActiveUsers()
		{
			return _userList.Count;
		}

		public static bool UserExists(string name)
		{
			return _userList.Contains(name);
		}

		public void SendToAll(string message)
		{
			string user = UserName();
			Clients_All_MessageIncoming(message, user, false);
		}

		public void SendToUser(string message, string toUser)
		{
			string sender = UserName();
			User_MessageIncoming(message, toUser, sender);
		}

		[Authorize(Roles = "SystemAdmin")]
		public void SendToAllAsAdmin(string message)
		{
			string user = "System Admin";
			Clients_All_MessageIncoming(message, user, false);
		}

		private void Clients_All_MessageIncoming(string message, string sender, bool isPrivate)
		{
			Clients.All.MessageIncoming(message, sender, isPrivate);
			Clients_All_UpdatedUserList(_userList);
		}

		private void Clients_All_UpdatedUserList(IList<string> newUserList)
		{
			Clients.All.UpdatedUserList(newUserList);
		}

		private void User_MessageIncoming(string message, string toUser, string sender)
		{
			Clients.Group(toUser).MessageIncoming(message, "From " + sender, true);
			Clients.Group(toUser).UpdateUserList(_userList);

			Clients.Group(sender).MessageIncoming(message, "To " + toUser, true);
			Clients.Group(sender).UpdateUserList(_userList);
		}

		public override System.Threading.Tasks.Task OnConnected()
		{
			Join();
			return base.OnConnected();
		}

		public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
		{
			string userName = UserName();
			if (_userList.Contains(userName))
			{
				_userList.Remove(userName);
				SendToAllAsAdmin("User Left: " + userName);
				Clients_All_UpdatedUserList(_userList);
			}
			return base.OnDisconnected(stopCalled);
		}

		public override System.Threading.Tasks.Task OnReconnected()
		{
			Join();
			return base.OnReconnected();
		}

		private void Join()
		{
			string userName = UserName();
			SendToAllAsAdmin("User joined: " + userName);
			Groups.Add(Context.ConnectionId, userName);
			if (!_userList.Contains(userName))
			{
				_userList.Add(userName);
				Clients_All_UpdatedUserList(_userList);
			}
			Clients.Caller.UpdateUserList(_userList);
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

	}
}