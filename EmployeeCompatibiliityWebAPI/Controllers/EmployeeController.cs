using Microsoft.AspNetCore.Mvc;

namespace EmployeeCompatibilityWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("FindLongestWorkingPair")]
        public async Task<IActionResult> FindLongestWorkingPair(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            string content;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                content = await reader.ReadToEndAsync();
            }

            try
            {
                var result = _employeeService.FindLongestWorkingPair(content);

                if (result != null)
                {
                    return Ok(new { Message = $"The pair of employees who have worked together the longest is: {result.Value.Item1}, {result.Value.Item2} with {result.Value.Item3} days." });
                }
                else
                {
                    return NotFound("No pairs found");
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
