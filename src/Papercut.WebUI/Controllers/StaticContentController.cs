﻿

namespace Papercut.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Web.Http;


    public class StaticContentController: ApiController
    {

        public HttpResponseMessage Get()
        {
            var resourceName = GetRequetedResourceName(Request.RequestUri);
            var resourceContent = GetResourceStream(resourceName);
            if (resourceContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The requested file does not exist.");
            }

            var response = new HttpResponseMessage
            {
                Content = new StreamContent(resourceContent)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(GetMimeType(resourceName));
            return response;
        }

        static string GetRequetedResourceName(Uri requestUri)
        {
            var filename = requestUri.PathAndQuery
                        .TrimStart('/')
                        .TrimStart('.')
                        .Replace("%", "")
                        .Replace("$", "")
                        .Replace('/', Path.DirectorySeparatorChar)
                        .Replace(Path.DirectorySeparatorChar, '.');

            if (string.IsNullOrEmpty(filename))
            {
                filename = "index.html";
            }

            return filename;
        }

        static Stream GetResourceStream(string relativePath)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var resource = string.Format(ResourcePath, currentAssembly.GetName().Name, relativePath);

            return currentAssembly.GetManifestResourceStream(resource);
        }

        static string GetMimeType(string filename)
        {
            var extension = Path.GetExtension(filename)?.TrimStart('.');
            string mimeType;
            if (extension == null || !MimeMapping.TryGetValue(extension, out mimeType))
            {
                mimeType = "application/octet-stream";
            }
            return mimeType;
        }

        const string ResourcePath = "{0}.assets.{1}";
        static readonly Dictionary<string, string> MimeMapping = new Dictionary<string, string>()
        {
            { "htm", "text/html" },
            { "html", "text/html" },
            { "txt", "text/plain" },
            { "js", "text/javascript" },
            { "css", "text/css" },
            { "ico", "image/x-icon" },
            { "png", "image/png" },
            { "jpeg", "image/jpeg" },
            { "jpg", "image/jpeg" },
            { "gif", "image/gif" },
            { "svg", "image/svg+xml" },
            { "ttf", "application/x-font-ttf" },
            { "woff", "application/font-woff" },
            { "woff2", "application/font-woff2" },
        };

    }
}
