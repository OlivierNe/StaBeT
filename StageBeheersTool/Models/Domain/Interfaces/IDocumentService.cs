
namespace StageBeheersTool.Models.Domain
{
    public interface IDocumentService
    {
        byte[] StagecontractTemplate { get; set; }
        byte[] GenerateStagecontract(Stage stage);
    }
}
