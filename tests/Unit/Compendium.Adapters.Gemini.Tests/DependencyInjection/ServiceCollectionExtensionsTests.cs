// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsTests.cs" company="Sassy Solutions">
//     Copyright (c) 2026 Sassy Solutions. Licensed under the MIT License.
//     See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------------

using Compendium.Adapters.Gemini.DependencyInjection;
using Compendium.Adapters.Gemini.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.Adapters.Gemini.Tests.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCompendiumGeminiAdapter_WithConfiguration_RegistersAdapterAndOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Compendium:Adapters:Gemini:BaseUrl"] = "https://api.example.com",
                ["Compendium:Adapters:Gemini:ApiKey"] = "k1",
            })
            .Build();

        // Act
        var actual = services.AddCompendiumGeminiAdapter(configuration);
        var sp = actual.BuildServiceProvider();

        // Assert
        actual.Should().BeSameAs(services);
        sp.GetRequiredService<GeminiAdapter>().Should().NotBeNull();
    }

    [Fact]
    public void AddCompendiumGeminiAdapter_WithCallback_RegistersAdapterAndOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddCompendiumGeminiAdapter(o =>
        {
            o.BaseUrl = "https://api.example.com";
            o.ApiKey = "k1";
        });
        var sp = services.BuildServiceProvider();

        // Assert
        sp.GetRequiredService<GeminiAdapter>().Should().NotBeNull();
    }

    [Fact]
    public void AddCompendiumGeminiAdapter_NullServices_Throws()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act
        var act = () => services!.AddCompendiumGeminiAdapter(_ => { });

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
