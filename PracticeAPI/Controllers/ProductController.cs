using Microsoft.AspNetCore.Mvc;
using Models.Entites;

namespace PracticeAPI.Controllers
{
    public class ProductController : BaseController<ProductController>
    {
        public ProductController ( PracticeDbContext context, ILogger<ProductController> logger, 
            IConfiguration config) : base(context, logger, config)
        {

        }

        [HttpGet]
        public IActionResult GetList()
        {
            var res = _context.Products;
            return Ok(res);
        }

        [HttpGet("{id:long}")]
        public IActionResult GetDetail(long id)
        {
            var res = _context.Products.Find(id);
            return Ok(res);
        }

        [HttpPost]
        public IActionResult Add([FromBody] Product model)
        {
            _context.Products.Add(model);
            var eff = _context.SaveChanges();
            return eff > 0 ? Ok("thanhcong") : BadRequest("loi");
        }

        [HttpPut]
        public IActionResult Edit([FromBody] Product model)
        {
            var data = _context.Products.Find(model.Id);
            if (data == null) return NotFound("No data found");

            data.Name = model.Name;
            data.ExpDate = model.ExpDate;
            data.Status = model.Status;
            data.Price = model.Price;
            data.Amount = model.Amount;
            data.Description = model.Description;
            data.UpdatedBy = "";
            data.UpdatedDate = DateTime.Now;

            _context.Products.Update(data);
            var eff = _context.SaveChanges();
            return eff > 0 ? Ok("thanhcong") : BadRequest("loi");
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] long id)
        {
            var data = _context.Products.Find(id);
            if (data == null) return NotFound("No data found");

            _context.Products.Remove(data);
            var eff = _context.SaveChanges();
            return eff > 0 ? Ok("thanhcong") : BadRequest("loi");
        }
    }
}
