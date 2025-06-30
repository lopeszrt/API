using Microsoft.AspNetCore.Mvc;

namespace API.Structure
{
    public interface IController<in T>
    {
        Task<IActionResult> Get();

        Task<IActionResult> GetById(int id);

        Task<IActionResult> GetByForeignId(int foreignId);

        Task<IActionResult> Update(int id,[FromBody] T item);

        Task<IActionResult> Add([FromBody] T item);

        Task<IActionResult> Delete(int id);
    }
}
