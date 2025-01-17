﻿using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Towel.DataStructures;
using System.Linq;

namespace Towel
{
	/// <summary>Constains static analysis methods of the code (reflection).</summary>
	public static class Meta
	{
		#region Has Implicit/Explicit Cast

		/// <summary>Determines if an implicit casting operator exists from one type to another.</summary>
		/// <typeparam name="From">The parameter type of the implicit casting operator.</typeparam>
		/// <typeparam name="To">The return type fo the implicit casting operator.</typeparam>
		/// <returns>True if the implicit casting operator exists or false if not.</returns>
		public static bool HasImplicitCast<From, To>() => HasCastCache<From, To>.Implicit;
		/// <summary>Determines if an implicit casting operator exists from one type to another.</summary>
		/// <typeparam name="From">The parameter type of the implicit casting operator.</typeparam>
		/// <typeparam name="To">The return type fo the implicit casting operator.</typeparam>
		/// <returns>True if the implicit casting operator exists or false if not.</returns>
		public static bool HasExplicitCast<From, To>() => HasCastCache<From, To>.Implicit;
		/// <summary>Determines if an implicit casting operator exists from one type to another.</summary>
		/// <param name="fromType">The parameter type of the implicit casting operator.</param>
		/// <param name="toType">The return type fo the implicit casting operator.</param>
		/// <returns>True if the implicit casting operator exists or false if not.</returns>
		public static bool HasImplicitCast(Type fromType, Type toType) => HasCast(fromType, toType, true);
		/// <summary>Determines if an implicit casting operator exists from one type to another.</summary>
		/// <param name="fromType">The parameter type of the implicit casting operator.</param>
		/// <param name="toType">The return type fo the implicit casting operator.</param>
		/// <returns>True if the implicit casting operator exists or false if not.</returns>
		public static bool HasExplicitCast(Type fromType, Type toType) => HasCast(fromType, toType, false);

		internal static bool HasCast(Type fromType, Type toType, bool @implicit)
		{
			if (fromType is null)
			{
				throw new ArgumentNullException(nameof(fromType));
			}
			if (toType is null)
			{
				throw new ArgumentNullException(nameof(toType));
			}
			string methodName = @implicit
				? "op_Implicit"
				: "op_Explicit";
			Type[] parameterTypes = new Type[] { fromType, };
			bool CheckType(Type type)
			{
				MethodInfo methodInfo = type.GetMethod(
					methodName,
					BindingFlags.Static |
					BindingFlags.Public |
					BindingFlags.NonPublic,
					null,
					parameterTypes,
					null);
				return !(methodInfo is null)
					&& methodInfo.ReturnType == toType
					&& methodInfo.IsSpecialName;
			}
			if (CheckType(fromType) || CheckType(toType))
			{
				return true;
			}
			return false;
		}

		internal static class HasCastCache<From, To>
		{
			internal static readonly bool Implicit = HasCast(typeof(From), typeof(To), true);
			internal static readonly bool Explicit = HasCast(typeof(From), typeof(To), false);
		}

		#endregion

		#region ConvertToCsharpSourceDefinition

		/// <summary>Converts a <see cref="System.Type"/> into a string as it would appear in C# source code.</summary>
		/// <param name="type">The <see cref="System.Type"/> to convert to a string.</param>
		/// <returns>The string as the <see cref="System.Type"/> would appear in C# source code.</returns>
		public static string ConvertToCsharpSource(this Type type, bool showGenericParameters = false)
		{
			IQueue<Type> genericParameters = new QueueArray<Type>();
			type.GetGenericArguments().Stepper(x => genericParameters.Enqueue(x));
			return ConvertToCsharpSource(type, genericParameters, showGenericParameters);
		}

		internal static string ConvertToCsharpSource(Type type, IQueue<Type> genericParameters, bool showGenericParameters)
		{
			if (type is null)
			{
				throw new ArgumentNullException(nameof(type));
			}
			string result = type.IsNested
				? ConvertToCsharpSource(type.DeclaringType, genericParameters, showGenericParameters) + "."
				: type.Namespace + ".";
			result += Regex.Replace(type.Name, "`.*", string.Empty);
			if (type.IsGenericType)
			{
				result += "<";
				bool firstIteration = true;
				foreach (Type generic in type.GetGenericArguments())
				{
					if (genericParameters.Count <= 0)
					{
						break;
					}
					Type correctGeneric = genericParameters.Dequeue();
					result += (firstIteration ? string.Empty : ",") +
						(correctGeneric.IsGenericParameter
						? (showGenericParameters ? (firstIteration ? string.Empty : " ") + correctGeneric.Name : string.Empty)
						: (firstIteration ? string.Empty : " ") + ConvertToCsharpSource(correctGeneric));
					firstIteration = false;
				}
				result += ">";
			}
			return result;
		}

		#endregion

		#region Enum

		/// <summary>Gets a custom attribute on an enum value by generic type.</summary>
		/// <typeparam name="AttributeType">The type of attribute to get.</typeparam>
		/// <param name="enum">The enum value to get the attribute of.</param>
		/// <returns>The attribute on the enum value of the provided type.</returns>
		public static AttributeType GetEnumAttribute<AttributeType>(this Enum @enum)
			where AttributeType : Attribute
		{
			Type type = @enum.GetType();
			MemberInfo memberInfo = type.GetMember(@enum.ToString())[0];
			return memberInfo.GetCustomAttribute<AttributeType>();
		}

		/// <summary>Gets custom attributes on an enum value by generic type.</summary>
		/// <typeparam name="AttributeType">The type of attribute to get.</typeparam>
		/// <param name="enum">The enum value to get the attribute of.</param>
		/// <returns>The attributes on the enum value of the provided type.</returns>
		public static System.Collections.Generic.IEnumerable<AttributeType> GetEnumAttributes<AttributeType>(this Enum @enum)
			where AttributeType : Attribute
		{
			Type type = @enum.GetType();
			MemberInfo memberInfo = type.GetMember(@enum.ToString())[0];
			return memberInfo.GetCustomAttributes<AttributeType>();
		}

		/// <summary>Gets the maximum value of an enum.</summary>
		/// <typeparam name="ENUM">The enum type to get the maximum value of.</typeparam>
		/// <returns>The maximum enum value of the provided type.</returns>
		public static ENUM GetLastEnumValue<ENUM>()
		{
			ENUM[] values = (ENUM[])Enum.GetValues(typeof(ENUM));
			if (values.Length == 0)
			{
				throw new InvalidOperationException("Attempting to get the last enum value of an enum type with no values.");
			}
			return values[values.Length - 1];
		}

		#endregion

		#region Assembly

		/// <summary>Enumerates through all the events with a custom attribute.</summary>
		/// <typeparam name="AttributeType">The type of the custom attribute.</typeparam>
		/// <param name="assembly">The assembly to iterate through the events of.</param>
		/// <returns>The IEnumerable of the events with the provided attribute type.</returns>
		public static System.Collections.Generic.IEnumerable<EventInfo> GetEventInfosWithAttribute<AttributeType>(this Assembly assembly)
			where AttributeType : Attribute
		{
			foreach (Type type in assembly.GetTypes())
			{
				foreach (EventInfo eventInfo in type.GetEvents())
				{
					if (eventInfo.GetCustomAttributes(typeof(AttributeType), true).Length > 0)
					{
						yield return eventInfo;
					}
				}
			}
		}

		/// <summary>Enumerates through all the constructors with a custom attribute.</summary>
		/// <typeparam name="AttributeType">The type of the custom attribute.</typeparam>
		/// <param name="assembly">The assembly to iterate through the constructors of.</param>
		/// <returns>The IEnumerable of the constructors with the provided attribute type.</returns>
		public static System.Collections.Generic.IEnumerable<ConstructorInfo> GetConstructorInfosWithAttribute<AttributeType>(this Assembly assembly)
			where AttributeType : Attribute
		{
			foreach (Type type in assembly.GetTypes())
			{
				foreach (ConstructorInfo constructorInfo in type.GetConstructors())
				{
					if (constructorInfo.GetCustomAttributes(typeof(AttributeType), true).Length > 0)
					{
						yield return constructorInfo;
					}
				}
			}
		}

		/// <summary>Enumerates through all the properties with a custom attribute.</summary>
		/// <typeparam name="AttributeType">The type of the custom attribute.</typeparam>
		/// <param name="assembly">The assembly to iterate through the properties of.</param>
		/// <returns>The IEnumerable of the properties with the provided attribute type.</returns>
		public static System.Collections.Generic.IEnumerable<PropertyInfo> GetPropertyInfosWithAttribute<AttributeType>(this Assembly assembly)
			where AttributeType : Attribute
		{
			foreach (Type type in assembly.GetTypes())
			{
				foreach (PropertyInfo propertyInfo in type.GetProperties())
				{
					if (propertyInfo.GetCustomAttributes(typeof(AttributeType), true).Length > 0)
					{
						yield return propertyInfo;
					}
				}
			}
		}

		/// <summary>Enumerates through all the fields with a custom attribute.</summary>
		/// <typeparam name="AttributeType">The type of the custom attribute.</typeparam>
		/// <param name="assembly">The assembly to iterate through the fields of.</param>
		/// <returns>The IEnumerable of the fields with the provided attribute type.</returns>
		public static System.Collections.Generic.IEnumerable<FieldInfo> GetFieldInfosWithAttribute<AttributeType>(this Assembly assembly)
			where AttributeType : Attribute
		{
			foreach (Type type in assembly.GetTypes())
			{
				foreach (FieldInfo fieldInfo in type.GetFields())
				{
					if (fieldInfo.GetCustomAttributes(typeof(AttributeType), true).Length > 0)
					{
						yield return fieldInfo;
					}
				}
			}
		}

		/// <summary>Enumerates through all the methods with a custom attribute.</summary>
		/// <typeparam name="AttributeType">The type of the custom attribute.</typeparam>
		/// <param name="assembly">The assembly to iterate through the methods of.</param>
		/// <returns>The IEnumerable of the methods with the provided attribute type.</returns>
		public static System.Collections.Generic.IEnumerable<MethodInfo> GetMethodInfosWithAttribute<AttributeType>(this Assembly assembly)
			where AttributeType : Attribute
		{
			foreach (Type type in assembly.GetTypes())
			{
				foreach (MethodInfo methodInfo in type.GetMethods())
				{
					if (methodInfo.GetCustomAttributes(typeof(AttributeType), true).Length > 0)
					{
						yield return methodInfo;
					}
				}
			}
		}

		/// <summary>Enumerates through all the types with a custom attribute.</summary>
		/// <typeparam name="AttributeType">The type of the custom attribute.</typeparam>
		/// <param name="assembly">The assembly to iterate through the types of.</param>
		/// <returns>The IEnumerable of the types with the provided attribute type.</returns>
		public static System.Collections.Generic.IEnumerable<Type> GetTypesWithAttribute<AttributeType>(this Assembly assembly)
			where AttributeType : Attribute
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(AttributeType), true).Length > 0)
				{
					yield return type;
				}
			}
		}

		/// <summary>Gets all the types in an assembly that derive from a base.</summary>
		/// <typeparam name="Base">The base type to get the deriving types of.</typeparam>
		/// <param name="assembly">The assmebly to perform the search on.</param>
		/// <returns>The IEnumerable of the types that derive from the provided base.</returns>
		public static System.Collections.Generic.IEnumerable<Type> GetDerivedTypes<Base>(this Assembly assembly)
		{
			Type @base = typeof(Base);
			return assembly.GetTypes().Where(type =>
				type != @base &&
				@base.IsAssignableFrom(type));
		}

		/// <summary>Gets the file path of an assembly.</summary>
		/// <param name="assembly">The assembly to get the file path of.</param>
		/// <returns>The file path of the assembly.</returns>
		public static string GetDirectoryPath(this Assembly assembly)
		{
			string codeBase = assembly.CodeBase;
			UriBuilder uri = new UriBuilder(codeBase);
			string path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}

		#endregion

		#region XML Code Documentation

		internal static System.Collections.Generic.HashSet<Assembly> loadedAssemblies = new System.Collections.Generic.HashSet<Assembly>();
		internal static System.Collections.Generic.Dictionary<string, string> loadedXmlDocumentation = new System.Collections.Generic.Dictionary<string, string>();

		internal static void LoadXmlDocumentation(Assembly assembly)
		{
			if (loadedAssemblies.Contains(assembly))
			{
				return;
			}
			string directoryPath = assembly.GetDirectoryPath();
			string xmlFilePath = Path.Combine(directoryPath, assembly.GetName().Name + ".xml");
			if (File.Exists(xmlFilePath))
			{
				using (StreamReader streamReader = new StreamReader(xmlFilePath))
				{
					LoadXmlDocumentation(streamReader);
				}
			}
			// currently marking assembly as loaded even if the XML file was not found
			// may want to adjust in future, but I think this is good for now
			loadedAssemblies.Add(assembly);
		}

		/// <summary>Loads the XML code documentation into memory so it can be accessed by extension methods on reflection types.</summary>
		/// <param name="xmlDocumentation">The content of the XML code documentation.</param>
		public static void LoadXmlDocumentation(string xmlDocumentation)
		{
			using (StringReader stringReader = new StringReader(xmlDocumentation))
			{
				LoadXmlDocumentation(stringReader);
			}
		}

		/// <summary>Loads the XML code documentation into memory so it can be accessed by extension methods on reflection types.</summary>
		/// <param name="textReader">The text reader to process in an XmlReader.</param>
		public static void LoadXmlDocumentation(TextReader textReader)
		{
			using (XmlReader xmlReader = XmlReader.Create(textReader))
			{
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "member")
					{
						string raw_name = xmlReader["name"];
						loadedXmlDocumentation[raw_name] = xmlReader.ReadInnerXml();
					}
				}
			}
		}

		/// <summary>Clears the currently loaded XML documentation.</summary>
		public static void ClearXmlDocumentation()
		{
			loadedAssemblies.Clear();
			loadedXmlDocumentation.Clear();
		}

		/// <summary>Gets the XML documentation on a type.</summary>
		/// <param name="type">The type to get the XML documentation of.</param>
		/// <returns>The XML documentation on the type.</returns>
		/// <remarks>The XML documentation must be loaded into memory for this function to work.</remarks>
		public static string GetDocumentation(this Type type)
		{
			LoadXmlDocumentation(type.Assembly);
			string key = "T:" + XmlDocumentationKeyHelper(type.FullName, null);
			loadedXmlDocumentation.TryGetValue(key, out string documentation);
			return documentation;
		}

		/// <summary>Gets the XML documentation on a method.</summary>
		/// <param name="methodInfo">The method to get the XML documentation of.</param>
		/// <returns>The XML documentation on the method.</returns>
		/// <remarks>The XML documentation must be loaded into memory for this function to work.</remarks>
		public static string GetDocumentation(this MethodInfo methodInfo)
		{
			LoadXmlDocumentation(methodInfo.DeclaringType.Assembly);

			// build the generic index mappings
			System.Collections.Generic.Dictionary<string, int> typeGenericMap = new System.Collections.Generic.Dictionary<string, int>();
			int tempTypeGeneric = 0;
			Array.ForEach(methodInfo.DeclaringType.GetGenericArguments(), x => typeGenericMap[x.Name] = tempTypeGeneric++);

			System.Collections.Generic.Dictionary<string, int> methodGenericMap = new System.Collections.Generic.Dictionary<string, int>();
			int tempMethodGeneric = 0;
			Array.ForEach(methodInfo.GetGenericArguments(), x => methodGenericMap.Add(x.Name, tempMethodGeneric++));

			ParameterInfo[] parameterInfos = methodInfo.GetParameters();

			string memberTypePrefix = "M:";
			string declarationTypeString = GetXmlDocumenationFormattedString(methodInfo.DeclaringType, false, typeGenericMap, methodGenericMap);
			string memberNameString = methodInfo.Name;
			string methodGenericArgumentsString =
				methodGenericMap.Count > 0 ?
				"``" + methodGenericMap.Count :
				string.Empty;
			string parametersString =
				parameterInfos.Length > 0 ?
				"(" + string.Join(",", methodInfo.GetParameters().Select(x => GetXmlDocumenationFormattedString(x.ParameterType, true, typeGenericMap, methodGenericMap))) + ")" :
				string.Empty;

			string key =
				memberTypePrefix +
				declarationTypeString +
				"." +
				memberNameString +
				methodGenericArgumentsString +
				parametersString;

			// handle casting operators witch include the return type in the XML key
			if (methodInfo.Name.Equals("op_Implicit") ||
				methodInfo.Name.Equals("op_Explicit"))
			{
				key += "~" + GetXmlDocumenationFormattedString(methodInfo.ReturnType, true, typeGenericMap, methodGenericMap);
			}

			loadedXmlDocumentation.TryGetValue(key, out string documentation);
			return documentation;
		}

		/// <summary>Gets the XML documentation on a constructor.</summary>
		/// <param name="constructorInfo">The constructor to get the XML documentation of.</param>
		/// <returns>The XML documentation on the constructor.</returns>
		/// <remarks>The XML documentation must be loaded into memory for this function to work.</remarks>
		public static string GetDocumentation(this ConstructorInfo constructorInfo)
		{
			LoadXmlDocumentation(constructorInfo.DeclaringType.Assembly);

			// build the generic index mappings
			System.Collections.Generic.Dictionary<string, int> typeGenericMap = new System.Collections.Generic.Dictionary<string, int>();
			int tempTypeGeneric = 0;
			Array.ForEach(constructorInfo.DeclaringType.GetGenericArguments(), x => typeGenericMap[x.Name] = tempTypeGeneric++);

			// constructors don't support generic types so this will always be empty
			System.Collections.Generic.Dictionary<string, int> methodGenericMap = new System.Collections.Generic.Dictionary<string, int>();

			ParameterInfo[] parameterInfos = constructorInfo.GetParameters();

			string memberTypePrefix = "M:";
			string declarationTypeString = GetXmlDocumenationFormattedString(constructorInfo.DeclaringType, false, typeGenericMap, methodGenericMap);
			string memberNameString = "#ctor";
			string parametersString =
				parameterInfos.Length > 0 ?
				"(" + string.Join(",", constructorInfo.GetParameters().Select(x => GetXmlDocumenationFormattedString(x.ParameterType, true, typeGenericMap, methodGenericMap))) + ")" :
				string.Empty;

			string key =
				memberTypePrefix +
				declarationTypeString +
				"." +
				memberNameString +
				parametersString;

			loadedXmlDocumentation.TryGetValue(key, out string documentation);
			return documentation;
		}

		// convert types to strings as they appear as keys in the XML documentation files
		internal static string GetXmlDocumenationFormattedString(
			Type type, bool isMethodParameter,
			System.Collections.Generic.Dictionary<string, int> typeGenericMap,
			System.Collections.Generic.Dictionary<string, int> methodGenericMap)
		{
			string result;

			if (type.IsGenericParameter)
			{
				if (methodGenericMap.TryGetValue(type.Name, out int methodIndex))
				{
					result = "``" + methodIndex;
				}
				else
				{
					result = "`" + typeGenericMap[type.Name];
				}
				goto FormatComplete;
			}

			// ref types
			string refTypeString = string.Empty;
			if (type.IsByRef)
			{
				refTypeString = "@";
			}

			// element type
			Type elementType = null;
			string elementTypeString = string.Empty;
			if (type.HasElementType)
			{
				elementType = type.GetElementType();
				elementTypeString = GetXmlDocumenationFormattedString(
					elementType,
					isMethodParameter,
					typeGenericMap,
					methodGenericMap);
			}

			// pointer types
			if (type.IsPointer)
			{
				result = elementTypeString + "*" + refTypeString;
				goto FormatComplete;
			}

			// array types
			if (type.IsArray)
			{
				int rank = type.GetArrayRank();
				string arrayDimensionsString =
					rank > 1 ?
					"[" + string.Join(",", Enumerable.Repeat("0:", rank)) + "]" :
					"[]";
				result = elementTypeString + arrayDimensionsString + refTypeString;
				goto FormatComplete;
			}

			// generic types
			string genericArgumentsString = string.Empty;
			if (type.IsGenericType && isMethodParameter)
			{
				System.Collections.Generic.IEnumerable<string> formattedStrings = type.GetGenericArguments().Select(x =>
					methodGenericMap.TryGetValue(x.Name, out int methodIndex) ?
					"``" + methodIndex :
					typeGenericMap.TryGetValue(x.Name, out int typeIndex) ?
					"`" + typeIndex :
					GetXmlDocumenationFormattedString(x, isMethodParameter, typeGenericMap, methodGenericMap));
				genericArgumentsString = "{" + string.Join(",", formattedStrings) + "}";
			}

			// preface string
			string prefaceString;
			if (type.IsNested)
			{
				prefaceString = GetXmlDocumenationFormattedString(
					type.DeclaringType,
					isMethodParameter,
					typeGenericMap,
					methodGenericMap) + ".";
			}
			else
			{
				prefaceString = type.Namespace + ".";
			}

			if (type.IsByRef)
			{
				result = elementTypeString + genericArgumentsString + "@";
			}
			else
			{
				string typeNameString =
					isMethodParameter ?
					typeNameString = Regex.Replace(type.Name, @"`\d+", string.Empty) :
					typeNameString = type.Name;
				result = prefaceString + typeNameString + genericArgumentsString;
			}

			FormatComplete:
			return result;
		}

		/// <summary>Gets the XML documentation on a property.</summary>
		/// <param name="propertyInfo">The property to get the XML documentation of.</param>
		/// <returns>The XML documentation on the property.</returns>
		/// <remarks>The XML documentation must be loaded into memory for this function to work.</remarks>
		public static string GetDocumentation(this PropertyInfo propertyInfo)
		{
			LoadXmlDocumentation(propertyInfo.DeclaringType.Assembly);
			string key = "P:" + XmlDocumentationKeyHelper(propertyInfo.DeclaringType.FullName, propertyInfo.Name);
			loadedXmlDocumentation.TryGetValue(key, out string documentation);
			return documentation;
		}

		/// <summary>Gets the XML documentation on a field.</summary>
		/// <param name="fieldInfo">The field to get the XML documentation of.</param>
		/// <returns>The XML documentation on the field.</returns>
		/// <remarks>The XML documentation must be loaded into memory for this function to work.</remarks>
		public static string GetDocumentation(this FieldInfo fieldInfo)
		{
			LoadXmlDocumentation(fieldInfo.DeclaringType.Assembly);
			string key = "F:" + XmlDocumentationKeyHelper(fieldInfo.DeclaringType.FullName, fieldInfo.Name);
			loadedXmlDocumentation.TryGetValue(key, out string documentation);
			return documentation;
		}

		/// <summary>Gets the XML documentation on an event.</summary>
		/// <param name="eventInfo">The event to get the XML documentation of.</param>
		/// <returns>The XML documentation on the event.</returns>
		/// <remarks>The XML documentation must be loaded into memory for this function to work.</remarks>
		public static string GetDocumentation(this EventInfo eventInfo)
		{
			LoadXmlDocumentation(eventInfo.DeclaringType.Assembly);
			string key = "E:" + XmlDocumentationKeyHelper(eventInfo.DeclaringType.FullName, eventInfo.Name);
			loadedXmlDocumentation.TryGetValue(key, out string documentation);
			return documentation;
		}

		internal static string XmlDocumentationKeyHelper(string typeFullNameString, string memberNameString)
		{
			string key = Regex.Replace(typeFullNameString, @"\[.*\]", string.Empty).Replace('+', '.');
			if (memberNameString != null)
			{
				key += "." + memberNameString;
			}
			return key;
		}

		/// <summary>Gets the XML documentation on a member.</summary>
		/// <param name="memberInfo">The member to get the XML documentation of.</param>
		/// <returns>The XML documentation on the member.</returns>
		/// <remarks>The XML documentation must be loaded into memory for this function to work.</remarks>
		public static string GetDocumentation(this MemberInfo memberInfo)
		{
			if (memberInfo.MemberType.HasFlag(MemberTypes.Field))
			{
				return ((FieldInfo)memberInfo).GetDocumentation();
			}
			else if (memberInfo.MemberType.HasFlag(MemberTypes.Property))
			{
				return ((PropertyInfo)memberInfo).GetDocumentation();
			}
			else if (memberInfo.MemberType.HasFlag(MemberTypes.Event))
			{
				return ((EventInfo)memberInfo).GetDocumentation();
			}
			else if (memberInfo.MemberType.HasFlag(MemberTypes.Constructor))
			{
				return ((ConstructorInfo)memberInfo).GetDocumentation();
			}
			else if (memberInfo.MemberType.HasFlag(MemberTypes.Method))
			{
				return ((MethodInfo)memberInfo).GetDocumentation();
			}
			else if (memberInfo.MemberType.HasFlag(MemberTypes.TypeInfo) ||
				memberInfo.MemberType.HasFlag(MemberTypes.NestedType))
			{
				return ((TypeInfo)memberInfo).GetDocumentation();
			}
			else
			{
				return null;
			}
		}

		/// <summary>Gets the XML documentation for a parameter.</summary>
		/// <param name="parameterInfo">The parameter to get the XML documentation for.</param>
		/// <returns>The XML documenation of the parameter.</returns>
		public static string GetDocumentation(this ParameterInfo parameterInfo)
		{
			string memberDocumentation = parameterInfo.Member.GetDocumentation();
			if (memberDocumentation != null)
			{
				string regexPattern =
					Regex.Escape(@"<param name=" + "\"" + parameterInfo.Name + "\"" + @">") +
					".*?" +
					Regex.Escape(@"</param>");

				Match match = Regex.Match(memberDocumentation, regexPattern);
				if (match.Success)
				{
					return match.Value;
				}
			}
			return null;
		}

		#endregion

		#region MethodInfo

		/// <summary>Determines if a MethodInfo is a local function.</summary>
		/// <param name="methodInfo">The method info to determine if it is a local function.</param>
		/// <returns>True if the MethodInfo is a local function. False if not.</returns>
		public static bool IsLocalFunction(this MethodInfo methodInfo)
		{
			return Regex.Match(methodInfo.Name, @"g__.+\|\d+_\d+").Success;
		}

		#endregion
	}
}
