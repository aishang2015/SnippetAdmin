using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.RBAC.Element;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [SnippetAdminAuthorize]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ElementController : ControllerBase
    {
        private readonly SnippetAdminDbContext _dbContext;

        public ElementController(SnippetAdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取元素详细信息
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(GetElementOutputModel))]
        public async Task<CommonResult> GetElement([FromBody] IntIdInputModel inputModel,
            [FromServices] IMapper mapper)
        {
            var element = await _dbContext.Elements.FindAsync(inputModel.Id);
            return this.SuccessCommonResult(mapper.Map<GetElementOutputModel>(element));
        }

        /// <summary>
        /// 获取元素树信息
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(List<GetElementTreeOutputModel>))]
        public async Task<CommonResult> GetElementTree()
        {
            var elements = await _dbContext.Elements.ToListAsync();
            var elementTrees = await _dbContext.ElementTrees.ToListAsync();

            // 找到最上层,即只做为自己的子节点
            var topElementKeys = from et in elementTrees
                                 group et by et.Descendant into etg
                                 where etg.Count() == 1
                                 select etg.Key;
            var topElements = (from e in elements
                               where topElementKeys.Contains(e.Id)
                               select e).ToList();

            // 递归生成树数据
            var result = MakeTreeData(elements, elementTrees, topElements);
            return this.SuccessCommonResult(result);
        }

        /// <summary>
        /// 创建页面元素
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> CreateElement([FromBody] CreateElementInputModel inputModel,
            [FromServices] IMapper mapper)
        {
            // 开启事务
            using var tran = await _dbContext.Database.BeginTransactionAsync();

            // 保存节点
            var entity = await _dbContext.Elements.AddAsync(mapper.Map<Element>(inputModel));
            await _dbContext.SaveChangesAsync();

            // 保存节点关系
            var treeData = _dbContext.ElementTrees.Where(t => t.Descendant == inputModel.UpId);
            foreach (var treeNode in treeData)
            {
                await _dbContext.ElementTrees.AddAsync(new ElementTree
                {
                    Ancestor = treeNode.Ancestor,
                    Descendant = entity.Entity.Id,
                    Length = treeNode.Length + 1
                });
            }
            await _dbContext.ElementTrees.AddAsync(new ElementTree
            {
                Ancestor = entity.Entity.Id,
                Descendant = entity.Entity.Id,
                Length = 0
            });
            await _dbContext.SaveChangesAsync();
            await tran.CommitAsync();
            return this.SuccessCommonResult(MessageConstant.ELEMENT_INFO_0001);
        }

        /// <summary>
        /// 删除页面元素
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> DeleteElement([FromBody] IntIdInputModel inputModel)
        {
            var elements = from e in _dbContext.Elements
                           join et in _dbContext.ElementTrees on e.Id equals et.Descendant
                           where et.Ancestor == inputModel.Id
                           select e;
            _dbContext.Elements.RemoveRange(elements);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.ELEMENT_INFO_0002);
        }

        /// <summary>
        /// 修改页面元素
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> UpdateElement([FromBody] UpdateElementInputModel inputModel,
            [FromServices] IMapper mapper)
        {
            var element = mapper.Map<Element>(inputModel);
            _dbContext.Elements.Update(element);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.ELEMENT_INFO_0003);
        }

        /// <summary>
        /// 导出元素数据
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<IActionResult> ExportElementData()
        {
            var elements = await _dbContext.Elements.ToListAsync();
            var elementTrees = await _dbContext.ElementTrees.ToListAsync();

            var ms = new MemoryStream(2048);
            var sw = new StreamWriter(ms)
            {
                AutoFlush = true
            };

            sw.WriteLine("// 元素数据");
            foreach (var e in elements)
            {
                sw.WriteLine($"_dbContext.Elements.Add(new Element {{ Id = {e.Id}, Name = \"{e.Name}\", Identity = \"{e.Identity}\", Type = ElementType.{e.Type}, AccessApi = \"{e.AccessApi}\" }});");
            }

            sw.WriteLine();
            sw.WriteLine("// 元素树数据");
            foreach (var e in elementTrees)
            {
                sw.WriteLine($"_dbContext.ElementTrees.Add(new ElementTree {{ Id = {e.Id}, Ancestor = {e.Ancestor}, Descendant = {e.Descendant} ,Length = {e.Length} }});");
            }
            sw.WriteLine("await _dbContext.SaveChangesAsync();");

            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "text/plain", "element.txt");
        }

        private List<GetElementTreeOutputModel> MakeTreeData(List<Element> elements, List<ElementTree> elementTrees,
            List<Element> childElements)
        {
            var outputModels = childElements.Select(e => new GetElementTreeOutputModel
            {
                Title = e.Name,
                Key = e.Id,
                Type = (int)e.Type,
                Children = new List<GetElementTreeOutputModel>()
            }).ToList();

            // 循环每个节点，找到子节点
            foreach (var node in outputModels)
            {
                // 当前节点的子节点
                var childs = (from e in elements
                              join et in elementTrees on e.Id equals et.Descendant
                              where et.Length == 1 && et.Ancestor == node.Key
                              select e).ToList();
                node.Children.AddRange(MakeTreeData(elements, elementTrees, childs));
            }

            return outputModels;
        }
    }
}