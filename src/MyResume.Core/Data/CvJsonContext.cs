using System.Text.Json.Serialization;
using MyResume.Core.Models;

namespace MyResume.Core.Data;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true,
    WriteIndented = true)]
[JsonSerializable(typeof(Cv))]
[JsonSerializable(typeof(Experience))]
public sealed partial class CvJsonContext : JsonSerializerContext;
