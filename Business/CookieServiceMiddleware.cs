using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using VirusTracker.Services;

namespace VirusTracker.Business
{ //https://www.seeleycoder.com/blog/cookie-management-asp-net-core/
    public class CookieServiceMiddleware
    {

        private readonly RequestDelegate _next;

        public CookieServiceMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        //Injecting a scoped service into a middleware cannot be done at the constructor level. You’ll notice that I’m injecting it in the Invoke method and it probably seems a bit like magic. Somewhere deep in the bowels of DotNetCore it is smart enough to know how to inject there.
        public async Task Invoke(HttpContext context, ICookieService cookieService)
        {
            // write cookies to response right before it starts writing out from MVC/api responses...
            context.Response.OnStarting(() =>
               {
                   // cookie service should not write out cookies on 500, possibly others as well
                   if (!context.Response.StatusCode.IsInRange(500, 599))
                   {
                       cookieService.WriteToResponse(context);
                   }
                   return Task.CompletedTask;
               });

            await _next(context);
        }
    }
    //InvalidOperationException: The model item passed into the ViewDataDictionary is of type 'System.String', but this ViewDataDictionary instance requires a model item of type 'System.Collections.Generic.IEnumerable`1[VirusTracker.Models.Tracker]'.

    //IsInRange Another thing to note is that I detect when the response is starting and then check to see if the status code is not within a certain range. If it is outside that range then we go ahead and write our cookies out to the response via the service. The IsInRange extension method is one I’ve added so, without further ado, here is a basic IntExtensions.cs I added to the project:
    public static class IntExtensions
    {
        public static bool IsInRange(this int checkVal, int value1, int value2)
        {
            // First check to see if the passed in values are in order. If so, then check to see if checkVal is between them
            if (value1 <= value2)
                return checkVal >= value1 && checkVal <= value2;

            // Otherwise invert them and check the checkVal to see if it is between them
            return checkVal >= value2 && checkVal <= value1;
        }
    }
    //Alrighty.One last thing we need to add to our middleware code and hook up.Convention with middleware is to create a static class and extension method that will handle registering the middleware.Let’s add CookieServiceMiddlewareExtensions:

    public static class CookieServiceMiddlewareExtensions
    {
        public static IApplicationBuilder UseCookieService(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CookieServiceMiddleware>();
        }
    }

}
