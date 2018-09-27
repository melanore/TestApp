using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using TestApp.Business.Domain;

namespace TestApp.Web.Filters
{
    public class SwaggerModelBinder : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            RebindModel(swaggerDoc, "Delta[Customer]", nameof(Customer));
            RebindModel(swaggerDoc, "Delta[Address]", nameof(Address));
        }

        private void RebindModel(SwaggerDocument swaggerDoc, string sourceModel, string targetModel)
        {
            if (swaggerDoc.Definitions.ContainsKey(sourceModel) && swaggerDoc.Definitions.ContainsKey(targetModel))
            {
                swaggerDoc.Definitions[sourceModel].Properties.Clear();
                foreach (var prop in swaggerDoc.Definitions[targetModel].Properties) 
                    swaggerDoc.Definitions[sourceModel].Properties.Add(prop.Key, prop.Value);
            }
        }
    }
}