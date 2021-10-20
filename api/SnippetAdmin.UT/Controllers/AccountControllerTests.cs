using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SnippetAdmin.Constants;
using SnippetAdmin.Controllers;
using SnippetAdmin.Core.Authentication;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Account;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SnippetAdmin.UT.Controllers
{
    public class AccountControllerTests
    {
        private Mock<UserManager<SnippetAdminUser>> _mockUserManager;

        private Mock<IJwtFactory> _mockJwtFactory;

        private Mock<IOptions<JwtOption>> _mockJwtOption;

        public AccountControllerTests()
        {
            _mockUserManager = new Mock<UserManager<SnippetAdminUser>>(
                   new Mock<IUserStore<SnippetAdminUser>>().Object,
                     new Mock<IOptions<IdentityOptions>>().Object,
                     new Mock<IPasswordHasher<SnippetAdminUser>>().Object,
                     new IUserValidator<SnippetAdminUser>[0],
                     new IPasswordValidator<SnippetAdminUser>[0],
                     new Mock<ILookupNormalizer>().Object,
                     new Mock<IdentityErrorDescriber>().Object,
                     new Mock<IServiceProvider>().Object,
                     new Mock<ILogger<UserManager<SnippetAdminUser>>>().Object);

            _mockJwtFactory = new Mock<IJwtFactory>();

            _mockJwtOption = new Mock<IOptions<JwtOption>>();
        }

        [Fact]
        public async Task LoginTest001()
        {
            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(TestUser1());

            _mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<SnippetAdminUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockJwtFactory.Setup(j => j.GenerateJwtToken(It.IsAny<string>()))
                .Returns("token");

            _mockJwtOption.Setup(o => o.Value)
                .Returns(new JwtOption { ExpireSpan = 20 });

            var controller = new AccountController(_mockUserManager.Object, null, _mockJwtFactory.Object,
                null, null, _mockJwtOption.Object, null, null);

            var result = (CommonResult<LoginOutputModel>)
                await controller.Login(new LoginInputModel("TestUser1", "TestUser1"));

            // asserts
            Assert.True(result.IsSuccess);
            Assert.Equal(MessageConstant.ACCOUNT_INFO_0001.Item1, result.Code);
            Assert.Equal(MessageConstant.ACCOUNT_INFO_0001.Item2, result.Message);
            Assert.Equal("TestUser1", result.Data.UserName);
            Assert.Equal("token", result.Data.AccessToken);
        }

        [Fact]
        public async Task LoginTest002()
        {
            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(TestUser1());

            _mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<SnippetAdminUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var controller = new AccountController(_mockUserManager.Object, null, _mockJwtFactory.Object,
                null, null, _mockJwtOption.Object, null, null);

            var result = await controller.Login(new LoginInputModel("TestUser1", "TestUser1"));

            // asserts
            Assert.IsType<CommonResult>(result);
            Assert.True(!result.IsSuccess);
            Assert.Equal(MessageConstant.ACCOUNT_ERROR_0001.Item1, result.Code);
            Assert.Equal(MessageConstant.ACCOUNT_ERROR_0001.Item2, result.Message);
        }

        [Fact]
        public async Task LoginTest003()
        {
            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var controller = new AccountController(_mockUserManager.Object, null, _mockJwtFactory.Object,
                null, null, _mockJwtOption.Object, null, null);

            var result = await controller.Login(new LoginInputModel("TestUser1", "TestUser1"));

            // asserts
            Assert.IsType<CommonResult>(result);
            Assert.True(!result.IsSuccess);
            Assert.Equal(MessageConstant.ACCOUNT_ERROR_0001.Item1, result.Code);
            Assert.Equal(MessageConstant.ACCOUNT_ERROR_0001.Item2, result.Message);
        }

        private SnippetAdminUser TestUser1() =>
            new SnippetAdminUser
            {
                UserName = "TestUser1"
            };
    }
}