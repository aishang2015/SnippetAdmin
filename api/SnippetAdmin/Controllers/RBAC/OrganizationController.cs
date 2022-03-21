using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attribute;
using SnippetAdmin.Core.Utils;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Common;
using SnippetAdmin.Models.RBAC.Organization;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [SnippetAdminAuthorize]
    [ApiExplorerSettings(GroupName = "v1")]
    public class OrganizationController : ControllerBase
    {
        private readonly SnippetAdminDbContext _dbContext;

        public OrganizationController(SnippetAdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 组织处理

        /// <summary>
        /// 获取组织机构详细信息
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(GetOrganizationOutputModel))]
        public async Task<CommonResult> GetOrganization([FromBody] IntIdInputModel inputModel,
            [FromServices] IMapper mapper)
        {
            var org = await _dbContext.Organizations.FindAsync(inputModel.Id);
            var result = mapper.Map<GetOrganizationOutputModel>(org);
            result.TypeName = _dbContext.OrganizationTypes.FirstOrDefault(t => t.Code == result.Type)?.Name;
            result.UpId = (from tree in _dbContext.OrganizationTrees
                           where tree.Descendant == inputModel.Id && tree.Length == 1
                           select tree).FirstOrDefault()?.Ancestor;
            return this.SuccessCommonResult(result);
        }

        /// <summary>
        /// 获取组织树信息
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(List<GetOrganizationTreeOutputModel>))]
        public async Task<CommonResult> GetOrganizationTree()
        {
            var orgs = await _dbContext.Organizations.ToListAsync();
            var orgTrees = await _dbContext.OrganizationTrees.ToListAsync();

            // 找到最上层,即只做为自己的子节点
            var topOrgKeys = from ot in orgTrees
                             group ot by ot.Descendant into otg
                             where otg.Count() == 1
                             select otg.Key;
            var topOrgs = (from org in orgs
                           where topOrgKeys.Contains(org.Id)
                           select org).ToList();

            // 递归生成树数据
            var result = MakeTreeData(orgs, orgTrees, topOrgs);
            return this.SuccessCommonResult(result);
        }

        /// <summary>
        /// 创建组织
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> CreateOrganization([FromBody] CreateOrganizationInputModel inputModel,
            [FromServices] IMapper mapper)
        {
            // 开启事务
            using var tran = await _dbContext.Database.BeginTransactionAsync();

            // 保存节点
            var newOrg = mapper.Map<Organization>(inputModel);
            newOrg.Code = GuidUtil.NewSequentialGuid().ToString("N");
            var entity = await _dbContext.Organizations.AddAsync(newOrg);
            await _dbContext.SaveChangesAsync();

            // 保存节点关系
            var treeData = _dbContext.OrganizationTrees.Where(t => t.Descendant == inputModel.UpId);
            foreach (var treeNode in treeData)
            {
                await _dbContext.OrganizationTrees.AddAsync(new OrganizationTree
                {
                    Ancestor = treeNode.Ancestor,
                    Descendant = entity.Entity.Id,
                    Length = treeNode.Length + 1
                });
            }
            await _dbContext.OrganizationTrees.AddAsync(new OrganizationTree
            {
                Ancestor = entity.Entity.Id,
                Descendant = entity.Entity.Id,
                Length = 0
            });
            await _dbContext.SaveChangesAsync();
            await tran.CommitAsync();
            return this.SuccessCommonResult(MessageConstant.ORGANIZATION_INFO_0001);
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> DeleteOrganization([FromBody] IntIdInputModel inputModel)
        {
            var organizations = from org in _dbContext.Organizations
                                join orgTree in _dbContext.OrganizationTrees on org.Id equals orgTree.Descendant
                                where orgTree.Ancestor == inputModel.Id
                                select org;
            _dbContext.Organizations.RemoveRange(organizations);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.ORGANIZATION_INFO_0002);
        }

        /// <summary>
        /// 修改组织
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> UpdateOrganization([FromBody] UpdateOrganizationInputModel inputModel,
            [FromServices] IMapper mapper)
        {
            // 判断树节点是否被移动
            var upNode = (from orgTree in _dbContext.OrganizationTrees
                          where orgTree.Descendant == inputModel.Id &&
                               orgTree.Length == 1
                          select orgTree).FirstOrDefault();

            // 父节点变化了
            if (upNode?.Ancestor != inputModel.UpId)
            {
                // 找到上一步祖先节点和后代节点之间的所有关系
                var treeNodes = from orgTree in _dbContext.OrganizationTrees
                                let parentNodeIds = (from parentTree in _dbContext.OrganizationTrees
                                                     where parentTree.Descendant == inputModel.Id && parentTree.Length > 0
                                                     select parentTree.Ancestor).ToList()
                                let childNodeIds = (from childTree in _dbContext.OrganizationTrees
                                                    where childTree.Ancestor == inputModel.Id
                                                    select childTree.Descendant).ToList()
                                where parentNodeIds.Contains(orgTree.Ancestor) && childNodeIds.Contains(orgTree.Descendant)
                                select orgTree;
                _dbContext.RemoveRange(treeNodes);

                // 当前文件夹的子树
                var childNodes = (from ft in _dbContext.OrganizationTrees
                                  where ft.Ancestor == inputModel.Id
                                  select ft).ToList();

                // 找到新父节点的全部祖先
                var newParents = (from ft in _dbContext.OrganizationTrees
                                  where ft.Descendant == inputModel.UpId
                                  select ft).ToList();

                // 笛卡尔积构造新的上级树和子树的关系
                foreach (var parent in newParents)
                {
                    foreach (var node in childNodes)
                    {
                        await _dbContext.OrganizationTrees.AddAsync(new OrganizationTree
                        {
                            Ancestor = parent.Ancestor,
                            Descendant = node.Descendant,
                            Length = parent.Length + node.Length + 1
                        });
                    }
                }
            }

            var organization = mapper.Map<Organization>(inputModel);
            _dbContext.Entry(organization).Property(o => o.Code).IsModified = false;
            _dbContext.Organizations.Update(organization);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.ORGANIZATION_INFO_0003);
        }

        #endregion

        #region 组织类型管理

        /// <summary>
        /// 查询组织类型列表
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(List<GetOrganizationTypesOutputModel>))]
        public CommonResult GetOrganizationTypes(
            [FromServices] IMapper mapper)
        {
            return this.SuccessCommonResult(
                mapper.Map<List<GetOrganizationTypesOutputModel>>(
                    _dbContext.OrganizationTypes.ToList()));
        }

        /// <summary>
        /// 添加或更新组织类型
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> AddOrUpdateOrganizationType(
            [FromBody] AddOrUpdateOrganizationTypeInputModel inputModel)
        {
            if (_dbContext.OrganizationTypes.Any(ot => ot.Name == inputModel.Name && ot.Id != inputModel.Id))
            {
                return this.FailCommonResult(MessageConstant.ORGANIZATION_ERROR_0008);
            }

            if (inputModel.Id != null)
            {
                var orgType = _dbContext.OrganizationTypes.Find(inputModel.Id);
                orgType.Name = inputModel.Name;
                _dbContext.OrganizationTypes.Update(orgType);
            }
            else
            {
                _dbContext.OrganizationTypes.Add(new OrganizationType()
                {
                    Name = inputModel.Name,
                    Code = GuidUtil.NewSequentialGuid().ToString("N")
                });
            }
            await _dbContext.SaveChangesAsync();

            return this.SuccessCommonResult(MessageConstant.ORGANIZATION_INFO_0005);
        }

        /// <summary>
        /// 删除组织类型
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> RemoveOrganizationType(
            RemoveOrganizationTypeInputModel inputModel)
        {
            var orgType = _dbContext.OrganizationTypes.Find(inputModel.Id);
            _dbContext.OrganizationTypes.Remove(orgType);
            var orgs = _dbContext.Organizations.Where(org => org.Type == orgType.Code).ToList();
            orgs.ForEach(org => org.Type = null);
            _dbContext.Organizations.UpdateRange(orgs);
            await _dbContext.SaveChangesAsync();
            return this.SuccessCommonResult(MessageConstant.ORGANIZATION_INFO_0002);
        }

        #endregion

        /// <summary>
        /// 生成树数据
        /// </summary>
        private List<GetOrganizationTreeOutputModel> MakeTreeData(
            List<Organization> orgs,
            List<OrganizationTree> orgTrees,
            List<Organization> childOrgs)
        {
            var outputModels = childOrgs.Select(o => new GetOrganizationTreeOutputModel
            {
                Title = o.Name,
                Key = o.Id,
                Icon = o.Icon,
                IconId = o.IconId,
                Children = new List<GetOrganizationTreeOutputModel>()
            }).ToList();

            // 循环每个节点，找到子节点
            foreach (var node in outputModels)
            {
                // 当前节点的子节点
                var childs = (from org in orgs
                              join orgTree in orgTrees on org.Id equals orgTree.Descendant
                              where orgTree.Length == 1 && orgTree.Ancestor == node.Key
                              select org).ToList();
                node.Children.AddRange(MakeTreeData(orgs, orgTrees, childs));
            }

            return outputModels;
        }
    }
}