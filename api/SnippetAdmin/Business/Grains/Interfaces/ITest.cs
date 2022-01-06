using Orleans;

namespace SnippetAdmin.Business.Grains.Interfaces
{
    public interface ITest : IGrainWithIntegerKey
    {
        Task Do();
    }
}
