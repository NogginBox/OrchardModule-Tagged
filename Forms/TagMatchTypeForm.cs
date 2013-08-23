using System;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Tags.Projections;

namespace NogginBox.Tagged.Forms
{
	public class TagMatchTypeForm : IFormProvider
	{
		protected dynamic Shape { get; set; }
		public Localizer T { get; set; }

		public TagMatchTypeForm(IShapeFactory shapeFactory)
		{
			Shape = shapeFactory;
			T = NullLocalizer.Instance;
		}

		public const String FormId = "TagMatchTypeForm";

		public void Describe(dynamic context)
		{
			Func<IShapeFactory, dynamic> form =
				shape => {

					var f = Shape.Form(
						Id: FormId,
						_Exclusion: Shape.FieldSet(
							Title: T("Tags in querystring match"),
							_OperatorOneOf: Shape.Radio(
								Id: "operator-is-one-of", Name: "Operator",
								Title: T("one tag in content"), Value: "0", Checked: true
								),
							_OperatorIsAllOf: Shape.Radio(
								Id: "operator-is-all-of", Name: "Operator",
								Title: T("all tags in content"), Value: "1"
								)
							));

					return f;
				};

			context.Form(FormId, form);

		}
	}
}