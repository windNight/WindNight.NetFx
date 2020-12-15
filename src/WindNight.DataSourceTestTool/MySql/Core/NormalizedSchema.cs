using System;
using System.Text.RegularExpressions;

#nullable enable
namespace MySqlConnector.Core
{
	internal sealed class NormalizedSchema
	{
		private const string ReQuoted = @"`((?:[^`]|``)+)`";
		private const string ReUnQuoted = @"([^\.`]+)";
		private static readonly string ReEither = $@"(?:{ReQuoted}|{ReUnQuoted})";

		private static readonly Regex NameRe = new(
			$@"^\s*{ReEither}\s*(?:\.\s*{ReEither}\s*)?$",
			RegexOptions.Compiled);

		internal static NormalizedSchema MustNormalize(string name, string? defaultSchema = null)
		{
			var normalized = new NormalizedSchema(name, defaultSchema);
			if (normalized.Component is null)
				throw new ArgumentException("Could not determine function/procedure name", nameof(name));
			if (normalized.Schema is null)
				throw new ArgumentException("Could not determine schema", nameof(defaultSchema));
			return normalized;
		}

		public NormalizedSchema(string name, string? defaultSchema = null)
		{
			var match = NameRe.Match(name);
			if (match.Success)
			{
				if (match.Groups[3].Success)
					Component = match.Groups[3].Value.Trim();
				else if (match.Groups[4].Success)
					Component = match.Groups[4].Value.Trim();

				string firstGroup = "";
				if (match.Groups[1].Success)
					firstGroup = match.Groups[1].Value.Trim();
				else if (match.Groups[2].Success)
					firstGroup = match.Groups[2].Value.Trim();
				if (Component is null)
					Component = firstGroup.Trim();
				else
					Schema = firstGroup.Trim();

				Schema ??= defaultSchema;
			}
		}

		internal readonly string? Schema;
		internal readonly string? Component;

		internal string FullyQualified => $"`{Schema}`.`{Component}`";
	}
}
