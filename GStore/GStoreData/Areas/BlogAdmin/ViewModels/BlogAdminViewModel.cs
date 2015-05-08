using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData.Areas.BlogAdmin.ViewModels
{
	public class BlogAdminViewModel
	{
		public BlogAdminViewModel() { }

		public BlogAdminViewModel(StoreFrontConfiguration currentStoreFrontConfig, UserProfile userProfile)
		{
			if (currentStoreFrontConfig == null)
			{
				throw new ApplicationException("BlogAdminMenuViewModel: currentStoreFrontConfig is null, currentStoreFrontConfig must be specified.");
			}
			if (userProfile == null)
			{
				throw new ApplicationException("BlogAdminMenuViewModel: userProfile is null, UserProfile must be specified.");
			}
			this.StoreFrontConfig = currentStoreFrontConfig;
			this.StoreFront = currentStoreFrontConfig.StoreFront;
			this.UserProfile = userProfile;
			this.Client = currentStoreFrontConfig.Client;
		}

		public void UpdateClient(Client client)
		{
			this.Client = client;
		}

		public AdminMenuViewModel AdminMenuViewModel
		{
			get
			{
				return new AdminMenuViewModel(this.StoreFront, this.UserProfile, "BlogAdmin");
			}
		}

		[Display(Name = "Store Front Configuration")]
		public StoreFrontConfiguration StoreFrontConfig { get; protected set; }

		[Display(Name = "Store Front")]
		public StoreFront StoreFront { get; protected set; }

		[Display(Name = "Client")]
		public Client Client { get; protected set; }

		[Display(Name = "User Profile")]
		public UserProfile UserProfile { get; protected set; }

		[Display(Name = "Return to Front End")]
		public bool ReturnToFrontEnd { get; set; }

		public int? FilterBlogId { get; set; }

		public string SortBy { get; set; }

		public bool? SortAscending { get; set; }

		public List<Blog> Blogs
		{
			get
			{
				if (_blogs != null)
				{
					return _blogs;
				}
				if (this.StoreFront == null)
				{
					throw new ArgumentNullException("storeFront");
				}

				_blogs = this.StoreFront.Blogs.AsQueryable().ApplyDefaultSort().ToList();
				return _blogs;
			}
		}

		public void UpdateSortedBlogs(IOrderedQueryable<Blog> sortedBlogs)
		{
			_blogs = sortedBlogs.ToList();
		}
		protected List<Blog> _blogs = null;

		public List<BlogEntry> BlogEntries
		{
			get
			{
				if (_blogEntries != null)
				{
					return _blogEntries;
				}
				if (this.StoreFront == null)
				{
					throw new ArgumentNullException("storeFront");
				}

				if (!this.FilterBlogId.HasValue)
				{
					_blogEntries = new List<BlogEntry>();
				}
				if (this.FilterBlogId.HasValue)
				{
					Blog blog = this.StoreFront.Blogs.Where(b => b.BlogId == this.FilterBlogId.Value).SingleOrDefault();
					if (blog == null)
					{
						throw new ApplicationException("blog not found by id: " + this.FilterBlogId.Value);
					}
					_blogEntries = blog.BlogEntries.AsQueryable().ApplyDefaultSort().ToList();
				}
				return _blogEntries;
			}
		}

		public void UpdateSortedBlogEntries(IOrderedQueryable<Blog> sortedBlogEntries)
		{
			_blogEntries = _blogEntries.ToList();
		}
		protected List<BlogEntry> _blogEntries = null;

		public Blog FilterBlog
		{
			get
			{
				if (!this.FilterBlogId.HasValue)
				{
					return null;
				}
				if (_filterBlog != null)
				{
					return _filterBlog;
				}

				_filterBlog = this.Blogs.Find(b => b.BlogId == this.FilterBlogId.Value);
				return _filterBlog;
			}
		}
		Blog _filterBlog = null;



	}
}