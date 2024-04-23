using AutoMapper.Internal;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.Linq;

namespace Project.Auth.Api.Extensions
{
    /// <summary>
    /// Enum tipleri için Swagger şeması filtresi.
    /// Swagger UI'da enum tiplerinin daha anlaşılır biçimde sergilenmesi için kullanılır.
    /// </summary>
    public class EnumTypesSchemaFilter : ISchemaFilter
    {
        private readonly XDocument? _xmlComments;

        /// <summary>
        /// EnumTypesSchemaFilter örneği oluşturur ve XML yorum dosyasını yükler.
        /// </summary>
        /// <param name="xmlPath">XML yorumlarının dosya yolu.</param>
        public EnumTypesSchemaFilter(string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                _xmlComments = XDocument.Load(xmlPath);
            }
        }
        /// <summary>
        /// Verilen şemaya enum tipleri için şema filtresini uygular.
        /// </summary>
        /// <param name="schema">Uygulanacak OpenApi şeması.</param>
        /// <param name="context">Şema filtresi için bağlam bilgisi.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (_xmlComments == null) return;

            if (schema.Enum != null && schema.Enum.Count > 0 &&
               context.Type != null && context.Type.IsEnum)
            {
                schema.Description += "<p>Members:</p><ul>";

                var fullTypeName = context.Type.FullName;

                foreach (var enumMemberName in context.Type.GetMembers(BindingFlags.Static | BindingFlags.Public))
                {
                    var fullEnumMemberName = $"F:{fullTypeName}.{enumMemberName.Name}";

                    var enumMemberComments = _xmlComments.Descendants("member")
                        .FirstOrDefault(m => m.Attribute("name")!.Value.Equals
                            (fullEnumMemberName, StringComparison.OrdinalIgnoreCase));

                    if (enumMemberComments == null) continue;

                    var summary = enumMemberComments.Descendants("summary").FirstOrDefault();

                    if (summary == null) continue;

                    schema.Description += $"<li>{(int)enumMemberName.GetMemberValue(null)} - <i>{enumMemberName.Name}</i> - {summary.Value.Trim()}</li>";
                }

                schema.Description += "</ul>";
            }
        }
    }
}
