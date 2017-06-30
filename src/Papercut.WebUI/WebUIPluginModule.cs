﻿// Papercut
// 
// Copyright © 2008 - 2012 Ken Robertson
// Copyright © 2013 - 2017 Jaben Cargman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License. 


namespace Papercut.WebUI
{
    using System;
    using System.Web.Http;
    using System.Web.Http.SelfHost;

    using Autofac;
    using Autofac.Core;

    using Core.Infrastructure.Lifecycle;
    using Core.Infrastructure.Plugins;

    using Common.Domain;

    public class WebUIPluginModule : Module, IPluginModule
    {
        public IModule Module => this;
        public string Name => "WebUI";
        public string Version => "1.0.0";
        public string Description => "Provides a web UI to manage the email messages for Papercut.";

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WebServer>().As<IEventHandler<PapercutServiceReadyEvent>>().SingleInstance();
            base.Load(builder);
        }
    }

    class WebServer : IEventHandler<PapercutServiceReadyEvent>
    {
        public void Handle(PapercutServiceReadyEvent @event)
        {
            var configuration = new HttpSelfHostConfiguration("http://localhost:6789");

            configuration.Routes.MapHttpRoute("health", "health", new {controller = "Health"});

            var server = new HttpSelfHostServer(configuration);

            Console.WriteLine("WebUI Server Start ...");

            server.OpenAsync().Wait();
        }
    }
}