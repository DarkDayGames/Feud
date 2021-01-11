using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Feud.Server.Extensions
{
	public static class JSInteropFocusExtensions
	{
		public static ValueTask FocusAsync(this IJSRuntime jsRuntime, ElementReference elementReference)
		{
			return jsRuntime.InvokeVoidAsync("BlazorFocusElement", elementReference);
		}
	}
}
