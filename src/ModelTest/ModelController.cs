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
        private INopDataProvider _dataProvider;
        public ModelController(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public virtual async Task<IActionResult> Category(int categoryId, CatalogProductsCommand commandIgnored)
        {
            string viewTemplateHardcoded = "CategoryTemplate.ProductsInGridOrLines";

            var categoryTable = _dataProvider.GetTable<Category>();

            var query =
                from cat in categoryTable
                where cat.Id == categoryId
                select cat;

            var category = query.FirstOrDefault<Category>();

            var model = new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                MetaKeywords = category.MetaKeywords, 
                MetaDescription = category.MetaDescription,
                MetaTitle = category.MetaTitle,
            };

            return View(viewTemplateHardcoded, model);
        }
    }
}
