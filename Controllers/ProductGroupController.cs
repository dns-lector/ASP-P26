using ASP_P26.Data;
using ASP_P26.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers
{
    [Route("api/product-group")]
    [ApiController]
    public class ProductGroupController(DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpGet]
        public IEnumerable<ProductGroup> GetList()
        {
            return _dataAccessor.GetProductGroups();
        }
    }
}
