using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum Units
	{
		Gr,
		Kg,
		Piece,
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum OrderStatus
	{
		Accepted,
		Waiting,
		Completed
	}


	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum UserType
	{
		[Description("Volunteer")]
		Volunteer,
		[Description("Beneficiary")]
		Beneficiary
	}
}
