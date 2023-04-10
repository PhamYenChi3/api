using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Request;
using Models.Models.Response;
using Models.Entites;
using System.Net.Cache;
using Common;

namespace PracticeAPI.Controllers
{
    public class CustomerController : BaseController<CustomerController>
    {
        public CustomerController(PracticeDbContext context, ILogger<CustomerController> logger,
            IConfiguration config) : base(context, logger, config)
        {

        }

        [HttpGet]
        public IActionResult GetList()
        {
            var res = _context.Customers.Select(m
                => new CustomerListModel()
                {
                    Address = m.Address,
                    Age = m.Age,
                    Gender = m.Gender,
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Status = m.Status,
                    Username = m.Username
                });
            return Ok(res);
        }

        [HttpGet("{id:long}")]
        public IActionResult GetDetail(long id)
        {
            var customer = _context.Customers.Find(id);
            var products = (from o in _context.Orders
                       join pr in _context.Products on o.ProductId equals pr.Id
                       where o.CustomerId == id
                       select new Product()
                       {
                           Id = pr.Id,
                           Name = pr.Name,
                           Amount = o.Amount,
                           Price = o.Price,
                           Description = pr.Description,
                           Status = pr.Status,
                       });
            return Ok(new CustomerDetailModel()
            {
                Id = customer.Id,
                Name = customer.Name,
                Age = customer.Age,
                Gender = customer.Gender,
                Address = customer.Address,
                Username = customer.Username,
                Description = customer.Description,
                Status = customer.Status,
                Products = products.ToList()
            }) ;
        }

        [HttpPost]
        public IActionResult Add([FromBody] CustomerListModel model)
        {
            var data = new Customer()
            {
                Address = model.Address,
                Age = model.Age,
                Gender = model.Gender,
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Status = model.Status,
                Username = model.Username
            };
            _context.Customers.Add(data);
            var eff = _context.SaveChanges();
            return eff > 0 ? Ok("thanhcong") : BadRequest("loi");
        }

        [HttpPut]
        public IActionResult Edit([FromBody] CustomerListModel model)
        {
            var data = _context.Customers.Find(model.Id);
            if (data == null) return NotFound("No data found");

            data.Name = model.Name;
            data.Age = model.Age;
            data.Address = model.Address;
            data.Gender = model.Gender;
            data.Status = model.Status;
            data.Description = model.Description;
            data.UpdatedBy = "";
            data.UpdatedDate = DateTime.Now;

            _context.Customers.Update(data);
            var eff = _context.SaveChanges();
            return eff > 0 ? Ok("thanhcong") : BadRequest("loi");
        }

        [HttpPut]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            var data = _context.Customers.Find(model.CustomerId);
            if (data == null) return NotFound("No data found");

            data.Salt = data.Salt ?? Guid.NewGuid().ToString();
            var passHash = data.Username.ComputeSha256Hash(data.Salt, model.Password);
            data.Password = passHash;
            data.UpdatedBy = "";
            data.UpdatedDate = DateTime.Now;

            _context.Customers.Update(data);
            var eff = _context.SaveChanges();
            return eff > 0 ? Ok("thanhcong") : BadRequest("loi");
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] long id)
        {
            var data = _context.Customers.Find(id);
            if (data == null) return NotFound("No data found");

            _context.Customers.Remove(data);
            var eff = _context.SaveChanges();
            return eff > 0 ? Ok("thanhcong") : BadRequest("loi");
        }

        [HttpPost]
        public IActionResult Order(OrderModel model)
        {
            var dataProduct = _context.Products.Find(model.ProductId);
            if (dataProduct == null) return NotFound("No data found");

            var dataCustomer = _context.Customers.Find(model.CustomerId);
            if (dataCustomer == null) return NotFound("No data found");

            var data = _context.Orders.Find(model.ProductId, model.CustomerId);
            if (data != null) return NotFound("No data found");

            var order = new Order
            {
                ProductId = model.ProductId,
                CustomerId = model.CustomerId,
                Amount = model.Amount,
                Price = model.Price
            };
            _context.Orders.Add(order);
            var eff = _context.SaveChanges();
            return eff > 0 ? Ok("thanhcong") : BadRequest("loi");
        }
    }
}
