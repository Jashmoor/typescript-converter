using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TypescriptClassConverter.Converter;

namespace TypescriptClassConverter.Extensions
{
    public static class StartupExtensions
    {
        public static void Convert(this IApplicationBuilder builder, IHostEnvironment env, string path = null)
            => builder.Convert<ControllerBase>(env, path);

        public static void Convert<T>(this IApplicationBuilder builder, IHostEnvironment env, string path = null)
        {
            if (env.IsDevelopment())
            {
                Assembly target = Assembly.GetCallingAssembly();
                string localPath = Path.Combine(env.ContentRootPath, path ?? "./GeneratedClientModels/");
                ConvertClient.GenerateModels<T>(localPath,  target);
            }
        }
    }
}
