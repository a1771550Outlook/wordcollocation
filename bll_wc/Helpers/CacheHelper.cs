using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace BLL.Helpers
{
	public static class CacheHelper
	{
		/// <summary>
		/// Insert value into the cache using
		/// appropriate name/value pairs
		/// </summary>
		/// <typeparam name="T">Type of cached item</typeparam>
		/// <param name="o">Item to be cached</param>
		/// <param name="key">Name of item</param>
		/// <param name="Timeout"></param>
		/// <param name="priority">Default High</param>
		/// <param name="dependency">Default null</param>
		public static void Add<T>(T o, string key, double Timeout, CacheItemPriority priority= CacheItemPriority.High, CacheDependency dependency=null)
		{
			/*
			 * HttpContext.Current.Cache.Insert(GetPossCacheName, poss, null,
			//									 DateTime.Now.AddMinutes(ModelAppSettings.CacheExpiration_Minutes),
			//									 TimeSpan.Zero, CacheItemPriority.High, null);
			 */
			HttpContext.Current.Cache.Insert(
				key,
				o,
				dependency,
				DateTime.Now.AddMinutes(Timeout),
				Cache.NoSlidingExpiration,
				priority, null);
		}

		/// <summary>
		/// Remove item from cache
		/// </summary>
		/// <param name="key">Name of cached item</param>
		public static void Clear(string key)
		{
			HttpContext.Current.Cache.Remove(key);
		}

		/// <summary>
		/// Check for item in cache
		/// </summary>
		/// <param name="key">Name of cached item</param>
		/// <returns></returns>
		public static bool Exists(string key)
		{
			return HttpContext.Current.Cache[key] != null;
		}

		/// <summary>
		/// Retrieve cached item
		/// </summary>
		/// <typeparam name="T">Type of cached item</typeparam>
		/// <param name="key">Name of cached item</param>
		/// <param name="value">Cached value. Default(T) if item doesn't exist.</param>
		/// <returns>Cached item as type</returns>
		public static bool Get<T>(string key, out T value)
		{
			try
			{
				if (!Exists(key))
				{
					value = default(T);
					return false;
				}

				value = (T)HttpContext.Current.Cache[key];
			}
			catch
			{
				value = default(T);
				return false;
			}

			return true;
		}
	}
}
