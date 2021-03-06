using System;

namespace GaiaProject.ViewModels.Users
{
	public class UserViewModel
	{
		private string _id;
		public string Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
			}
		}

		private string _username;
		public string Username
		{
			get => _username;
			set
			{
				if (_username == value) return;
				_username = value;
			}
		}

		private string _avatar;
		public string Avatar
		{
			get => _avatar;
			set
			{
				if (_avatar == value) return;
				_avatar = value;
			}
		}

		private string _firstName;
		public string FirstName
		{
			get => _firstName;
			set
			{
				if (_firstName == value) return;
				_firstName = value;
			}
		}

		private string _lastName;
		public string LastName
		{
			get => _lastName;
			set
			{
				if (_lastName == value) return;
				_lastName = value;
			}
		}

		private DateTime _memberSince;
		public DateTime MemberSince
		{
			get => _memberSince;
			set
			{
				if (_memberSince == value) return;
				_memberSince = value;
			}
		}
	}
}