using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Web.Controllers;
using Nop.Web.Models.Catalog;

namespace ModelTest
{
    public class ModelController : BasePublicController
    {
        private IRepository<Category> _categoryRepository;
        public ModelController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }

        public virtual async Task<IActionResult> Category(int categoryId, CatalogProductsCommand commandIgnored)
        {
            string viewTemplateHardcoded = "CategoryTemplate.ProductsInGridOrLines";

            var model = new CategoryModel();

            return View(viewTemplateHardcoded, model);
        }
    }
}
