using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitViewer.API.Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerProducesAttribute : Attribute
    {
        private IEnumerable<string> m_ContentTypes = null;

        public SwaggerProducesAttribute(params string[] contentTypes)
        {
            m_ContentTypes = contentTypes;
        }

        public IEnumerable<string> ContentTypes
        {
            get { return m_ContentTypes; }
        }
    }
}
