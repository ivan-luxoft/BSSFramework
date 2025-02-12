﻿using System;

using Automation.ServiceEnvironment;

using SampleSystem.BLL;

namespace SampleSystem.IntegrationTests.__Support.TestData.Helpers;

public class AuthHelper : AuthHelperBase<ISampleSystemBLLContext>
{
    public AuthHelper(IServiceProvider rootServiceProvider) : base(rootServiceProvider)
    {
    }
}
