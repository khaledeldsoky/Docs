using Employees.Models;
using Employees.Services;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Controllers
{
  [ApiController]
  [Route("api/employees")]
  public class EmployeeContollers : ControllerBase
  {
    readonly private EmployeeServices Services = new EmployeeServices();

    [HttpGet]
    public IActionResult Get()
    {
      var employee = Services.GetAll();
      return Ok(employee);
    }

    [HttpGet("id/{id}")]
    public IActionResult GetEmployeeById(int id)
    {
      var employee = Services.GetById(id);

      if (employee == null)
        return NotFound("Employee not found");
      return Ok(employee);
    }

    [HttpPost("id/{id}")]
    public IActionResult DeletEmployee(int id)
    {
      bool deleted = Services.Delete(id);

      if (!deleted)
        return NotFound("Employee not found");

      return Ok("Employee Delete Success");
    }

    [HttpPost]
    public IActionResult AddEmployee([FromBody] Employee employee)
    {
      Services.Add(employee);
      return Ok("Employee Add Success");
    }
  
  }
}