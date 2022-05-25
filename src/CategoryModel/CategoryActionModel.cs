
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;

public class CategoryActionModel : BaseController
{

    public async Task Category(int id, CatalogProductsCommand commandIGNORED)
    {
        var model = new CategoryModel();
        throw new NotImplementedException();
    }

}