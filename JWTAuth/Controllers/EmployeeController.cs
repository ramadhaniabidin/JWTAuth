using JWTAuth.Interface;
using JWTAuth.Models;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IEmployee _employee;
        public EmployeeController(IEmployee employee)
        {
            _employee = employee;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> Get()
        {
            return await Task.FromResult(_employee.GetEmployeeDetails());
        }
    }
}
