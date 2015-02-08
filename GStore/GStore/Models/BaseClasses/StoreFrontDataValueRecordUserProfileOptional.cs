using GStore.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models.BaseClasses
{
	public abstract class StoreFrontDataValueRecordUserProfileOptional: StoreFrontRecordUserProfileOptional
	{
		[Required]
		[Display(Name = "Value Data Type")]
		public GStoreValueDataType DataType { get; set; }

		[MaxLength(50)]
		[Display(Name = "Value Data Type String")]
		public string DataTypeString { get; set; }

		[Display(Name = "Value 1 String")]
		public string Value1String { get; set; }

		[Display(Name = "Value 2 String")]
		public string Value2String { get; set; }

		[Display(Name = "Value 1 Integer")]
		public int? Value1Int { get; set; }

		[Display(Name = "Value 2 Integer")]
		public int? Value2Int { get; set; }

		[Display(Name = "Value 1 Boolean")]
		public bool? Value1Bool { get; set; }

		[Display(Name = "Value 2 Boolean")]
		public bool? Value2Bool { get; set; }

		[Display(Name = "Value 1 Decimal")]
		public decimal? Value1Decimal { get; set; }

		[Display(Name = "Value 2 Decimal")]
		public decimal? Value2Decimal { get; set; }

		[Display(Name = "Value 1 Value List Item Id")]
		public int? Value1ValueListItemId { get; set; }

		[Display(Name = "Value 1 Value List Item Name")]
		public string Value1ValueListItemName { get; set; }

		[Display(Name = "Value 1 Value List Item Id List")]
		public string Value1ValueListItemIdList { get; set; }

		[Display(Name = "Value 1 Value List Item Name List")]
		public string Value1ValueListItemNameList { get; set; }

		[ForeignKey("Value1ValueListItemId")]
		[Display(Name = "Value 1 Value List Item")]
		public virtual ValueListItem Value1ValueListItem { get; set; }

		[Display(Name = "Value 1 Page Id")]
		public int? Value1PageId { get; set; }

		[ForeignKey("Value1PageId")]
		[Display(Name = "Value 1 Page")]
		public virtual ValueListItem Value1Page { get; set; }

	}
}