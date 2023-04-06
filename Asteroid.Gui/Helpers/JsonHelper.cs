using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Asteroid.Gui.Helpers
{
	public static class JsonHelper
	{
		private static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions() {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
		public static string ToJson(this object obj)
		{
			var json = JsonSerializer.Serialize(obj, JsonSerializerOptions);
			return json;
		}

		public static T ToObject<T>(this string json)
		{
			var obj = JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
			return obj;
		}
	}
}
