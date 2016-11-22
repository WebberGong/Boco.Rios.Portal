using System.ComponentModel.Composition;
using Boco.Rios.Portal.HomePage.Data;
using Boco.Rios.Portal.HomePage.DomainModel;

namespace Boco.Rios.Portal.HomePage.Service
{
    [Export("Service", typeof (ServiceStack.ServiceInterface.Service))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HomePageService : ServiceStack.ServiceInterface.Service
    {
        public CellResponse Post(CellRequest request)
        {
            return new CellResponse
            {
                Data = CellDomainModel.Instance.GetCellsByCondition(request.Name, request.Lac, request.Ci)
            };
        }

        public CellPagingResponse Post(CellPagingRequest request)
        {
            return new CellPagingResponse
            {
                Data =
                    CellDomainModel.Instance.GetCellsByConditionForPaging(request.Name, request.Lac, request.Ci,
                        request.Page, request.PageSize),
                TotalCount = CellDomainModel.Instance.GetCellsByConditionForCount(request.Name, request.Lac, request.Ci)
            };
        }

        public CellResponse Get(CellRequest request)
        {
            return new CellResponse
            {
                Data = CellDomainModel.Instance.GetCellsByCondition(request.Name, request.Lac, request.Ci)
            };
        }

        public CellPagingResponse Get(CellPagingRequest request)
        {
            return new CellPagingResponse
            {
                Data =
                    CellDomainModel.Instance.GetCellsByConditionForPaging(request.Name, request.Lac, request.Ci,
                        request.Page, request.PageSize),
                TotalCount = CellDomainModel.Instance.GetCellsByConditionForCount(request.Name, request.Lac, request.Ci)
            };
        }

        public ItemResponse Post(ItemRequest request)
        {
            return new ItemResponse {Data = ItemDomainModel.Instance.GetItems()};
        }

        public ItemResponse Get(ItemRequest request)
        {
            return new ItemResponse {Data = ItemDomainModel.Instance.GetItems()};
        }

        public TreeNodeResponse Post(TreeNodeRequest request)
        {
            return new TreeNodeResponse {Data = TreeNodeDomainModel.Instance.GetTreeNodes()};
        }

        public TreeNodeResponse Get(TreeNodeRequest request)
        {
            return new TreeNodeResponse {Data = TreeNodeDomainModel.Instance.GetTreeNodes()};
        }
    }
}