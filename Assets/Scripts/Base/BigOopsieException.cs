using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class BigOopsieException : Exception
{
	public BigOopsieException() { }
	public BigOopsieException(string message) : base(message) { }
	public BigOopsieException(string message, Exception inner) : base(message, inner) { }
	protected BigOopsieException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}