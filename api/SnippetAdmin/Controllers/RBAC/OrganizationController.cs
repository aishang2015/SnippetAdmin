using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Endpoint.Models.RBAC.Organization;
using System.ComponentModel;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "AccessApi")]
    //[SnippetAdminAuthorize]
    [ApiExplorerSettings(GroupName = "v1")]
    public class OrganizationController : ControllerBase, IOrganizationApi
    {
        private readonly SnippetAdminDbContext _dbContext;

        private readonly IMapper _mapper;

        public OrganizationController(SnippetAdminDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #region 组织处理

        /// <summary>
        /// 获取组织机构详细信息
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<GetOrganizationOutputModel>]
        public async Task<CommonResult<GetOrganizationOutputModel>> GetOrganization([FromBody] IdInputModel<int> inputModel)
        {
            var org = await _dbContext.RbacOrganizations.FindAsync(inputModel.Id);
            var result = _mapper.Map<GetOrganizationOutputModel>(org);
            result.TypeName = _dbContext.RbacOrganizationTypes.FirstOrDefault(t => t.Code == result.Type)?.Name;
            result.UpId = (from tree in _dbContext.RbacOrganizationTrees
                           where tree.Descendant == inputModel.Id && tree.Length == 1
                           select tree).FirstOrDefault()?.Ancestor;
            return CommonResult.Success(result);
        }

        /// <summary>
        /// 获取组织树信息
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<List<GetOrganizationTreeOutputModel>>]
        [Description("获取组织树信息")]
        public async Task<CommonResult<List<GetOrganizationTreeOutputModel>>> GetOrganizationTree()
        {
            var orgs = await _dbContext.RbacOrganizations.OrderBy(org => org.Sorting).ToListAsync();
            var orgTrees = await _dbContext.RbacOrganizationTrees.ToListAsync();

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
            return CommonResult.Success(result);
        }

        /// <summary>
        /// 创建组织
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> CreateOrganization([FromBody] CreateOrganizationInputModel inputModel)
        {
            // 组织编码重复
            if (_dbContext.RbacOrganizations.Any(r => r.Code == inputModel.Code))
            {
                return CommonResult.Fail(MessageConstant.ORGANIZATION_ERROR_0004);
            }

            // 开启事务
            using var tran = await _dbContext.Database.BeginTransactionAsync();

            // 保存节点
            var newOrg = _mapper.Map<RbacOrganization>(inputModel);
            var entity = await _dbContext.RbacOrganizations.AddAsync(newOrg);
            await _dbContext.SaveChangesAsync();

            // 保存节点关系
            var treeData = _dbContext.RbacOrganizationTrees.Where(t => t.Descendant == inputModel.UpId);
            foreach (var treeNode in treeData)
            {
                await _dbContext.RbacOrganizationTrees.AddAsync(new RbacOrganizationTree
                {
                    Ancestor = treeNode.Ancestor,
                    Descendant = entity.Entity.Id,
                    Length = treeNode.Length + 1
                });
            }
            await _dbContext.RbacOrganizationTrees.AddAsync(new RbacOrganizationTree
            {
                Ancestor = entity.Entity.Id,
                Descendant = entity.Entity.Id,
                Length = 0
            });
            await _dbContext.SaveChangesAsync();
            await tran.CommitAsync();
            return CommonResult.Success(MessageConstant.ORGANIZATION_INFO_0001);
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> DeleteOrganization([FromBody] IdInputModel<int> inputModel)
        {
            var organizations = from org in _dbContext.RbacOrganizations
                                join orgTree in _dbContext.RbacOrganizationTrees on org.Id equals orgTree.Descendant
                                where orgTree.Ancestor == inputModel.Id
                                select org;
            _dbContext.RbacOrganizations.RemoveRange(organizations);
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.ORGANIZATION_INFO_0002);
        }

        /// <summary>
        /// 修改组织
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> UpdateOrganization([FromBody] UpdateOrganizationInputModel inputModel)
        {
            // 组织编码重复
            if (_dbContext.RbacOrganizations.Any(r => r.Id != inputModel.Id && r.Code == inputModel.Code))
            {
                return CommonResult.Fail(MessageConstant.ORGANIZATION_ERROR_0004);
            }

            // 组织不能移入自己树下的组织
            var isChildNode = from orgTree in _dbContext.RbacOrganizationTrees
                              where orgTree.Ancestor == inputModel.Id && orgTree.Descendant == inputModel.UpId
                              select orgTree;
            if (isChildNode.Any())
            {
                return CommonResult.Fail(MessageConstant.ORGANIZATION_ERROR_0009);
            }

            // 更新组织节点
            var organization = _mapper.Map<RbacOrganization>(inputModel);
            _dbContext.RbacOrganizations.Update(organization);

            // 判断树节点是否被移动
            var upNode = (from orgTree in _dbContext.RbacOrganizationTrees
                          where orgTree.Descendant == inputModel.Id &&
                               orgTree.Length == 1
                          select orgTree).FirstOrDefault();

            // 父节点变化了
            if (upNode?.Ancestor != inputModel.UpId)
            {
                // 找到上一步祖先节点和后代节点之间的所有关系
                var treeNodes = from orgTree in _dbContext.RbacOrganizationTrees
                                let parentNodeIds = (from parentTree in _dbContext.RbacOrganizationTrees
                                                     where parentTree.Descendant == inputModel.Id && parentTree.Length > 0
                                                     select parentTree.Ancestor).ToList()
                                let childNodeIds = (from childTree in _dbContext.RbacOrganizationTrees
                                                    where childTree.Ancestor == inputModel.Id
                                                    select childTree.Descendant).ToList()
                                where parentNodeIds.Contains(orgTree.Ancestor) && childNodeIds.Contains(orgTree.Descendant)
                                select orgTree;
                _dbContext.RemoveRange(treeNodes);

                // 当前文件夹的子树
                var childNodes = (from ft in _dbContext.RbacOrganizationTrees
                                  where ft.Ancestor == inputModel.Id
                                  select ft).ToList();

                // 找到新父节点的全部祖先
                var newParents = (from ft in _dbContext.RbacOrganizationTrees
                                  where ft.Descendant == inputModel.UpId
                                  select ft).ToList();

                // 笛卡尔积构造新的上级树和子树的关系
                foreach (var parent in newParents)
                {
                    foreach (var node in childNodes)
                    {
                        await _dbContext.RbacOrganizationTrees.AddAsync(new RbacOrganizationTree
                        {
                            Ancestor = parent.Ancestor,
                            Descendant = node.Descendant,
                            Length = parent.Length + node.Length + 1
                        });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.ORGANIZATION_INFO_0003);
        }

        #endregion

        #region 组织类型管理

        /// <summary>
        /// 查询组织类型列表
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<List<GetOrganizationTypesOutputModel>>]
        public async Task<CommonResult<List<GetOrganizationTypesOutputModel>>> GetOrganizationTypes()
        {
            var result = await _dbContext.RbacOrganizationTypes.ToListAsync();
            return CommonResult.Success(
                _mapper.Map<List<GetOrganizationTypesOutputModel>>(result));
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
            if (_dbContext.RbacOrganizationTypes.Any(ot => ot.Code == inputModel.Code && ot.Id != inputModel.Id))
            {
                return CommonResult.Fail(MessageConstant.ORGANIZATION_ERROR_0007);
            }

            if (_dbContext.RbacOrganizationTypes.Any(ot => ot.Name == inputModel.Name && ot.Id != inputModel.Id))
            {
                return CommonResult.Fail(MessageConstant.ORGANIZATION_ERROR_0008);
            }

            if (inputModel.Id != null)
            {
                var orgType = _dbContext.RbacOrganizationTypes.Find(inputModel.Id);
                orgType.Name = inputModel.Name;
                orgType.Code = inputModel.Code;
                _dbContext.RbacOrganizationTypes.Update(orgType);
            }
            else
            {
                _dbContext.RbacOrganizationTypes.Add(new RbacOrganizationType()
                {
                    Name = inputModel.Name,
                    Code = inputModel.Code
                });
            }
            await _dbContext.SaveChangesAsync();

            return CommonResult.Success(MessageConstant.ORGANIZATION_INFO_0005);
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
            var orgType = _dbContext.RbacOrganizationTypes.Find(inputModel.Id);
            _dbContext.RbacOrganizationTypes.Remove(orgType);
            var orgs = _dbContext.RbacOrganizations.Where(org => org.Type == orgType.Code).ToList();
            orgs.ForEach(org => org.Type = null);
            _dbContext.RbacOrganizations.UpdateRange(orgs);
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.ORGANIZATION_INFO_0002);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 生成树数据
        /// </summary>
        private List<GetOrganizationTreeOutputModel> MakeTreeData(
            List<RbacOrganization> orgs,
            List<RbacOrganizationTree> orgTrees,
            List<RbacOrganization> childOrgs)
        {
            var outputModels = childOrgs.Select(o => new GetOrganizationTreeOutputModel
            {
                Title = o.Name,
                Value = o.Id,
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

        #endregion
    }
}