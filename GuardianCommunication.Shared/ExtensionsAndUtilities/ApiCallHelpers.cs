using System;
using System.Net.Http;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
	public static class ApiCallHelpers
	{
		public static HttpClient GetHttpClient(string baseAddress)
		{
			return new HttpClient {BaseAddress = new Uri(baseAddress)};
		}


		public static string CombineUri(string baseUri, string relativeUri)
		{
            Uri.TryCreate(new Uri(baseUri), relativeUri, out Uri result);
			return result.ToString();
        }
	}
}
