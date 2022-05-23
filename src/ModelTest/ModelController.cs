using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Controllers;
using Nop.Web.Models.Catalog;

namespace ModelTest
{
    public class ModelController : BasePublicController
    {
        public ModelController()
        {

        }

        public virtual async Task<IActionResult> Category(int categoryId, CatalogProductsCommand commandIgnored)
        {
            throw new NotImplementedException();
        }
    }
}
