﻿using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using WebApplication.Models;
using System.Linq;

namespace WebApplicationTest.Controllers
{
    [Route("statistics/sales")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public StatisticsController(DatabaseContext context) => _context = context;

        [HttpGet("products/{id}")]
        public IActionResult GetProductStatistics(int id)
        {
            Console.WriteLine($"🟢 Получен запрос на статистику по продукту ID={id}");

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                Console.WriteLine($"🔴 Продукт {id} не найден!");
                return NotFound("Продукт не найден.");
            }

            var orders = _context.OrderItems
                .Where(oi => oi.ProductId == id)
                .Join(_context.Orders,
                      oi => oi.OrderId,
                      o => o.Id,
                      (oi, o) => new
                      {
                          OrderId = oi.OrderId,
                          Count = oi.Quantity,
                          Summ = oi.Quantity * oi.Price,
                          UserName = _context.Customers
                              .Where(c => c.Id == o.CustomerId)
                              .Select(c => c.Name)
                              .FirstOrDefault() ?? "[No name]"
                      })
                .ToList();

            Console.WriteLine($"✅ Найдено заказов: {orders.Count}");

            return Ok(new
            {
                LeftCount = product.Quantity,
                Orders = orders
            });
        }
    }
}
