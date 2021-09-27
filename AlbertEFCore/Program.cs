using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AlbertEFCore
{
    internal class Program
    {
        /// <summary>
        /// <see cref="RemoveAllLine(AlbertDbContext)"/>
        /// <see cref="InitDataBase(DbContext)"/>
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>If you use migration for the first time,
        /// you must run command like:add-migration notes and update-database.
        /// </remarks>
        /// <returns></returns>
        static async Task Main(string[] args)
        {          
            using (var ctx = new AlbertDbContext())
            {
                await RemoveAllLine(ctx);
                await InitDataBase(ctx);
            }        
        }

        /// <summary>
        /// 移除配置信息数据库下的ProduceToolConfig表中的所有内容
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        static async Task RemoveAllLine(AlbertDbContext dbCtx)
        {
            var produceToolEntities = dbCtx.ProduceToolEntity;
            foreach (var item in produceToolEntities)
            {
                dbCtx.Remove(item);
            }
            await dbCtx.SaveChangesAsync();
        }

        /// <summary>
        /// 根据Json回读的数据，将Json的Key和Value写入数据库ProduceToolConfig表中
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        static async Task InitDataBase(DbContext ctx)
        {
            var jsonHelper = new JsonHelper("Configs\\ProduceTool.json");
            foreach (var item in jsonHelper.ReadJsonSub())
            {
                var produceToolEntity = new ProduceToolEntity()
                {
                    Name = item.Key,
                    Value = item.Value.ToString(),
                };
                ctx.Add(produceToolEntity);
            }
            await ctx.SaveChangesAsync();
        }
    }
}
