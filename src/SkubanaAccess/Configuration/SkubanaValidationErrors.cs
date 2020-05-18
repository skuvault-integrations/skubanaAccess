using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkubanaAccess.Configuration
{
	public static class SkubanaValidationErrors
	{
		public static SkubanaValidationError ProductDoesNotExist = new SkubanaValidationError( "ERR053" );
		public static SkubanaValidationError ProductStockNotFound = new SkubanaValidationError( "ERR097" );
	}

	public struct SkubanaValidationError
	{
		public string Code { get; private set; }

		public SkubanaValidationError( string code )
		{
			Condition.Requires( code, "code" ).IsNotNullOrEmpty();

			this.Code = code;
		}
	}
}
