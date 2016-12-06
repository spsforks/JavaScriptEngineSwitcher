﻿using System;
using System.IO;
#if !NETSTANDARD1_3
using System.Linq;
#endif
using System.Reflection;
using System.Runtime.CompilerServices;
#if NETSTANDARD1_3
using System.Runtime.InteropServices;
#endif
using System.Text;

using JavaScriptEngineSwitcher.Core.Resources;

namespace JavaScriptEngineSwitcher.Core.Utilities
{
	public static class Utils
	{
#if !NETSTANDARD1_3
		/// <summary>
		/// List of Windows platform identifiers
		/// </summary>
		private static readonly PlatformID[] _windowsPlatformIDs =
		{
			PlatformID.Win32NT,
			PlatformID.Win32S,
			PlatformID.Win32Windows,
			PlatformID.WinCE
		};
#endif


		/// <summary>
		/// Determines whether the current operating system is Windows
		/// </summary>
		/// <returns>true if the operating system is Windows; otherwise, false</returns>
		public static bool IsWindows()
		{
#if NETSTANDARD1_3
			bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
			bool isWindows = _windowsPlatformIDs.Contains(Environment.OSVersion.Platform);
#endif

			return isWindows;
		}

		/// <summary>
		/// Determines whether the current process is a 64-bit process
		/// </summary>
		/// <returns>true if the process is 64-bit; otherwise, false</returns>
		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		public static bool Is64BitProcess()
		{
#if NETSTANDARD1_3
			bool is64Bit = IntPtr.Size == 8;
#else
			bool is64Bit = Environment.Is64BitProcess;
#endif

			return is64Bit;
		}

		/// <summary>
		/// Gets a content of the embedded resource as string
		/// </summary>
		/// <param name="resourceName">Resource name</param>
		/// <param name="type">Type from assembly that containing an embedded resource</param>
		/// <returns>Сontent of the embedded resource as string</returns>
		public static string GetResourceAsString(string resourceName, Type type)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException(
					"resourceName", string.Format(Strings.Common_ArgumentIsNull, "resourceName"));
			}

			if (type == null)
			{
				throw new ArgumentNullException(
					"type", string.Format(Strings.Common_ArgumentIsNull, "type"));
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			Assembly assembly = type.GetTypeInfo().Assembly;
			string nameSpace = type.Namespace;

			if (nameSpace != null && resourceName.StartsWith(nameSpace, StringComparison.Ordinal))
			{
				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				{
					return ReadResourceAsString(resourceName, stream);
				}
			}

			using (Stream stream = assembly.GetManifestResourceStream(type, resourceName))
			{
				return ReadResourceAsString(resourceName, stream);
			}
		}

		/// <summary>
		/// Gets a content of the embedded resource as string
		/// </summary>
		/// <param name="resourceName">Resource name</param>
		/// <param name="assembly">Assembly that containing an embedded resource</param>
		/// <returns>Сontent of the embedded resource as string</returns>
		public static string GetResourceAsString(string resourceName, Assembly assembly)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException(
					"resourceName", string.Format(Strings.Common_ArgumentIsNull, "resourceName"));
			}

			if (assembly == null)
			{
				throw new ArgumentNullException(
					"assembly", string.Format(Strings.Common_ArgumentIsNull, "assembly"));
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(Strings.Common_ArgumentIsEmpty, "resourceName"), "resourceName");
			}

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				return ReadResourceAsString(resourceName, stream);
			}
		}

		private static string ReadResourceAsString(string resourceName, Stream stream)
		{
			if (stream == null)
			{
				throw new NullReferenceException(
					string.Format(Strings.Resources_ResourceIsNull, resourceName));
			}

			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Gets a text content of the specified file
		/// </summary>
		/// <param name="path">File path</param>
		/// <param name="encoding">Content encoding</param>
		/// <returns>Text content</returns>
		public static string GetFileTextContent(string path, Encoding encoding = null)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(
					string.Format(Strings.Common_FileNotExist, path), path);
			}

			string content;

			using (var stream = File.OpenRead(path))
			using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
			{
				content = reader.ReadToEnd();
			}

			return content;
		}

		/// <summary>
		/// Converts a value of source enumeration type to value of destination enumeration type
		/// </summary>
		/// <typeparam name="TSource">Source enumeration type</typeparam>
		/// <typeparam name="TDest">Destination enumeration type</typeparam>
		/// <param name="value">Value of source enumeration type</param>
		/// <returns>Value of destination enumeration type</returns>
		public static TDest GetEnumFromOtherEnum<TSource, TDest>(TSource value)
		{
			string name = value.ToString();
			var destEnumValues = (TDest[])Enum.GetValues(typeof(TDest));

			foreach (var destEnum in destEnumValues)
			{
				if (string.Equals(destEnum.ToString(), name, StringComparison.OrdinalIgnoreCase))
				{
					return destEnum;
				}
			}

			throw new InvalidCastException(
				string.Format(Strings.Common_EnumValueConversionFailed,
					name, typeof(TSource), typeof(TDest))
			);
		}
	}
}