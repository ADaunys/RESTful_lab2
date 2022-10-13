namespace Server;

using Microsoft.AspNetCore.Mvc;


/// <summary>
/// Service. Class must be marked public, otherwise ASP.NET core runtime will not find it.
/// </summary>
[Route("/service")]
[ApiController]
public class ServiceController : ControllerBase
{
    /// <summary>
    /// Service logic. Use a singleton instance, since controller is instance-per-request.
    /// </summary>
    private static readonly ServiceLogic logic = new ServiceLogic();

    /// <summary>
    /// Add given numbers.
    /// </summary>
    /// <param name="left">Left number.</param>
    /// <param name="right">Right number.</param>
    /// <returns>left + right</returns>
    [HttpGet]
    [Route("AddLiteral/{left}")]
    public ActionResult<int> AddLiteral([FromRoute] int left, [FromQuery] int right)
    {
        lock (logic)
        {
            return logic.AddLiteral(left, right);
        }
    }
}