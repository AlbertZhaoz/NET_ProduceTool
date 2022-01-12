using AlbertZhao.cn.DbContextExtension;
using AlbertZhao.cn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AlbertZhao.cn.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataManagerController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<DataManagerController> logger;

        public DataManagerController(IMemoryCache memoryCache, ILogger<DataManagerController> logger)
        {
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Student?>> GetStuByID(int id)
        {
            logger.LogInformation("开始查询数据....");

            //GetOrCreateAsync天然避免缓存穿透，里面会将空值作为一个有效值
            Student? stu = await memoryCache.GetOrCreateAsync("Student_" + id, async e =>
            {
                //避免缓存雪崩
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Random.Shared.Next(5, 10));

                //从数据库拿取数据
                using (var ctx = new AlbertDbContext())
                {
                    logger.LogInformation("从数据库查询数据....");
                    Student? stu = ctx.Students.Where(e=>e.ID == id).FirstOrDefault();
                    logger.LogInformation($"从数据库查询的结果是:{(stu == null ? "null" : stu)}");
                    return stu;
                } 
            });
            if(stu == null)
            {
                return NotFound("查询的学生不存在");
            }
            else
            {
                return stu;
            }
        }
        
    }
}
