using AutoMapper;
using BulkyBook.BLL.Interfaces;
using BulkyBook.DAL.Context;
using BulkyBook.DAL.Entities;
using BulkyBook.PL.Helpers;
using BulkyBook.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace BulkyBook.PL.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        //private readonly IProductRepository _productRepository;
        //private readonly ICategoryRepository _categoryRepository;
        //private readonly ICoverTypeRepository _coverTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _DbContext;
        private readonly IMapper _mapper;

        public ProductController(IMapper mapper, IUnitOfWork unitOfWork, ApplicationDbContext DbContext)
        {
            _unitOfWork = unitOfWork;
            _mapper =mapper;
            _DbContext = DbContext;
        //    _productRepository=productRepository;
        //    _categoryRepository=categoryRepository;
        //    _coverTypeRepository=coverTypeRepository;
        }

        public IActionResult Index()
        {
            var AllProducts = _unitOfWork.Products.GetAll();  
            return View(AllProducts);
        }

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CateegoriesList = _unitOfWork.Categories.GetAll().Select(c=>
            new SelectListItem()
            {
                Text = c.Name,
                Value = c.CategoryId.ToString()
            }
            );
            IEnumerable<SelectListItem> CoversList = _unitOfWork.CoverTypes.GetAll().Select(c =>
            new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }
            );

            ViewBag.CategoryList = CateegoriesList;
            ViewData["CoversList"] = CoversList;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel productViewModel, IFormFile file, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            //if (ModelState.IsValid)
            //{
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
             
                productViewModel.file = "/uploads/" + uniqueFileName;
               
            }

            var product = new BulkyBook.DAL.Entities.Product
                {
                    Title = productViewModel.Title,
                    Description = productViewModel.Description,
                    ISBN = productViewModel.ISBN,
                    Author = productViewModel.Author,
                    ListPrice = productViewModel.ListPrice,
                    Price = productViewModel.Price,
                    Price5 = productViewModel.Price5,
                    Price10 = productViewModel.Price10,
                    CategoryId = productViewModel.CategoryId,
                    CoverTypeId = productViewModel.CoverTypeId,
                    ImageUrl= "360_F.jpg",
                    file= productViewModel.file
            };

                _DbContext.Products.Add(product);
                _DbContext.SaveChanges();

                // Handle file upload
               
                TempData["Message"] = "Product created successfully.";
                return RedirectToAction("Index");
          //  }
            // If ModelState is not valid, return the view with validation errors
          //  return View(productViewModel);
        }

        public IActionResult Edit(int? id)
        {
            if(id==null || id==0)
                return NotFound();

            IEnumerable<SelectListItem> CateegoriesList = _unitOfWork.Categories.GetAll().Select(c =>
           new SelectListItem()
           {
               Text = c.Name,
               Value = c.CategoryId.ToString()
           }
           );
            IEnumerable<SelectListItem> CoversList = _unitOfWork.CoverTypes.GetAll().Select(c =>
            new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }
            );

            ViewBag.CategoryList = CateegoriesList;
            ViewData["CoversList"] = CoversList;

            var ProductToUpdate = _unitOfWork.Products.GetByIdSpecification(p => p.Id ==id);
            ProductViewModel ProductViewModel = new ProductViewModel()
            {
                Id=ProductToUpdate.Id,
                Title = ProductToUpdate.Title,
                Description = ProductToUpdate.Description,
                ISBN = ProductToUpdate.ISBN,
                Author = ProductToUpdate.Author,
                Price = ProductToUpdate.Price,
                ListPrice = ProductToUpdate.ListPrice,
                Price10 = ProductToUpdate.Price10,
                Price5 = ProductToUpdate.Price5,
                CategoryId = ProductToUpdate.CategoryId,
                CoverTypeId = ProductToUpdate.CoverTypeId,  
                ImageUrl = ProductToUpdate.ImageUrl,
                file= ProductToUpdate.file



            };


            return View(ProductViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute]int? id , ProductViewModel productViewModel)
        {
            if(productViewModel.Id != id)
                return BadRequest();


            if (ModelState.TryGetValue("Image", out ModelStateEntry entry) && entry.Errors.Count > 0)
            {
                var CastedProduct = _mapper.Map<ProductViewModel, BulkyBook.DAL.Entities.Product>(productViewModel);
                _unitOfWork.Products.Update(CastedProduct);
                _unitOfWork.Products.Save();
                TempData["Success"] = "Product Updated Succcessfully";
                return RedirectToAction(nameof(Index));
            }

            else 
            //if (ModelState.IsValid)
            //{
                try
                {
                    //DocumetSetting.DeleteFile(productViewModel.ImageUrl, "Images");
                    //productViewModel.ImageUrl =  DocumetSetting.UploadDocument(productViewModel.Image, "Images");
                    var CastedProduct = _mapper.Map<ProductViewModel, BulkyBook.DAL.Entities.Product>(productViewModel);
                     _unitOfWork.Products.Update(CastedProduct);
                    _unitOfWork.Products.Save();
                    TempData["Success"] = "Product Updated Succcessfully";
                    return RedirectToAction(nameof(Index));
                }
                catch 
                {
                    return View(productViewModel);
                }
            //}


           // return View(productViewModel);
        }

        public IActionResult Delete(int? id)
        {
            if (id==null || id==0)
                return NotFound();

            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Categories.GetAll().Select(c =>
           new SelectListItem()
           {
               Text = c.Name,
               Value = c.CategoryId.ToString()
           }
           );
            IEnumerable<SelectListItem> CoversList = _unitOfWork.CoverTypes.GetAll().Select(c =>
            new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }
            );

            ViewBag.CategoryList = CategoryList;
            ViewData["CoversList"] = CoversList;

            var ProductToDelete = _unitOfWork.Products.GetByIdSpecification(p => p.Id ==id);
            //ProductViewModel ProductViewModel = new ProductViewModel()
            //{
            //    Id=ProductToUpdate.Id,
            //    Title = ProductToUpdate.Title,
            //    Description = ProductToUpdate.Description,
            //    ISBN = ProductToUpdate.ISBN,
            //    Author = ProductToUpdate.Author,
            //    Price = ProductToUpdate.Price,
            //    ListPrice = ProductToUpdate.ListPrice,
            //    Price10 = ProductToUpdate.Price10,
            //    Price5 = ProductToUpdate.Price5,
            //    CategoryId = ProductToUpdate.CategoryId,
            //    CoverTypeId = ProductToUpdate.CoverTypeId,
            //    ImageUrl = ProductToUpdate.ImageUrl,
            //    Image = DocumetSetting.GetFile("Images", ProductToUpdate.ImageUrl)

            //};

            var ProductViewModel = _mapper.Map<BulkyBook.DAL.Entities.Product, ProductViewModel>(ProductToDelete);


            return View(ProductViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute]int? id , ProductViewModel productViewModel )
        {
            if(productViewModel.Id!=id)
                return BadRequest();


            var CastedProduct = _mapper.Map<ProductViewModel, BulkyBook.DAL.Entities.Product>(productViewModel);
            DocumetSetting.DeleteFile(productViewModel.ImageUrl, "Images");
            _unitOfWork.Products.Delete(CastedProduct);
            _unitOfWork.Products.Save();
            TempData["Success"]="Product Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
