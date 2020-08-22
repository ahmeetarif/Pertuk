﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pertuk.Business.Installers;
using System;
using System.Linq;
using System.Reflection;

namespace Pertuk.Business.Extensions.InstallerExt
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = Assembly.GetExecutingAssembly().ExportedTypes.Where(x => typeof(IBaseInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(Activator.CreateInstance).Cast<IBaseInstaller>().ToList();

            installers.ForEach(x => x.InstallServices(services, configuration));
        }
    }
}