using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookWeb.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using BookWeb.Models;
using BookWeb.Context;

using ExcelDataReader;
using System.Text;

namespace BookWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> UploadExcel(IFormFile file)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (file != null && file.Length > 0)
        {
            var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\uploads\\";

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        bool isHeaderSkipped = false;

                        while (reader.Read())
                        {
                            if (!isHeaderSkipped)
                            {
                                isHeaderSkipped = true;
                                continue;
                            }

                            Transaction t = new Transaction();
                            t.AccountName = reader.GetValue(0).ToString();
                            t.AccountNumber = reader.GetValue(1).ToString();

                            _context.Add(t);
                            await _context.SaveChangesAsync();
                        }
                    } while (reader.NextResult());

                    ViewBag.Message = "success";
                }
            }
        }
        else
            ViewBag.Message = "empty";
        return RedirectToAction("Index");
    }

    public IActionResult Index()
    {
        return View();
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

    public IActionResult ImportExcel()
    {
        using (var package = new ExcelPackage(new FileInfo("TemporaryExcelUpload/Book1.xlsx")))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                Transaction entity = new Transaction
                {
                    // Map Excel columns to entity properties
                    AccountName = worksheet.Cells[row, 1].Value.ToString(),
                    AccountNumber = worksheet.Cells[row, 2].Value.ToString(),
                    // ... map other properties
                };

                _context.Transactions.Add(entity);
            }

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}

