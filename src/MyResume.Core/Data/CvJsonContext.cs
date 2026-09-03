using System.Text.Json.Serialization;
using MyResume.Core.Models;

namespace MyResume.Core.Data;

/// <summary>
/// Source-generated JSON contract for <c>cv.json</c>. Strict by design: a missing non-nullable member or an
/// unknown property is a <see cref="System.Text.Json.JsonException"/> at load time rather than a null at render time.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
[JsonSerializable(typeof(Cv))]
public sealed partial class CvJsonContext : JsonSerializerContext;
