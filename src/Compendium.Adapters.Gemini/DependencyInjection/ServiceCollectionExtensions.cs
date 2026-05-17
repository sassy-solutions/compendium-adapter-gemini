// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Sassy Solutions">
//     Copyright (c) 2026 Sassy Solutions. Licensed under the MIT License.
//     See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------------

using Compendium.Adapters.Gemini.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.Adapters.Gemini.DependencyInjection;

/// <summary>
/// DI registration helpers for the Gemini adapter.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="GeminiAdapter"/> and its options.
    /// </summary>
    /// <param name="services">DI container.</param>
    /// <param name="configuration">Source configuration; section <see cref="GeminiOptions.SectionName"/> is bound.</param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddCompendiumGeminiAdapter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<GeminiOptions>()
            .Bind(configuration.GetSection(GeminiOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<GeminiAdapter>();

        return services;
    }

    /// <summary>
    /// Registers <see cref="GeminiAdapter"/> with an inline configuration callback.
    /// </summary>
    /// <param name="services">DI container.</param>
    /// <param name="configure">Callback to mutate <see cref="GeminiOptions"/>.</param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddCompendiumGeminiAdapter(
        this IServiceCollection services,
        Action<GeminiOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddOptions<GeminiOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<GeminiAdapter>();

        return services;
    }
}
