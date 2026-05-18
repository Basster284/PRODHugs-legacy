namespace PRODHugs_frontend.Util
{
    internal static class HttpMessageCreator
    {
        public static HttpRequestMessage CreateGet(string token, Uri uri)
        {
            HttpRequestMessage handler = new(HttpMethod.Get, uri);
            handler.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return handler;
        }

        public static HttpRequestMessage CreatePost(string token, Uri uri)
        {
            HttpRequestMessage handler = new(HttpMethod.Post, uri);
            handler.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return handler;
        }
    }
}
