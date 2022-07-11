using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.RBAC.Element;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "AccessApi")]
    //[SnippetAdminAuthorize]
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
        public async Task<CommonResult> GetElement([FromBody] IdInputModel<int> inputModel,
            [FromServices] IMapper mapper)
        {
            var element = await _dbContext.RbacElements.FindAsync(inputModel.Id);
            return this.SuccessCommonResult(mapper.Map<GetElementOutputModel>(element));
        }

        /// <summary>
        /// 获取元素树信息
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(List<GetElementTreeOutputModel>))]
        public async Task<CommonResult> GetElementTree()
        {
            var elements = await _dbContext.RbacElements.OrderBy(e => e.Sorting).ToListAsync();
            var elementTrees = await _dbContext.RbacElementTrees.ToListAsync();

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
            var entity = await _dbContext.RbacElements.AddAsync(mapper.Map<RbacElement>(inputModel));
            await _dbContext.SaveChangesAsync();

            // 保存节点关系
            var treeData = _dbContext.RbacElementTrees.Where(t => t.Descendant == inputModel.UpId);
            foreach (var treeNode in treeData)
            {
                await _dbContext.RbacElementTrees.AddAsync(new RbacElementTree
                {
                    Ancestor = treeNode.Ancestor,
                    Descendant = entity.Entity.Id,
                    Length = treeNode.Length + 1
                });
            }
            await _dbContext.RbacElementTrees.AddAsync(new RbacElementTree
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
        public async Task<CommonResult> DeleteElement([FromBody] IdInputModel<int> inputModel)
        {
            var elements = from e in _dbContext.RbacElements
                           join et in _dbContext.RbacElementTrees on e.Id equals et.Descendant
                           where et.Ancestor == inputModel.Id
                           select e;
            _dbContext.RbacElements.RemoveRange(elements);
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
            var element = mapper.Map<RbacElement>(inputModel);
            _dbContext.RbacElements.Update(element);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.ELEMENT_INFO_0003);
        }

        private List<GetElementTreeOutputModel> MakeTreeData(List<RbacElement> elements, List<RbacElementTree> elementTrees,
            List<RbacElement> childElements)
        {
            var outputModels = childElements.Select(e => new GetElementTreeOutputModel
            {
                Title = e.Name,
                Key = e.Id,
                Value = e.Id,
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