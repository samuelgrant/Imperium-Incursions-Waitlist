﻿using Microsoft.Extensions.Logging;

namespace Imperium_Incursions_Waitlist.Services
{
    /// <summary>
    /// Shared Logger
    /// </summary>
    internal static class ApplicationLogging
    {
        internal static ILoggerFactory LoggerFactory { get; set; }
        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        internal static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
    }
}