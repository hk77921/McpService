using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Text.Json;

namespace McpService.Application.Validators
{
    public class JsonSchemaValidator
    {
        public async Task<bool> ValidateAsync(JsonDocument schema, JsonDocument instance)
        {
            try
            {
                var schemaJson = schema.RootElement.GetRawText();
                var instanceJson = instance.RootElement.GetRawText();

                var jSchema = JSchema.Parse(schemaJson);
                var jToken = JToken.Parse(instanceJson);

                return jToken.IsValid(jSchema);
            }
            catch
            {
                return false;
            }
        }
    }
}
