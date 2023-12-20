using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookWeb.Context;
using BookWeb.Models;
using PagedList;

namespace BookWeb.Controllers
{
    public class TransactionTmpController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionTmpController(ApplicationDbContext context)
        {
            _context = context;
        }

        private IEnumerable<Transaction> sortTransactions(IEnumerable<Transaction> transactions, bool sortByAccountNameASC, bool sortByAccountNumberASC)
        {

            if (!sortByAccountNameASC && !sortByAccountNumberASC)
            {
                transactions = transactions.OrderByDescending(t => t.AccountName).ThenByDescending(t => t.AccountNumber);
            }

            if (sortByAccountNameASC && !sortByAccountNumberASC)
            {
                transactions = transactions.OrderBy(t => t.AccountName).ThenByDescending(t => t.AccountNumber);
            }

            if (!sortByAccountNameASC && sortByAccountNumberASC)
            {
                transactions = transactions.OrderByDescending(t => t.AccountName).ThenBy(t => t.AccountNumber);
            }

            if (sortByAccountNameASC && sortByAccountNumberASC)
            {
                transactions = transactions.OrderBy(t => t.AccountName).ThenBy(t => t.AccountNumber);
            }
            return transactions;
        }

        // GET: TransactionTmp
        public async Task<IActionResult> Index(bool sortByAccountNameASC, bool sortByAccountNumberASC, string search, int? page)
        {
            ViewBag.sortByAccountNameASC = !sortByAccountNameASC;
            ViewBag.sortByAccountNumberASC = !sortByAccountNumberASC;
            ViewBag.search = search;

            if (search != null)
            {
                page = 1;
            }

            IEnumerable<Transaction> transactions = _context.Transactions;
            transactions = this.sortTransactions(transactions, sortByAccountNameASC, sortByAccountNumberASC);

            if (!String.IsNullOrEmpty(search))
            {
                transactions = transactions.Where(t => t.AccountName.ToLower().Contains(search.ToLower()) || t.AccountNumber.ToLower().Contains(search.ToLower()));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(transactions.ToPagedList(pageNumber, pageSize));

            //IQueryable<Transaction> transactions = _context.Transactions;
            //string sql = transactions.ToQueryString();
            //string sql1 = transactions.ToQueryString();
            //return View();
            //return _context.Transactions != null ? 
            //              View(await _context.Transactions.ToListAsync()) :
            //              Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
        }

        // GET: TransactionTmp/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: TransactionTmp/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TransactionTmp/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountName,AccountNumber")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: TransactionTmp/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: TransactionTmp/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountName,AccountNumber")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: TransactionTmp/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: TransactionTmp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
          return (_context.Transactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
