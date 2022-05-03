﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlIngresoGasto.Data;
using ControlIngresoGasto.Models;

namespace ControlIngresoGasto.Controllers
{
    public class IngresoGastoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngresoGastoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IngresoGasto
        public async Task<IActionResult> Index(int? mes, int? year)
        {
            if(mes == null)
			{
                mes = DateTime.Now.Month;
			}
            if(year == null)
			{
                year = DateTime.Now.Year;
			}

            // Para poder utilizar las variables mes y year en la vista creamos un ViewData
            ViewData["mes"] = mes;
            ViewData["year"] = year;

            // Include(): Hace posible que podamos traer los datos de categoría.
            // Incluimos todo el modelo de categoría en IngresoGasto
            var applicationDbContext = _context.IngresoGasto.Include(i => i.Categoria)
                .Where(x => x.Fecha.Month == mes && x.Fecha.Year == year);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: IngresoGasto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresoGasto = await _context.IngresoGasto
                .Include(i => i.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingresoGasto == null)
            {
                return NotFound();
            }

            return View(ingresoGasto);
        }

        // GET: IngresoGasto/Create
        public IActionResult Create()
        {
            //! El ViewData contendrá un listado de las categorías y lo utilizaremos para referenciarlo en una vista.
            //! Agregaremos un filtro con Where para que traiga solo las categorías activas.
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.Where(x => x.Estado == true), "Id", "NombreCategoria");
            return View();
        }

        // POST: IngresoGasto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoriaId,Fecha,Valor")] IngresoGasto ingresoGasto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingresoGasto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "NombreCategoria", ingresoGasto.CategoriaId);
            return View(ingresoGasto);
        }

        // GET: IngresoGasto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var ingresoGasto = await _context.IngresoGasto.FindAsync(id);
            if (ingresoGasto == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "NombreCategoria", ingresoGasto.CategoriaId);
            return View(ingresoGasto);
        }

        // POST: IngresoGasto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoriaId,Fecha,Valor")] IngresoGasto ingresoGasto)
        {
            if (id != ingresoGasto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingresoGasto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngresoGastoExists(ingresoGasto.Id))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "NombreCategoria", ingresoGasto.CategoriaId);
            return View(ingresoGasto);
        }

        // GET: IngresoGasto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresoGasto = await _context.IngresoGasto
                .Include(i => i.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingresoGasto == null)
            {
                return NotFound();
            }

            return View(ingresoGasto);
        }

        // POST: IngresoGasto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingresoGasto = await _context.IngresoGasto.FindAsync(id);
            _context.IngresoGasto.Remove(ingresoGasto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngresoGastoExists(int id)
        {
            return _context.IngresoGasto.Any(e => e.Id == id);
        }
    }
}