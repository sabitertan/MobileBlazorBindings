﻿using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MobileBlazorBindings.HostingNew
{
    /// <summary>
    /// Platform-agnostic parts of BlazorWebView
    /// </summary>
    public abstract class BlazorWebViewCore : IDisposable
    {
        private static readonly FileExtensionContentTypeProvider FileExtensionContentTypeProvider = new();

        private readonly string _contentHost;
        private readonly string _contentRootPath;
        private readonly string _hostPageRelativeUrl;

        public BlazorWebViewCore(string hostPageFilePath)
        {
            var hostPageAbsolute = Path.GetFullPath(hostPageFilePath);
            _contentHost = "0.0.0.0";
            _contentRootPath = Path.GetDirectoryName(hostPageAbsolute);
            _hostPageRelativeUrl = Path.GetRelativePath(_contentRootPath, hostPageAbsolute).Replace(Path.DirectorySeparatorChar, '/');
        }

        public void AddRootComponent(Type type, string selector)
        {
            // TODO
        }

        protected abstract void Navigate(Uri uri);

        public virtual Task StartAsync()
        {
            var startUri = new Uri(new Uri($"https://{_contentHost}/"), _hostPageRelativeUrl);
            Navigate(startUri);
            return Task.CompletedTask;
        }

        protected bool TryGetResponseContent(Uri requestUri, out int statusCode, out string statusMessage, out Stream content, out string headers)
        {
            if (requestUri is null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (string.Equals(requestUri.Host, _contentHost, StringComparison.Ordinal))
            {
                var filePath = Path.GetFullPath(Path.Combine(_contentRootPath, requestUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)));
                if (filePath.StartsWith(_contentRootPath, StringComparison.Ordinal)
                    && File.Exists(filePath))
                {
                    var responseContentType = FileExtensionContentTypeProvider.TryGetContentType(filePath, out var matchedContentType)
                        ? matchedContentType
                        : "application/octet-stream";

                    statusCode = 200;
                    statusMessage = "OK";
                    headers = $"Content-Type: {responseContentType}{Environment.NewLine}Cache-Control: no-cache, max-age=0, must-revalidate, no-store";
                    content = File.OpenRead(filePath);
                }
                else
                {
                    // Always provide a response to requests on the virtual domain, even if no file matches
                    var message = $"There is no file at {filePath}";
                    statusCode = 404;
                    statusMessage = "Not found";
                    headers = "Content-Type: text/plain";
#pragma warning disable CA2000 // Dispose objects before losing scope
                    content = new MemoryStream(Encoding.UTF8.GetBytes(message));
#pragma warning restore CA2000 // Dispose objects before losing scope
                }

                return true;
            }

            statusCode = default;
            statusMessage = default;
            headers = default;
            content = default;
            return false;
        }

        public void Dispose()
        {
            // Nothing to do yet
        }
    }
}