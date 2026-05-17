# Changelog

All notable changes to `Compendium.Adapters.Gemini` are documented here.
The project follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Initial implementation of `Compendium.Adapters.Gemini`, a direct adapter against the Google Gemini REST API (`generativelanguage.googleapis.com`) for [`Compendium.Abstractions.AI`](https://www.nuget.org/packages/Compendium.Abstractions.AI) 1.0.1.
- `GeminiAIProvider` implementing `IAIProvider` with:
  - Chat completions (`POST /v1beta/models/{model}:generateContent`).
  - Streaming completions via SSE (`streamGenerateContent?alt=sse`).
  - Embeddings — single input through `embedContent`, multiple inputs auto-batched through `batchEmbedContents` (default cap 100 inputs per request) with index re-mapping.
  - Tool / function calling round-trip — request via `WithTools(...)`; response surfaced as `AgentToolInvocation` list on `CompletionResponse.Metadata` and read with `GetToolCalls()`. `toolChoice` maps to Gemini's `functionCallingConfig.mode` (`AUTO` / `ANY` / `NONE`) with single-tool selection via `allowedFunctionNames`.
  - Structured outputs (`responseMimeType: "application/json"` + `responseSchema`) — opt-in per request via `WithStructuredOutput(...)` / `WithJsonMode()` or per-options via `GeminiOptions.UseStructuredOutputsByDefault`.
  - System instructions sent as the top-level `systemInstruction` field, not as a content part.
  - Vision inputs (image / file URIs) via pre-built `GeminiPart` entries attached under `AdditionalParameters["gemini.vision_parts"]`.
  - Tool-result feedback via `Message { Role = Tool, Name = "<fn>", Content = "<json|text>" }`, translated into a `functionResponse` content.
- `GeminiHttpClient` typed `HttpClient` with `?key=` query-param auth, JSON serialization (camelCase wire format, null-omit), and SSE stream reader.
- `Microsoft.Extensions.Http.Resilience` standard pipeline wired via `AddCompendiumGemini`.
- Error mapping for HTTP 401 / 403 / 402 / 404 / 429 and 5xx into `AIErrors.*` codes; cancellation re-thrown to caller, other `TaskCanceledException`s mapped to `AIErrors.Timeout`. Gemini's `{error: {code, message, status}}` payload is parsed when present.
- `GeminiOptions` with sensible defaults (`gemini-2.0-flash` chat, `text-embedding-004` embeddings, 120s timeout, 100 batch).
- Sample [`samples/01-chat-with-tool`](samples/01-chat-with-tool) demonstrating a one-tool agent.
- Optional integration tests at [`tests/Integration/Compendium.Adapters.Gemini.IntegrationTests`](tests/Integration), gated on the `GEMINI_API_KEY` env var, excluded from the default unit-test job via `--filter "FullyQualifiedName!~IntegrationTests"`.

### Notes

- Unit test suite: 120 tests, 98.8 % line coverage / 93.5 % branch coverage on the unit-testable surface.
- No NuGet release tagged yet — orchestrator will tag `v1.0.0-preview.0` after the initial-implementation PR merges.
