using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Recore;

namespace FileSync.Service
{
    static class Endpoints
    {
        public static RelativeUri Listing { get; } = GetEndpoint(typeof(DirectoryV1Controller));

        public static RelativeUri Content { get; } = GetEndpoint(typeof(FileContentV1Controller));

        private static RelativeUri GetEndpoint(Type controller)
        {
            var contentRoute = controller
                .GetTypeInfo()
                .GetCustomAttribute<RouteAttribute>()
                .Template;

            return new RelativeUri(contentRoute);
        }
    }
}
