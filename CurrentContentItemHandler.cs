using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;

namespace NogginBox.Tagged
{
	public class CurrentContentItemHandler : ContentHandler
	{
		private readonly IWorkContextAccessor _workContextAccessor;

		public CurrentContentItemHandler(IWorkContextAccessor workContextAccessor)
		{
			_workContextAccessor = workContextAccessor;
		}

		protected override void BuildDisplayShape(BuildDisplayContext context)
		{
			if (context.DisplayType != "Detail") return;

			var workContext = _workContextAccessor.GetContext();
			var contentItems = workContext.GetState<List<IContent>>("ContentItems");
			if (contentItems == null)
			{
				workContext.SetState("ContentItems", contentItems = new List<IContent>());
			}

			contentItems.Add(context.ContentItem);
		}
	}
}