// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Sassy Solutions">
//     Copyright (c) 2026 Sassy Solutions. Licensed under the MIT License.
//     See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------------

using Compendium.Abstractions.AI;
using Compendium.Abstractions.AI.Agents.Models;
using Compendium.Abstractions.AI.Models;
using Compendium.Adapters.Gemini.DependencyInjection;
using Compendium.Adapters.Gemini.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.Error.WriteLine("Set GEMINI_API_KEY first.");
    return 1;
}

var services = new ServiceCollection();
services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Information));
services.AddCompendiumGemini(opt =>
{
    opt.ApiKey = apiKey;
    opt.DefaultModel = "gemini-2.0-flash";
});

await using var sp = services.BuildServiceProvider();
var provider = sp.GetRequiredService<IAIProvider>();

var tools = new List<AgentTool>
{
    new(
        Name: "get_weather",
        Description: "Returns the current weather for the named city.",
        InputSchemaJson: """{"type":"object","properties":{"city":{"type":"string"}},"required":["city"]}""")
};

var request = new CompletionRequest
{
    Model = "gemini-2.0-flash",
    SystemPrompt = "You are a helpful weather assistant. Use the get_weather tool when asked about weather.",
    Messages = new List<Message> { Message.User("What's the weather in Paris right now?") },
    MaxTokens = 256
}.WithTools(tools, toolChoice: "auto");

var result = await provider.CompleteAsync(request);
if (result.IsFailure)
{
    Console.Error.WriteLine($"Error: {result.Error.Code} - {result.Error.Message}");
    return 1;
}

Console.WriteLine($"Finish reason: {result.Value.FinishReason}");
Console.WriteLine($"Assistant content: {result.Value.Content}");

var calls = result.Value.GetToolCalls();
foreach (var call in calls)
{
    Console.WriteLine($"Tool call -> {call.ToolName}({call.ArgumentsJson})");
}

return 0;
