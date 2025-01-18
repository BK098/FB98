using System;

namespace FB98.Shared.Abstractions.Exceptions
{
	public class CustomException : Exception
	{
		protected CustomException(string message) : base(message)
		{
		}
	}
}