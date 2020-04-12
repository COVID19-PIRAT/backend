﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "This is an application context which should relie on default behavior https://devblogs.microsoft.com/dotnet/configureawait-faq/")]
[assembly: SuppressMessage("Security", "SEC0121:CORS Allow Origin Wildcard", Justification = "So far we do not have any specific policies", Scope = "member", Target = "~M:Pirat.Startup.ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection)")]
