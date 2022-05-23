using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Tests;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;
using FsCheck.NUnit;
using FsCheck;
using System.Collections.Generic;

namespace ModelTest
{
    [TestFixture]
    public class ModelControllerTests : BaseNopTest
    {

        private const int CATEGORY_ID = 1;
        private const string EXPECTED_VIEW = "CategoryTemplate.ProductsInGridOrLines";

        private ModelController _catalogController;
        private CatalogProductsCommand _command;
        private ICategoryService _categoryService;
        private ViewResult _baselineResult;

        [OneTimeSetUp]
        public async Task Init()
        {
            var catalogSettings = new CatalogSettings
            {
                AllowViewUnpublishedProductPage = true,
                DisplayDiscontinuedMessageForUnpublishedProducts = true,
                PublishBackProductWhenCancellingOrders = false,
                ShowSkuOnProductDetailsPage = true,
                ShowSkuOnCatalogPages = false,
                ShowManufacturerPartNumber = false,
                ShowGtin = false,
                ShowFreeShippingNotification = true,
                AllowProductSorting = true,
                AllowProductViewModeChanging = true,
                DefaultViewMode = "grid",
                ShowProductsFromSubcategories = false,
                ShowCategoryProductNumber = false,
                ShowCategoryProductNumberIncludingSubcategories = false,
                CategoryBreadcrumbEnabled = true,
                ShowShareButton = true,
                PageShareCode = "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script src=\"http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions\"></script><!-- AddThis Button END -->",
                ProductReviewsMustBeApproved = false,
                OneReviewPerProductFromCustomer = false,
                DefaultProductRatingValue = 5,
                AllowAnonymousUsersToReviewProduct = false,
                ProductReviewPossibleOnlyAfterPurchasing = false,
                NotifyStoreOwnerAboutNewProductReviews = false,
                NotifyCustomerAboutProductReviewReply = false,
                EmailAFriendEnabled = true,
                AllowAnonymousUsersToEmailAFriend = false,
                RecentlyViewedProductsNumber = 3,
                RecentlyViewedProductsEnabled = true,
                NewProductsEnabled = true,
                NewProductsPageSize = 6,
                NewProductsAllowCustomersToSelectPageSize = true,
                NewProductsPageSizeOptions = "6, 3, 9",
                CompareProductsEnabled = true,
                CompareProductsNumber = 4,
                ProductSearchAutoCompleteEnabled = true,
                ProductSearchEnabled = true,
                ProductSearchAutoCompleteNumberOfProducts = 10,
                ShowLinkToAllResultInSearchAutoComplete = false,
                ProductSearchTermMinimumLength = 3,
                ShowProductImagesInSearchAutoComplete = false,
                ShowBestsellersOnHomepage = false,
                NumberOfBestsellersOnHomepage = 4,
                SearchPageProductsPerPage = 6,
                SearchPageAllowCustomersToSelectPageSize = true,
                SearchPagePageSizeOptions = "6, 3, 9, 18",
                SearchPagePriceRangeFiltering = true,
                SearchPageManuallyPriceRange = true,
                SearchPagePriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                SearchPagePriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                ProductsAlsoPurchasedEnabled = true,
                ProductsAlsoPurchasedNumber = 4,
                AjaxProcessAttributeChange = true,
                NumberOfProductTags = 15,
                ProductsByTagPageSize = 6,
                IncludeShortDescriptionInCompareProducts = false,
                IncludeFullDescriptionInCompareProducts = false,
                IncludeFeaturedProductsInNormalLists = false,
                UseLinksInRequiredProductWarnings = true,
                DisplayTierPricesWithDiscounts = true,
                IgnoreDiscounts = false,
                IgnoreFeaturedProducts = false,
                IgnoreAcl = true,
                IgnoreStoreLimitations = true,
                CacheProductPrices = false,
                ProductsByTagAllowCustomersToSelectPageSize = true,
                ProductsByTagPageSizeOptions = "6, 3, 9, 18",
                ProductsByTagPriceRangeFiltering = true,
                ProductsByTagManuallyPriceRange = true,
                ProductsByTagPriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                ProductsByTagPriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
                MaximumBackInStockSubscriptions = 200,
                ManufacturersBlockItemsToDisplay = 2,
                DisplayTaxShippingInfoFooter = false,
                DisplayTaxShippingInfoProductDetailsPage = false,
                DisplayTaxShippingInfoProductBoxes = false,
                DisplayTaxShippingInfoShoppingCart = false,
                DisplayTaxShippingInfoWishlist = false,
                DisplayTaxShippingInfoOrderDetailsPage = false,
                DefaultCategoryPageSizeOptions = "6, 3, 9",
                DefaultCategoryPageSize = 6,
                DefaultManufacturerPageSizeOptions = "6, 3, 9",
                DefaultManufacturerPageSize = 6,
                ShowProductReviewsTabOnAccountPage = true,
                ProductReviewsPageSizeOnAccountPage = 10,
                ProductReviewsSortByCreatedDateAscending = false,
                ExportImportProductAttributes = true,
                ExportImportProductSpecificationAttributes = true,
                ExportImportUseDropdownlistsForAssociatedEntities = true,
                ExportImportProductsCountInOneFile = 500,
                ExportImportSplitProductsFile = false,
                ExportImportRelatedEntitiesByName = true,
                CountDisplayedYearsDatePicker = 1,
                UseAjaxLoadMenu = false,
                UseAjaxCatalogProductsLoading = true,
                EnableManufacturerFiltering = true,
                EnablePriceRangeFiltering = true,
                EnableSpecificationAttributeFiltering = true,
                DisplayFromPrices = false,
                AttributeValueOutOfStockDisplayType = AttributeValueOutOfStockDisplayType.AlwaysDisplay,
                AllowCustomersToSearchWithCategoryName = true,
                AllowCustomersToSearchWithManufacturerName = true,
                DisplayAllPicturesOnCatalogPages = false
            };
            var mediaSettings = new MediaSettings
            {
                AvatarPictureSize = 120,
                ProductThumbPictureSize = 415,
                ProductDetailsPictureSize = 550,
                ProductThumbPictureSizeOnProductDetailsPage = 100,
                AssociatedProductPictureSize = 220,
                CategoryThumbPictureSize = 450,
                ManufacturerThumbPictureSize = 420,
                VendorThumbPictureSize = 450,
                CartThumbPictureSize = 80,
                OrderThumbPictureSize = 80,
                MiniCartThumbPictureSize = 70,
                AutoCompleteSearchThumbPictureSize = 20,
                ImageSquarePictureSize = 32,
                MaximumImageSize = 1980,
                DefaultPictureZoomEnabled = false,
                DefaultImageQuality = 80,
                MultipleThumbDirectories = false,
                ImportProductImagesUsingHash = true,
                AzureCacheControlHeader = string.Empty,
                UseAbsoluteImagePath = true
            };
            var vendorSettings = new VendorSettings
            {
                DefaultVendorPageSizeOptions = "6, 3, 9",
                VendorsBlockItemsToDisplay = 0,
                ShowVendorOnProductDetailsPage = true,
                AllowCustomersToContactVendors = true,
                AllowCustomersToApplyForVendorAccount = true,
                TermsOfServiceEnabled = false,
                AllowVendorsToEditInfo = false,
                NotifyStoreOwnerAboutVendorInformationChange = true,
                MaximumProductNumber = 3000,
                AllowVendorsToImportProducts = true
            };

            var aclService = GetService<IAclService>();
            var catalogModelFactory = GetService<ICatalogModelFactory>();
            var categoryService = GetService<ICategoryService>();
            var customerActivityService = GetService<ICustomerActivityService>();
            var genericAttributeService = GetService<IGenericAttributeService>();
            var localizationService = GetService<ILocalizationService>();
            var manufacturerService = GetService<IManufacturerService>();
            var permissionService = GetService<IPermissionService>();
            var productModelFactory = GetService<IProductModelFactory>();
            var productService = GetService<IProductService>();
            var productTagService = GetService<IProductTagService>();
            var storeContext = GetService<IStoreContext>();
            var storeMappingService = GetService<IStoreMappingService>();
            var urlRecordService = GetService<IUrlRecordService>();
            var vendorService = GetService<IVendorService>();
            var webHelper = GetService<IWebHelper>();
            var workContext = GetService<IWorkContext>();
            _categoryService = GetService<ICategoryService>();

            /*
            _catalogController = new CatalogController(
                catalogSettings,
                aclService,
                catalogModelFactory,
                categoryService,
                customerActivityService,
                genericAttributeService,
                localizationService,
                manufacturerService,
                permissionService,
                productModelFactory,
                productService,
                productTagService,
                storeContext,
                storeMappingService,
                urlRecordService,
                vendorService,
                webHelper,
                workContext,
                mediaSettings,
                vendorSettings);
            */
            _catalogController = new ModelController();

            _command = InitCommand();

            _baselineResult = (ViewResult)await _catalogController.Category(CATEGORY_ID, _command);
        }

        private CatalogProductsCommand InitCommand()
        {
            return new CatalogProductsCommand
            {
                PageNumber = 0,
                PageSize = 0,
                TotalItems = 0,
                TotalPages = 0,
                FirstItem = 0,
                LastItem = 0,
                HasPreviousPage = false,
                HasNextPage = false,
                Price = null,
                SpecificationOptionIds = null,
                ManufacturerIds = null,
                OrderBy = null,
                ViewMode = null
            };
        }

        [Test]
        public void ShouldReturnViewResult()
        {
            Assert.IsNotNull(_baselineResult);
        }

        [Test]
        public async Task ShouldPreserveModelData()
        {
            var category = await _categoryService.GetCategoryByIdAsync(CATEGORY_ID);
            var model = (CategoryModel)_baselineResult.Model;
            PropertiesShouldEqual(category, model);
        }

        [Test]
        public void ShouldUseCorrectView()
        {
            Assert.AreEqual(EXPECTED_VIEW, _baselineResult.ViewName);
        }

        [FsCheck.NUnit.Property]
        public bool ModelIsIndependantOfCommand(
            int pageNumber,
            int pageSize,
            int totalItems,
            int totalPages,
            int firstItem,
            int lastItem,
            bool hasPreviousPage,
            bool hasNextPage,
            List<int> specificationOptionIds,
            List<int> manufacturerIds,
            int? orderBy,
            string viewMode)
        {
            var testCommand = InitCommand();
            testCommand.PageNumber = pageNumber;
            testCommand.PageSize = pageSize;
            testCommand.TotalItems = totalItems;
            testCommand.TotalPages = totalPages;
            testCommand.FirstItem = firstItem;
            testCommand.LastItem = lastItem;
            testCommand.HasPreviousPage = hasPreviousPage;
            testCommand.HasNextPage = hasNextPage;
            testCommand.SpecificationOptionIds = specificationOptionIds;
            testCommand.ManufacturerIds = manufacturerIds;
            testCommand.OrderBy = orderBy;
            testCommand.ViewMode = viewMode;

            var testResult = (ViewResult) _catalogController.Category(CATEGORY_ID, testCommand).Result;

            return _baselineResult.Model == testResult.Model;
        }

        [FsCheck.NUnit.Property]
        public bool ViewIsIndependantOfCommand(
            int pageNumber,
            int pageSize,
            int totalItems,
            int totalPages,
            int firstItem,
            int lastItem,
            bool hasPreviousPage,
            bool hasNextPage,
            List<int> specificationOptionIds,
            List<int> manufacturerIds,
            int? orderBy,
            string viewMode)
        {
            var testCommand = InitCommand();
            testCommand.PageNumber = pageNumber;
            testCommand.PageSize = pageSize;
            testCommand.TotalItems = totalItems;
            testCommand.TotalPages = totalPages;
            testCommand.FirstItem = firstItem;
            testCommand.LastItem = lastItem;
            testCommand.HasPreviousPage = hasPreviousPage;
            testCommand.HasNextPage = hasNextPage;
            testCommand.SpecificationOptionIds = specificationOptionIds;
            testCommand.ManufacturerIds = manufacturerIds;
            testCommand.OrderBy = orderBy;
            testCommand.ViewMode = viewMode;

            var testResult = (ViewResult) _catalogController.Category(CATEGORY_ID, testCommand).Result;

            return _baselineResult.ViewName == testResult.ViewName;
        }
    }
}