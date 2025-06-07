using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica9.Data;
using Practica9.Models;
using Practica9.PdfReports;
using QuestPDF.Fluent;
using ClosedXML.Excel;

namespace Practica9.Controllers;

//[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IdentityDbContext _context;

    public HomeController(IdentityDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? SelectedCategoryID)
    {

        var totalOrders = _context.Orders.Count();
        var totalCustomers = _context.Customers.Count();

        var productsQuery = _context.Products.AsQueryable();

        if (SelectedCategoryID.HasValue) {
            productsQuery = productsQuery.Where(p => p.CategoryID == SelectedCategoryID.Value);
        }

        var topProducts = productsQuery.OrderByDescending(p => p.UnitsInStock)
            .Take(10)
            .ToList();

        var categories = _context.Categories.ToList();

        var model = new DashboardViewModel
        {
            TotalOrders = totalOrders,
            TotalCustomers = totalCustomers,
            TopProducts = topProducts,
            Categories = categories,
            SelectedCategoryID = SelectedCategoryID
        };

        return View(model);
    }

    public async Task<IActionResult> ExportToPdf()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DownloadPdf(int? categoryId, decimal? minPrice, decimal? maxPrice)
    { 
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryID == categoryId.Value);      
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice <= maxPrice.Value);
        }

        var products = await query.ToListAsync();

        var pdfDocument = new ProductPdfDocument(products);
        var pdf = pdfDocument.GeneratePdf();

        return File(pdf, "application/pdf", "products_report.pdf");
    }

    public async Task<IActionResult> ExportToExcel()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DownloadExcel(int? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryID == categoryId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.UnitPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.UnitPrice <= maxPrice.Value);

        var products = await query.ToListAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Products");

        ws.Cell(1, 1).Value = "Product ID";
        ws.Cell(1, 2).Value = "Product Name";
        ws.Cell(1, 3).Value = "Category";
        ws.Cell(1, 4).Value = "Unit Price";

        for (int i = 0; i < products.Count; i++)
        {
            var p = products[i];
            ws.Cell(i + 2, 1).Value = p.ProductID;
            ws.Cell(i + 2, 2).Value = p.ProductName;
            ws.Cell(i + 2, 3).Value = p.Category?.CategoryName;
            ws.Cell(i + 2, 4).Value = p.UnitPrice;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products_filtered.xlsx");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
