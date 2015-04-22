
namespace StageBeheersTool.Models.Domain
{
    public interface IDocumentService
    {
        byte[] GenerateStagecontract(Stage stage);
    }
}
