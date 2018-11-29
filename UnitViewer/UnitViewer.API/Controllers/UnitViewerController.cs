using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace UnitViewer.API.Controllers
{
    [Route("api")]
    public class UnitViewerController : Controller
    {
        private ModelAccess.Interfaces.IModelAccess_DB m_ModelAccessDB = null;
        private DescriptorsShared.Interfaces.IDescriptorSourceConnector m_DescriptorSourceConnector = null;
        private ForgeAPI.Interface.Authentication.IService m_ForgeAPI_AuthenticationService = null;

        public UnitViewerController(
            ModelAccess.Interfaces.IModelAccess_DB modelAccessDB,
            DescriptorsShared.Interfaces.IDescriptorSourceConnector descriptorSourceConnector,
            ForgeAPI.Interface.Authentication.IService forgeAPIAuthService)
        {
            m_ModelAccessDB = modelAccessDB;
            m_DescriptorSourceConnector = descriptorSourceConnector;
            m_ForgeAPI_AuthenticationService = forgeAPIAuthService;
        }

        [HttpGet("unit/{orderNo}/current")]
        [Produces(
            JSONFormatters.CConstants.MEDIA_TYPE_NAME__EXPANDED,
            JSONFormatters.CConstants.MEDIA_TYPE_NAME__MINIFIED,
            "application/xml")]
        [Swagger.SwaggerProduces(
            JSONFormatters.CConstants.MEDIA_TYPE_NAME__EXPANDED,
            JSONFormatters.CConstants.MEDIA_TYPE_NAME__MINIFIED,
            "application/json",
            "application/xml")]
        public async Task<IActionResult> DB_GetCurrentUnit(string orderNo)
        {
            ModelAHU.Interfaces.Configuration.Types.IUnit unit;

            unit = await Task.Run(() => m_ModelAccessDB.GetCurrentUnit_ByOrderNumber(orderNo));

            if (unit == null) return NotFound();

            return Ok(unit);
        }

        [HttpGet("forge/authenticate")]
        public async Task<IActionResult> ForgeAuthenticateViewer()
        {
            ForgeAPI.Interface.Authentication.IToken token;

            token = await Task.Run(
                () => 
                m_ForgeAPI_AuthenticationService.Authenticate(new List<ForgeAPI.Interface.Enums.E_AccessScope>()
                    {
                        ForgeAPI.Interface.Enums.E_AccessScope.Data_Read,
                        ForgeAPI.Interface.Enums.E_AccessScope.Viewables_Read
                    }));

            return Ok(token);
        }

        [HttpGet("descriptorstore/{language}/{library}")]
        public IActionResult GetDescriptorData(
            string language,
            string library)
        {
            List<DescriptorsShared.Interfaces.IDescriptorData> list;

            list = m_DescriptorSourceConnector.GetDescriptorDataList(
                language,
                library);

            return Ok(list);
        }
    }
}
