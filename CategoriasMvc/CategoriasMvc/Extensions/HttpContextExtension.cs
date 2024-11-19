namespace CategoriasMvc.Extensions
{
    public static class HttpContextExtension
    {
        public static string ObtemTokenJwt(this HttpContext httpContext)
        {
            if (httpContext.Request.Cookies.ContainsKey("X-Access-Token"))
                return httpContext.Request.Cookies["X-Access-Token"].ToString();

            return string.Empty;
        }
    }
}
