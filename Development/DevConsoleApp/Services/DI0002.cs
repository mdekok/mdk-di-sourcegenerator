﻿using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

// For testing diagnostic: DI002, Class does not implement service type
[AddScoped<IDummyInterface>]
// internal class RegisterInterfaceNotImplemented { } // Uncomment to test diagnostic
internal class RegisterInterfaceNotImplemented : IDummyInterface { }