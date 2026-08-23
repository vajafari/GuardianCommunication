using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
	public static class ObjectHelper
	{
		public static string SerializeAsXml<T>(T dataToSerialize)
		{
			if (dataToSerialize == null) return string.Empty;

			StringBuilder builder = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(builder))
			{
				Type typeOfObject = dataToSerialize.GetType();
				writer.WriteStartElement(typeOfObject.Name);

				var allProperties = typeOfObject.GetProperties();
				foreach (var prop in allProperties)
				{
					if (IsSimpleType(prop.PropertyType))
					{
						object valueOfProperty = prop.GetValue(dataToSerialize);
						writer.WriteElementString(prop.Name, valueOfProperty == null ? "NULL" : valueOfProperty.ToString());
					}
				}
				writer.WriteEndElement();
				writer.Flush();
			}
			return builder.ToString();
		}

        public static string SerializeAsJsonFormatted(object dataToSerialize)
        {
            return JsonConvert.SerializeObject(dataToSerialize, Formatting.Indented);
        }

        public static string SerializeAsJson(object dataToSerialize)
		{
			return JsonConvert.SerializeObject(dataToSerialize);
		}

		public static T DeserializeAsJson<T>(string dataToDeserialize)
		{
			return JsonConvert.DeserializeObject<T>(dataToDeserialize);
		}

        public static bool IsSimpleType(Type type)
		{
			return
				type.IsPrimitive ||
				new List<Type>
				{
					typeof(string),
					typeof(decimal),
					typeof(DateTime),
					typeof(DateTimeOffset),
					typeof(TimeSpan),
					typeof(Guid)
				}.Contains(type) ||
				type.IsEnum ||
				Convert.GetTypeCode(type) != TypeCode.Object ||
				(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
				 IsSimpleType(type.GetGenericArguments()[0]));

		}

        public static bool IsEmbeddedProperty(Type propertyType)
		{

			if ((propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().UnderlyingSystemType == typeof(ICollection<>)))
			{
				return true;
			}
			if (!(propertyType == typeof(string) || propertyType == typeof(byte[]) || propertyType.IsValueType || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().UnderlyingSystemType == typeof(Nullable<>))))
			{
				return true;
			}
			return false;
		}
        
        public static T DeepClone<T>(this T source)
        {
            if (source == null)
                return default;

            var json = ObjectHelper.SerializeAsJson(source);
            return DeserializeAsJson<T>(json);
        }





        #region Overwrite


        public static bool IsNavigationProperty(Type propertyType)
		{

			if ((propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().UnderlyingSystemType == typeof(ICollection<>)))
			{
				return true;
			}
			if (!(propertyType == typeof(string) || propertyType == typeof(byte[]) || propertyType.IsValueType || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().UnderlyingSystemType == typeof(Nullable<>))))
			{
				return true;
			}
			return false;
		}

		private static List<PropertyInfo> GetValidPropertyForWriteValueWithExceptionList(IReflect typeOfObject, List<string> exceptionList = null)
		{
			if (exceptionList == null)
			{
				exceptionList = [];
			}
			
			//List<string> newExceptionList = new List<string>();
			//foreach (var item in exceptionList)
			//{
			//	newExceptionList.Add(item);
			//	newExceptionList.Add(item + Constants.GeneralStandardDatePostfix);
			//	newExceptionList.Add(item + Constants.GeneralStandardTimePostfix);
			//	newExceptionList.Add(item + Constants.GeneralStandardDatePersianPostfix);
			//}

			return typeOfObject.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty)
				.Where(row => !IsNavigationProperty(row.PropertyType)
				              &&
				              row.CanWrite
				              &&
				              row.GetSetMethod(true).IsPublic
				).ToList();
		}

		private static List<PropertyInfo> GetValidPropertyForWriteValueWithIncludedList(IReflect typeOfObject, List<string> includedList = null)
		{
			if (includedList == null)
			{
				includedList = [];
			}

			return typeOfObject.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty)
				.Where(row => includedList.Contains(row.Name)
				              &&
				              !IsNavigationProperty(row.PropertyType)
				              &&
				              row.CanWrite
				              &&
				              row.GetSetMethod(true).IsPublic
				).ToList();
		}

		private static void OverWriteValues<TSource, TTarget>(TSource sourceOfValues, TTarget objectForWriteValues,
			List<PropertyInfo> sourceTypeProperties, IEnumerable<PropertyInfo> targetTypeProperties)
			where TSource : class
			where TTarget : class
		{

			foreach (var targetPropertyInfo in targetTypeProperties)
			{
				var propertyOfSource = sourceTypeProperties.FirstOrDefault(row => row.Name == targetPropertyInfo.Name);
				if (propertyOfSource != null)
				{
					targetPropertyInfo.SetValue(objectForWriteValues, propertyOfSource.GetValue(sourceOfValues));
				}
			}

		}

		public static void OverWriteAllValues<TSource, TTarget>(TSource sourceOfValues, TTarget objectForWriteValues)
			where TSource : class
			where TTarget : class
		{
			if (objectForWriteValues == null) objectForWriteValues = Activator.CreateInstance<TTarget>();

			var sourceTypeProperties = GetValidPropertyForWriteValueWithExceptionList(typeof(TSource));
			var targetTypeProperties = GetValidPropertyForWriteValueWithExceptionList(typeof(TTarget));

			OverWriteValues(sourceOfValues, objectForWriteValues, sourceTypeProperties, targetTypeProperties);

		}


		public static void OverWriteValuesWithExceptionList<TSource, TTarget>(TSource sourceOfValues, TTarget objectForWriteValues, List<string> exceptionList)
			where TSource : class
			where TTarget : class
		{
			if (objectForWriteValues == null) objectForWriteValues = Activator.CreateInstance<TTarget>();

			var sourceTypeProperties = GetValidPropertyForWriteValueWithExceptionList(typeof(TSource), exceptionList);
			var targetTypeProperties = GetValidPropertyForWriteValueWithExceptionList(typeof(TTarget), exceptionList);

			OverWriteValues(sourceOfValues, objectForWriteValues, sourceTypeProperties, targetTypeProperties);

		}

		public static void OverWriteValuesWithIncludedList<TSource, TTarget>(TSource sourceOfValues, TTarget objectForWriteValues, List<string> includedList)
			where TSource : class
			where TTarget : class
		{
			if (objectForWriteValues == null) objectForWriteValues = Activator.CreateInstance<TTarget>();

			var sourceTypeProperties = GetValidPropertyForWriteValueWithIncludedList(typeof(TSource), includedList);
			var targetTypeProperties = GetValidPropertyForWriteValueWithIncludedList(typeof(TTarget), includedList);

			OverWriteValues(sourceOfValues, objectForWriteValues, sourceTypeProperties, targetTypeProperties);

		}

		#endregion






	}
}
