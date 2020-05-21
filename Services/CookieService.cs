using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace VirusTracker.Services
{
    //https://www.seeleycoder.com/blog/cookie-management-asp-net-core/
    //https://github.com/fooberichu150/CookieService/tree/master/CookieManagerDemo

    public class CachedCookie
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public CookieOptions Options { get; set; }

        public bool IsDeleted { get; set; }
    }



    public class CookieService : ICookieService
    {
        private readonly HttpContext _httpContext;
        private Dictionary<string, CachedCookie> _pendingCookies = null;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {//Our constructor is injecting an IHttpContextAccessor which enables us to get access to the current HttpContext for the request.
            _httpContext = httpContextAccessor.HttpContext;
            _pendingCookies = new Dictionary<string, CachedCookie>(); //Another thing you’ll notice in our constructor is we’re setting up an empty dictionary for instances of our CachedCookie. This is where we’re going to track state of our cookies for the duration of a request before the middleware dumps them out to the response.
        }

        void ICookieService.Delete(string cookieName)
        {
            Delete(cookieName);
        }

        protected CachedCookie Delete(string cookieName)
        {
            //We want to make sure that subsequent queries for that same cookie know it is deleted as we’ve seen in the Get<T> call. For this to work properly we need our local cache to track that.
            if (_pendingCookies.TryGetValue(cookieName, out CachedCookie cookie))
                cookie.IsDeleted = true;
            else
            {
                cookie = new CachedCookie
                {
                    Name = cookieName,
                    IsDeleted = true
                };
                _pendingCookies.Add(cookieName, cookie);
            }

            return cookie;
        }

        public T Get<T>(string cookieName, bool isBase64 = false) where T : class
        {
            return ExceptionHandler.SwallowOnException(() =>
            {
                // check local cache first...
                if (_pendingCookies.TryGetValue(cookieName, out CachedCookie cookie))
                {
                    // don't retrieve a "deleted" cookie
                    if (cookie.IsDeleted)
                        return default(T);

                    return isBase64 ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cookie.Value.FromBase64String())
                        : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cookie.Value);
                }

                if (_httpContext.Request.Cookies.TryGetValue(cookieName, out string cookieValue))
                    return isBase64 ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cookieValue.FromBase64String())
                        : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cookieValue);

                return default(T);
            });
        }

        public T GetOrSet<T>(string cookieName, Func<T> setFunc, DateTimeOffset? expiry = null, bool isBase64 = false) where T : class
        {//Sometimes you want a cookie to exist no matter what but if it is already there you want to get it’s value. A use-case for this is if you want to load a cookie if present or set defaults otherwise. On one site I worked on we have a “trip planner” that fits this use-case. I want to know their details if they have them otherwise I’m going to set some defaults so the rest of the session experience is based on the same information. This is pretty simple to set up:
            T cookie = Get<T>(cookieName, isBase64);

            if (cookie != null)
                return cookie;

            T data = setFunc();
            Set(cookieName, data, expiry, isBase64);
            //If the cookie exists, we get it. If it doesn’t, we set it. Easy peasy.
            return data;
        }

        public void Set<T>(string cookieName, T data, DateTimeOffset? expiry = null, bool base64Encode = false) where T : class
        {
            // info about cookieoptions
            CookieOptions options = new CookieOptions()
            {
                Secure = _httpContext.Request.IsHttps
            };
            /*   if (expiry.HasValue)*/
            options.Expires = DateTime.MaxValue;    //expiry.Value;

            if (!_pendingCookies.TryGetValue(cookieName, out CachedCookie cookie))
                cookie = Add(cookieName);

            // always set options and value;
            cookie.Options = options;
            cookie.Value = base64Encode
                        ? Newtonsoft.Json.JsonConvert.SerializeObject(data).ToBase64String()
                        : Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        protected CachedCookie Add(string cookieName)
        {
            var cookie = new CachedCookie
            {
                Name = cookieName
            };
            _pendingCookies.Add(cookieName, cookie);

            return cookie;
        }

        //All the above code really doesn’t matter if we never write it back out to the response
        public void WriteToResponse(HttpContext context)
        {
            foreach (var cookie in _pendingCookies.Values)
            {
                if (cookie.IsDeleted)
                    context.Response.Cookies.Delete(cookie.Name);
                else
                    context.Response.Cookies.Append(cookie.Name, cookie.Value, cookie.Options);
            }
            //We iterate each of our pending cookies and will either Delete or Append them based on our cached value. Now we only ever have a single copy of each cookie being written out instead of our classic ASP.NET debacle we introduced at the beginning of this post.
        }
    }

    public static class ExceptionHandler
    {
        public static T SwallowOnException<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch
            {
                return default(T);
            }
        }
    }

    public static class StringExtensions
    {
        public static string FromBase64String(this string value, bool throwException = true)
        {
            try
            {
                byte[] decodedBytes = System.Convert.FromBase64String(value);
                string decoded = System.Text.Encoding.UTF8.GetString(decodedBytes);

                return decoded;
            }
            catch (Exception ex)
            {
                if (throwException)
                    throw new Exception(ex.Message, ex);
                else
                    return value;
            }
        }

        public static string ToBase64String(this string value)
        {
            byte[] bytes = System.Text.ASCIIEncoding.UTF8.GetBytes(value);
            string encoded = System.Convert.ToBase64String(bytes);

            return encoded;
        }
    }

}
